using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JellyEntity : MonoBehaviour
{
	private Rigidbody2D m_Rigidbody2D;
	private BoxCollider2D m_Collider2D;
	private float m_LocalFootHeight = 0;
	[SerializeField] private SpriteRenderer m_SpriteRenderer;

	[SerializeField] private Flavours m_Flavours;
	private FlavourData m_CurrentFlavour;

	[SerializeField] private float m_Acceleration = 30f;
	[SerializeField] private float m_MaxSpeed = 2f;
	[SerializeField] private float m_DragForce = 1f;
	[SerializeField] private float m_AirMultiplier = 0.3f;
	[SerializeField] private float m_JumpForce = 8f;

	private bool m_CanMove = false;
	private float m_MovementInputValue = 0;
	private bool m_HasRequestedJump = false;
	private HashSet<GameObject> m_Grounds = new HashSet<GameObject>();

	private float m_CurrentVolume = 1f;
	[SerializeField] private float s_MinimalVolume = 0.1f;
	[SerializeField] private float s_SplitHalfSpace = 1.5f;

	[SerializeField] private GameObject m_JellyPrefab;

	private JelliesManager m_JellysManager;
	private JelliesController m_JelliesController;

	// some schenanigan to counter weird animation positioning when splitting and merging
	// fuck unity
	private bool m_ResetPhysics = false;
	private bool m_EnableAnimPhysics = true;
	private List<Rigidbody2D> m_AnimRigidbody2Ds = new List<Rigidbody2D>();
	private List<FixedJoint2D> m_AnimPhysicsGlue = new List<FixedJoint2D>();
	private List<Vector3> m_AnimRigidbody2DsPos = new List<Vector3>();

	[SerializeField] private AudioSource m_SfxPlayer;
	[SerializeField] private AudioClip m_JumpSound;
	[SerializeField] private AudioClip m_MergeSound;
	[SerializeField] private AudioClip m_SplitSound;
	[SerializeField] private AudioClip m_BounceSound;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Collider2D = GetComponent<BoxCollider2D>();
		m_LocalFootHeight = m_Collider2D.offset.y - m_Collider2D.size.y * 0.5f;
		m_JellysManager = JelliesManager.Instance;
		m_JelliesController = FindFirstObjectByType<JelliesController>();

		FindAnimComponents();
		if(m_AnimRigidbody2DsPos.Count == 0)
			InitAnimPos();
		else
			m_ResetPhysics = true;
	}

	private void FindAnimComponents()
	{
		GetComponentsInChildren(m_AnimRigidbody2Ds);
		m_AnimRigidbody2Ds.Remove(GetComponent<Rigidbody2D>());
		GetComponentsInChildren(m_AnimPhysicsGlue);
	}

	private void InitAnimPos()
	{
		foreach(Rigidbody2D rb in m_AnimRigidbody2Ds)
			m_AnimRigidbody2DsPos.Add(rb.transform.localPosition);
	}

	private void SetAnimPos(List<Vector3> iPos)
	{
		for(int rbIdx = 0; rbIdx < m_AnimRigidbody2Ds.Count; rbIdx++)
		{
			Rigidbody2D rb = m_AnimRigidbody2Ds[rbIdx];
			rb.transform.localPosition = iPos[rbIdx];
			rb.position = rb.transform.position;
			rb.transform.localRotation = Quaternion.identity;
			rb.rotation = 0;
			rb.velocity = Vector2.zero;
			rb.angularVelocity = 0;
		}
		m_MovementInputValue = 0;
		foreach(FixedJoint2D joint in m_AnimPhysicsGlue)
			joint.enabled = true;
		m_EnableAnimPhysics = true;
	}

	private void Start()
	{
		_UpdateFlavour(m_CurrentFlavour.Flavour);
		SetVolume(m_CurrentVolume);
	}

	private void FixedUpdate()
	{
		if(!m_CanMove)
			return;

		if(m_ResetPhysics)
		{
			m_ResetPhysics = false;
			m_Rigidbody2D.velocity = Vector3.zero;
			m_Rigidbody2D.position = transform.position;
			SetAnimPos(m_AnimRigidbody2DsPos);
			return;
		}
		if(m_EnableAnimPhysics)
		{
			m_EnableAnimPhysics = false;
			foreach(FixedJoint2D joint in m_AnimPhysicsGlue)
				joint.enabled = false;
			return;
		}

		bool isGrounded = _IsGrounded();

		float xVelocity = m_Rigidbody2D.velocity.x;

		float accelerationForce = m_MovementInputValue * m_Acceleration;
		if(Mathf.Abs(xVelocity) >= m_MaxSpeed && xVelocity * m_MovementInputValue > 0)
			accelerationForce = 0;
		float dragForce = (Mathf.Abs(xVelocity) * xVelocity + xVelocity) * -m_DragForce;

		m_Rigidbody2D.AddForce(Vector2.right * ((accelerationForce + dragForce) * m_CurrentVolume * (isGrounded ? 1 : m_AirMultiplier)), ForceMode2D.Force);

		if(m_HasRequestedJump)
		{
			m_HasRequestedJump = false;
			if(isGrounded)
			{
				m_Rigidbody2D.velocity = Vector2.right * m_Rigidbody2D.velocity.x; // canceling vertical momentum
				m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce * m_CurrentVolume, ForceMode2D.Impulse);
				m_SfxPlayer.PlayOneShot(m_JumpSound);
			}
		}
	}

	private bool _IsGrounded()
	{
		return m_Grounds.Count > 0;
	}

	public void SetCanMove(bool iCanMove)
	{
		m_CanMove = iCanMove;
		/*
				if(m_CanMove)
					m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
				else
					m_Rigidbody2D.constraints |= RigidbodyConstraints2D.FreezePositionX;*/
	}

	public bool CanMove()
	{
		return m_CanMove;
	}

	public void SetFlavour(Flavour iFlavour)
	{
		if(iFlavour == GetFlavour())
			return;

		_UpdateFlavour(iFlavour);
	}

	private void _UpdateFlavour(Flavour iFlavour)
	{
		m_JellysManager.UnregisterJelly(this);
		m_CurrentFlavour = m_Flavours.GetData(iFlavour);
		m_JellysManager.RegisterJelly(this);
		SetCanMove(m_JelliesController.GetCurrentControlledFlavour() == GetFlavour());

		gameObject.layer = m_CurrentFlavour.Layer;
		m_SpriteRenderer.color = m_CurrentFlavour.Color;
	}

	public Flavour GetFlavour()
	{
		return m_CurrentFlavour.Flavour;
	}

	public void SetVolume(float iVolume)
	{
		m_CurrentVolume = iVolume;
		float volumeToScale = Mathf.Pow(m_CurrentVolume, 0.33333f);
		transform.localScale = Vector3.one * volumeToScale;

		m_Rigidbody2D.mass = m_CurrentVolume;
	}

	public float GetVolume()
	{
		return m_CurrentVolume;
	}

	public Rect GetBBox()
	{
		return new Rect((Vector2)transform.position + (m_Collider2D.offset - m_Collider2D.size * 0.5f) * transform.localScale,
			m_Collider2D.size * transform.localScale);
	}

	[Button]
	public bool Split()
	{
		if(m_CurrentVolume <= s_MinimalVolume * 2)
			return false;

		SetVolume(m_CurrentVolume * 0.5f);
		GameObject newJellyObject = Instantiate(m_JellyPrefab, transform.position, transform.rotation, transform.parent);
		JellyEntity newJellyEntity = newJellyObject.GetComponent<JellyEntity>();
		newJellyEntity.SetFlavour(GetFlavour());
		newJellyEntity.SetVolume(m_CurrentVolume);
		newJellyEntity.SetMovementInputValue(m_MovementInputValue);

		Vector3 offset = (m_Collider2D.size.x * transform.localScale.x * 0.5f * s_SplitHalfSpace) * Vector3.right;
		transform.position -= offset;
		newJellyEntity.transform.position += offset;

		m_ResetPhysics = true;
		newJellyEntity.m_AnimRigidbody2DsPos = m_AnimRigidbody2DsPos;
		newJellyEntity.m_ResetPhysics = true;
		m_SfxPlayer.PlayOneShot(m_SplitSound);

		return true;
	}

	public void Merge(JellyEntity iOther)
	{
		if(iOther.GetFlavour() != GetFlavour())
			return;
		if(iOther == this)
			return;

		float prevVolume = m_CurrentVolume;
		SetVolume(m_CurrentVolume + iOther.m_CurrentVolume);
		transform.position = Vector3.LerpUnclamped(iOther.transform.position, transform.position, prevVolume / m_CurrentVolume);
		m_ResetPhysics = true;
		m_SfxPlayer.PlayOneShot(m_MergeSound);

		Destroy(iOther.gameObject);
	}

	public void Jump()
	{
		m_HasRequestedJump = true;
	}

	public void SetMovementInputValue(float iVal)
	{
		m_MovementInputValue = iVal;
	}

	private bool _IsGroundCollision(Collision2D iCollision)
	{
		List<ContactPoint2D> contacts = new List<ContactPoint2D>();
		iCollision.GetContacts(contacts);
		foreach(ContactPoint2D contact in contacts)
		{
			Vector2 localContactPoint = transform.worldToLocalMatrix.MultiplyPoint(contact.point);
			if(Mathf.Abs(Vector2.Dot(contact.normal, Vector2.up)) >= 0.8f && Mathf.Abs(localContactPoint.y - m_LocalFootHeight) <= 0.1f)
				return true;
		}
		return false;
	}

	private void _DebugCollision(Collision2D iCollision)
	{
		List<ContactPoint2D> contacts = new List<ContactPoint2D>();
		iCollision.GetContacts(contacts);
		foreach(ContactPoint2D contact in contacts)
			Debug.DrawLine(contact.point, contact.point + contact.normal, Color.magenta, 2f);
	}

	private void OnCollisionEnter2D(Collision2D iCollision)
	{
		m_SfxPlayer.PlayOneShot(m_BounceSound);
		if(_IsGroundCollision(iCollision))
			m_Grounds.Add(iCollision.gameObject);
	}

	private void OnCollisionStay2D(Collision2D iCollision)
	{
		JellyEntity otherJelly;
		if(!iCollision.gameObject.TryGetComponent(out otherJelly))
			return;
		if(otherJelly.GetFlavour() != GetFlavour())
			return;
		if(!_IsGrounded() || !otherJelly._IsGrounded())
			return;

		if(otherJelly.GetHashCode() < GetHashCode())
			Merge(otherJelly);
	}

	private void OnCollisionExit2D(Collision2D iCollision)
	{
		m_Grounds.Remove(iCollision.gameObject);
	}

	private void OnDestroy()
	{
		m_JellysManager?.UnregisterJelly(this);
		m_JellysManager = null;
		m_JelliesController = null;
	}

	/////////////////////////////
	// TESTING
	/////////////////////////////

	[Button]
	public void SetStrawberry()
	{
		SetFlavour(Flavour.Strawberry);
	}
	[Button]
	public void SetOrange()
	{
		SetFlavour(Flavour.Orange);
	}
	[Button]
	public void SetLemon()
	{
		SetFlavour(Flavour.Lemon);
	}
	[Button]
	public void SetApple()
	{
		SetFlavour(Flavour.Apple);
	}
	[Button]
	public void SetBlueberry()
	{
		SetFlavour(Flavour.Blueberry);
	}
}
