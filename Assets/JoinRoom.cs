using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using Photon.Realtime;
using Photon.VR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class JoinRoom : MonoBehaviourPunCallbacks
{

    int maxcout = 20;
    string name;
    // Start is called before the first frame update
  



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HandTag"))
        {

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Debug.Log("user doesnt have mic perms can we please have them?");
                Permission.RequestUserPermission(Permission.Microphone);
            }
            if (PhotonNetwork.IsConnected && Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                PhotonNetwork.JoinRandomRoom();
                PhotonVRManager.SetUsername(name);


            }
        }
        void OnJoinRandomFailed()
        {

            PhotonVRManager.SetUsername(name);
            string roomName = GenerateRandomRoomName();
            RoomOptions roomOptions = new RoomOptions { MaxPlayers = (byte)maxcout };
            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }


        private string GenerateRandomRoomName()
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int roomNameLength = 4; 
            string roomName = string.Empty;

            for (int i = 0; i < roomNameLength; i++)
            {
                roomName += characters[Random.Range(0, characters.Length)];
            }

            return roomName;

            // Update is called once per frame
            void Update()
        {

        }
    }
}
