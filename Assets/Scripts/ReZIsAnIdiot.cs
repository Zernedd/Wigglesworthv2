using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.VR;

public class ReZIsAnIdiot : MonoBehaviour
{
    //public PhotonVRManager man;
    // Start is called before the first frame update
    bool enabled;

    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
       // bool enabled;
        if (other.gameObject.CompareTag("HandTag"))
        {
            enabled = !enabled;
            if (enabled)
            {
                PhotonVRManager.SetCosmetic("LeftHand", "RPG");
            }
            else
            {
                PhotonVRManager.SetCosmetic("LeftHand", "");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
