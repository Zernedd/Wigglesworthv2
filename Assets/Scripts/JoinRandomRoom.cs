using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
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
            PhotonVRManager.JoinRandomRoom("hub", 10);
        }
        }
    }

