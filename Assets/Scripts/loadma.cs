using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadma : MonoBehaviour
{
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
