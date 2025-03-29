using UnityEngine;
using UnityEngine.Audio;

public class MusicTriggerController : MonoBehaviour
{
    public AudioResource newTrack;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MusicManagerController musicManagerController = transform.parent.gameObject.GetComponent<MusicManagerController>();
            if (musicManagerController != null)
            {
                musicManagerController.ChangeTrack(newTrack);
            }
        }
    }
}
