using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuperHeroTycoonMan : MonoBehaviourPunCallbacks
{
    [Header("Setup")]
    public int baseId;
    public TextMeshPro statusText;
    public TextMeshPro balanceText;
    public GameObject claimCube;

    [Header("Room Check")]
    public string requiredRoomProp = "Hero_Tycoon";

    [Header("Pads")]
    public BuyPad[] pads;



    [Header("Balance UI")]
    public TextMeshPro walletText;
    public TextMeshPro bankText;

    private int ownerId = -1;
    private Renderer rend;

    private static Dictionary<int, int> playerBalances = new Dictionary<int, int>();
    private static Dictionary<int, int> playerBanks = new Dictionary<int, int>();

    void Start()
    {
        int localId = PhotonNetwork.LocalPlayer.ActorNumber;

        if (!playerBalances.ContainsKey(localId))
            playerBalances[localId] = 500;

        if (!playerBanks.ContainsKey(localId))
            playerBanks[localId] = 0;

        if (PhotonNetwork.CurrentRoom == null ||
            !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(requiredRoomProp))
        {
            Debug.LogWarning($"Room does not contain required prop '{requiredRoomProp}'. Tycoon base inactive.");
            return;
        }

        rend = GetComponent<Renderer>();
        if (rend == null)
            Debug.LogWarning($"Renderer missing on base {baseId}");

        SetOwner(-1);

        if (claimCube != null && claimCube.GetComponent<claim>() == null)
        {
            var trigger = claimCube.AddComponent<claim>();
            trigger.parentBase = this;
        }

        UpdateBalanceText();
    }

    [PunRPC]
    public void RPC_ClaimBase(int newOwnerId)
    {
        SetOwner(newOwnerId);
        ResetPads();
    }

    [PunRPC]
    public void RPC_EnablePadObjects(int padIndex)
    {
        if (padIndex < 0 || padIndex >= pads.Length) return;
        pads[padIndex].EnableObjectsLocally();
    }

    [PunRPC]
    public void RPC_ResetPadObjects()
    {
        foreach (var pad in pads)
            pad.ResetPad();
    }

    private void SetOwner(int newOwnerId)
    {
        ownerId = newOwnerId;

        if (rend != null)
        {
            if (ownerId == -1) rend.material.color = Color.white;
            else if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber) rend.material.color = Color.green;
            else rend.material.color = Color.red;
        }

        if (statusText != null)
        {
            if (ownerId == -1) statusText.text = "Unclaimed";
            else if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber) statusText.text = "Your Base!";
            else statusText.text = "Claimed";
        }

        UpdateBalanceText();
    }

    public int OwnerId => ownerId;

 
    public static int GetPlayerBalance(int playerId)
    {
        if (!playerBalances.ContainsKey(playerId))
            playerBalances[playerId] = 0;
        return playerBalances[playerId];
    }

    public static void AddCurrency(int playerId, int amount)
    {
        if (!playerBalances.ContainsKey(playerId))
            playerBalances[playerId] = 0;
        playerBalances[playerId] += amount;
    }

 
    public static int GetPlayerBank(int playerId)
    {
        if (!playerBanks.ContainsKey(playerId))
            playerBanks[playerId] = 0;
        return playerBanks[playerId];
    }

    public static void AddToBank(int playerId, int amount)
    {
        if (!playerBanks.ContainsKey(playerId))
            playerBanks[playerId] = 0;
        playerBanks[playerId] += amount;
    }

    public static void RedeemBank(int playerId)
    {
        int banked = GetPlayerBank(playerId);
        if (banked > 0)
        {
            AddCurrency(playerId, banked);
            playerBanks[playerId] = 0;
        }
    }

    public void UpdateBalanceText()
    {
        if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            if (walletText != null)
            {
                walletText.gameObject.SetActive(true);
                walletText.text = $"Wallet: {GetPlayerBalance(PhotonNetwork.LocalPlayer.ActorNumber)}";
            }

            if (bankText != null)
            {
                bankText.gameObject.SetActive(true);
                bankText.text = $"Bank: {GetPlayerBank(PhotonNetwork.LocalPlayer.ActorNumber)}";
            }
        }
        else
        {
            if (walletText != null) walletText.gameObject.SetActive(false);
            if (bankText != null) bankText.gameObject.SetActive(false);
        }
    }

    public void TryClaim(int playerId)
    {
        var allBases = GameObject.FindObjectsOfType<SuperHeroTycoonMan>();
        foreach (var b in allBases)
        {
            if (b.OwnerId == playerId)
            {
                Debug.Log($"Player {playerId} already owns base {b.baseId}!");
                return;
            }
        }

        if (ownerId == -1)
        {
            photonView.RPC("RPC_ClaimBase", RpcTarget.AllBuffered, playerId);
        }
    }

    public void ResetPads()
    {
        foreach (var pad in pads)
            pad.ResetPad();
    }
    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.ActorNumber == ownerId)
        {
            photonView.RPC("RPC_ClaimBase", RpcTarget.AllBuffered, -1);
            photonView.RPC("RPC_ResetPadObjects", RpcTarget.AllBuffered);
        }
    }
}
