using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;   // dark full-screen panel with Resume / Restart / Menu
    public GameObject pauseButton;  // the II button in the HUD

    void Start()
    {
        SetPaused(false);           // the level always starts unpaused
    }

    public void Pause()  { SetPaused(true); }
    public void Resume() { SetPaused(false); }

    void SetPaused(bool paused)
    {
        pausePanel.SetActive(paused);
        pauseButton.SetActive(!paused);
        Time.timeScale = paused ? 0f : 1f;   // physics and movement stop
    }

    public void Restart()
    {
        Time.timeScale = 1f;                  // otherwise the new level starts frozen
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu");
    }
}