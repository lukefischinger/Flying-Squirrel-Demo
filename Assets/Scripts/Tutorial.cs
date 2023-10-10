using TMPro;
using UnityEngine;
using static PlayerStateController;

public class Tutorial : MonoBehaviour {

    string[] instructions = new string[] {
        "Press a or d to run",
        "Press w or s in front of a tree to climb it",
        "Press space to jump",
        "Press space while in the air to jump again",
        "Hold space while in the air to glide",
        "Hold s while gliding to dive quickly",
        "Hold w while diving to pull up",
        "Almost! Try diving more quickly, then holding w to pull up",
        "Wow, you're an old pro! Go test out those skills"
    };

    PlayerInput input;
    TextMeshProUGUI text;
    PlayerStateController stateController;
    PlayerPhysics physics;

    int finished = 0;
    int Finished {
        get {
            return finished;
        }
        set {
            textTimeRemaining = textTime;
            finished = value;
        }
    }

    float textTime = 2f,
          textTimeRemaining,
          glidingTimer = 3f;

    private void Awake() {
        input = GameObject.Find("Player").GetComponent<PlayerInput>();
        stateController = input.GetComponent<PlayerStateController>();
        physics = input.GetComponent<PlayerPhysics>();

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = instructions[Finished];
    }


    private void Update() {
        if (textTimeRemaining > 0) {
            textTimeRemaining -= Time.deltaTime;
            return;
        }

        switch (Finished) {
            case 0:
                if (input.moveInput.x != 0)
                    Finished++;

                break;

            case 1:
                if (stateController.myPlayerState == PlayerState.Climbing)
                    Finished++;

                break;

            case 2:
                if (input.isJumpTriggered)
                    Finished++;

                break;

            case 3:
                if (input.isJumpTriggered && !physics.isGrounded)
                    Finished++;
                break;

            case 4:
                if (stateController.myPlayerState == PlayerState.Gliding) {
                    if (glidingTimer < 0)
                        Finished++;
                    else glidingTimer -= Time.deltaTime;
                }
                break;

            case 5:
                if (stateController.myPlayerState == PlayerState.Gliding && input.moveInput.y < 0 && physics.myRigidbody.velocity.y < -15f)
                    Finished++;
                break;

            case 6:
                if (stateController.myPlayerState == PlayerState.Gliding && input.moveInput.y > 0) {
                    if (physics.myRigidbody.velocity.y > 0 && physics.myRigidbody.velocity.x < 1f)
                        Finished++;
                    else if (physics.myRigidbody.velocity.y > 6)
                        Finished += 2;
                }

                break;
            case 7:
                if (stateController.myPlayerState == PlayerState.Gliding && input.moveInput.y > 0 && physics.myRigidbody.velocity.y > 6)
                    Finished++;

                break;
            case 8:
                Destroy(gameObject);
                break;



            default:
                break;
        }

        text.text = instructions[finished];
    }

}
