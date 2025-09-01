using UnityEngine;
using Photon.Pun;

public class TouchHelper : MonoBehaviour
{
    public BuyPad pad;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("HandTag")) return;
        if (pad != null)
            pad.TryPurchase(PhotonNetwork.LocalPlayer.ActorNumber);
    }
}
