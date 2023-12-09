using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The following script contents was found and adapted from the following source: *
 * https://www.sharpcoderblog.com/blog/unity-implementing-footstep-sounds         *
 * This script was implemented and adapted to our project by Jhet Birchem.        */

public class FootstepController : MonoBehaviour
{
    #region Audio Variables
    // Player Walking Audio Sound Effect Variable
    [SerializeField] private AudioSource walkSoundEffect;
    #endregion

    public bool isWalking = false;           // Flag to track if the player is walking.
    public float timeBetweenFootsteps = 0.5f; // Time between footstep sounds.
    private float timeSinceLastFootstep;      // Time since the last footstep sound.

    // Automatically updates every frame.
    private void Update()
    {
        // Check if the player is walking.
        if (isWalking)
        {
            // Check if enough time has passed to play the next footstep sound.
            if ( (Time.time - timeSinceLastFootstep) >= timeBetweenFootsteps )
            {
                walkSoundEffect.Play();
                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound.
            }
        }
    }

    // Call this method when the player starts walking.
    public void StartWalking()
    {
        isWalking = true;
    }

    // Call this method when the player stops walking.
    public void StopWalking()
    {
        isWalking = false;
    }
}
