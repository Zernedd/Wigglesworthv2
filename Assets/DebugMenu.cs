using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int frames;
    private void Start()
    {
        StartCoroutine(d());
    }
    private void Update()
    {
        frames++;
    }
    IEnumerator d()
    {
        yield return new WaitForSeconds(1);
        text.text = "FPS: " + frames.ToString() + " RTT: " + PhotonNetwork.GetPing().ToString();
        frames = 0;
        StartCoroutine(d());
    }
}