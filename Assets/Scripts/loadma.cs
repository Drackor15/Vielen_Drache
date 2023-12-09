using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadma : MonoBehaviour
{
    #region Audio Variables
    // Jump_1 Audio Sound Effect Variable
    [SerializeField] private AudioSource jumpSoundEffect1;

    // Jump_2 Audio Sound Effect Variable
    [SerializeField] private AudioSource jumpSoundEffect2;

    // Jump_3 Audio Sound Effect Variable
    [SerializeField] private AudioSource jumpSoundEffect3;
    #endregion

    public void playGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void toggleFullscreen()
    {
        if (Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen)
            Screen.fullScreenMode = FullScreenMode.Windowed;
        else Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
    }
}
