using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Player Variables
    /* Player Variables
     * "rb"     linked to the sprite's rigidbody(physics) component
     * "sprite" linked to the sprite's renderer component
     * "anim"   linked to the animation component
     * "dirX"   linked to Player's Horizontal Axis data
     * 
     * "moveSpeed"  player constant
     * "jumpForce"  player constant
     */
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private float dirX = 0f;

    [SerializeField] private float moveSpeed = 11f;
    [SerializeField] private float jumpForce = 30f;
    #endregion

    // Start is called before the first frame update.
    private void Start()
    {
        // GetComponent at start, because it doesn't need to be done more than once.
        rb = GetComponent<Rigidbody2D>();
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
        if(Input.GetButtonDown("Jump"))
        {
            /* Vector3 is a data holder for 3 different values (X, Y, and Z coordinates). In this case, Vector2 is just X and Y values.
             * You must specify how much speed/velocity goes into each direction X and Y. --> (Don't do Z usually because it's depth!)*/
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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
    #endregion
}
