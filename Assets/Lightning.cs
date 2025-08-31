using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Lightning : MonoBehaviourPun, IPunObservable
{
    public float offMin = 10f;
    public float offMax = 60f;
    public float onMin = 0.25f;
    public float onMax = 0.8f;
    public Light light;
    public AudioClip[] thunderSounds;
    public AudioClip[] distantThunderSounds;
    public AudioSource audioSource;

    [Header("Distant Thunder Settings")]
    public float distantThunderMinDelay = 30f;
    public float distantThunderMaxDelay = 120f;
    [Range(0f, 1f)] public float distantThunderVolume = 0.5f;

    private bool isLightningActive = false;
    private float nextLightningTime = 0f;
    private float nextDistantThunderTime = 0f;

    void Start()
    {
        Debug.Log("Script started.");
        if (light == null) light = GetComponent<Light>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (light == null || audioSource == null || thunderSounds.Length == 0)
        {
            Debug.LogError("Critical components missing! Assign in Inspector.");
            return;
        }

        
        nextLightningTime = Time.time + Random.Range(offMin, offMax);
        nextDistantThunderTime = Time.time + Random.Range(distantThunderMinDelay, distantThunderMaxDelay);
    }

    void Update()
    {
        if (!PhotonNetwork.IsConnected) 
        {
            if (!isLightningActive && Time.time > nextLightningTime)
            {
                StartCoroutine(DoLightning());
                nextLightningTime = Time.time + Random.Range(offMin, offMax);
            }

            if (Time.time > nextDistantThunderTime)
            {
                StartCoroutine(DoDistantThunder());
                nextDistantThunderTime = Time.time + Random.Range(distantThunderMinDelay, distantThunderMaxDelay);
            }
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            if (!isLightningActive && Time.time > nextLightningTime)
            {
                photonView.RPC("DoLightningRPC", RpcTarget.All);
                nextLightningTime = Time.time + Random.Range(offMin, offMax);
            }

            if (Time.time > nextDistantThunderTime)
            {
                photonView.RPC("DoDistantThunderRPC", RpcTarget.All);
                nextDistantThunderTime = Time.time + Random.Range(distantThunderMinDelay, distantThunderMaxDelay);
            }
        }
    }

    [PunRPC]
    IEnumerator DoLightningRPC()
    {
        isLightningActive = true;

       
        light.enabled = true;
        Debug.Log("Lightning ON");

       
        float soundDelay = Random.Range(0.25f, 1.75f);
        yield return new WaitForSeconds(soundDelay);

        if (thunderSounds.Length > 0)
        {
            int soundIndex = Random.Range(0, thunderSounds.Length);
            audioSource.PlayOneShot(thunderSounds[soundIndex]);
            Debug.Log("Played thunder: " + thunderSounds[soundIndex].name);
        }

        
        float duration = Random.Range(onMin, onMax);
        yield return new WaitForSeconds(duration);
        light.enabled = false;
        Debug.Log("Lightning OFF");

        isLightningActive = false;
    }

    [PunRPC]
    IEnumerator DoDistantThunderRPC()
    {
        if (!light.enabled && distantThunderSounds.Length > 0)
        {
            int soundIndex = Random.Range(0, distantThunderSounds.Length);
            audioSource.PlayOneShot(distantThunderSounds[soundIndex], distantThunderVolume);
            Debug.Log("Played distant thunder: " + distantThunderSounds[soundIndex].name);
        }
        yield return null;
    }

    
    IEnumerator DoLightning()
    {
        isLightningActive = true;

        light.enabled = true;
        Debug.Log("Lightning ON");

        float soundDelay = Random.Range(0.25f, 1.75f);
        yield return new WaitForSeconds(soundDelay);

        if (thunderSounds.Length > 0)
        {
            int soundIndex = Random.Range(0, thunderSounds.Length);
            audioSource.PlayOneShot(thunderSounds[soundIndex]);
            Debug.Log("Played thunder: " + thunderSounds[soundIndex].name);
        }

        float duration = Random.Range(onMin, onMax);
        yield return new WaitForSeconds(duration);
        light.enabled = false;
        Debug.Log("Lightning OFF");

        isLightningActive = false;
    }

    IEnumerator DoDistantThunder()
    {
        if (!light.enabled && distantThunderSounds.Length > 0)
        {
            int soundIndex = Random.Range(0, distantThunderSounds.Length);
            audioSource.PlayOneShot(distantThunderSounds[soundIndex], distantThunderVolume);
            Debug.Log("Played distant thunder: " + distantThunderSounds[soundIndex].name);
        }
        yield return null;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this object: send the others our data
            stream.SendNext(isLightningActive);
            stream.SendNext(light.enabled);
        }
        else
        {
            // Network client, receive data
            this.isLightningActive = (bool)stream.ReceiveNext();
            this.light.enabled = (bool)stream.ReceiveNext();
        }
    }
}