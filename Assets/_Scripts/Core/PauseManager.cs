using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;

    bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    public void TogglePause()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ResumeGame()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        isPaused = false;

        pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public void MainMenu()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu_Scene");
    }

    public void QuitGame()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }

        Application.Quit();

        Debug.Log("QUIT GAME");
    }
}