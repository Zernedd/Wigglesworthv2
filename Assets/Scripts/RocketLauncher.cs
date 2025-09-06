using UnityEngine;
using Photon.Pun;
using UnityEngine.XR; // for VR input

public class RocketLauncher : MonoBehaviourPun
{
    [Header("Launcher Settings")]
    public GameObject rocketPrefab;    // assign your rocket prefab
    public Transform firePoint;        // assign the muzzle position
    public float launchForce = 25f;

    [Header("VR Input")]
    public XRNode inputSource = XRNode.RightHand; 
    private InputDevice device;
    private bool triggerPressed = false;

    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (!device.isValid)
        {
            device = InputDevices.GetDeviceAtXRNode(inputSource);
        }

        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            if (triggerValue && !triggerPressed)
            {
                triggerPressed = true;

                GameObject rocket = PhotonNetwork.Instantiate(rocketPrefab.name, firePoint.position, Quaternion.LookRotation(firePoint.forward));

                Rigidbody rb = rocket.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = firePoint.forward * launchForce;
                }
            }

            else if (!triggerValue)
            {
                triggerPressed = false;
            }
        }
    }

    [PunRPC]
    void RPC_FireRocket(Vector3 spawnPos, Vector3 direction)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject rocket = PhotonNetwork.InstantiateRoomObject(
            rocketPrefab.name, spawnPos, Quaternion.LookRotation(direction));

        Rigidbody rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = direction * launchForce;
    }

}
