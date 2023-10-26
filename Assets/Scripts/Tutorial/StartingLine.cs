using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class StartingLine : MonoBehaviour
{

    [SerializeField] Trainer trainer;
    [SerializeField] DialogueRunner dialogueRunner;

    AudioSource audioSource;
    bool started = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if(started) {
            if(!audioSource.isPlaying) {
                started = false;
                gameObject.SetActive(false);
            }
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        trainer.StartTimer();
        audioSource.Play();
        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        started = true;
    }







}
