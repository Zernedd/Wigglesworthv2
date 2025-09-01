using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

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

    private int ownerId = -1;
    private Renderer rend;

    private static Dictionary<int, int> playerBalances = new Dictionary<int, int>();

    void Start()
    {
        int localId = PhotonNetwork.LocalPlayer.ActorNumber;

        if (!playerBalances.ContainsKey(localId))
            playerBalances[localId] = 500;

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

        if (playerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            var allBases = GameObject.FindObjectsOfType<SuperHeroTycoonMan>();
            foreach (var baseScript in allBases)
            {
                if (baseScript.OwnerId == playerId)
                    baseScript.UpdateBalanceText();
            }
        }
    }

    public void UpdateBalanceText()
    {
        if (balanceText == null) return;

        if (ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            balanceText.gameObject.SetActive(true);
            balanceText.text = $"Balance: {GetPlayerBalance(PhotonNetwork.LocalPlayer.ActorNumber)}";
        }
        else
        {
            balanceText.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    public void RPC_EnablePadObjects(int padIndex)
    {
        if (padIndex < 0 || padIndex >= pads.Length) return;

        pads[padIndex].EnableObjectsLocally();
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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.ActorNumber == ownerId)
        {
            photonView.RPC("RPC_ClaimBase", RpcTarget.AllBuffered, -1);

            // Reset all pads
            foreach (var pad in pads)
                pad.ResetPad();
        }
    }
}
