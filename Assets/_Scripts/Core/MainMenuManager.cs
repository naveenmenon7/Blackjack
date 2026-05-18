using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
        
        SceneManager.LoadScene("Game_Scene");
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