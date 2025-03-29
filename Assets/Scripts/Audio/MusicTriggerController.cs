using UnityEngine;
using UnityEngine.Audio;

public class MusicTriggerController : MonoBehaviour
{
    public AudioResource newTrack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManagerController audioManagerController = transform.parent.gameObject.GetComponent<AudioManagerController>();
            if (audioManagerController != null)
            {
                audioManagerController.ChangeTrack(newTrack);
            }
        }
    }
}
