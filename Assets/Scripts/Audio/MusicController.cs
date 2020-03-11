using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioManagerMain audioManager;

    private void Start()
    {        
        audioManager.Play(audioManager.homebase);
        audioManager.Play(audioManager.moodboard);
        audioManager.Play(audioManager.strings);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("The trigger is: " + other.name);
        if (other.name == "Homebase")
            audioManager.SetHomebaseVolume();
        else if (other.name == "Training")
            audioManager.SetTrainingVolume();
        else if (other.name == "Alley")
            audioManager.SetAlleysVolume();
        else if (other.name == "Arena")
            audioManager.SetArenaVolume();

    }
}
