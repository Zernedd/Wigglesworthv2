using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRandomRoom : MonoBehaviour
{

    
    [System.Serializable]
    public class properties
    {
        public string key;
        public string value;
    }
    public List<properties> customProperties;
    public string roomname;
    private RoomOptions roomcustomprops;
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

            roomname = Random.Range(0, 9999999999999).ToString();



            PhotonNetwork.JoinOrCreateRoom(roomname, roomcustomprops, TypedLobby.Default);
        }
        }
    }

