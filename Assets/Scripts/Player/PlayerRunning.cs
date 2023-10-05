using UnityEngine;

public class PlayerRunning : MonoBehaviour
{
    PlayerInput input;
    PlayerJumping jump;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        jump = GetComponent<PlayerJumping>();
    }

    public void Run()
    {
        jump.ResetJumpCount();
        input.SetMovementX();
        input.myRigidbody.gravityScale = input.runInformation.baseGravity;
    }
}
