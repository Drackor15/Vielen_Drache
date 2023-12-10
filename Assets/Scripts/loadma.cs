using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadma : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.OnWin.AddListener(Victory);
    }
    private void OnDisable()
    {
        EventManager.OnWin.RemoveListener(Victory);
    }

    public void playGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void playAgain()
    {
        SceneManager.LoadScene("Level");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    public void Victory()
    {
        SceneManager.LoadScene("Victory");
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
