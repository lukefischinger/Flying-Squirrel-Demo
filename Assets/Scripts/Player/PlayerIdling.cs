using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdling : MonoBehaviour
{
    PlayerInput input;
    RunInformation runInfo;
    PlayerJumping jump;

    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        jump = GetComponent<PlayerJumping>();
        runInfo = input.runInformation;
    }


    public void Idle()
    {
        input.SetMovementX();
        jump.ResetJumpCount();
        input.myRigidbody.gravityScale = runInfo.baseGravity;
    }

}
