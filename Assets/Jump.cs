using Photon.VR.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    // Start is called before the first frame update
    public Collider col;
    public GameObject scare;



    public Transform tpplace;
    public GameObject player;

    Rigidbody scary;
    public AudioSource source;
    public AudioClip clip;
    void Start()
    {
        scary = player.GetComponent<Rigidbody>();
    }


    private void OnTriggerEnter(Collider other)
    {
        
    
    
        {
            if (other.CompareTag("HandTag"))
            {
                Debug.Log("hitsum");
                StartCoroutine(scarethem());

            }
        }




        IEnumerator scarethem()
        {
            source.PlayOneShot(clip);
            player.transform.position = tpplace.transform.position;
            scary.isKinematic = true;
            scare.SetActive(true);
            float turnoff = Random.Range(2f, 2.5f);
            yield return new WaitForSeconds(turnoff);
            scary.isKinematic = false;
            scare.SetActive(false);

        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
