using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Player Variables
    /* Player Variables
     * "rb"     linked to the sprite's rigidbody(physics) component
     * "coll"   linked to the sprite's boxcollider component
     * "sprite" linked to the sprite's renderer component
     * "anim"   linked to the animation component
     * "dirX"   linked to Player's Horizontal Axis data
     * 
     * "jumpableGround" linked to the LayerMask which we want to jump on
     * 
     * "moveSpeed"  player constant
     * "jumpForce"  player constant
     * "jumpForce2" A player constant might remove or change 
     */
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;
    
    private bool doublejump;
    private bool powerG = true;

    [SerializeField] private LayerMask jumpableGround;

    [SerializeField] private float moveSpeed = 11f;
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private float grav = 7.5f;
    #endregion

    // Start is called before the first frame update.
    private void Start()
    {
        // GetComponent at start, because it doesn't need to be done more than once.
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame.
    private void Update()
    {
        HandlePlayerInput();
        UpdatePlayerAnimation();
    }

    #region Update Methods
    /// <summary>
    /// Updates Player Movement based on Player Input
    /// </summary>
    private void HandlePlayerInput() {
        /* "Horizontal" is used instead of hardcoding left and right keys.
         * rb.velocity.y and rb.velocity.x are used in place of 0 to not cut off momentum/set the speed to 0.
         * Input.GetAxisRaw("Horizontal") would stop the slide/deacceleration effect.*/
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        
        /* If spacebar is pressed, access sprite rigidbody2D component, and make it jump.
         * HARDCODED --> GetKey("space") = Continuous, GetKeyDown("space") = Only once.
         * VERSATILE --> GetButton("Jump") = Continuous, GetButtonDown("Jump") = Only once.
         * GetButtons connects to Unity, allowing better control schemes (multiple buttons can do the same thing).*/
        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            /* Vector3 is a data holder for 3 different values (X, Y, and Z coordinates). In this case, Vector2 is just X and Y values.
             * You must specify how much speed/velocity goes into each direction X and Y. --> (Don't do Z usually because it's depth!)*/
            doublejump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        else if(Input.GetButtonDown("Jump") && doublejump == true)
        {
            doublejump = false;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 2/3f );
        }
        else if (Input.GetButtonDown("Jump") && doublejump == false && powerG == true)
        {
            powerG = false;
            rb.gravityScale = grav / 1.5f;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * 1.4f);
            
        }

        if (IsGrounded() && powerG == false)
        {
            powerG = true;
            rb.gravityScale = grav;
        }
    }

    /// <summary>
    /// Updates Player Animations based on their state.
    /// (currently this is just based on player movement, but once this gets larger we may
    /// want to consider adding seperate helper functions for different conditions such as death & dmg, powerups, etc.)
    /// </summary>
    private void UpdatePlayerAnimation() {
        // Updates player based on movement along the X-Axis
        if(dirX > 0f) {
            sprite.flipX = false;
        }
        else if(dirX < 0f) {
            sprite.flipX = true;
        }
        else {
        
        }
    }

    /// <summary>
    /// Creates a Box Collider (equal in size to the Player BoxCollider)
    /// slightly below the Player collider to detect if the player is on jumpable ground.
    /// </summary>
    /// <returns>True if on jumpable ground; False if anywhere else</returns>
    private bool IsGrounded() {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);
    }
    #endregion
}
