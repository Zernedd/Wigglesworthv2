using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Buttons : MonoBehaviour
{
    public enum PressMode { Press, LongPress }

    [Header("Settings")]
    public PressMode pressMode = PressMode.Press;
    public float longPressTime = 1f;
    public float pushOffset = 0.1f;
    public float returnSpeed = 20f;

    [Header("References")]
    public GameObject visuals;
    public UnityEvent onPressed;      

    private Vector3 startPos;
    private int overlapCount;
    private bool hasPressed;
    private float holdTime;
    public AudioSource pressSound;
    void Awake()
    {
        if (visuals != null)
            startPos = visuals.transform.localPosition;
    }

    void OnTriggerEnter(Collider other)
    {
        if (overlapCount == 0)
        {
            holdTime = 0f;
            if (pressMode == PressMode.Press)
            {
                Press();
            }
        }
        overlapCount++;
    }

    void OnTriggerExit(Collider other)
    {
        overlapCount--;
        if (overlapCount <= 0)
        {
            overlapCount = 0;
            holdTime = 0f;
            hasPressed = false;
        }
    }

    void Update()
    {
        if (overlapCount > 0)
        {
            holdTime += Time.deltaTime;

            if (pressMode == PressMode.LongPress && !hasPressed && holdTime >= longPressTime)
            {
                Press();
            }
        }

        // Smoothly return visuals
        if (visuals != null)
        {
            visuals.transform.localPosition =
                Vector3.Lerp(visuals.transform.localPosition, startPos, Time.deltaTime * returnSpeed);
        }
    }

    void Press()
    {
        hasPressed = true;

        
        if (visuals != null)
            visuals.transform.localPosition = startPos + Vector3.forward * pushOffset;

   
        if (pressSound != null)
            pressSound.Play();

        
        onPressed?.Invoke();
    }
}