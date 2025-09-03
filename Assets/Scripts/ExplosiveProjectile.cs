using UnityEngine;
using Photon.Pun;
using System.Collections;

public class ExplosiveProjectile : MonoBehaviourPun
{
    [Header("Explosion Settings")]
    public float explosionRadius = 5f;
    public float explosionForce = 500f;
    public float lifetime = 5f;

    public GameObject explosionEffect; // optional VFX prefab

    void Start()
    {
        // Auto-destroy after some time if it doesn’t hit anything
        StartCoroutine(SelfDestruct());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            Explode();
        }
    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(lifetime);
        if (photonView.IsMine)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Spawn visual effect
        if (explosionEffect != null)
        {
            GameObject fx = PhotonNetwork.InstantiateRoomObject(explosionEffect.name, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // Find players within radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            PhotonView targetView = nearby.GetComponentInParent<PhotonView>();
            if (targetView != null && targetView.CompareTag("Player"))
            {
                Vector3 dir = (nearby.transform.position - transform.position).normalized;

                // Knockback RPC
                targetView.RPC("ApplyKnockback", targetView.Owner, dir * explosionForce);
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
