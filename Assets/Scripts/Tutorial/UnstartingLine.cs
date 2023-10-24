using UnityEngine;

public class UnstartingLine : MonoBehaviour
{
    [SerializeField] Timer timer;

    void OnTriggerEnter2D(Collider2D collider)
    {
        timer.StopTimer(false);
    }
}
