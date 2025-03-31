using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerController : MonoBehaviour
{
    public static AudioManagerController instance;
    public AudioSource audioSource;
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
        startTrack = transform.GetChild(0).GetComponent<MusicTriggerController>().newTrack;
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

    public void PlaySound(AudioClip sound)
    {
        float volume = 0.75f;
        audioSource.PlayOneShot(sound, volume);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
