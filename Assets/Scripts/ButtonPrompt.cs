using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPrompt : MonoBehaviour
{
    private GameObject pushPrompt;
    // Start is called before the first frame update
    void Start()
    {
        pushPrompt = GameObject.Find("push");


        // set all the be false
        pushPrompt.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
