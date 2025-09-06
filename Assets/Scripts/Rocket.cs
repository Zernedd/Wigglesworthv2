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

    private Rigidbody rb;
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (photonView.IsMine)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // Smoothly interpolate to the network position/rotation
            transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.fixedDeltaTime * 50f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.fixedDeltaTime * 10f);
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
            Vector3 velocity = (Vector3)stream.ReceiveNext();

            if (rb != null)
            {
                rb.velocity = velocity;
            }
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
            // Create temporary GameObject for audio
            GameObject audioObject = new GameObject("Explosion_Audio");
            audioObject.transform.position = pos;

            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.clip = explosionSound;
            audioSource.volume = soundVolume;
            audioSource.spatialBlend = 1f; // 3D sound
            audioSource.maxDistance = 50f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.Play();

            // Destroy audio object after clip finishes
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
            photonView.RPC("RPC_Explode", RpcTarget.All, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}