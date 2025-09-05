using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDoorButton : MonoBehaviour
{
    public LaserDoor linkedDoor;

    public void PressButton()
    {
        if (linkedDoor == null) return;

        bool newState = linkedDoor.isActive = !linkedDoor.isActive;

       
        linkedDoor.View.RPC("RPC_ToggleDoor", RpcTarget.AllBuffered, newState);
    }
}