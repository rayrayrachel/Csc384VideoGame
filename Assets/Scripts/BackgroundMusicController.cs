using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    private AudioSource audioSource;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;

        audioSource.mute = !isMusicOn;

        if (isMusicOn)
        {
            PlayMusic();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
