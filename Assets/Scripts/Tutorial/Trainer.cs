using System.Collections;
using Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class Trainer : MonoBehaviour
{

    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] Timer timer;
    [SerializeField] GameObject startingLine, unstartingLine, startingLineBlock;
    [SerializeField] GameObject player;
    [SerializeField] FinishLine finishLine;
    [SerializeField] CinemachineVirtualCamera followCam, fadeOutCam;
    [SerializeField] CinemachineBrain brainCam;
    [SerializeField] PlayerInput input;

    GameObject prompt;
    Transform myTransform, playerTransform, promptTransform;

    const string finishDialogue = "FinishLine";
    Vector3 resetLocation;

    bool dialogueRunning = false;

    bool canInteract = false;

    void Awake()
    {
        prompt = transform.GetChild(0).gameObject;
        prompt.SetActive(false);
        promptTransform = prompt.transform;
        myTransform = transform;
        playerTransform = player.transform;
        resetLocation = playerTransform.position;

    }

    void Update()
    {
        if (canInteract && input.myControls.Player.Interact.triggered && !dialogueRunning)
        {
            StartDialogue();
        }

        dialogueRunning = dialogueRunner.IsDialogueRunning;

        FlipSprite();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        canInteract = true;
        prompt.SetActive(true);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canInteract = false;
        prompt.SetActive(false);
    }

    void FlipSprite()
    {
        float direction = -Mathf.Sign(playerTransform.position.x - myTransform.position.x);
        myTransform.localScale = new Vector3(direction, 1, 1);
        promptTransform.localScale = new Vector3(direction, 1, 1);

    }

    public void StartDialogue()
    {
        if (!dialogueRunning)
            dialogueRunner.StartDialogue("Concierge");
    }


    public void StartTimer()
    {
        timer.gameObject.SetActive(true);
        timer.StartTimer();
        finishLine.Reset();
    }
    public void StopTimer()
    {
        timer.StopTimer(true);
        dialogueRunner.VariableStorage.SetValue("$best_time", timer.GetBestTime());
        dialogueRunner.VariableStorage.SetValue("$last_time", timer.GetLastTime());

        StartCoroutine(FadeToStart());
    }

    [YarnCommand("prime_timer")]
    public void PrimeTimer()
    {
        startingLine.SetActive(true);
        unstartingLine.SetActive(true);
        startingLineBlock.SetActive(false);
    }

    [YarnCommand("unprime_timer")]
    public void UnprimeTimer()
    {
        startingLine.SetActive(false);
        unstartingLine.SetActive(false);
        startingLineBlock.SetActive(true);
    }




    IEnumerator FadeToStart()
    {

        yield return new WaitForSeconds(1f);

        followCam.Priority = 0;
        fadeOutCam.Priority = 1;

        yield return new WaitForSeconds(1f);

        Vector3 posDelta = resetLocation - player.transform.position;
        player.transform.position = resetLocation;
        player.transform.localScale = Vector3.one;
        followCam.OnTargetObjectWarped(player.transform, posDelta);
        fadeOutCam.OnTargetObjectWarped(player.transform, posDelta);
        yield return new WaitForSeconds(1f);

        followCam.Priority = 1;
        fadeOutCam.Priority = 0;

        yield return new WaitForSeconds(brainCam.m_DefaultBlend.BlendTime);

        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        dialogueRunner.StartDialogue(finishDialogue);
    }

}
