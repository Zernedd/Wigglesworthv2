using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum HandType
{
    Left,
    Right
}

public class FingerMovements : MonoBehaviour
{
    public HandType handType;
    public float fingerMoveSpeed = 10f;

    private Animator animator;

    private float pose1Value;
    private float pose2Value;
    private float pose3Value;

    private float indexValue;
    private float gripValue;
    private float thumbValue;

    public PhotonView view;

    void Start()
    {
        animator = GetComponent<Animator>();
        view = GetComponent<PhotonView>();

        if (view == null)
            Debug.LogError("[FingerMovements] No PhotonView found on " + gameObject.name);
        else
            Debug.Log("[FingerMovements] PhotonView found, IsMine: " + view.IsMine);
    }

    void Update()
    {
        if (view == null)
        {
            return;
        }

        if (!view.IsMine)
        {
            Debug.Log("[FingerMovements] Not owner, skipping animation for " + gameObject.name);
            return;
        }

        AnimateHand();
    }

    void AnimateHand()
    {
        InputDevice device = (handType == HandType.Right) ?
            InputDevices.GetDeviceAtXRNode(XRNode.RightHand) :
            InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);

        if (!device.isValid)
        {
            Debug.LogWarning("[FingerMovements] XR Device not valid for " + handType + " hand");
            return;
        }

        // Get trigger and grip values
        if (!device.TryGetFeatureValue(CommonUsages.trigger, out indexValue))
            Debug.LogWarning("[FingerMovements] Failed to read trigger for " + handType + " hand");

        if (!device.TryGetFeatureValue(CommonUsages.grip, out gripValue))
            Debug.LogWarning("[FingerMovements] Failed to read grip for " + handType + " hand");

        // Thumb detection
        bool primaryTouched = false;
        bool secondaryTouched = false;
        device.TryGetFeatureValue(CommonUsages.primaryButton, out primaryTouched);
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryTouched);
        thumbValue = (primaryTouched || secondaryTouched) ? 1f : 0f;

        Debug.LogFormat("[FingerMovements] {0} Hand - Trigger: {1:F2}, Grip: {2:F2}, Thumb: {3:F2}",
            handType, indexValue, gripValue, thumbValue);

        // Smoothly animate fingers
        pose1Value = Mathf.Lerp(pose1Value, indexValue, fingerMoveSpeed * Time.deltaTime);
        pose2Value = Mathf.Lerp(pose2Value, gripValue, fingerMoveSpeed * Time.deltaTime);
        pose3Value = Mathf.Lerp(pose3Value, thumbValue, fingerMoveSpeed * Time.deltaTime);

        // Update animator parameters
        if (handType == HandType.Right)
        {
            animator.SetFloat("pose4", pose1Value);
            animator.SetFloat("pose5", pose2Value);
            animator.SetFloat("pose6", pose3Value);
            Debug.Log("[FingerMovements] Updated Right Hand Animator parameters");
        }
        else
        {
            animator.SetFloat("pose1", pose1Value);
            animator.SetFloat("pose2", pose2Value);
            animator.SetFloat("pose3", pose3Value);
            Debug.Log("[FingerMovements] Updated Left Hand Animator parameters");
        }
    }
}
