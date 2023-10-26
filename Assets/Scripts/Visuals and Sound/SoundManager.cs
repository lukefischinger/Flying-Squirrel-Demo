using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource wind, birds, leaves;

    [SerializeField] List<AudioClip> birdsClips, leavesClips;



    const float leavesDelayMax = 10f;
    const float birdsDelayMax = 12f;

    float leavesDelayTimer, birdsDelayTimer;


    void Awake()
    {
        ResetBirdDelay();
        ResetLeavesDelay();
    }


    void Update()
    {
        if (!birds.isPlaying)
        {
            if (birdsDelayTimer < 0)
            {
                ResetBirdDelay();
                birds.clip = GetRandom(birdsClips);
                birds.Play();
            }
            else
            {
                birdsDelayTimer -= Time.deltaTime;
            }
        }

        if (!leaves.isPlaying)
        {
            if (leavesDelayTimer < 0)
            {
                ResetLeavesDelay();
                leaves.clip = GetRandom(leavesClips);
                leaves.Play();
            }
            else
            {
                leavesDelayTimer -= Time.deltaTime;
            }
        }


    }


    void ResetBirdDelay()
    {
        birdsDelayTimer = Random.Range(0, birdsDelayMax);
    }

    void ResetLeavesDelay()
    {
        leavesDelayTimer = Random.Range(0, leavesDelayMax);
    }

    AudioClip GetRandom(List<AudioClip> list)
    {
        int index = (int)Random.Range(0, list.Count);
        Debug.Log(index);
        return list[index];
    }
}
