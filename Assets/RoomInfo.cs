using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomInfo : MonoBehaviourPunCallbacks
{
    public TMP_Text text;
    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
        {
            text.text = "NOT IN ROOM";
        }
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (PhotonNetwork.InRoom)
        {
            text.text = $"In Room \n {PhotonNetwork.CurrentRoom}";
        }
    }
}
