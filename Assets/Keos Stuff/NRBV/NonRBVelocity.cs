using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonRBVelocity : MonoBehaviour
{
    public Vector3 Velocity;

    private Vector3 LastPosition;

    private void Start()
    {
        LastPosition = transform.position;
    }
    private void Update()
    {
        Velocity = transform.hasChanged ? (transform.position - LastPosition) / Time.deltaTime : Vector3.zero;
        LastPosition = transform.position;
        transform.hasChanged = false;
    }
}
