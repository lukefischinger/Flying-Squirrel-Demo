using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerClimbing : MonoBehaviour
{
    PlayerInput input;
    RunInformation runInfo;
    PlayerJumping jump;
    Tilemap trees;

    // climbing variables
    float climbingPosition = Mathf.Infinity, climbingDistance, yMovement, climbingSnapSpeed;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        jump = GetComponent<PlayerJumping>();
        runInfo = input.runInformation;

        //trees = GameObject.Find("Trees Tilemap").GetComponent<Tilemap>();
    }

    public void Climb()
    {
        RaycastHit2D closestTree = GetClosestTreeCollision(input.myBodyCollider.size.x);

        // if no trees nearby, exit
        if (closestTree.collider == null) return;

        climbingPosition = GetNearestClimbingX(closestTree.point);
        climbingDistance = climbingPosition - input.myRigidbody.position.x;

        if (climbingDistance < 0.05)
            climbingSnapSpeed = runInfo.treeSnappingSpeed * climbingDistance;
        else climbingSnapSpeed = Mathf.Sign(climbingDistance) * runInfo.treeSnappingSpeed * Mathf.Pow(Mathf.Abs(climbingDistance), 0.1f);

        yMovement = climbingDistance < 0.01 ? input.moveInput.y * runInfo.climbSpeed : 0;
        if (AtTopOfTree())
            yMovement = Mathf.Min(yMovement, 0);

        input.myRigidbody.velocity = new Vector2(climbingSnapSpeed, yMovement);
        input.myRigidbody.gravityScale = 0;
        jump.ResetJumpCount();
    }


    RaycastHit2D GetClosestTreeCollision(float radius)
    {
        DrawSphere((Vector2)transform.position - radius * Vector2.right * input.directionFacing, radius);
        DrawSphere((Vector2)transform.position + radius * Vector2.right * input.directionFacing, radius);

        RaycastHit2D hit = Physics2D.CircleCast(
            transform.position + radius * Vector3.right * input.directionFacing, 
            radius, 
            -input.directionFacing * Vector3.right, 
            2 * radius, 
            LayerMask.GetMask("Climbing")
        );

        return hit;
    }

    // debugging function
    void DrawSphere(Vector2 pos, float radius)
    {
        for(int i = 0; i < 36; i++)
        {
            Color color = Color.magenta;
            color.a = 0.2f;
            Debug.DrawRay(pos, Quaternion.Euler(Vector3.forward * i * 10) * Vector2.right * radius, color, 0.25f);
        }
    }



    float GetNearestClimbingX(Vector2 pos)
    {
        return Mathf.Floor(pos.x) + 0.5f;
    }


    bool AtTopOfTree()
    {
        Vector2 startingPos = (Vector2)transform.position + 0.5f * input.myBodyCollider.size.x * Vector2.left + input.myBodyCollider.size.y * Vector2.up;
        float distance = input.myBodyCollider.size.x;
        DrawSphere(startingPos, 0.5f * input.myBodyCollider.size.y);
        DrawSphere(startingPos + Vector2.right * distance, 0.5f * input.myBodyCollider.size.y);
        
        RaycastHit2D hit = Physics2D.CircleCast(startingPos, 0.5f * input.myBodyCollider.size.y, Vector2.right, distance, LayerMask.GetMask("Climbing"));
        return hit.Equals(null) || hit.point == Vector2.zero;
    }
}
