#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Photon.Pun;
using Photon.Voice.Unity;
using Photon.Realtime;
using System.Collections.Generic;

public class godvoice : MonoBehaviourPun
{
    public Recorder recorder;

    private bool isGodVoiceActive = false;
    private bool godVoiceUnlocked = false;

    private List<OVRInput.Button> inputBuffer = new List<OVRInput.Button>();
    private float comboResetTime = 2f;
    private float lastInputTime;

    private readonly OVRInput.Button[] correctCombo = new OVRInput.Button[]
    {
        OVRInput.Button.One,
        OVRInput.Button.Two,
        OVRInput.Button.SecondaryHandTrigger,
        OVRInput.Button.SecondaryIndexTrigger
    };

    void Update()
    {
        if (!photonView.IsMine) return;

        if (!godVoiceUnlocked)
        {
            HandleComboInput();
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                ToggleGodVoice();
            }
        }
    }

    void HandleComboInput()
    {
        if (Time.time - lastInputTime > comboResetTime)
        {
            inputBuffer.Clear();
        }

        foreach (OVRInput.Button button in correctCombo)
        {
            if (OVRInput.GetDown(button))
            {
                inputBuffer.Add(button);
                lastInputTime = Time.time;
                CheckCombo();
                break;
            }
        }
    }

    void CheckCombo()
    {
        if (inputBuffer.Count > correctCombo.Length)
        {
            inputBuffer.Clear();
            return;
        }

        for (int i = 0; i < inputBuffer.Count; i++)
        {
            if (inputBuffer[i] != correctCombo[i])
            {
                inputBuffer.Clear();
                return;
            }
        }

        if (inputBuffer.Count == correctCombo.Length)
        {
            godVoiceUnlocked = true;
            Debug.Log("God Voice Unlocked!");
            inputBuffer.Clear();
        }
    }

    public void ForceToggleGodVoice()
    {
        if (!godVoiceUnlocked)
        {
            godVoiceUnlocked = true;
            Debug.Log("God Voice forcibly unlocked via inspector button");
        }
        ToggleGodVoice();
    }

    void ToggleGodVoice()
    {
        isGodVoiceActive = !isGodVoiceActive;
        if (photonView == null)
        {
            Debug.LogError("ToggleGodVoice: photonView is null!");
            return;
        }
        if (PhotonNetwork.LocalPlayer == null)
        {
            Debug.LogError("ToggleGodVoice: LocalPlayer is null!");
            return;
        }

        isGodVoiceActive = !isGodVoiceActive;

        if (isGodVoiceActive)
        {
            photonView.RPC("RPC_EnableGodVoice", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            photonView.RPC("RPC_DisableGodVoice", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }


    [PunRPC]
    void RPC_EnableGodVoice(int godActorId)
    {
        foreach (var speaker in FindObjectsOfType<Speaker>())
        {
            var tag = speaker.GetComponent<tag>();
            if (tag == null) continue;

            var audioSource = speaker.GetComponent<AudioSource>();
            if (audioSource == null) continue;

            if (tag.ActorId == godActorId)
            {
                audioSource.volume = 1.5f;
            }
            else
            {
                audioSource.volume = 0.1f;
            }
        }

        Debug.Log($"God voice activated by player {godActorId}");
    }

    [PunRPC]
    void RPC_DisableGodVoice(int godActorId)
    {
        foreach (var speaker in FindObjectsOfType<Speaker>())
        {
            var audioSource = speaker.GetComponent<AudioSource>();
            if (audioSource == null) continue;

            audioSource.volume = 1f;
        }

        Debug.Log($"God voice deactivated by player {godActorId}");
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        if (!Application.isPlaying) return;

        if (photonView.IsMine)
        {
            // Position the button somewhere small at the bottom left of the Game view
            if (GUI.Button(new Rect(10, Screen.height - 50, 180, 40), "Toggle God Voice (Inspector)"))
            {
                ForceToggleGodVoice();
            }
        }
    }
#endif
}
