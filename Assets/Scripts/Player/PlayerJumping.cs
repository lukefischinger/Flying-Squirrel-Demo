using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerJumping : MonoBehaviour
{
    PlayerInput input;
    RunInformation runInfo;
    PlayerStateController stateController;

    // jump variables
    public int jumpCount;
    public float jumpTime = 0.25f;
    public float jumpTimeRemaining = 0f;
    public int savedJumpCount = 0;


    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        runInfo = input.runInformation;
        stateController = GetComponent<PlayerStateController>();

        jumpCount = 0;

    }


    private void Update()
    {
        if (input.myControls.Player.Jump.triggered)
        {
            if (jumpTimeRemaining <= 0)
            {
                OnJump();
            }
            else
            {
                savedJumpCount++;
                savedJumpCount = Mathf.Min(savedJumpCount, runInfo.numJumpsAllowed - jumpCount);
            }
        }
        else if (savedJumpCount > 0)
        {
            if (jumpTimeRemaining <= 0)
            {
                OnJump();
                savedJumpCount--;
            }
        }
    }

    public void Jump()
    {
        input.myRigidbody.gravityScale = runInfo.baseGravity;
        input.SetMovementX();

        jumpTimeRemaining -= Time.deltaTime;
    }

    void OnJump()
    {
        if (jumpCount < runInfo.numJumpsAllowed)
        {
            jumpCount++;
            input.myRigidbody.velocity = new Vector2(input.myRigidbody.velocity.x, 0);
            input.myRigidbody.AddForce(new Vector2(0, runInfo.jumpSpeed));
            jumpTimeRemaining = jumpTime;

            stateController.SetStateToJumping();

        }
    }

    public void ResetJumpCount()
    {
        jumpCount = 0;
    }

    public float GetJumpTimeRemaining()
    {
        return jumpTimeRemaining;
    }

}
