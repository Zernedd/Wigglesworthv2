using UnityEngine;
using Photon.Pun;

public class PlayerKnockback : MonoBehaviourPun
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [PunRPC]
    public void ApplyKnockback(Vector3 force)
    {
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
