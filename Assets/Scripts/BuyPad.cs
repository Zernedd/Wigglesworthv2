using Photon.Pun;
using UnityEngine;
using System.Collections;

public class BuyPad : MonoBehaviour
{
    public SuperHeroTycoonMan parentBase;
    public int price = 0;
    public int incomePerSecond = 5;

    [Header("Objects to Enable")]
    public GameObject[] objectsToEnable;

    [Header("Trigger Collider")]
    public GameObject cubeWithCollider;

    private bool purchased = false;

    public AudioSource audioSource;
    public AudioClip boughtClip;
    public AudioClip notEnoughClip;

    void Start()
    {
        if (cubeWithCollider != null)
        {
            Collider col = cubeWithCollider.GetComponent<Collider>();
            if (col == null) col = cubeWithCollider.AddComponent<BoxCollider>();
            col.isTrigger = true;

            TouchHelper helper = cubeWithCollider.AddComponent<TouchHelper>();
            helper.pad = this;
        }
    }

    public void TryPurchase(int localId)
    {
        if (purchased) return;
        if (parentBase.OwnerId != localId) return;

        int balance = SuperHeroTycoonMan.GetPlayerBalance(localId);
        if (balance >= price)
        {
            SuperHeroTycoonMan.AddCurrency(localId, -price);
            purchased = true;

            if (audioSource != null && boughtClip != null)
                audioSource.PlayOneShot(boughtClip);

            StartCoroutine(GenerateIncome(localId));

            if (cubeWithCollider != null)
                cubeWithCollider.SetActive(false);

            EnableObjectsLocally();

            int padIndex = System.Array.IndexOf(parentBase.pads, this);
            parentBase.photonView.RPC("RPC_EnablePadObjects", RpcTarget.OthersBuffered, padIndex);
        }
        else
        {
            if (audioSource != null && notEnoughClip != null)
                audioSource.PlayOneShot(notEnoughClip);

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

    public void EnableObjectsLocally()
    {
        foreach (var obj in objectsToEnable)
            if (obj != null) obj.SetActive(true);
    }

    public void ResetPad()
    {
        purchased = false;

        if (cubeWithCollider != null)
            cubeWithCollider.SetActive(true);

        foreach (var obj in objectsToEnable)
            if (obj != null) obj.SetActive(false);
    }
}
