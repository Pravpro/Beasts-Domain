using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    private Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();

    [Header("Snapshots")]
    public AudioMixerSnapshot introFight;
    public AudioMixerSnapshot Boss;

    [Header("Clips")]
    public AudioClip[] landingClips;
    public AudioClip[] jumpingClips;  
    public AudioClip[] staminaClips;
    public AudioClip[] footstepClips;
    public AudioClip[] slingshotClips;
    public AudioClip[] rockClips;
    public AudioClip[] spellClips;
    public AudioClip[] moodboardClips;
    public AudioClip[] musicClips;
    public AudioClip[] roarClips;
    public AudioClip[] damageClips;
    public AudioClip[] treeClips;

    [HideInInspector] public AudioSource landing; // pitch: 0.7, 1.3
    [HideInInspector] public AudioSource jumping; // pitch: 0.8, 1.2
    [HideInInspector] public AudioSource stamina;
    [HideInInspector] public AudioSource footstep; //pitch: 0.8, 1.2
    [HideInInspector] public AudioSource slingshot;
    [HideInInspector] public AudioSource rock; //pitch: 0.6, 1.3
    [HideInInspector] public AudioSource spell;
    [HideInInspector] public AudioSource moodboard;
    [HideInInspector] public AudioSource music;
    [HideInInspector] public AudioSource roar;
    [HideInInspector] public AudioSource damage;
    [HideInInspector] public AudioSource tree; //pitch: 0.8, 1.1

    void Awake()
    {
        // Params: 1.Loop 2.Play on Awake 3.Volume
        landing = AddAudio(false, false, 0.6f);
        jumping = AddAudio(false, false, 0.7f);
        stamina = AddAudio(false, false, 0.7f);
        footstep = AddAudio(false, false, 0.4f);
        slingshot = AddAudio(false, false, 0.7f);
        rock = AddAudio(false, false, 0.7f);
        spell = AddAudio(false, false, 0.8f);
        moodboard = AddAudio(true, false, 1f);
        music = AddAudio(true, true, 1f);
        roar = AddAudio(false, false, 1f);
        damage = AddAudio(false, false, 1f);
        tree = AddAudio(false, false, 0.7f);

        sourceClipRelation.Add(landing, landingClips);
        sourceClipRelation.Add(jumping, jumpingClips);
        sourceClipRelation.Add(stamina, staminaClips);
        sourceClipRelation.Add(footstep, footstepClips);
        sourceClipRelation.Add(slingshot, slingshotClips);
        sourceClipRelation.Add(rock, rockClips);
        sourceClipRelation.Add(spell, spellClips);
        sourceClipRelation.Add(moodboard, moodboardClips);
        sourceClipRelation.Add(music, musicClips);
        sourceClipRelation.Add(roar, roarClips);
        sourceClipRelation.Add(damage, damageClips);
        sourceClipRelation.Add(tree, treeClips);
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
        if (toPlay == music)
        {

        }
        else
        {
            toPlay.clip = sourceClipRelation[toPlay][Random.Range(0, sourceClipRelation[toPlay].Length)];
        }
        toPlay.pitch = Random.Range(low, high);
        toPlay.Play();

    }
}
