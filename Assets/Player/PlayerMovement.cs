using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using UnityEngine;

public enum PlayerPower
{
    None,
    Smash,
    Mobility
}

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
//    private Animator anim;
    private float dirX = 0f;

    private bool isGrounded = false;
    private int holdingJumpCounter = 0;
    private bool canDoubleJump = false;
    private bool canTripleJump = false;
    private double gOff = 0;

    private PlayerPower playerPower = PlayerPower.Mobility;

    [SerializeField] private LayerMask jumpableGround;
    #endregion

    // Start is called before the first frame update.
    private void Start()
    {
        // GetComponent at start, because it doesn't need to be done more than once.
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
//        anim = GetComponent<Animator>();
    }

    // Update is called once per frame.
    private void Update()
    {
        UpdatePlayerState();
        HandlePlayerInput();
        UpdatePlayerAnimation();
    }

    #region Update Methods

    private void UpdatePlayerState()
    {
        isGrounded = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, 0.1f, jumpableGround);   // investigate
        if ( isGrounded )
        {
            canDoubleJump = true;
            canTripleJump = true;
        }
    }

    private void HandlePlayerInput()
    {
        dirX = Input.GetAxisRaw("Horizontal");  // -1, 0, 1

        rb.velocity = new Vector2(dirX * 11.0f, rb.velocity.y);

        /*
        if (Input.GetButtonDown("Escape"))
        {
            Application.Quit();
        }
        */

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                holdingJumpCounter = 20;
                rb.velocity = new Vector2(rb.velocity.x, 14);
            }
            else if (canDoubleJump)
            {
                canDoubleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, 22);
            }
            else if (playerPower == PlayerPower.Mobility && canTripleJump)
            {
                canTripleJump = false;
                rb.velocity = new Vector2(rb.velocity.x, 22);
            }
        }
        else if (Input.GetButtonUp("Jump"))
            holdingJumpCounter = 0;
        else if (holdingJumpCounter > 0)
        {
            if (holdingJumpCounter < 8)
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 1.45f);
            --holdingJumpCounter;
        }
    }

    /// <summary>
    /// Updates Player Animations based on their state.
    /// (currently this is just based on player movement, but once this gets larger we may
    /// want to consider adding seperate helper functions for different conditions such as death & dmg, powerups, etc.)
    /// </summary>
    private void UpdatePlayerAnimation()
    {
        if (dirX > 0f)
            sprite.flipX = false;
        else if (dirX < 0f)
            sprite.flipX = true;
    }

    #endregion
}
