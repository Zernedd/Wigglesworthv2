using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class JetBoosters : MonoBehaviour
{

    public Transform LeftBooster;
    public Transform RightBooster;


    public float forceStrength = 20f;
    public bool lefthand;

    public bool righthand;

    private Rigidbody playerRb;

    void Start()
    {
        playerRb = GorillaLocomotion.Player.Instance.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (lefthand)
        {


            if (InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPressed) && leftPressed)
            {
                FLy(LeftBooster);
            }

        }
        if (righthand)
        {
            if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPressed) && rightPressed)
            {
                FLy(RightBooster);
            }
        }


        void FLy(Transform booster)
        {
            if (booster == null) return;


            Vector3 worldThrustDir = booster.forward;


            playerRb.AddForceAtPosition(worldThrustDir * forceStrength, booster.position, ForceMode.Force);
        }


        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            if (LeftBooster != null)
            {
                Gizmos.DrawSphere(LeftBooster.position, 0.05f);
                Gizmos.DrawRay(LeftBooster.position, LeftBooster.forward * 0.5f);
            }

            if (RightBooster != null)
            {
                Gizmos.DrawSphere(RightBooster.position, 0.05f);
                Gizmos.DrawRay(RightBooster.position, RightBooster.forward * 0.5f);
            }
        }
    }
}