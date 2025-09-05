using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvaBeltColldectEffect : MonoBehaviour
{

    public ParticleSystem ParticleSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ConveyorCube"))
        {
            Debug.Log("Uh no");
            ParticleSystem.Play();   
        }
        Debug.Log("Uh yeah");
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
