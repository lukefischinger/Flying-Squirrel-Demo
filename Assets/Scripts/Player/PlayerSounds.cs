using UnityEngine;
using static PlayerStateController;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] AudioSource gliding, running, climbing;

    PlayerStateController myStateController;
    PlayerPhysics physics;

    void Awake()
    {
        myStateController = GetComponent<PlayerStateController>();
        physics = GetComponent<PlayerPhysics>();
    }

    const float fadeOutTime = 1f;
    float glidingFadeTimer;

    private void Update()
    {
        PauseAllExceptCurrent();

        switch (myStateController.myPlayerState)
        {
            case PlayerState.Gliding:
                if (!gliding.isPlaying)
                    gliding.UnPause();

                gliding.volume = GlidingVol();
                glidingFadeTimer = fadeOutTime;
                break;

            case PlayerState.Running:
                if (!running.isPlaying)
                    running.UnPause();

                break;
            case PlayerState.Climbing:
                if (!climbing.isPlaying) {
                    if(physics.myRigidbody.velocity.magnitude > 0.1f) {
                        climbing.UnPause();
                        Debug.Log("unpausing climbing sound");
                    }
                } else if(physics.myRigidbody.velocity.magnitude <= 0.1f) {
                    climbing.Pause();
                    Debug.Log("Pausing climbing sound");
                }
                break;

            default: break;
        }
    }


    void PauseAllExceptCurrent()
    {
        if (gliding.isPlaying && myStateController.myPlayerState != PlayerState.Gliding)
        {
            if (glidingFadeTimer < 0)
                gliding.Pause();
            else
            {
                gliding.volume = Mathf.Pow(Mathf.Clamp01(glidingFadeTimer), 0.25f) * gliding.volume;
                glidingFadeTimer -= Time.deltaTime;
            }
        }
        if (running.isPlaying && myStateController.myPlayerState != PlayerState.Running)
        {
            running.Pause();
        }
        if (climbing.isPlaying && myStateController.myPlayerState != PlayerState.Climbing)
        {
            climbing.Pause();
        }
    }

    float GlidingVol()
    {
        return Mathf.Max(
            gliding.volume * (1 - Time.deltaTime),
            Mathf.Clamp01(Mathf.Pow((physics.myRigidbody.velocity.magnitude - 5) / 35, 2))
        );
    }



}
