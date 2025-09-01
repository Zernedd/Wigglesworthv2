using Meta.Voice.Audio.Decoding;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPad : MonoBehaviour
{
    [Header("Setup")]
    public SuperHeroTycoonMan parentBase;  // The base this pad belongs to
    public int price = 0;
    public int incomePerSecond = 5;

    [Header("Visuals & Trigger")]
    public GameObject cubeWithCollider;    // Assign the cube that has the trigger collider

    [Header("Objects to Enable After Purchase")]
    public GameObject[] objectsToEnable;   // conveyor belts, next pads, etc.

    [Header("Audio")]
    public AudioSource src;
    public AudioClip bought;
    public AudioClip notEnough;

    private bool purchased = false;

    void Start()
    {
        if (cubeWithCollider != null)
        {
            // Ensure the cube has a trigger collider
            Collider col = cubeWithCollider.GetComponent<Collider>();
            if (col == null) col = cubeWithCollider.AddComponent<BoxCollider>();
            col.isTrigger = true;

            // Add a small helper component to detect trigger events
            TouchHelper helper = cubeWithCollider.AddComponent<TouchHelper>();
            helper.pad = this;
        }
        else
        {
            Debug.LogWarning("TycoonPad: cubeWithCollider not assigned!");
        }
    }

    public void TryPurchase(int localId)
    {
        if (purchased || parentBase.OwnerId != localId) return;

        int balance = SuperHeroTycoonMan.GetPlayerBalance(localId);
        if (balance >= price)
        {
            // Deduct money
            SuperHeroTycoonMan.AddCurrency(localId, -price);

            // Play purchase sound
            if (src != null && bought != null) src.PlayOneShot(bought);

            purchased = true;
            Debug.Log($"Player {localId} purchased pad for {price}");

            // Start generating income
            StartCoroutine(GenerateIncome(localId));

            // Disable the buy cube
            if (cubeWithCollider != null)
                cubeWithCollider.SetActive(false);

            // Enable linked objects locally
            foreach (var obj in objectsToEnable)
                if (obj != null) obj.SetActive(true);

            // Send enable info to other clients
            int[] viewIDs = new int[objectsToEnable.Length];
            for (int i = 0; i < objectsToEnable.Length; i++)
            {
                if (objectsToEnable[i] != null)
                {
                    PhotonView pv = objectsToEnable[i].GetComponent<PhotonView>();
                    if (pv == null) pv = objectsToEnable[i].AddComponent<PhotonView>();
                    viewIDs[i] = pv.ViewID;
                }
            }

            parentBase.photonView.RPC("RPC_EnableObjects", RpcTarget.OthersBuffered, viewIDs);
        }
        else
        {
            // Play "not enough money" sound
            if (src != null && notEnough != null) src.PlayOneShot(notEnough);

            Debug.Log($"Player {localId} cannot afford pad. Balance: {balance}, Price: {price}");
        }
    }

    private IEnumerator GenerateIncome(int playerId)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SuperHeroTycoonMan.AddCurrency(playerId, incomePerSecond);
        }
    }
}
