using UnityEngine;

public class PlayerInput : MonoBehaviour {
 
    // movement variables
    public Vector2 moveInput = Vector2.zero;    
    PlayerControls myControls;
    public bool isPressingGrab => myControls.Player.Climb.ReadValue<float>() > Mathf.Epsilon;
    public bool isPressingGlide => myControls.Player.Glide.ReadValue<float>() > Mathf.Epsilon;
    public bool isJumpTriggered => myControls.Player.Jump.triggered;
    public bool isMoveXTriggered => myControls.Player.Move.triggered && myControls.Player.Move.ReadValue<Vector2>().x != 0;

    void Awake() {
        myControls = new PlayerControls();
    }

    private void OnEnable() => myControls.Enable();
    private void OnDisable() => myControls.Disable();

    void Update() {
        SetInputs();
    }

    void SetInputs() {
        moveInput = myControls.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.1f)
            moveInput.x = Mathf.Sign(moveInput.x);
        else moveInput.x = 0;

        if (Mathf.Abs(moveInput.y) > 0.6f)
            moveInput.y = Mathf.Sign(moveInput.y) * (Mathf.Abs(moveInput.y) - 0.6f) / (1 - 0.6f);
        else
            moveInput.y = 0;
    }



   


}
