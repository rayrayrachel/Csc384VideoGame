using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    public void StartGame()
    {
        if (audioSource && clickSound)
        {
            audioSource.PlayOneShot(clickSound);
            Invoke("LoadGameScene", clickSound.length); 
        }
        else
        {
            LoadGameScene();
        }
    }

    public void QuitGame()
    {


        if (audioSource && clickSound)
        {
            audioSource.PlayOneShot(clickSound);
            Invoke("LoadQuitGame", clickSound.length); 
        }
        else
        {
            LoadQuitGame();
        }
    }

    public void OpenSettings()
    {
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("GamePlay");
    }


    void LoadQuitGame()
    {
        UnityEngine.Application.Quit();

#if UNITY_EDITOR
    EditorApplication.ExitPlaymode();
#endif
    }


}
