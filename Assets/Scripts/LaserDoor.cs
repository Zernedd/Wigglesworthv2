using Photon.Pun;
using UnityEngine;
using System.Collections;

public class LaserDoor : MonoBehaviour
{
    public GorillaLocomotion.Player player;

    [Header("Damage Effect")]
    public CanvasGroup damageOverlay;
    public float damageDuration = 2f;

    [Header("Damage Sound")]
    public AudioSource damageAudio;
    public float damageSoundInterval = 1f;

    [Header("Damage Settings")]
    public float damageCooldown = 3f;

    private bool isTakingDamage = false;
    private float lastDamageTime = 0f;

    [Header("References")]
    public Collider doorCollider; // assign externally

    private void Awake()
    {
        if (doorCollider == null)
        {
            Debug.LogError("[LaserDoor] Door Collider is missing! Assign it externally.");
        }
        else
        {
            doorCollider.isTrigger = true; // initially passable
        }
    }

    private void Start()
    {
        if (damageOverlay != null)
            damageOverlay.alpha = 0;

        Debug.Log("[LaserDoor] Initialized for local player.");
    }

    private void Update()
    {
        // Fade out damage overlay after cooldown
        if (!isTakingDamage && damageOverlay != null && damageOverlay.alpha > 0 &&
            Time.time - lastDamageTime >= damageCooldown)
        {
            damageOverlay.alpha = Mathf.Lerp(damageOverlay.alpha, 0, Time.deltaTime * 5f);
        }
    }

    /// <summary>
    /// Call this externally when the player interacts with a door
    /// </summary>
    public void CheckDoor(SuperHeroTycoonMan baseman)
    {
        if (baseman == null)
        {
            Debug.LogWarning("[LaserDoor] CheckDoor called with null baseman!");
            return;
        }

        // Enemy base: block and damage
        if (baseman.ownerId != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            doorCollider.isTrigger = false;
            Debug.Log($"[LaserDoor] Enemy door detected. Blocking player and applying damage. Base owner ID: {baseman.ownerId}");

            if (!isTakingDamage)
                StartCoroutine(DamageCoroutine());
        }
        else
        {
            // Own base: pass through
            doorCollider.isTrigger = true;
            Debug.Log("[LaserDoor] Own base detected. Door is passable.");
        }
    }

    private IEnumerator DamageCoroutine()
    {
        isTakingDamage = true;
        lastDamageTime = Time.time;

        float elapsed = 0f;
        float soundTimer = 0f;

        while (elapsed < damageDuration)
        {
            elapsed += Time.deltaTime;
            lastDamageTime = Time.time;

            if (damageOverlay != null)
                damageOverlay.alpha = Mathf.Lerp(0, 1, elapsed / damageDuration);

            if (damageAudio != null)
            {
                soundTimer += Time.deltaTime;
                if (soundTimer >= damageSoundInterval)
                {
                    damageAudio.Play();
                    soundTimer = 0f;
                }
            }

            yield return null;
        }

        RespawnAtMyBase();

        // Fade out overlay
        elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            if (damageOverlay != null)
                damageOverlay.alpha = Mathf.Lerp(1, 0, elapsed / 0.5f);
            yield return null;
        }

        isTakingDamage = false;
        Debug.Log("[LaserDoor] Damage coroutine ended.");
    }

    private void RespawnAtMyBase()
    {
        SuperHeroTycoonMan myBase = FindMyBase();
        if (myBase != null)
        {
            Transform respawnPoint = myBase.transform.Find("RespawnPoint");
            if (respawnPoint != null)
            {
                player.TeleportTo(respawnPoint.position);
                Debug.Log($"[LaserDoor] Player respawned at {respawnPoint.position} of their base.");
            }
            else
            {
                Debug.LogWarning("[LaserDoor] RespawnPoint not found on your base!");
            }
        }
        else
        {
            Debug.LogWarning("[LaserDoor] No base found for local player!");
        }
    }

    private SuperHeroTycoonMan FindMyBase()
    {
        SuperHeroTycoonMan[] allBases = FindObjectsOfType<SuperHeroTycoonMan>();
        foreach (var b in allBases)
        {
            if (b.ownerId == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return b;
            }
        }
        return null;
    }
}
