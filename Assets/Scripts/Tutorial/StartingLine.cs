using UnityEngine;
using Yarn.Unity;

public class StartingLine : MonoBehaviour
{

    [SerializeField] Trainer trainer;
    [SerializeField] DialogueRunner dialogueRunner;

    AudioSource audioSource;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        trainer.StartTimer();
        audioSource.Play();
        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
    }




}
