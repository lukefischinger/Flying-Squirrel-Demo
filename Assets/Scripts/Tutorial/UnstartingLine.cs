using UnityEngine;

public class UnstartingLine : MonoBehaviour
{
    [SerializeField] Timer timer;
    [SerializeField] Trainer trainer;

    void OnTriggerEnter2D(Collider2D collider)
    {
        timer.StopTimer(false);
        
        trainer.PrimeTimer();
        
    }
}
