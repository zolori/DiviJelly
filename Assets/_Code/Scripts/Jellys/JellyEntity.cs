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

	[SerializeField] private Flavours m_Flavours;
	private FlavourData m_CurrentFlavour;

	private float m_CurrentVolume = 1f;

	[SerializeField] private float m_Acceleration = 30f;
	[SerializeField] private float m_MaxSpeed = 2f;
	[SerializeField] private float m_DragForce = 1f;
	[SerializeField] private float m_AirMultiplier = 0.3f;
	[SerializeField] private float m_JumpForce = 8f;

	private float m_MovementInputValue = 0;
	private bool m_HasRequestedJump = false;
	private HashSet<GameObject> m_Grounds = new HashSet<GameObject>();

	private bool m_CanMove = false;

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		m_Collider2D = GetComponent<BoxCollider2D>();
		m_LocalFootHeight = m_Collider2D.offset.y - m_Collider2D.size.y * 0.5f;
	}

	private void Start()
	{
		SetFlavour(Flavour.Strawberry);
		SetCanMove(true);
	}

	private void FixedUpdate()
	{
		if(!m_CanMove)
			return;

		bool isGrounded = _IsGrounded();

		float xVelocity = m_Rigidbody2D.velocity.x;

		float accelerationForce = m_MovementInputValue * m_Acceleration;
		if(Mathf.Abs(xVelocity) >= m_MaxSpeed && xVelocity * m_MovementInputValue > 0)
			accelerationForce = 0;
		float dragForce = (Mathf.Abs(xVelocity) * xVelocity + xVelocity) * -m_DragForce;

		m_Rigidbody2D.AddForce(Vector2.right * ((accelerationForce + dragForce) * (isGrounded ? 1 : m_AirMultiplier)), ForceMode2D.Force);

		if(m_HasRequestedJump)
		{
			m_HasRequestedJump = false;
			if(isGrounded)
				m_Rigidbody2D.AddForce(Vector2.up * m_JumpForce, ForceMode2D.Impulse);
		}
	}

	private bool _IsGrounded()
	{
		return m_Grounds.Count > 0;
	}

	public void SetCanMove(bool iCanMove)
	{
		m_CanMove = iCanMove;

		if(m_CanMove)
			m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
		else
			m_Rigidbody2D.constraints |= RigidbodyConstraints2D.FreezePositionX;
	}

	public void SetFlavour(Flavour iFlavour)
	{
		JellysManager.Instance.UnregisterJelly(this);
		m_CurrentFlavour = m_Flavours.Data.Find(flavourData => flavourData.Flavour == iFlavour);
		JellysManager.Instance.RegisterJelly(this);

		gameObject.layer = m_CurrentFlavour.Layer;
		m_SpriteRenderer.sprite = m_CurrentFlavour.Sprite;
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

		// TODO: change animation speed
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
		if(_IsGroundCollision(iCollision))
			m_Grounds.Add(iCollision.gameObject);
	}

	private void OnCollisionExit2D(Collision2D iCollision)
	{
		m_Grounds.Remove(iCollision.gameObject);
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
}
