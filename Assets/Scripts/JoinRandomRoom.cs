using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRandomRoom : MonoBehaviourPunCallbacks
{
    public string nameofit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void JoinQueue(string queueName)
    {
        Debug.Log($"Attempting to join queue: {queueName}");

        ExitGames.Client.Photon.Hashtable expectedProps = new ExitGames.Client.Photon.Hashtable() { { "type", queueName } };
        bool joinStarted = PhotonNetwork.JoinRandomRoom(expectedProps, 0);

        if (!joinStarted)
        {
            // If no room exists or all are full, create a new one
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 10;
            options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", queueName } };
            options.CustomRoomPropertiesForLobby = new string[] { "type" };
            PhotonNetwork.CreateRoom(null, options);
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"JoinRandomRoom failed: {message}. Creating a new room.");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;
        options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", nameofit } };
        options.CustomRoomPropertiesForLobby = new string[] { "type" };
        PhotonNetwork.CreateRoom(null, options);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
          JoinQueue(nameofit);
        }
        }
    }

