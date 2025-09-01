using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR;

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
            PhotonVRManager.JoinRandomRoom("tycoon1", 10);
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left current room, waiting to return to Master...");
       
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Back on Master Server.");
        if (pendingJoin)
        {
            pendingJoin = false;
            PhotonVRManager.JoinRandomRoom("tycoon1", 10);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        playerr.isKinematic = false;
    }
}
