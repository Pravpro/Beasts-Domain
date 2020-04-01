using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used some code from https://gist.github.com/sinbad/4a9ded6b00cf6063c36a4837b15df969 for smoothing

[RequireComponent(typeof(Light))]
public class FlickerEffect : MonoBehaviour
{
    public float minIntensity, maxIntensity;
    // Higher value means smoother flickering.
    [Range(1,50)]
    public int smoothing = 5;

    // Use Queueing for averages
    Queue<float> smoothQueue;
    float lastSum;

    Light pointLight;


    void Start()
    {
        pointLight = GetComponent<Light>();
        smoothQueue = new Queue<float>(smoothing);
    }

    // Update is called once per frame
    void Update()
    {
        while(smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;
        
        // Use average for new intensity. Creates smoother intensity changes
        pointLight.intensity = lastSum / (float)smoothQueue.Count;
    }
}
