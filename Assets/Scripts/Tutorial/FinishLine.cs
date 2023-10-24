using UnityEngine;

public class FinishLine : MonoBehaviour
{

    [SerializeField] Trainer trainer;
    AudioSource audioSource;

    bool crossed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!crossed)
        {
            trainer.StopTimer();
            crossed = true;
            audioSource.Play();
        }
    }

    public void Reset()
    {
        crossed = false;
    }

}
