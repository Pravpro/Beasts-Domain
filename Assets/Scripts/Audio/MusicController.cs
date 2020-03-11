using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public GameObject player;
    public AudioManagerMain audioManager;

    private void Start()
    {
        audioManager.Play(audioManager.homebase);
        audioManager.Play(audioManager.moodboard);
        audioManager.Play(audioManager.strings);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Homebase")
            audioManager.HomebaseVolume();
        else if (other.name == "Training")
            audioManager.MoodboardVolume();
        else if (other.name == "Alley")
            audioManager.BossVolume();
        else if (other.name == "Arena")
            audioManager.HomebaseVolume();

    }
}
