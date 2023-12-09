using UnityEngine.Audio;
using UnityEngine;

[System.Serializable] //Allows for Sound vars to be declared in other scripts (see AudioManager) & editable in the Unity Inspector
public class Sound {
    //Class Sound is essentially a public struct
    public string name;
    public AudioClip clip;

    [Range(0f, 1f)] //Adds Min/Max Slider to Volume Attribute
    public float volume;
    [Range(0.1f, 3f)] //Adds Min/Max Slider to Pitch Attribute
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
