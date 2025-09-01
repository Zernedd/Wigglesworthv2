using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchHelper : MonoBehaviour
{
    public BuyPad pad; 

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("HandTag")) return;

        if (pad != null)
            pad.TryPurchase(Photon.Pun.PhotonNetwork.LocalPlayer.ActorNumber);
    }
}

