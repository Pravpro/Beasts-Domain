using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public GameObject player;
    private AudioManagerMain audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManagerMain>();
        audioManager.Play(audioManager.homebase);
        audioManager.Play(audioManager.moodboard);
        audioManager.Play(audioManager.strings);
    }

    private void OnTriggerEnter(Collider other)
    {
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
