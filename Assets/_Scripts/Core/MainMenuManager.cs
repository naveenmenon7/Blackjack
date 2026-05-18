using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game_Scene");
    }

    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("QUIT GAME");
    }
}