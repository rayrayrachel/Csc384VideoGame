using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;  
using UnityEngine.UI;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    public GameObject highScoreTable;
    public TextMeshProUGUI[] highScoreTexts;  
    public Button closeHighScoreButton;
    private const string HIGH_SCORE_KEY = "PlayerScore";

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

    public void ShowHighScoreTable()
    {
        highScoreTable.SetActive(true);

        LoadHighScores();

        //HighlightLastScore();
    }

    public void HideHighScoreTable()
    {
        highScoreTable.SetActive(false);
    }
    void LoadHighScores()
    {
        string savedScores = PlayerPrefs.GetString("PlayerScores", "");
        string[] scoreStrings = savedScores.Split(',');

        List<int> highScores = new List<int>();

        foreach (string score in scoreStrings)
        {
            if (int.TryParse(score, out int parsedScore))
            {
                highScores.Add(parsedScore);
            }
        }

        highScores.Sort((a, b) => b.CompareTo(a));

        int recentScore = PlayerPrefs.GetInt("RecentScore", 0); 
        if (!highScores.Contains(recentScore))
        {
            highScores.Add(recentScore); 
            highScores.Sort((a, b) => b.CompareTo(a)); 
        }

        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            if (i < highScores.Count)
            {
                highScoreTexts[i].text = "Score " + (i + 1) + ": " + highScores[i];
                if (highScores[i] == recentScore)
                {
                    highScoreTexts[i].color = Color.green; 
                }
                else
                {
                    highScoreTexts[i].color = Color.white;  
                }
            }
            else
            {
                highScoreTexts[i].text = "Score " + (i + 1) + ": 0"; 
            }
        }
    }




    //void HighlightLastScore()
    //{
    //    if (highScoreTexts.Length > 0)
    //    {
    //        highScoreTexts[highScoreTexts.Length - 1].color = Color.green;  
    //    }
    //}

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
