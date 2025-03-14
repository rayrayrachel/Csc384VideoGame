using UnityEngine;
using TMPro;
using System.Diagnostics;
using System.Collections;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Interactionontroller : MonoBehaviour
{

    public TextMeshProUGUI instructionText;
    public UnityEngine.UI.Image backgroundSprite;
    public float fadeDuration = 2f;
    private bool isPlayerInRange = false;

#if UNITY_EDITOR
    public SceneAsset sceneToLoad; // Drag and drop scene in Inspector
#endif
    private string sceneName;
    private bool isChangingScene = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetAlpha(0f);

#if UNITY_EDITOR
        if (sceneToLoad != null)
            sceneName = sceneToLoad.name; 
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine(FadeOutInstruction());
                StartCoroutine(ChangeScene());

            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            StartCoroutine(FadeInInstruction());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            StartCoroutine(FadeOutInstruction());
        }
    }

    IEnumerator FadeInInstruction()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(timeElapsed / fadeDuration);

            SetAlpha(alpha);

            yield return null;
        }

        SetAlpha(1f);
    }

    IEnumerator FadeOutInstruction()
    {
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(timeElapsed / fadeDuration);

            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);

    }

    void SetAlpha(float alpha)
    {
        Color textColor = instructionText.color;
        textColor.a = alpha;
        instructionText.color = textColor;

        Color backgroundColor = backgroundSprite.color;
        backgroundColor.a = alpha;
        backgroundSprite.color = backgroundColor;
    }

    IEnumerator ChangeScene()
    {
        if (isChangingScene) yield break;
        isChangingScene = true;

        if (!string.IsNullOrEmpty(sceneName))
        {
            yield return StartCoroutine(FadeOutInstruction());
            SceneManager.LoadScene(sceneName);
        }

    }
}