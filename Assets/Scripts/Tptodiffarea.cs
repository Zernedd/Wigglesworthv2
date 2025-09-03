
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
using ExitGames.Client.Photon; // for Hashtable

public class Tptodiffarea : MonoBehaviourPunCallbacks
{
    public GameObject tppoint;
    public GorillaLocomotion.Player player;
    public Rigidbody playerr;
    public Collider col;
    public string que; 


    private bool pendingJoin = false;
    private string nextQueue;


    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<GorillaLocomotion.Player>();

        if (!playerr && player)
            playerr = player.GetComponent<Rigidbody>();

        if (!col && player)
            col = player.GetComponent<Collider>();
    }
    


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {
         
                    
           SuperHeroTycoonMan[] allBases = FindObjectsOfType<SuperHeroTycoonMan>();

                    
           foreach (SuperHeroTycoonMan baseInstance in allBases)
           {
               baseInstance.ResetPads();
           }
            player.TeleportTo(tppoint.gameObject.transform.position);
            player.disableMovement = true;

            nextQueue = que;

            if (PhotonNetwork.InRoom)
            {
                // leave current room first
                pendingJoin = true;
                PhotonNetwork.LeaveRoom();
            }
            else if (PhotonNetwork.IsConnectedAndReady)
            {
                if (que == "pvp")
                {
                    GameObject player = GameObject.FindWithTag("User");
                    if (player != null)
                    {
                        player.gameObject.AddComponent<PlayerKnockback>();
                    }
                    GameObject rpg = GameObject.FindWithTag("RPG");
                    if (rpg != null)
                    {
                        rpg.gameObject.SetActive(true);


                    }
                }
                JoinQueue(nextQueue);
            }
            else
            {
                // not connected yet, flag join for later
                pendingJoin = true;
            }
        }
    }



   

    public override void OnLeftRoom()
    {
        Debug.Log("Left current room, waiting to return to Master...");
        // Do NOT call JoinQueue here; just wait for OnConnectedToMaster
        if (!PhotonNetwork.IsConnected)
        {
            PhotonVRManager.Connect();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Back on Master Server.");
        if (pendingJoin)
        {
            pendingJoin = false;
            JoinQueue(nextQueue); // safe to join now
        }
    }

    private void JoinQueue(string queueName)
    {
        Debug.Log($"Attempting to join queue: {queueName}");

        Hashtable expectedProps = new Hashtable() { { "type", queueName } };
        bool joinStarted = PhotonNetwork.JoinRandomRoom(expectedProps, 0);

        if (!joinStarted)
        {
            // If no room exists or all are full, create a new one
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 10;
            options.CustomRoomProperties = new Hashtable() { { "type", queueName } };
            options.CustomRoomPropertiesForLobby = new string[] { "type" };
            PhotonNetwork.CreateRoom(null, options);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        player.disableMovement = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"JoinRandomRoom failed: {message}. Creating a new room.");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;
        options.CustomRoomProperties = new Hashtable() { { "type", nextQueue } };
        options.CustomRoomPropertiesForLobby = new string[] { "type" };
        PhotonNetwork.CreateRoom(null, options);
    }
}
