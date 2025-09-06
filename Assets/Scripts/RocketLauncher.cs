using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;

public class RocketLauncher : MonoBehaviourPun
{
    [Header("Launcher Settings")]
    public GameObject rocketPrefab;
    public Transform firePoint;
    public float launchForce = 25f;
    public float fireRate = 0.5f;  // Delay between shots in seconds

    [Header("VR Input")]
    public XRNode inputSource = XRNode.RightHand;
    private InputDevice device;
    private bool triggerPressed = false;
    private float nextFireTime = 0f;

    [Header("Audio")]
    public AudioClip shootSound;
    public float soundVolume = 1f;

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
            if (triggerValue && !triggerPressed && Time.time >= nextFireTime)
            {
                triggerPressed = true;
                nextFireTime = Time.time + fireRate;  // Set next allowed fire time

                GameObject rocket = PhotonNetwork.Instantiate(
                    rocketPrefab.name,
                    firePoint.position,
                    firePoint.rotation
                );

                Rigidbody rb = rocket.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = firePoint.forward * launchForce;
                }

                // Play shoot sound locally
                if (shootSound != null)
                {
                    AudioSource.PlayClipAtPoint(shootSound, firePoint.position, soundVolume);
                }
            }
            else if (!triggerValue)
            {
                triggerPressed = false;
            }
        }
    }
}