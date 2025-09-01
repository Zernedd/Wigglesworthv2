using Photon.Pun;
using TMPro;
using UnityEngine;

public class RedeemMoneyPad : MonoBehaviour
{
    [Header("Setup")]
    public SuperHeroTycoonMan parentBase;
    public TMP_Text redeemText;
    public Collider touchCollider;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip redeemClip;
    public AudioClip noFundsClip;

    [Header("Settings")]
    public float redeemCooldown = 1f; 

    private float lastRedeemTime = -999f;

    private void Awake()
    {
        if (touchCollider != null)
            touchCollider.isTrigger = true;
    }

    private void Update()
    {
        UpdateRedeemText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("HandTag")) return;

       
        if (Time.time - lastRedeemTime < redeemCooldown)
            return;

        lastRedeemTime = Time.time;

        int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        TryRedeem(playerId);
    }

    public void TryRedeem(int playerId)
    {
        if (parentBase == null || parentBase.OwnerId != playerId) return;

        int banked = SuperHeroTycoonMan.GetPlayerBank(playerId);
        if (banked > 0)
        {
            SuperHeroTycoonMan.RedeemBank(playerId);
            Debug.Log($"Player {playerId} redeemed {banked} coins from bank!");

            if (audioSource != null && redeemClip != null)
                audioSource.PlayOneShot(redeemClip);
        }
        else
        {
            if (audioSource != null && noFundsClip != null)
                audioSource.PlayOneShot(noFundsClip);

            Debug.Log("No banked income to redeem.");
        }
    }

    public void UpdateRedeemText()
    {
        if (redeemText == null || parentBase == null) return;

        if (parentBase.OwnerId == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            redeemText.gameObject.SetActive(true);
            int banked = SuperHeroTycoonMan.GetPlayerBank(PhotonNetwork.LocalPlayer.ActorNumber);
            redeemText.text = $"${banked}\nTouch to redeem!";
        }
        else
        {
            redeemText.gameObject.SetActive(false);
        }
    }
}
