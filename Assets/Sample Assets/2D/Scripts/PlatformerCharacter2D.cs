using UnityEngine;

public class PlatformerCharacter2D : MonoBehaviour 
{
    // For determining which way the player is currently facing.
	bool facingRight = true;

    // The fastest the player can travel in the x axis.
    [SerializeField]
    float maxSpeed = 10f;
    // Amount of force added when the player jumps.
    [SerializeField]
    float jumpForce = 400f;

    // Whether or not a player can steer while jumping;
    [SerializeField]
    bool airControl = false;
    // A mask determining what is ground to the character
    [SerializeField]
    LayerMask whatIsGround;
    // Amount of force added when the player jumps.
    [SerializeField]
    float ghostJumpDuration = 0.2f;
    [SerializeField]
    Transform dontFlip = null;
    [SerializeField]
    public AudioMutator runningAudio = null;
    [SerializeField]
    AudioMutator jumpAudio = null;
    [SerializeField]
    AudioMutator landAudio = null;
    [SerializeField]
    ParticleSystem jumpParticles = null;

	Transform groundCheck;								// A position marking where to check if the player is grounded.
	float groundedRadius = .2f;							// Radius of the overlap circle to determine if grounded
	bool grounded = false;								// Whether or not the player is grounded.
    float timeNotGrounded = -1;
	Animator anim;										// Reference to the player's animator component.

    public bool IsGrounded
    {
        get
        {
            if(grounded == true)
            {
                return true;
            }
            else
            {
                return ((Time.time - timeNotGrounded) < ghostJumpDuration);
            }
        }
        set
        {
            if(grounded != value)
            {
                // Check the current and previous values
                if(value == true)
                {
                    timeNotGrounded = -1;
                    landAudio.Play();
                }
                else if(grounded == true)
                {
                    timeNotGrounded = Time.time;
                }
                grounded = value;
            }
        }
    }

    void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("GroundCheck");
		anim = GetComponent<Animator>();
	}


	void FixedUpdate()
	{
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		anim.SetBool("Ground", IsGrounded);

		// Set the vertical animation
		anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);
	}


	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
        float moveAbs = Mathf.Abs(move);
		if(IsGrounded || airControl)
		{
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			anim.SetFloat("Speed", moveAbs);

			// Move the character
			GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
				// ... flip the player.
				Flip();
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				Flip();
		}

        // If the player should jump...
        if (IsGrounded == true)
        {
            if (jump == true)
            {
                // Add a vertical force to the player.
                anim.SetBool("Ground", false);
                GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

                // Play the jump sound
                jumpAudio.Play();
                jumpParticles.Stop();
                jumpParticles.Play();

                // Stop the running audio
                runningAudio.Stop();
            }
            else if ((moveAbs > 0.1f) && (runningAudio.Audio.isPlaying == false))
            {
                runningAudio.Play();
            }
            else if ((moveAbs < 0.1f) && (runningAudio.Audio.isPlaying == true))
            {
                runningAudio.Stop();
            }
        }
        else if(runningAudio.Audio.isPlaying == true)
        {
            // Stop the running audio
            runningAudio.Stop();
        }
	}

	
	void Flip ()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;

        if(dontFlip != null)
        {
            theScale = dontFlip.localScale;
            theScale.x *= -1;
            dontFlip.localScale = theScale;
        }
	}
}
