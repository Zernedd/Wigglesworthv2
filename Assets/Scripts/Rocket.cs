using UnityEngine;
using Photon.Pun;

public class Rocket : MonoBehaviourPun
{
    public string explosionEffectName = "explosion";
    public float explosionRadius = 5f;
    public float explosionForce = 700f;
    public float upwardsModifier = 1f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        // Spawn explosion particles across network
        PhotonNetwork.InstantiateRoomObject(explosionEffectName, transform.position, Quaternion.identity);

        // Knockback nearby rigidbodies
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier, ForceMode.Impulse);
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
