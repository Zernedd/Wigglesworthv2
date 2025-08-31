using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.VR;
using UnityEngine;

public class OculusUserManager : MonoBehaviour
{

    private void Awake()
    {
      
            string name = $"Wiggle {UnityEngine.Random.RandomRange(1, 90000).ToString()}";

            PhotonVRManager.SetUsername(name);
            
        }
      

    }
