using UnityEngine;

public class PlayerPhysics : MonoBehaviour {

    // constants
    private const float baseGravity = 4f;
    private const float acceleration = 14f;
    private const float jumpSpeed = 1300f;
    private const float savedVelocityTime = 0.5f;
    private const float fallingThreshold = 0.0001f;
    private const float runSpeed = 7;
    private const float glideGravity = 0.8f;
    private const float treeSnappingSpeed = 20f;
    private const float climbSpeed = 10f;
    private const int numJumpsAllowed = 2;

    PlayerInput input;
    PlayerStateController stateController;
    CapsuleCollider2D myBodyCollider;
    CircleCollider2D myFeetCollider;
    CircleCollider2D myHeadCollider;
    public Rigidbody2D myRigidbody { get; private set; }
    private Vector2 savedVelocity = Vector2.zero;
    private float savedVelocityTimeRemaining = 0f;
    public int directionMoving { get; private set; }
    public int directionFacing { get; private set; }

    public bool isGrounded => myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    public bool isFalling => myRigidbody.velocity.y < fallingThreshold;
    public bool isGroundedHead => myHeadCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
    public bool isTouchingTrees => myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Trees"));

    // X movement
    float forceX;

    // gliding variables
    float absX, absY, absV, deltaX, deltaY, clampedY;

    // jumping variables
    public int jumpCount = 0;
    public float jumpTime = 0.25f;
    public float jumpTimeRemaining = 0f;
    private int savedJumpCount;
    public int SavedJumpCount {
        get { return savedJumpCount; }
        set { savedJumpCount = Mathf.Clamp(value, 0, numJumpsAllowed - jumpCount); }
    }

    // climbing variables
    float climbingPosition = Mathf.Infinity,
          climbingDistance,
          yMovement,
          climbingSnapSpeed;


    void Awake() {
        input = GetComponent<PlayerInput>();
        stateController = GetComponent<PlayerStateController>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<CircleCollider2D>();
        myHeadCollider = GetComponentInChildren<CircleCollider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();

        jumpCount = savedJumpCount = 0;
        directionMoving = directionFacing = 1;
    }


    private void Update() {
        SetDirections();
    }

    private void LateUpdate() {
        TrackVelocity();
    }

    private void SetDirections() {
        if (myRigidbody.velocity.x != 0) {
            directionMoving = (int)Mathf.Sign(myRigidbody.velocity.x);
        }

        if (input.moveInput.x != 0) {
            directionFacing = (int)Mathf.Sign(input.moveInput.x);
        }
    }

    public void Fall() {
        myRigidbody.gravityScale = baseGravity;
        SetMovementX();
    }

    public void Idle() {
        SetMovementX();
        ResetJumpCount();
        myRigidbody.gravityScale = baseGravity;
    }

    public void Run() {
        ResetJumpCount();
        SetMovementX();
        myRigidbody.gravityScale = baseGravity;
    }

    // when gliding, the player can perform a few different actions:
    // (1) press down to increase downward velocity
    // (2) press left/right to convert any downward velocity into sideways velocity
    // (3) press up to convert any sideways velocity into a small amount of upwards velocity
    public void Glide() {
        myRigidbody.gravityScale = glideGravity;

        absX = Mathf.Abs(myRigidbody.velocity.x);
        absY = Mathf.Abs(myRigidbody.velocity.y);
        clampedY = Mathf.Clamp(absY, 2.5f, 16f);
        absV = myRigidbody.velocity.magnitude;

        // if moving upward, add force to slow x movement
        // deltaY is equal to a positive, constant multiple of deltaX
        if (input.moveInput.y > Mathf.Epsilon && absX > Mathf.Epsilon && !isGroundedHead) {
            deltaX = -directionMoving * Mathf.Min(absX, input.moveInput.y * acceleration);
            deltaY = Mathf.Clamp(0.2f * absV, 0.5f, 3f) * Mathf.Abs(deltaX);
            if (myRigidbody.velocity.y < 0) {
                deltaX = directionMoving * deltaY / Mathf.Clamp(0.2f * absV, 0.5f, 3f);
            }
        }
        // pressing down => increase x & y velocity in the direction facing
        else if (input.moveInput.y < -Mathf.Epsilon) {
            deltaX = 2f * directionFacing * acceleration;
            deltaY = -1.5f * Mathf.Abs(deltaX);
        }
        // not pressing down or up, or head is hitting ceiling
        else {
            deltaX = directionFacing * acceleration;
            deltaY = -myRigidbody.velocity.y + 6 - 3 * clampedY / 7 - 21 / clampedY;
        }

        // apply the force calculated above
        myRigidbody.AddForce(new Vector2(deltaX, deltaY));


    }


