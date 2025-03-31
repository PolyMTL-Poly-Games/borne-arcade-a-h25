using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerController : MonoBehaviour
{
    public AudioSource audioSource;
    private static AudioManagerController instance;
    private AudioResource startTrack;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject); // Keep music playing on scene reload
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate music managers
        }
    }

    void Start()
    {
        startTrack = audioSource.resource;
    }

    public void ChangeTrack(AudioResource newTrack)
    {
        if (audioSource.resource != newTrack)
        {
            audioSource.resource = newTrack;
            audioSource.Play();
        }
    }

    public void ResetTrack()
    {
        ChangeTrack(startTrack);
    }

    public void playSound(AudioClip sound)
    {
        float volume = 0.75f;
        audioSource.PlayOneShot(sound, volume);
    }
}
