using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("Clips")]
    public AudioClip landingClip;


    [HideInInspector] public AudioSource landing;


    void Awake()
    {
        landing = AddAudio(landingClip, false, false, 0.7f);
    }

    AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.clip = clip;
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    public void Play(AudioSource toPlay, bool randomPitch = false)
    {
        //if (toPlay == sourceWithMultipleClips)
        //{
        //    sourceWithMultipleClips.clip = clips[Random.Range(0, clips.Length)];
        //}

        if (randomPitch)
        {
            toPlay.pitch = Random.Range(0.8f, 1.2f);
        }
        toPlay.Play();
        toPlay.pitch = 1f;
    }
}
