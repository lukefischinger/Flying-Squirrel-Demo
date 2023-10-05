using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling : MonoBehaviour
{
    PlayerInput input;
    RunInformation runInfo;

    // Start is called before the first frame update
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        runInfo = input.runInformation;
    }

    public void Fall()
    {
        input.myRigidbody.gravityScale = runInfo.baseGravity;
        input.SetMovementX();


    }

}
