using UnityEngine;

public class PlayerGliding : MonoBehaviour {

    PlayerInput input;
    RunInformation runInfo;

    float absX, absY, absV, deltaX, deltaY, clampedY;

    void Awake() {
        input = GetComponent<PlayerInput>();
        runInfo = input.runInformation;
    }

    // when gliding, the player can perform a few different actions:
    // (1) press down to increase downward velocity
    // (2) press left/right to convert any downward velocity into sideways velocity
    // (3) press up to convert any sideways velocity into a small amount of upwards velocity
    public void Glide() {
        input.myRigidbody.gravityScale = runInfo.glideGravity;

        absX = Mathf.Abs(input.myRigidbody.velocity.x);
        absY = Mathf.Abs(input.myRigidbody.velocity.y);
        clampedY = Mathf.Clamp(absY, 2.5f, 16f);
        absV = input.myRigidbody.velocity.magnitude;

        // if moving upward, add force to slow x movement
        // deltaY is equal to a positive, constant multiple of deltaX
        if (input.moveInput.y > Mathf.Epsilon && absX > Mathf.Epsilon && !input.isGroundedHead) {
            deltaX = -input.directionMoving * Mathf.Min(absX, input.moveInput.y * runInfo.acceleration);
            deltaY = Mathf.Clamp(0.2f * absV, 0.5f, 3f) * Mathf.Abs(deltaX);
            if (input.myRigidbody.velocity.y < 0) {
                deltaX = input.directionMoving * deltaY / Mathf.Clamp(0.2f * absV, 0.5f, 3f);
            }
        }
        // pressing down => increase x & y velocity in the direction facing
        else if (input.moveInput.y < -Mathf.Epsilon) {
            deltaX = 2f * input.directionFacing * runInfo.acceleration;
            deltaY = -1.5f * Mathf.Abs(deltaX);
        }
        // not pressing down or up, or head is hitting ceiling
        else { 
            deltaX = input.directionFacing * runInfo.acceleration;
            deltaY = -input.myRigidbody.velocity.y + 6 - 3 * clampedY / 7 - 21 / clampedY;
        }

        // apply the force calculated above
        input.myRigidbody.AddForce(new Vector2(deltaX, deltaY));


    }
}
