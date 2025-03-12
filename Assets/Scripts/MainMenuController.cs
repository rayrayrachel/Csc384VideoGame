using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void OpenSettings()
    {
    }
}
