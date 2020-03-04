using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{

    // Add Audio Clips variables here as type AudioClip[] and set these in the inspector
    [Header("Clips")]
    public AudioClip[] landingClips;
   


    [HideInInspector] public AudioSource landing;


    Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();

    void Awake()
    {
        // 1. Create AuidoSource by calling AddAudio and passing in following parameters:
            // loop (true or false)
            // playAwake (true or false)
            // volume (value of volume followed by an 'f')
        landing = AddAudio(false, false, 0.7f);


        // 2. Add AudioSource and AudioClip relation into dictionary by doing:
            // sourceClipRelation.Add(<name of AudioSource>, <name of AudioClip>)
        sourceClipRelation.Add(landing, landingClips);
    }



    AudioSource AddAudio(bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    public void Play(AudioSource toPlay, float low = 1f, float high = 1f)
    {
        toPlay.clip = sourceClipRelation[toPlay][Random.Range(0, sourceClipRelation[toPlay].Length)];
        toPlay.pitch = Random.Range(low, high);
        toPlay.Play();

    }
}
