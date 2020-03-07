using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManagerTitle : MonoBehaviour
{
    private Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();

    [Header("Clips")]
    public AudioClip[] musicClips;
    public AudioClip[] UIToggleClips;
    public AudioClip[] UISelectionClips;
    public AudioClip[] narratorClips;

    [HideInInspector] public AudioSource music;
    [HideInInspector] public AudioSource UIToggle;
    [HideInInspector] public AudioSource UISelection;
    [HideInInspector] public AudioSource narrator;

    void Awake()
    {
        // Params: 1.Loop 2.Play on Awake 3.Volume
        music = AddAudio(true, true, 1f, musicClips[0]);
        UIToggle = AddAudio(false, false, 0.4f);
        UISelection = AddAudio(false, false, 0.5f);
        narrator = AddAudio(false, true, 1f, narratorClips[0]);
        
        sourceClipRelation.Add(music, musicClips);
        sourceClipRelation.Add(UIToggle, UIToggleClips);
        sourceClipRelation.Add(UISelection, UISelectionClips);
        sourceClipRelation.Add(narrator, narratorClips);
    }



    AudioSource AddAudio(bool loop, bool playAwake, float vol, AudioClip Clip = null)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = Clip;
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
