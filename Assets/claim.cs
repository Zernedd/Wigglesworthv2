using UnityEngine;
using Photon.Pun;

public class claim : MonoBehaviour
{
   public SuperHeroTycoonMan parentBase;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("HandTag")) return;
        if (!PhotonNetwork.IsConnected) return;

        parentBase.TryClaim(PhotonNetwork.LocalPlayer.ActorNumber);
    }
}