    public void Jump() {
        myRigidbody.gravityScale = baseGravity;
        SetMovementX();

        jumpTimeRemaining -= Time.deltaTime;
    }

    public void StartJump() {
        if (jumpCount < numJumpsAllowed) {
            jumpCount++;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, 0);
            myRigidbody.AddForce(new Vector2(0, jumpSpeed));
            jumpTimeRemaining = jumpTime;

            stateController.myPlayerState = PlayerStateController.PlayerState.Jumping;

        }
    }

    public void ResetJumpCount() {
        jumpCount = 0;
    }

    public void Climb() {
        RaycastHit2D closestTree = GetClosestTreeCollision(myBodyCollider.size.x);
        if (closestTree.collider == null) return;

        climbingPosition = GetNearestClimbingX(closestTree.point);
        climbingDistance = climbingPosition - myRigidbody.position.x;

        if (climbingDistance < 0.05)
            climbingSnapSpeed = treeSnappingSpeed * climbingDistance;
        else climbingSnapSpeed = Mathf.Sign(climbingDistance) * treeSnappingSpeed * Mathf.Pow(Mathf.Abs(climbingDistance), 0.1f);

        yMovement = climbingDistance < 0.01 ? input.moveInput.y * climbSpeed : 0;
        if (AtTopOfTree())
            yMovement = Mathf.Min(yMovement, 0);

        myRigidbody.velocity = new Vector2(climbingSnapSpeed, yMovement);
        myRigidbody.gravityScale = 0;
        ResetJumpCount();
    }

    // saves player velocity if the player very briefly clips the ground or ceiling and doesn't change direction
    // velocity is restored if the player exits the collision quickly
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

    // player x movement is set by applying force in the direction of player input
    public void SetMovementX() {
        float absX = Mathf.Abs(myRigidbody.velocity.x);
        if (input.moveInput.x == 0 && absX > Mathf.Epsilon)  // if no x input and player has nonzero velocity, apply a force in the opposite direction of velocity to reduce speed
            forceX = -10 * directionMoving *
                     absX > 1 ?
                        absX :                         // for velocity > 1, force ∝ |velocity|
                        Mathf.Pow(absX, 0.25f);        // for velocity <= 1, force ∝ |velocity| ^ 0.25
        else if (absX >= runSpeed)      // apply no force if already at max speed
            forceX = 0;
        else forceX = 3 * acceleration * input.moveInput.x;

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

    RaycastHit2D GetClosestTreeCollision(float radius) {
        DrawSphere((Vector2)transform.position - radius * Vector2.right * directionFacing, radius);
        DrawSphere((Vector2)transform.position + radius * Vector2.right * directionFacing, radius);

        RaycastHit2D hit = Physics2D.CircleCast(
            (Vector2)transform.position + radius * Vector2.right * directionFacing,
            radius,
            -directionFacing * Vector2.right,
            2 * radius,
            LayerMask.GetMask("Trees")
        );

        return hit;
    }

    // debugging function
    void DrawSphere(Vector2 pos, float radius) {
        for (int i = 0; i < 36; i++) {
            Color color = Color.magenta;
            color.a = 0.2f;
            Debug.DrawRay(pos, Quaternion.Euler(Vector3.forward * i * 10) * Vector2.right * radius, color, 0.25f);
        }
    }



    float GetNearestClimbingX(Vector2 pos) {
        return Mathf.Floor(pos.x) + 0.5f;
    }


    bool AtTopOfTree() {
        Vector2 startingPos = (Vector2)transform.position + 0.5f * myBodyCollider.size.x * Vector2.left + myBodyCollider.size.y * Vector2.up;
        float distance = myBodyCollider.size.x;
        DrawSphere(startingPos, 0.5f * myBodyCollider.size.y);
        DrawSphere(startingPos + Vector2.right * distance, 0.5f * myBodyCollider.size.y);

        RaycastHit2D hit = Physics2D.CircleCast(startingPos, 0.5f * myBodyCollider.size.y, Vector2.right, distance, LayerMask.GetMask("Trees"));
        return hit.Equals(null) || hit.point == Vector2.zero;
    }
}
