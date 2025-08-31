using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class RandomSound : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource source;
    public AudioClip[] songp;
    public AudioClip othersound;
    Dictionary<string, AudioClip[]> song;


    void Start()
    {
        song = new Dictionary<string, AudioClip[]> {
            { "StartSound", songp },
        };


        playrandom(songp, source);
        playtheothersound();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void playtheothersound()
    {
        if (!PlayerPrefs.HasKey("soundthing"))
        {
            source.clip = othersound;
            source.Play();
            PlayerPrefs.SetString("soundthing", "true");
        }
    }

    void playrandom(AudioClip[] clips, AudioSource audioSource)
    {
        if (PlayerPrefs.HasKey("soundthing"))
        {
            audioSource.clip = clips[Random.Range(0, clips.Length)];
            audioSource.Play();
        }

    }
}
