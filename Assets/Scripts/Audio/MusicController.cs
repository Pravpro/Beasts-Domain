using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioManagerMain audioManager;
    public GameObject musicTriggerer;

    private AudioManagerMain.SnapshotState pauseState, resumeState;

    private void Start()
    {
        audioManager.Play(audioManager.homebase);
        audioManager.Play(audioManager.moodboard);
        audioManager.Play(audioManager.strings);
        audioManager.Play(audioManager.birds);

        /* Used to add this component onto the Music Area trigerrer (most likely the player).
         * This will not go through once it's added onto the object since musicTriggere will be null 
         */
        if (musicTriggerer)
        {
            MusicController musicManager = musicTriggerer.AddComponent<MusicController>();
            musicManager.audioManager = this.audioManager;
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SetMusic(other.name);
    }

    private void SetMusic(string trigger)
    {
        if (trigger == "Homebase")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Homebase);
            pauseState = AudioManagerMain.SnapshotState.PauseHombase;
            resumeState = AudioManagerMain.SnapshotState.ResumeHomebase;
        }
        else if (trigger == "Training")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Training);
            pauseState = AudioManagerMain.SnapshotState.PauseTraining;
            resumeState = AudioManagerMain.SnapshotState.ResumeTraining;
        }
        else if (trigger == "Alley")
        {
            audioManager.SetVolume(AudioManagerMain.SnapshotState.Alleys);
            pauseState = AudioManagerMain.SnapshotState.Alleys;
            resumeState = AudioManagerMain.SnapshotState.Alleys;
        }
        else if (trigger == "Arena")
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
