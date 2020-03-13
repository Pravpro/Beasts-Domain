using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAudio : MonoBehaviour
{
    public AudioManagerMain audioManager;
    // Start is called before the first frame update
    void Start()
    {
        audioManager.fire = audioManager.Localize(gameObject, audioManager.fire);
        audioManager.Play(audioManager.fire, 0.9f);
    }
    
}
