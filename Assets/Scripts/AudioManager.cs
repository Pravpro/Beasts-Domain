using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    private Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();

    [Header("Clips")]
    public AudioClip[] landingClips;
    
    [HideInInspector] public AudioSource landing;

    void Awake()
    {
        landing = AddAudio(false, false, 0.6f);

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
