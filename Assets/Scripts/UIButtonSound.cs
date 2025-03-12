using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();

        foreach (Button btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(hoverEntry);

            btn.onClick.AddListener(PlayClickSound);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayHoverSound()
    {
        if (audioSource && hoverSound)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    void PlayClickSound()
    {
        if (audioSource && clickSound)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
