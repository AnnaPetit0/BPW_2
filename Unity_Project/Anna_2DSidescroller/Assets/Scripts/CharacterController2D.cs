using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround = -1;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck = null;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck = null;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider = null;				// A collider that will be disabled when crouching
    [SerializeField] List<GameObject> cameras = null;
    [SerializeField] private LayerMask playermask = -1;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	public bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 velocity = Vector3.zero;
    private bool jumpisnotactive = true;
    private Vector3 camerascale;
    private bool onPlatform = false;
    private Rigidbody2D rb_Platform;
    private Vector3 checkdirection;
    private Vector3 prevposition;
    public bool keepCrouch = false;
    private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
	}


	private void FixedUpdate()
	{
		m_Grounded = false;
        if (rb_Platform != null)
        {
            checkdirection = rb_Platform.gameObject.transform.position - prevposition;
            prevposition = rb_Platform.gameObject.transform.position;
        }

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (colliders[i].gameObject.layer == 12)
                {
                    transform.parent = colliders[i].gameObject.transform;
                    onPlatform = true;
                    rb_Platform = colliders[i].gameObject.GetComponent<Rigidbody2D>();
                }
                else if (transform.parent != null)
                {
                    transform.parent = null;
                    onPlatform = false;
                    rb_Platform = null;
                }

                if (colliders[i].gameObject.layer == 14)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }

                if (colliders[i].gameObject.layer == 13)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
            }
       	}
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), Mathf.Infinity, playermask);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer != 12)
            {
                onPlatform = false;
                transform.parent = null;
                rb_Platform = null;
            }
        }
        else
        {
            onPlatform = false;
            transform.parent = null;
            rb_Platform = null;
        }
        if (onPlatform && !m_Grounded && !jumpisnotactive)
        {
            if (checkdirection.y < 0)
            {
                transform.parent = null;
                onPlatform = false;
                rb_Platform = null;
            }
            else
            {
                onPlatform = true;
            }
        }
        if (checkdirection.y == 0)
        {
            if (checkdirection.x != 0)
            {
                if (!m_Grounded)
                {
                    transform.parent = null;
                    onPlatform = false;
                    rb_Platform = null;
                }
            }
        }
    }

	public void Move(float move, bool crouch, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
                keepCrouch = true;
			}
            else
            {
                keepCrouch = false;
            }
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
            if (jumpisnotactive)
            {
                StartCoroutine(Dojump());
            }
        }
	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

        foreach (GameObject camera in cameras)
        {
            camerascale = new Vector3(camera.GetComponent<Transform>().localPosition.x, camera.GetComponent<Transform>().localPosition.y, camera.GetComponent<Transform>().localPosition.z);
            camerascale.x *= -1;
            camera.transform.localPosition = camerascale;
        }
    }

    IEnumerator Dojump()
    {
        jumpisnotactive = false;
        m_Grounded = false;
        m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        yield return new WaitForSeconds(.1f);
        jumpisnotactive = true;
    }
}
