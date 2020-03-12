using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioManagerMain audioManager;

    private AudioManagerMain.SnapshotState pauseState, resumeState;

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
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Homebase);
            pauseState = AudioManagerMain.SnapshotState.PauseHombase;
            resumeState = AudioManagerMain.SnapshotState.ResumeHomebase;
        }
        else if (other.name == "Training")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Training);
            pauseState = AudioManagerMain.SnapshotState.PauseTraining;
            resumeState = AudioManagerMain.SnapshotState.ResumeTraining;
        }
        else if (other.name == "Alley")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Alleys);
            pauseState = AudioManagerMain.SnapshotState.Alleys;
            resumeState = AudioManagerMain.SnapshotState.Alleys;
        }
        else if (other.name == "Arena")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Arena);
            pauseState = AudioManagerMain.SnapshotState.PauseArena;
            resumeState = AudioManagerMain.SnapshotState.ResumeArena;
        }

    }

    public AudioManagerMain.SnapshotState getPauseState()
    {
        return pauseState;
    }
    public AudioManagerMain.SnapshotState getResumeState()
    {
        return resumeState;
    }

}
