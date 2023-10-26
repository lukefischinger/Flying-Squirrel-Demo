using UnityEngine;

public class FinishLine : MonoBehaviour
{

    [SerializeField] Trainer trainer;
    [SerializeField] GameObject startingLine;

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
            trainer.PrimeTimer();
        }
    }

    public void Reset()
    {
        crossed = false;
    }

}
