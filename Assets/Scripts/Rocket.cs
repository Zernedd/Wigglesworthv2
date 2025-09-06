using UnityEngine;
using Photon.Pun;

public class Rocket : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionRadius = 5f;
    public float explosionForce = 5f;
    public float upwardsModifier = 1f;
    public float lifeTime = 5f;

    [Header("Audio")]
    public AudioClip explosionSound;
    public float soundVolume = 1f;

    [Header("Network Settings")]
    public float lerpRate = 10f;  // Smoothing speed for network interpolation

    private Rigidbody rb;
    private Vector3 networkPosition;
    private Vector3 networkVelocity;
    private Quaternion networkRotation;
    private float lastNetworkUpdate;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            // Schedule network destruction after lifetime
            Invoke(nameof(DestroyRocket), lifeTime);
        }
        else
        {
            // Initialize network values for non-owners
            networkPosition = transform.position;
            networkRotation = transform.rotation;
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine && rb != null)
        {
            // Smooth interpolation for non-owners
            float timeSinceUpdate = Time.time - lastNetworkUpdate;

            // Predict position based on velocity
            Vector3 predictedPos = networkPosition + (networkVelocity * timeSinceUpdate);

            // Smoothly lerp to predicted position
            transform.position = Vector3.Lerp(transform.position, predictedPos, Time.fixedDeltaTime * lerpRate);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * lerpRate);

            // Apply network velocity to rigidbody for physics consistency
            rb.velocity = Vector3.Lerp(rb.velocity, networkVelocity, Time.fixedDeltaTime * lerpRate);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send data to other players
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
        }
        else
        {
            // Receive data from owner
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkVelocity = (Vector3)stream.ReceiveNext();
            lastNetworkUpdate = Time.time;
        }
    }

    private void DestroyRocket()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    [PunRPC]
    void RPC_Explode(Vector3 pos)
    {
        // Play explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, pos, Quaternion.identity);

            // Auto-destroy based on particle system duration
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                float totalDuration = ps.main.duration + ps.main.startLifetime.constantMax;
                Destroy(explosion, totalDuration);
            }
            else
            {
                Destroy(explosion, 3f);
            }
        }

        // Play explosion sound
        if (explosionSound != null)
        {
            GameObject audioObject = new GameObject("Explosion_Audio");
            audioObject.transform.position = pos;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.volume = soundVolume;
            audioSource.spatialBlend = 1f;
            audioSource.maxDistance = 50f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.Play();

            Destroy(audioObject, explosionSound.length);
        }

        // Apply knockback
        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius);
        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, pos, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            // Cancel the lifetime destruction since we're exploding
            CancelInvoke(nameof(DestroyRocket));

            // Tell all clients to show explosion
            photonView.RPC("RPC_Explode", RpcTarget.All, transform.position);

            // Network destroy the rocket
            PhotonNetwork.Destroy(gameObject);
        }
    }
}