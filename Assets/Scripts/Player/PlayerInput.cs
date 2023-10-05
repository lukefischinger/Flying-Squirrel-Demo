using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour {
    [SerializeField] public RunInformation runInformation;

    // movement variables
    public Vector2 moveInput = Vector2.zero;
    public bool isPressingGlide = false;
    public bool isPressingGrab = false;
    public bool isGrounded = false;
    public bool isGroundedHead = false;
    public bool isTouchingTrees = false;
    public int directionMoving = 1;
    public int directionFacing = 1;
    private Vector2 savedVelocity = Vector2.zero;
    private const float savedVelocityTime = 0.5f;
    private float savedVelocityTimeRemaining = 0f;

    // shooting variables
    public bool isPressingFire = false;
    public Vector2 aimInput = Vector2.right;

    public Rigidbody2D myRigidbody;
    public CapsuleCollider2D myBodyCollider;
    public CircleCollider2D myFeetCollider;
    public CircleCollider2D myHeadCollider;
    public PlayerControls myControls;

    float forceX;

    void Awake() {
        myRigidbody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<CircleCollider2D>();
        myHeadCollider = GameObject.Find("Head").GetComponent<CircleCollider2D>();

        myRigidbody.gravityScale = runInformation.baseGravity;
        myControls = new PlayerControls();

        runInformation.Reset();
    }

    private void Start() {
        runInformation.runInProgress = true;
    }

    private void OnEnable() => myControls.Enable();
    private void OnDisable() => myControls.Disable();

    void Update() {
        SetInputs();
        SetLayerTouchingVariables();
    }

    private void LateUpdate() {
        TrackVelocity();
    }

    // saves player velocity if the player very briefly clips the ground or ceiling and doesn't change direction
    // velocity is restored once the player stops making contact with the ground/ceiling
    void TrackVelocity() {
        if (savedVelocityTimeRemaining <= 0) {
            savedVelocity = myRigidbody.velocity;
            return;
        }
        else {
            savedVelocityTimeRemaining -= Time.deltaTime;
        }

        if (isGrounded || Mathf.Sign(savedVelocity.x) != directionFacing || isGroundedHead) {
            savedVelocityTimeRemaining = -1f;
        }
    }

    void SetInputs() {
        // movement variables
        moveInput = myControls.Player.Move.ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.1f)
            moveInput.x = Mathf.Sign(moveInput.x);
        else moveInput.x = 0;

        if (Mathf.Abs(moveInput.y) > 0.6f)
            moveInput.y = Mathf.Sign(moveInput.y) * (Mathf.Abs(moveInput.y) - 0.6f) / (1 - 0.6f);
        else
            moveInput.y = 0;

        // on/off variables
        isPressingGrab = (myControls.Player.GrabWall.ReadValue<float>() > Mathf.Epsilon);
        isPressingGlide = (myControls.Player.Glide.ReadValue<float>() > Mathf.Epsilon);
        isPressingFire = myControls.Player.Fire.ReadValue<float>() > Mathf.Epsilon;

        aimInput = myControls.Player.Look.ReadValue<Vector2>();
    }

    void SetLayerTouchingVariables() {
        isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        isGroundedHead = myHeadCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        isTouchingTrees = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
    }

    // player x movement is set by applying force in the direction of player input
    public void SetMovementX() {
        float absX = Mathf.Abs(myRigidbody.velocity.x);
        if (moveInput.x == 0 && absX > Mathf.Epsilon)  // if no x input and player has nonzero velocity, apply a force in the opposite direction of velocity to reduce speed
            forceX = -10 * directionMoving *
                     absX > 1 ?
                        absX :                         // for velocity > 1, force ∝ |velocity|
                        Mathf.Pow(absX, 0.25f);        // for velocity <= 1, force ∝ |velocity| ^ 0.25
        else if (absX >= runInformation.runSpeed)      // apply no force if already at max speed
            forceX = 0;
        else forceX = 3 * runInformation.acceleration * moveInput.x;

        myRigidbody.AddForce(Vector2.right * forceX);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        savedVelocityTimeRemaining = savedVelocityTime;
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (savedVelocityTimeRemaining > 0) {
            myRigidbody.velocity = new Vector2(savedVelocity.x, myRigidbody.velocity.y);
        }
    }
}
