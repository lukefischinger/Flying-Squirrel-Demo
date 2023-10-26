using System.Collections;
using Cinemachine;
using UnityEngine;
using Yarn.Unity;

public class Trainer : MonoBehaviour
{

    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] Timer timer;
    [SerializeField] GameObject startingLine, unstartingLine, startingLineBlock, player, quickRestart;
    [SerializeField] FinishLine finishLine;
    [SerializeField] CinemachineVirtualCamera followCam, fadeOutCam;
    [SerializeField] CinemachineBrain brainCam;
    [SerializeField] PlayerInput input;
    [SerializeField] BackgroundManager backgroundManager;


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
        if (canInteract && input.isInteractTriggered && !dialogueRunning)
        {
            StartDialogue();
        }

        dialogueRunning = dialogueRunner.IsDialogueRunning;

        FlipSprite();
        UpdateQuickRestart();

    }

    void UpdateQuickRestart()
    {
        if(!timer.IsVisible)
            return;

        if (input.isCancelTriggered)
        {
            quickRestart.SetActive(!quickRestart.activeInHierarchy);
            Time.timeScale = quickRestart.activeInHierarchy ? 0f : 1f;
        }

        if (quickRestart.activeInHierarchy)
        {
            if (input.isInteractTriggered)
            {
                Time.timeScale = 1f;
                StopTimer(isTracked: false);
                quickRestart.SetActive(false);

            }
        }
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
    public void StopTimer(bool isTracked = true)
    {
        timer.StopTimer(isTracked);
        if (isTracked)
        {
            dialogueRunner.VariableStorage.SetValue("$best_time", timer.GetBestTime());
            dialogueRunner.VariableStorage.SetValue("$last_time", timer.GetLastTime());
        }

        StartCoroutine(FadeToStart(isTracked));
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




    IEnumerator FadeToStart(bool finished = true)
    {

        float waitTime = finished ? 1f : 0.25f;


        if(finished)
            yield return new WaitForSeconds(waitTime);

        followCam.Priority = 0;
        fadeOutCam.Priority = 1;

        yield return new WaitForSeconds(waitTime);

        Vector3 posDelta = resetLocation - player.transform.position;
        player.transform.position = resetLocation;
        player.GetComponent<PlayerStateController>().myPlayerState = PlayerStateController.PlayerState.Idling;
        player.GetComponent<PlayerPhysics>().directionFacing = 1;
        player.GetComponent<PlayerPhysics>().myRigidbody.velocity = Vector3.zero;


        backgroundManager.ResetBackground();

        followCam.OnTargetObjectWarped(player.transform, posDelta);
        fadeOutCam.OnTargetObjectWarped(player.transform, posDelta);
        yield return new WaitForSeconds(waitTime);

        followCam.Priority = 1;
        fadeOutCam.Priority = 0;

        if (dialogueRunner.IsDialogueRunning)
            dialogueRunner.Stop();
        if (finished)
            dialogueRunner.StartDialogue(finishDialogue);
    }




}
