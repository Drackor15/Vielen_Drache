using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The following script contents was found and adapted from the following source: *
 * https://www.sharpcoderblog.com/blog/unity-implementing-footstep-sounds         *
 * This script was implemented and adapted to our project by Jhet Birchem.        */

public class FootstepController : MonoBehaviour
{
    #region Audio Variables
    // Walking Audio Sound Effect Variable
    [SerializeField] private AudioSource walkSoundEffect;
    #endregion

    #region Walking Variables
    public bool isWalking = false;           // Flag to track if the character is walking.
    public float timeBetweenFootsteps = 0.5f; // Time between footstep sounds.
    private float timeSinceLastFootstep;      // Time since the last footstep sound.
    #endregion

    #region Runtime Updater
    // Automatically updates every frame.
    private void Update()
    {
        // Check if the character is walking.
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
    #endregion

    #region Start/Stop Walking Methods
    // Call this method when the character starts walking.
    public void StartWalking()
    {
        isWalking = true;
    }

    // Call this method when the character stops walking.
    public void StopWalking()
    {
        isWalking = false;
    }
    #endregion
}
