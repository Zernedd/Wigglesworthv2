using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class loadScene : MonoBehaviour
{

    public Button buttonthing;
    // Start is called before the first frame update
    void Start()
    {
        buttonthing.onClick.AddListener(loadit);
    }

    private void OnDestroy()
    {
        buttonthing.onClick.RemoveListener(loadit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    private void loadit()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
