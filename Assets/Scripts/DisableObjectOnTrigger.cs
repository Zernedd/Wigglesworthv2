using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectOnTrigger : MonoBehaviour
{



    public GameObject ThingIWannaDisable;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Awake()
    {
       ThingIWannaDisable.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
