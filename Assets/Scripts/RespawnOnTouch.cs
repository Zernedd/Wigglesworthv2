using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnOnTouch : MonoBehaviour
{
    public GameObject RespawnPoint;
    public GorillaLocomotion.Player player;
    void Start()
    {
        if (!player)
            player = FindObjectOfType<GorillaLocomotion.Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        player.TeleportTo(RespawnPoint.gameObject.transform.position);
    }
}
