using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    // Declaring instance of private variable "rb" of type Rigidbody2D (linked to the sprite component).
    private Rigidbody2D rb;

    #endregion

    #region Start
    // Start is called before the first frame update.

    private void Start()
    {
        // GetComponent at start, because it doesn't need to be done more than once.
        rb = GetComponent<Rigidbody2D>();
    }

    #endregion

    #region Update
    // Update is called once per frame.

    private void Update()
    {
        #region Declarations / Input Preface / Misc Comments
        // "Horizontal" is used instead of hardcoding left and right keys.
        // f = floating point literal. Better to use the f whereever possible because it's explicit.
        // rb.velocity.y and rb.velocity.x are used in place of 0 to not cut off momentum/set the speed to 0.

        // Input.GetAxisRaw("Horizontal") would stop the slide/deacceleration effect.
        #endregion
        float dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * 11f, rb.velocity.y);

        #region Button Input Comments
        // If spacebar is pressed, access sprite rigidbody2D component, and make it jump.
        // HARDCODED --> GetKey("space") = Continuous, GetKeyDown("space") = Only once.
        // VERSATILE --> GetButton("Jump") = Continuous, GetButtonDown("Jump") = Only once.
        // GetButtons connects to Unity, allowing better control schemes (multiple buttons can do the same thing).
        #endregion
        if (Input.GetButtonDown("Jump"))
        {
            #region Vector2 / Vector3 Comments
            // Vector3 is a data holder for 3 different values (X, Y, and Z coordinates). In this case, Vector2 is just X and Y values.
            // You must specify how much speed/velocity goes into each direction X and Y. --> (Don't do Z usually because it's depth!)
            #endregion
            rb.velocity = new Vector2(rb.velocity.x, 30f);
        }
    }

    #endregion
}
