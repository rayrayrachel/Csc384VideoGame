using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private static PlayerAudioController _instance;
    public static PlayerAudioController Instance { get { return _instance; } }

    private AudioSource audioSource;

    void Awake() 
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
