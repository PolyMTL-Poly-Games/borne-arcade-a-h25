using UnityEngine;
using UnityEngine.Audio;

public class AudioManagerController : MonoBehaviour
{
    private static AudioManagerController instance;
    private AudioResource startTrack;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            startTrack = GetComponent<AudioSource>().resource;
            DontDestroyOnLoad(gameObject); // Keep music playing on scene reload
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate music managers
        }
    }

    public void ChangeTrack(AudioResource newTrack)
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.resource != newTrack)
        {
            audioSource.Stop();
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
        GetComponent<AudioSource>().PlayOneShot(sound);
    }
}
