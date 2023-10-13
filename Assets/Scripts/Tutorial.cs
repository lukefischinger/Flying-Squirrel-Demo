using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using static PlayerStateController;

public class Tutorial : MonoBehaviour
{

    [SerializeField] DialogueRunner dialogueRunner;
    [SerializeField] GameObject continueButton;

    private void Awake()
    {
        input = GameObject.Find("Player").GetComponent<PlayerInput>();
        stateController = input.GetComponent<PlayerStateController>();
        physics = input.GetComponent<PlayerPhysics>();
    }

    PlayerInput input;
    PlayerStateController stateController;
    PlayerPhysics physics;

    const float solvedDelay = 2f;



    static readonly string[] testNames = new string[] {
        "Running",
        "Climbing",
        "Jumping",
        "DoubleJumping",
        "Gliding",
        "Diving",
        "PullingUp",
    };

    public bool[] passed = new bool[9];

    bool IsPassedThisFrame(string title)
    {
        return title switch
        {
            "Running" => input.moveInput.x != 0,
            "Climbing" => stateController.myPlayerState == PlayerState.Climbing,
            "Jumping" => input.isJumpTriggered,
            "DoubleJumping" => input.isJumpTriggered && !physics.isGrounded,
            "Gliding" => stateController.myPlayerState == PlayerState.Gliding,
            "Diving" => stateController.myPlayerState == PlayerState.Gliding && input.moveInput.y < 0 && physics.myRigidbody.velocity.y < -15f,
            "PullingUp" => stateController.myPlayerState == PlayerState.Gliding && input.moveInput.y > 0 && physics.myRigidbody.velocity.y > 0f,
            _ => false
        };
    }

    bool IsPassed(string title)
    {
        return passed[Array.IndexOf(testNames, title)];
    }

    void SetPassed(string title, bool value = true)
    {
        passed[Array.IndexOf(testNames, title)] = value;
    }

    private void Update()
    {
        if (IsPassedThisFrame(dialogueRunner.CurrentNodeName))
            dialogueRunner.VariableStorage.SetValue("$step_complete", true);

        if (testNames.Contains(dialogueRunner.CurrentNodeName))
        {
            bool stepComplete;
            dialogueRunner.VariableStorage.TryGetValue("$step_complete", out stepComplete);
            continueButton.SetActive(stepComplete);
        }
        else ShowContinueButton();

    }



    [YarnFunction("get_next_node")]
    public static string GetNode(int curr)
    {
        if (curr >= testNames.Length)
            return "none";
        else return testNames[curr];
    }

    [YarnFunction("get_node_number")]
    public static int GetNodeNumber(string curr)
    {
        return Array.IndexOf(testNames, curr);
    }


    [YarnCommand("hide_continue_button")]
    public void ShowContinueButton() {
        continueButton.SetActive(true);
    }

    [YarnCommand("show_continue_button")]
    public void HideContinueButton() {
        continueButton.SetActive(false);
    }

    /*void UpdateDialogue(string next = "default")
        {
            dialogueText.color = Color.white;
            dialogueText.fontStyle = FontStyles.Normal;
            solvedTimer = solvedDelay;

            if (next == "default")
                finished = Array.FindIndex(passed, 0, (bool val) => val == false);
            else finished = Array.IndexOf(testNames, next);

            Debug.Log(finished + ": " + currentTestName);

            currentTestName = testNames[finished];

            if (dialogueRunner.IsDialogueRunning)
                dialogueRunner.Stop();
            dialogueRunner.StartDialogue(testNames[finished]);
        }

        void ShowPassed(bool delay = true, string next = "default")
        {
            if (!delay || !dialogueRunner.IsDialogueRunning || solvedTimer < 0)
                UpdateDialogue(next);
            else
            {
                dialogueText.color = Color.green;
                solvedTimer -= Time.deltaTime;
            }
        }*/
}
