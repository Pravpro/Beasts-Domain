using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mossAudio : MonoBehaviour
{
    public AudioManagerMain audioManager;

    // Start is called before the first frame update
    void Start()
    {
        AudioManagerMain audioManager = GameObject.FindObjectOfType<AudioManagerMain>();
        audioManager.moss = audioManager.Localize(gameObject, audioManager.moss);
        audioManager.Play(audioManager.moss, 0.9f);
    }

}
