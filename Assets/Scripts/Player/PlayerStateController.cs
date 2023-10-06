using UnityEngine;

public class PlayerStateController : MonoBehaviour {
    Animator myAnimator;
    PlayerInput input;
    RunInformation runInformation;
    Transform animationTransform;

    public enum PlayerState { Idling, Running, Gliding, Falling, Jumping, Climbing };
    public PlayerState myPlayerState;

    PlayerIdling idle;
    PlayerRunning run;
    PlayerClimbing climb;
    PlayerGliding glide;
    PlayerJumping jump;
    PlayerFalling fall;

    float climbingResetTime = 0.35f;
    float climbingResetTimeRemaining = 0f;
    const float pixelsPerUnit = 32;

    // Start is called before the first frame update
    void Awake() {
        myAnimator = GetComponentInChildren<Animator>();
        animationTransform = myAnimator.transform;
        input = GetComponent<PlayerInput>();
        runInformation = input.runInformation;

        myPlayerState = PlayerState.Idling;
        idle = GetComponent<PlayerIdling>();
        run = GetComponent<PlayerRunning>();
        climb = GetComponent<PlayerClimbing>();
        glide = GetComponent<PlayerGliding>();
        jump = GetComponent<PlayerJumping>();
        fall = GetComponent<PlayerFalling>();

        
    }


    private void Update() {
        SetPlayerAnimationVariables();
        FlipSprite();
        SnapAnimationTransform();
    }

    private void FixedUpdate() {
        SetPlayerState();
        climbingResetTimeRemaining -= Time.deltaTime;
    }

    void SetPlayerState() {
        switch (myPlayerState) {
            case PlayerState.Idling:
                idle.Idle();
                IdleTransition();
                break;
            case PlayerState.Running:
                run.Run();
                RunTransition();
                break;
            case PlayerState.Jumping:
                jump.Jump();
                JumpTransition();
                break;
            case PlayerState.Falling:
                fall.Fall();
                FallTransition();
                break;
            case PlayerState.Gliding:
                glide.Glide();
                GlideTransition();
                break;
            case PlayerState.Climbing:
                climb.Climb();
                ClimbTransition();
                break;
            default:
                break;
        }
    }


    void SetPlayerAnimationVariables() {
        myAnimator.SetBool("isClimbing", myPlayerState == PlayerState.Climbing);
        myAnimator.SetBool("isRunning", myPlayerState == PlayerState.Running);
        myAnimator.SetBool("isGliding", myPlayerState == PlayerState.Gliding);
        myAnimator.SetBool("isFalling", myPlayerState == PlayerState.Falling);
        myAnimator.SetBool("isJumping", myPlayerState == PlayerState.Jumping);

        // set climbing and running speeds if currently in one of those states
        if (myPlayerState == PlayerState.Climbing) {
            myAnimator.SetFloat("climbingSpeed", Mathf.Sign(input.myRigidbody.velocity.y) * Mathf.Max(0.2f, Mathf.Min(1, Mathf.Abs(input.myRigidbody.velocity.y))));
            if (input.myRigidbody.velocity.y == 0) {
                myAnimator.SetFloat("climbingSpeed", 0);
            }
        } else if(myPlayerState == PlayerState.Running) {
            myAnimator.SetFloat("runningSpeed", Mathf.Max(0.2f, Mathf.Abs(input.moveInput.x)));
        }
    }

