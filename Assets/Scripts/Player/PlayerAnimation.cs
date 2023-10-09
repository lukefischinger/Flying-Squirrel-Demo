using UnityEngine;
using static PlayerStateController;

public class PlayerAnimation : MonoBehaviour {
    Animator myAnimator;
    PlayerStateController myStateController;
    PlayerPhysics physics;
    PlayerInput input;
    Transform animationTransform;
    const float pixelsPerUnit = 32;

    private void Awake() {
        myAnimator = GetComponentInChildren<Animator>();
        animationTransform = myAnimator.transform;
        myStateController = GetComponent<PlayerStateController>();
        physics = GetComponent<PlayerPhysics>();
        input = GetComponent<PlayerInput>();
    }

    private void Update() {
        SetAnimationParameters();
        FlipSprite();
        SnapAnimationTransform();
    }

    void SetAnimationParameters() {
        myAnimator.SetBool("isClimbing", myStateController.myPlayerState == PlayerState.Climbing);
        myAnimator.SetBool("isRunning", myStateController.myPlayerState == PlayerState.Running);
        myAnimator.SetBool("isGliding", myStateController.myPlayerState == PlayerState.Gliding);
        myAnimator.SetBool("isFalling", myStateController.myPlayerState == PlayerState.Falling);
        myAnimator.SetBool("isJumping", myStateController.myPlayerState == PlayerState.Jumping);

        if (myStateController.myPlayerState == PlayerState.Climbing) {
            SetClimbingSpeedParameter();
        }
        else if (myStateController.myPlayerState == PlayerState.Running) {
            SetRunningSpeedParameter();
        }
        else if (myStateController.myPlayerState == PlayerState.Gliding) {
            SetGlidingAngleParameter();
        }
    }

    void SetGlidingAngleParameter() {
        Vector2 adjustedVelocity = new Vector2(
               physics.directionFacing * physics.directionMoving * physics.myRigidbody.velocity.x,
               physics.myRigidbody.velocity.y
               );

        myAnimator.SetFloat("glidingAngle", physics.directionFacing * Vector2.SignedAngle(Vector2.right * physics.directionFacing, adjustedVelocity));
    }

    void SetClimbingSpeedParameter() {
        myAnimator.SetFloat("climbingSpeed", Mathf.Sign(physics.myRigidbody.velocity.y) * Mathf.Max(0.2f, Mathf.Min(1, Mathf.Abs(physics.myRigidbody.velocity.y))));
        if (physics.myRigidbody.velocity.y == 0) {
            myAnimator.SetFloat("climbingSpeed", 0);
        }
    }

    void SetRunningSpeedParameter() {
        myAnimator.SetFloat("runningSpeed", Mathf.Max(0.2f, Mathf.Abs(input.moveInput.x)));

    }

    // determines the correct direction for the sprite to be facing
    // based on a combination of rigidbody velocity, and movement inputs
    void FlipSprite() {
        transform.localScale = new Vector2(physics.directionFacing * Mathf.Abs(transform.localScale.x), transform.localScale.y);
    }

    // snap player animation to the nearest pixel
    void SnapAnimationTransform() {
        float x = Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit,
              y = Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit;
        animationTransform.position = new Vector2(x, y);
    }
}
