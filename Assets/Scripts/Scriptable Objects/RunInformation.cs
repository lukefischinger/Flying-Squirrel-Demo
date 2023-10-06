using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class RunInformation : ScriptableObject
{

    public bool runInProgress = false;

    // attributes
    public int currentReputation;
    public int currentSeedCount;
    public int maxReputation;
    public int maxSeedCount;
    public bool isSharing;
    public float sharingPortion;

    // movement
    public float runSpeed;
    public float jumpSpeed;
    public float climbSpeed;
    public int numJumpsAllowed;
    public float fireRate;
    public int fillAmount;
    public float hugeSeedProbability;
    public float hugeSeedMultiplier;
    public float glideMultiplier;

    // other movement variables
    public float baseGravity;
    public float glideGravity;
    public float minGlidingSpeed;
    public float climbingSpeedFactor;
    public float glidingSpeedFactor;
    public float treeSnappingSpeed;
    public GameObject seed;
    public float seedSpeed;
    public GameObject prefabCrosshairs;
    public float fallingThreshold;
    public float acceleration;

    // list of upgrades already obtained by the player
    public List<string> upgrades;
    public List<string> upgradesToApply;

    public void Reset()
    {
        runInProgress = false;
        
        currentReputation = 20;
        currentSeedCount = 100000;
        maxReputation = 20;
        maxSeedCount = 100000;
        
        runSpeed = 7;
        jumpSpeed = 1300;
        climbSpeed = 7;
        numJumpsAllowed = 2;
        fireRate = 0.3f;
        fillAmount = 2;
        hugeSeedProbability = 0.05f;
        hugeSeedMultiplier = 2f;
        glideMultiplier = 0.5f;

        baseGravity = 4;
        glideGravity = 1;
        minGlidingSpeed = 4;
        climbingSpeedFactor = 0;
        glidingSpeedFactor = 1.5f;
        treeSnappingSpeed = 10f;
        seedSpeed = 30;
        fallingThreshold = -0.0001f;
        acceleration = 14f;

        upgrades = new List<string>();
        upgradesToApply = new List<string>();
    }



}
