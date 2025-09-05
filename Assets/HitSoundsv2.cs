using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HitSoundsv2 : MonoBehaviour
{
   // [Header("Tag-specific sounds")]
    public AudioClip[] water, stone, tree, grass, metal, glass, snow, dirt, carpet, wood;

    [Header("Fall Back Sounds")]
    public AudioClip[] defaultSounds;

    public AudioSource audioSource;
    public bool LeftController;
    private float hapticWaitSeconds = 0.05f;
    Dictionary<string, AudioClip[]> audio;

    void Start()
    {
        audio = new Dictionary<string, AudioClip[]> {
            { "Water", water },
            { "Stone", stone },
            { "Tree", tree },
            { "Grass", grass },
            { "Metal", metal },
            { "Glass", glass },
            { "Snow", snow },
            { "Dirt", dirt },
            { "Carpet", carpet },
            { "Wood", wood }
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("Hit object with tag: " + other.tag);

        if (!other.gameObject.CompareTag("Ignore"))
        {
            AudioClip[] chosenClips;
            if (!audio.TryGetValue(other.gameObject.tag, out chosenClips) || chosenClips.Length == 0)
            {
                chosenClips = defaultSounds;
            }

            if (chosenClips != null && chosenClips.Length > 0)
            {
                PlayRandomSound(chosenClips, audioSource);
            }

            StartVibration(LeftController, 0.15f, 0.15f);
        }
    }
    void PlayRandomSound(AudioClip[] audioClips, AudioSource audioSource)
    {
        audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
        audioSource.Play();
    }

    public void StartVibration(bool forLeftController, float amplitude, float duration)
    {
        StartCoroutine(HapticPulses(forLeftController, amplitude, duration));
    }

    private IEnumerator HapticPulses(bool forLeftController, float amplitude, float duration)
    {
        float startTime = Time.time;
        uint channel = 0u;
        InputDevice device = forLeftController
            ? InputDevices.GetDeviceAtXRNode(XRNode.LeftHand)
            : InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        while (Time.time < startTime + duration)
        {
            device.SendHapticImpulse(channel, amplitude, hapticWaitSeconds);
            yield return new WaitForSeconds(hapticWaitSeconds * 0.9f);
        }
    }
}
