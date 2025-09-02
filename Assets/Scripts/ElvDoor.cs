using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElvDoor : MonoBehaviour
{


    //chatgpt a bit bc i had errors dont smite me
    [Header("Doors")]
    public Transform leftDoor;
    public Transform rightDoor;

    public Transform leftOpenPos;
    public Transform rightOpenPos;
    public Transform leftClosedPos;
    public Transform rightClosedPos;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip dingClip;

    [Header("Settings")]
    public float doorSpeed = 2f;
    public float autoCloseDelay = 3f;

    private bool opening;
    private bool closing;
    private float closeTime;

    void Update()
    {
        if (opening)
        {
            MoveDoors(leftOpenPos.position, rightOpenPos.position);

            if (AtTarget(leftDoor, leftOpenPos) && AtTarget(rightDoor, rightOpenPos))
            {
                opening = false;
                closeTime = Time.time + autoCloseDelay;
                closing = true;
            }
        }

        if (closing && Time.time >= closeTime)
        {
            MoveDoors(leftClosedPos.position, rightClosedPos.position);

            if (AtTarget(leftDoor, leftClosedPos) && AtTarget(rightDoor, rightClosedPos))
            {
                closing = false;
            }
        }
    }

    /// <summary>
    /// Call this when the button is pressed idk uhh yeah
    /// </summary>
    public void Pressed()
    {
        if (audioSource && dingClip)
            audioSource.PlayOneShot(dingClip);

        opening = true;
    }

    private void MoveDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        leftDoor.position = Vector3.MoveTowards(leftDoor.position, leftTarget, doorSpeed * Time.deltaTime);
        rightDoor.position = Vector3.MoveTowards(rightDoor.position, rightTarget, doorSpeed * Time.deltaTime);
    }

    private bool AtTarget(Transform door, Transform target)
    {
        return Vector3.Distance(door.position, target.position) < 0.01f;
    }
}