using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;

    public static AudioManager instanceAudioManager; // Reference to the current AudioManager used in scene
    public AudioMixerGroup mixerMaster; // Reference to the Main Audio Mixer Group

    // Awake is called before the Start Method
    void Awake() {

        // Prevents duplicate AudioManagers in the same scene. Is having another AudioManager in pvpGame scene even necessary? Apparently this is called a singleton pattern?
        if(instanceAudioManager == null) {
            instanceAudioManager = this;
        } else {
            Destroy(gameObject);
            return;
        }

        // Allows AudioManager to persist through scenes
        DontDestroyOnLoad(gameObject);

        // Retrieves sound from sounds array & uses data to create a new AudioSource Component for each sound in the AudioManager
        foreach(Sound element in sounds) {
            element.source = gameObject.AddComponent<AudioSource>();
            element.source.clip = element.clip;
            element.source.volume = element.volume;
            element.source.pitch = element.pitch;
            element.source.loop = element.loop;
            element.source.outputAudioMixerGroup = mixerMaster;
        }
    }

    // Used for Themes & Soundtracks
    private void Start() {

    }

    // PlaySound
    public void PlaySound(string name){
        /* Array.Find loops through array & returns the first element that matches the expression
           The Expression sound => sound.name == name is a lambda expression
           using the data type of the sounds array we create a variable 'sound' which is a placeholder
           sound.name is then compared with the search string 'name'
           Thus Array.Find will return the first sound whose .name matches the input string
         */
        Sound s = Array.Find(sounds, sound => sound.name == name);
        // Checks if sound is found, prevents error from being thrown
        if(s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}