    // determines the correct direction for the sprite to be facing
    // based on a combination of rigidbody velocity and movement inputs
    void FlipSprite() {
        input.directionMoving = (int)Mathf.Sign(input.myRigidbody.velocity.x);

        bool hasXInput = Mathf.Abs(input.moveInput.x) > Mathf.Epsilon;
        if (hasXInput) {
            input.directionFacing = (int)Mathf.Sign(input.moveInput.x);
            transform.localScale = new Vector2(input.directionFacing * Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }

        // if gliding, the sprite rotates up to 60 degrees to account for vertical movement
        if (myPlayerState == PlayerState.Gliding) {
            //float maxXYRatio = Mathf.Pow(input.myRigidbody.velocity.x * 0.2f, 2);
            Vector2 adjustedVelocity = new Vector2(
                                            input.directionFacing * input.directionMoving * input.myRigidbody.velocity.x, 
                                            input.myRigidbody.velocity.y
                                       );
            
            myAnimator.SetFloat("glidingAngle", input.directionFacing * Vector2.SignedAngle(Vector2.right * input.directionFacing, adjustedVelocity));
            Debug.Log(input.directionFacing * Vector2.SignedAngle(Vector2.right * input.directionFacing, adjustedVelocity));

        }
        else {
            transform.up = Vector3.up;
        }
    }

    void GlideTransition() {
        if (IsTransitioningToClimbing())
            myPlayerState = PlayerState.Climbing;
        else if (input.isGrounded)
            if (input.moveInput.x != 0)
                myPlayerState = PlayerState.Running;
            else myPlayerState = PlayerState.Idling;
        else if (!input.isPressingGlide)
            myPlayerState = PlayerState.Falling;
    }

    void IdleTransition() {
        if (IsTransitioningToClimbing())
            myPlayerState = PlayerState.Climbing;
        else if (input.myRigidbody.velocity.y < runInformation.fallingThreshold)
            if (input.isPressingGlide)
                myPlayerState = PlayerState.Gliding;
            else myPlayerState = PlayerState.Falling;
        else if (input.moveInput.x != 0)
            myPlayerState = PlayerState.Running;
    }

    void ClimbTransition() {
        if (!(input.isTouchingTrees) || Mathf.Abs(input.moveInput.x) > 0.5f) {
            climbingResetTimeRemaining = climbingResetTime;
            myAnimator.SetFloat("climbingSpeed", 10f);

            if (input.isGrounded) {
                if (input.moveInput.x != 0)
                    myPlayerState = PlayerState.Running;
                else myPlayerState = PlayerState.Idling;
            }
            else if (input.isPressingGlide)
                myPlayerState = PlayerState.Gliding;
            else myPlayerState = PlayerState.Falling;


        }
    }


    void JumpTransition() {
        if (input.myRigidbody.velocity.y < runInformation.fallingThreshold && jump.GetJumpTimeRemaining() <= 0) {
            if (input.isPressingGlide) {
                myPlayerState = PlayerState.Gliding;
            }
            else myPlayerState = PlayerState.Falling;
        }
        else if (input.isGrounded && jump.GetJumpTimeRemaining() <= 0) {
            if (input.moveInput.x != 0)
                myPlayerState = PlayerState.Running;
            else myPlayerState = PlayerState.Idling;
        }
    }

    void FallTransition() {
        if (IsTransitioningToClimbing())
            myPlayerState = PlayerState.Climbing;
        else if (input.isGrounded)
            if (input.moveInput.x != 0)
                myPlayerState = PlayerState.Running;
            else myPlayerState = PlayerState.Idling;
        else if (input.isPressingGlide)
            myPlayerState = PlayerState.Gliding;
    }


    void RunTransition() {
        if (IsTransitioningToClimbing())
            myPlayerState = PlayerState.Climbing;
        else if (input.myRigidbody.velocity.y < runInformation.fallingThreshold) {
            if (input.isPressingGlide)
                myPlayerState = PlayerState.Gliding;
            else myPlayerState = PlayerState.Falling;
        }
        else if (input.moveInput.x == 0)
            myPlayerState = PlayerState.Idling;
    }

    bool IsTransitioningToClimbing() {
        return input.isPressingGrab && input.isTouchingTrees && climbingResetTimeRemaining <= 0;
    }

    public void SetStateToJumping() {
        myPlayerState = PlayerState.Jumping;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (myPlayerState == PlayerState.Jumping) {
            return;
        }

        if (collision.gameObject.tag == "Tree" & input.isPressingGrab) {
            myPlayerState = PlayerState.Climbing;
        }
    }

    void SnapAnimationTransform() {
        float x = Mathf.Round(transform.position.x * pixelsPerUnit) / pixelsPerUnit,
              y = Mathf.Round(transform.position.y * pixelsPerUnit) / pixelsPerUnit;
        animationTransform.position = new Vector2(x, y);
    }
}
