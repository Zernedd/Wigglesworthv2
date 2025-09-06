using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LaserDoor : MonoBehaviour
{
    [Header("Player")]
    public GorillaLocomotion.Player player;

    [Header("Post Processing")]
    public Volume postProcessVolume;
    private ColorAdjustments colorAdjustments;

    [Header("Damage Settings")]
    public float damageFlashDuration = 0.5f;
    public AudioSource damageAudio;

    [Header("Respawn Settings")]
    public float fadeDuration = 0.5f;

    [Header("Door Collider")]
    public Collider doorCollider;
    [Header("ReZ Wanted Me To Add This")]
    public GameObject DefaultSpawnPoint;

    


    [Header("DoorStuff")]
    public GameObject Lasers;
    public bool isActive;
    public PhotonView View;
    public GameObject dorcollideerobj;

    private void Awake()
    {
      
        if (doorCollider != null)
            doorCollider.isTrigger = true;

       
        if (postProcessVolume != null && postProcessVolume.profile.TryGet<ColorAdjustments>(out var ca))
        {
            colorAdjustments = ca;
            colorAdjustments.colorFilter.value = Color.white;
        }
        else
        {
            Debug.LogError("[LaserDoor] Post Process Volume or ColorAdjustments missing!");
        }
    }

    /// <summary>
    /// Call this when the player interacts with a door
    /// </summary>
    public void CheckDoor(SuperHeroTycoonMan baseman)
    {
        if (baseman == null) return;

        if (baseman.ownerId != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            doorCollider.gameObject.layer = LayerMask.NameToLayer("Default");
            doorCollider.isTrigger = false;
            StartCoroutine(DamageAndRespawn());
        }
        else
        {
            doorCollider.gameObject.layer = LayerMask.NameToLayer("Ignore");
            doorCollider.isTrigger = true;

        }
    }

    /// <summary>
    /// Flash red and then respawn the player
    /// </summary>
    private IEnumerator DamageAndRespawn()
    {
       
        if (damageAudio != null) damageAudio.Play();

        float elapsed = 0f;
        while (elapsed < damageFlashDuration)
        {
            elapsed += Time.deltaTime;
            if (colorAdjustments != null)
                colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.red, Mathf.PingPong(elapsed * 4f, 1f));
            yield return null;
        }

        if (colorAdjustments != null)
            colorAdjustments.colorFilter.value = Color.white;

        
        SuperHeroTycoonMan myBase = FindMyBase();
        if (myBase != null)
        {
            Transform respawnPoint = myBase.transform.Find("RespawnPoint");
            if (respawnPoint != null)
                yield return StartCoroutine(FadeToBlackAndTeleport(respawnPoint));
            else
                Debug.LogWarning("[LaserDoor] RespawnPoint not found on your base!");
        }
        else
        {
            yield return StartCoroutine(FadeToBlackAndTeleport(DefaultSpawnPoint.transform));
            //Debug.LogWarning("[LaserDoor] No base found for local player!");
        }
    }




    [PunRPC]
    public void RPC_ToggleDoor(bool active)
    {
        isActive = active;
        if (Lasers != null) Lasers.SetActive(isActive);
        if (doorCollider != null) doorCollider.enabled = isActive;
    }

    private SuperHeroTycoonMan FindMyBase()
    {
        SuperHeroTycoonMan[] allBases = FindObjectsOfType<SuperHeroTycoonMan>();
        foreach (var b in allBases)
        {
            if (b.ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
                return b;
        }
        return null;
    }

    private IEnumerator FadeToBlackAndTeleport(Transform target)
    {
        
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (colorAdjustments != null)
                colorAdjustments.colorFilter.value = Color.Lerp(Color.white, Color.black, elapsed / fadeDuration);
            yield return null;
        }

        player.TeleportTo(target.position);

      
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (colorAdjustments != null)
                colorAdjustments.colorFilter.value = Color.Lerp(Color.black, Color.white, elapsed / fadeDuration);
            yield return null;
        }
    }
}
