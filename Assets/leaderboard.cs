using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class leaderboard : MonoBehaviourPunCallbacks
{
    public TMP_Text boardtext;
    private void Awake()
    {
        if (!PhotonNetwork.InRoom)
        {
            boardtext.text = "NOT IN ROOM";
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        boardtext.text = String.Empty;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.NickName.IsNullOrEmpty())
            {
                boardtext.text += $"{p.NickName} \n";
            }
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        boardtext.text = String.Empty;
        foreach (Player p in PhotonNetwork.PlayerList)
        {

            if (!p.NickName.IsNullOrEmpty())
            {
                boardtext.text += $"{p.NickName} \n";
            }
        }

    }
}
