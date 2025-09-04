using Photon.Pun;
using UnityEngine;

public class CosmeticTesting : MonoBehaviourPun
{
    [Header("Booster Prefabs")]
    public GameObject rightBoosterPrefab;
    public GameObject leftBoosterPrefab;

    [Header("Player Mounts")]
    public Transform rightHandMount;
    public Transform leftHandMount;

    private GameObject rightBoosterInstance;
    private GameObject leftBoosterInstance;

    private bool spawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (spawned) return;
        spawned = true;

        
        if (!photonView.IsMine) return;
 
        if (rightBoosterPrefab != null && rightHandMount != null)
        {
            rightBoosterInstance = PhotonNetwork.Instantiate(
                rightBoosterPrefab.name,
                rightHandMount.position,
                rightHandMount.rotation
            );
          //  rightBoosterInstance.transform.parent = rightHandMount; 
        }

       
        if (leftBoosterPrefab != null && leftHandMount != null)
        {
            leftBoosterInstance = PhotonNetwork.Instantiate(
                leftBoosterPrefab.name,
                leftHandMount.position,
                leftHandMount.rotation
            );
          //  leftBoosterInstance.transform.parent = leftHandMount; 
        }
    }
}
