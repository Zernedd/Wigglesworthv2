using UnityEngine;
using Photon.Pun;

public class Rocket : MonoBehaviourPun
{
    public string explosionEffectName = "explosion";
    public float explosionRadius = 5f;
    public float explosionForce = 5f;
    public float upwardsModifier = 1f;
    public float lifeTime = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    [PunRPC]
    void RPC_Explode(Vector3 pos)
    {
        // Play particle effect
        Instantiate(explosionEffect, pos, Quaternion.identity);

        // Knockback
        Collider[] hits = Physics.OverlapSphere(pos, explosionRadius);
        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, pos, explosionRadius, 1f, ForceMode.Impulse);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine) // only owner tells others
        {
            photonView.RPC("RPC_Explode", RpcTarget.All, transform.position);
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
