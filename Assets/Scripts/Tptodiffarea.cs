using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Tptodiffarea : MonoBehaviourPunCallbacks
{
    public GameObject tppoint;
    public GameObject player;
    public Rigidbody playerr;
    public Collider col;

    [System.Serializable]
    public class properties
    {
        public string key;
        public string value;
    }

    public List<properties> customProperties;
    public string roomname;
    private RoomOptions roomcustomprops;

    // Track if we want to join after leaving
    private bool pendingJoin = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
            playerr.isKinematic = true;
            player.transform.localPosition = tppoint.transform.localPosition;

            ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable();
            foreach (var prop in customProperties)
            {
                roomProps[prop.key] = prop.value;
            }

            roomcustomprops = new RoomOptions();
            roomcustomprops.CustomRoomProperties = roomProps;
            roomcustomprops.CustomRoomPropertiesForLobby = new string[customProperties.Count];
            for (int i = 0; i < customProperties.Count; i++)
            {
                roomcustomprops.CustomRoomPropertiesForLobby[i] = customProperties[i].key;
            }

            roomname = Random.Range(0, 999999999).ToString();

            if (PhotonNetwork.InRoom)
            {
                // leave first, then wait for OnConnectedToMaster
                pendingJoin = true;
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                PhotonNetwork.JoinOrCreateRoom(roomname, roomcustomprops, TypedLobby.Default);
            }
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left current room, waiting to return to Master...");
        // Do nothing here except wait for OnConnectedToMaster
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Back on Master Server.");
        if (pendingJoin)
        {
            pendingJoin = false;
            PhotonNetwork.JoinOrCreateRoom(roomname, roomcustomprops, TypedLobby.Default);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerr.isKinematic = false;
    }
}
