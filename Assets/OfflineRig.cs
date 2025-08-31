using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineRig : MonoBehaviour
{

    public GameObject righthand;
    public GameObject lefthand;
    public GameObject righthandoffline;
    public GameObject lefthandoffline;
    // Start is called before the first frame update
    void Start()
    {
        
    }

      
    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.InRoom)
        {
            lefthandoffline.SetActive(true);
            righthandoffline.SetActive(true );
            lefthandoffline.transform.position = lefthand.transform.position;
            lefthandoffline.transform.rotation = lefthand.transform.rotation;
            righthandoffline.transform.position = righthand.transform.position;
            righthandoffline.transform.rotation = righthand.transform.rotation;
        }
        else if (PhotonNetwork.InRoom)
        {
           lefthandoffline.SetActive(false);
            righthandoffline.SetActive(false);
        }
    }
}
