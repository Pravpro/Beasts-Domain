using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mossAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManagerMain audioManager = GameObject.FindObjectOfType<AudioManagerMain>();
        AudioSource mossAudio = audioManager.Localize(gameObject, audioManager.moss);
        audioManager.Play(mossAudio, 0.9f);
    }

}
