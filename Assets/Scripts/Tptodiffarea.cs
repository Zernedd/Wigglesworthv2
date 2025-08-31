using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Animations.Rigging;

public class Tptodiffarea : MonoBehaviour
{

  // public Collider col;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


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

            RoomOptions options = new RoomOptions();
            options.CustomRoomProperties = roomProps;

            options.CustomRoomPropertiesForLobby = new string[customProperties.Count];
            for (int i = 0; i < customProperties.Count; i++)
            {
                options.CustomRoomPropertiesForLobby[i] = customProperties[i].key;
            }

            string roomName = Random.Range(0, 9999999999999).ToString();
            PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
        }
    }

    


    private void OnTriggerExit(Collider other)
    {
      
        playerr.isKinematic = false;
    }
}
