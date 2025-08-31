using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Voic : MonoBehaviourPun
{
   
     
    private PhotonVoiceView voiceView;
        private bool speakerTagged = false;

        void Awake()
        {
            voiceView = GetComponent<PhotonVoiceView>();
        }

        void Update()
        {
            // Only run this once per player
            if (!speakerTagged && voiceView != null && voiceView.SpeakerInUse != null)
            {
                Speaker speaker = voiceView.SpeakerInUse;
                if (speaker.GetComponent<tag>() == null)
                {
                    var tag = speaker.gameObject.AddComponent<tag>();
                    tag.ActorId = photonView.OwnerActorNr;

                    Debug.Log($"Tagged Speaker with ActorID {tag.ActorId}");
                }

                speakerTagged = true;
            }
        }
    }