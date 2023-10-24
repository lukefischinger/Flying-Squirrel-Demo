using System;
using System.Linq;
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

    static readonly string[] testNames = new string[] {
        "Running",
        "Climbing",
        "Jumping",
        "DoubleJumping",
        "Gliding",
        "Diving",
        "PullingUp",
        "Final"
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

    
}
