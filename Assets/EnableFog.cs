
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableFog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //RenderSettings
    }


    private void OnEnable()
    {
        RenderSettings.fog = true;
    }

    private void OnDisable()
    {
        RenderSettings.fog = false;
       // RenderSettings.fog

    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
