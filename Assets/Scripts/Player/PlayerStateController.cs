using UnityEngine;

public class PlayerStateController : MonoBehaviour {

    PlayerInput input;

    // Running & Idling are "Ground" states, falling & gliding are "Air states", and climbing/jumping are in their own categories
    // order of precedence: jumping -> climbing -> ground -> air
    public enum PlayerState { Idling, Running, Gliding, Falling, Jumping, Climbing };
    public PlayerState myPlayerState;

    PlayerPhysics physics;

    float climbingResetTime = 0.25f;
    float climbingResetTimeRemaining = 0f;

    void Awake() {
        input = GetComponent<PlayerInput>();
        myPlayerState = PlayerState.Idling;
        physics = GetComponent<PlayerPhysics>();
    }

    private void Update() {
        Transition();
    }

    private void FixedUpdate() {
        ExecuteStatePhysics();
    }

    void ExecuteStatePhysics() {
        switch (myPlayerState) {
            case PlayerState.Idling: physics.Idle(); break;
            case PlayerState.Running: physics.Run(); break;
            case PlayerState.Jumping: physics.Jump(); break;
            case PlayerState.Falling: physics.Fall(); break;
            case PlayerState.Gliding: physics.Glide(); break;
            case PlayerState.Climbing: physics.Climb(); break;
            default: break;
        }
    }

    void SetGroundState() {
        if (input.moveInput.x != 0)
            myPlayerState = PlayerState.Running;
        else myPlayerState = PlayerState.Idling;
    }

    void SetAirState() {
        if (physics.jumpTimeRemaining > 0 && myPlayerState == PlayerState.Jumping)
            myPlayerState = PlayerState.Jumping;
        else if (input.isPressingGlide)
            myPlayerState = PlayerState.Gliding;
        else myPlayerState = PlayerState.Falling;
    }

    void Transition() {
        if (IsJumping()) {
            myPlayerState = PlayerState.Jumping;
        }
        else if (IsClimbing()) {
            myPlayerState = PlayerState.Climbing;
            climbingResetTimeRemaining = climbingResetTime;
        }
        else if (physics.isGrounded)
            SetGroundState();
        else if(physics.isFalling)
            SetAirState();
    }

    bool IsClimbing() {
        if (climbingResetTimeRemaining > 0)
            climbingResetTimeRemaining -= Time.deltaTime;

        return (myPlayerState != PlayerState.Climbing && input.isPressingGrab && physics.isTouchingTrees && climbingResetTimeRemaining <= 0) ||
               (myPlayerState == PlayerState.Climbing && physics.isTouchingTrees && !input.isMoveXTriggered);
    }

    bool IsJumping() {
        if (physics.canJump && (input.isJumpTriggered || physics.SavedJumpCount > 0)) {
            if (physics.jumpTimeRemaining <= 0) {
                physics.StartJump();
                myPlayerState = PlayerState.Jumping;
                physics.SavedJumpCount--;

            }
            else {
                physics.SavedJumpCount += input.isJumpTriggered ? 1 : 0;
            }
            return true;
        }
        else if (physics.jumpTimeRemaining > 0 && myPlayerState == PlayerState.Jumping) {
            return true;
        }

        return false;
    }

    public void SetStateToJumping() {
        myPlayerState = PlayerState.Jumping;
    }

}
