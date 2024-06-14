using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JellyEntity : MonoBehaviour
{
	private Rigidbody2D m_Rigidbody2D;
	private BoxCollider2D m_Collider2D;
	private float m_LocalFootHeight = 0;
	[SerializeField] private SpriteRenderer m_SpriteRenderer;
	[SerializeField] private ParticleSystem m_Particles;

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

	private const float s_MaxVolume = 8f;
	private int m_NbParts = 8;
	private float m_Volume = 1f;
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

	private const float s_FootMargin = 0.01f;
	private const float s_FootHeight = 0.01f;
	private int m_LayerMask;
	private const float s_MinForceCollisionSq = 16;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Collider2D = GetComponent<BoxCollider2D>();
		m_LocalFootHeight = m_Collider2D.offset.y - m_Collider2D.size.y * 0.5f - m_Collider2D.edgeRadius;
		m_JellysManager = JelliesManager.Instance;
		m_JelliesController = FindFirstObjectByType<JelliesController>();
		m_LayerMask = LayerMask.GetMask("Default");
		foreach(FlavourData data in m_Flavours.Data)
			m_LayerMask |= (1 << data.Layer);

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
		SetVolume(m_NbParts);
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

		m_Rigidbody2D.AddForce(Vector2.right * ((accelerationForce + dragForce) * m_Volume * (isGrounded ? 1 : m_AirMultiplier)), ForceMode2D.Force);

		if(m_HasRequestedJump)
		{
			m_HasRequestedJump = false;
			if(isGrounded)
			{
				m_Rigidbody2D.velocity = Vector2.right * m_Rigidbody2D.velocity.x; // canceling vertical momentum
				m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce * m_Volume, ForceMode2D.Impulse);
				m_SfxPlayer.PlayOneShot(m_JumpSound);

				m_Particles.Stop();
				m_Particles.Play();
			}
		}
	}

	private bool _IsGrounded()
	{
		Vector2 middleFoot = (Vector2)transform.position + Vector2.up * (m_LocalFootHeight - s_FootHeight * 0.5f - s_FootMargin);
		float width = m_Collider2D.size.x * transform.localScale.x;

		RaycastHit2D[] hits = Physics2D.BoxCastAll(middleFoot, new Vector2(width, s_FootHeight), 0, Vector2.down, 0.01f, m_LayerMask);
		Debug.DrawRay(middleFoot + new Vector2(-width * 0.5f, s_FootHeight * 0.5f), Vector2.down * (s_FootHeight + 0.01f), Color.magenta);
		Debug.DrawRay(middleFoot + new Vector2(width * 0.5f, s_FootHeight * 0.5f), Vector2.down * (s_FootHeight + 0.01f), Color.magenta);
		Debug.DrawRay(middleFoot + new Vector2(-width * 0.5f, s_FootHeight * 0.5f), Vector2.right * width, Color.magenta);
		Debug.DrawRay(middleFoot + new Vector2(-width * 0.5f, s_FootHeight * -0.5f - 0.01f), Vector2.right * width, Color.magenta);

		foreach(RaycastHit2D hit in hits)
		{
			if(hit.collider == null)
				continue;
			if(hit.collider.isTrigger)
				continue;
			if((hit.collider.excludeLayers & (1 << gameObject.layer)) != 0)
				continue;

			return true;
		}

		return false;
	}

	public void SetCanMove(bool iCanMove)
	{
		m_CanMove = iCanMove;
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
		ParticleSystem.MainModule mainModule = m_Particles.main;
		mainModule.startColor = m_CurrentFlavour.Color;
	}

	public Flavour GetFlavour()
	{
		return m_CurrentFlavour.Flavour;
	}

	public void SetVolume(int iVolume)
	{
		m_NbParts = iVolume;
		m_Volume = m_NbParts / s_MaxVolume;
		Debug.Log($"{m_NbParts} -> {m_Volume}");
		float volumeToScale = Mathf.Pow(m_Volume, 0.33333f);
		transform.localScale = Vector3.one * volumeToScale;

		m_Rigidbody2D.mass = m_Volume;
		ParticleSystem.ShapeModule shapeModule = m_Particles.shape;
		shapeModule.scale = volumeToScale * 0.8f * new Vector3(1, 0.8f, 1);
	}

	public float GetVolume()
	{
		return m_Volume;
	}

	public Rect GetBBox()
	{
		return new Rect((Vector2)transform.position + (m_Collider2D.offset - m_Collider2D.size * 0.5f) * transform.localScale,
			m_Collider2D.size * transform.localScale);
	}

	[Button]
	public bool Split()
	{
		if(m_NbParts <= 1)
			return false;

		int vol1 = m_NbParts / 2;
		int vol2 = m_NbParts - vol1;

		SetVolume(vol1);
		GameObject newJellyObject = Instantiate(m_JellyPrefab, transform.position, transform.rotation, transform.parent);
		JellyEntity newJellyEntity = newJellyObject.GetComponent<JellyEntity>();
		newJellyEntity.SetFlavour(GetFlavour());
		newJellyEntity.SetVolume(vol2);
		newJellyEntity.SetMovementInputValue(m_MovementInputValue);

		Vector3 offset = (m_Collider2D.size.x * transform.localScale.x * 0.5f * s_SplitHalfSpace) * Vector3.right;
		transform.position -= offset;
		newJellyEntity.transform.position += offset;

		m_ResetPhysics = true;
		newJellyEntity.m_AnimRigidbody2DsPos = m_AnimRigidbody2DsPos;
		newJellyEntity.m_ResetPhysics = true;
		m_SfxPlayer.PlayOneShot(m_SplitSound);
		m_Particles.Stop();
		m_Particles.Play();

		return true;
	}

	public void Merge(JellyEntity iOther)
	{
		if(iOther.GetFlavour() != GetFlavour())
			return;
		if(iOther == this)
			return;

		float prevVolume = m_NbParts;
		SetVolume(m_NbParts + iOther.m_NbParts);
		transform.position = Vector3.LerpUnclamped(iOther.transform.position, transform.position, prevVolume / m_NbParts);
		m_ResetPhysics = true;
		m_SfxPlayer.PlayOneShot(m_MergeSound);
		m_Particles.Stop();
		m_Particles.Play();

		iOther.SetVolume(0);
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

	private void _DebugCollision(Collision2D iCollision)
	{
		List<ContactPoint2D> contacts = new List<ContactPoint2D>();
		iCollision.GetContacts(contacts);
		foreach(ContactPoint2D contact in contacts)
			Debug.DrawLine(contact.point, contact.point + contact.normal, Color.magenta, 2f);
	}

	private void OnCollisionEnter2D(Collision2D iCollision)
	{
		if(iCollision.relativeVelocity.sqrMagnitude < s_MinForceCollisionSq)
			return;

		m_SfxPlayer.PlayOneShot(m_BounceSound);
		m_Particles.Stop();
		m_Particles.Play();
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
