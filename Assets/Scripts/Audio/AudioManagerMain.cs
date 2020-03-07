using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManagerMain : MonoBehaviour
{
    private Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();

    [Header("Snapshots")]
    public AudioMixerSnapshot introFight;
    public AudioMixerSnapshot Boss;

    [Header("SfxClips")]
    public AudioClip[] landingClips;
    public AudioClip[] jumpingClips;  
    public AudioClip[] staminaClips;
    public AudioClip[] footstepClips;
    public AudioClip[] rockClips;
    public AudioClip[] spellClips;
    public AudioClip[] damageClips;
    public AudioClip[] deathClips;
    public AudioClip[] treeClips;
    [Header("MusicClips")]
    public AudioClip[] stringsClips;
    public AudioClip[] bossstringsClips;
    public AudioClip[] boss1Clips;
    public AudioClip[] boss2Clips;
    public AudioClip[] moodboardClips;
    public AudioClip[] homebaseClips;
    [Header("MonsterClips")]
    public AudioClip[] phase1Clips;
    public AudioClip[] phase2Clips;
    public AudioClip[] hurtClips;
    public AudioClip[] flareClips;
    public AudioClip[] hoofClips;

    [Header("SlingshotClips")]
    public AudioClip[] slingshotClips;
    

    [HideInInspector] public AudioSource landing; // pitch: 0.7, 1.3
    [HideInInspector] public AudioSource jumping; // pitch: 0.8, 1.2
    [HideInInspector] public AudioSource stamina;
    [HideInInspector] public AudioSource footstep; //pitch: 0.8, 1.2
    [HideInInspector] public AudioSource slingshot;
    [HideInInspector] public AudioSource rock; //pitch: 0.6, 1.3
    [HideInInspector] public AudioSource spell;
    [HideInInspector] public AudioSource moodboard;
    [HideInInspector] public AudioSource strings;
    [HideInInspector] public AudioSource bossstrings;
    [HideInInspector] public AudioSource boss1;
    [HideInInspector] public AudioSource boss2;
    [HideInInspector] public AudioSource homebase;
    [HideInInspector] public AudioSource phase1;
    [HideInInspector] public AudioSource phase2;
    [HideInInspector] public AudioSource hurt;
    [HideInInspector] public AudioSource flare;
    [HideInInspector] public AudioSource hoof;
    [HideInInspector] public AudioSource damage;
    [HideInInspector] public AudioSource death;
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
        moodboard = AddAudio(true, true, 1f);
        strings = AddAudio(true, true, 1f);
        bossstrings = AddAudio(true, false, 1f);
        boss1 = AddAudio(true, false, 1f);
        boss2 = AddAudio(true, false, 1f);
        homebase = AddAudio(true, true, 1f);
        phase1 = AddAudio(false, false, 1f);
        phase2 = AddAudio(false, false, 1f);
        hurt = AddAudio(false, false, 1f);
        flare = AddAudio(false, false, 1f);
        hoof = AddAudio(false, false, 1f);
        damage = AddAudio(false, false, 1f);
        death = AddAudio(false, false, 1f);
        tree = AddAudio(false, false, 0.7f);

        sourceClipRelation.Add(landing, landingClips);
        sourceClipRelation.Add(jumping, jumpingClips);
        sourceClipRelation.Add(stamina, staminaClips);
        sourceClipRelation.Add(footstep, footstepClips);
        sourceClipRelation.Add(slingshot, slingshotClips);
        sourceClipRelation.Add(rock, rockClips);
        sourceClipRelation.Add(spell, spellClips);
        sourceClipRelation.Add(moodboard, moodboardClips);
        sourceClipRelation.Add(strings, stringsClips);
        sourceClipRelation.Add(bossstrings, bossstringsClips);
        sourceClipRelation.Add(boss1, boss1Clips);
        sourceClipRelation.Add(boss2, boss2Clips);
        sourceClipRelation.Add(homebase, homebaseClips);
        sourceClipRelation.Add(phase1, phase1Clips);
        sourceClipRelation.Add(phase2, phase1Clips);
        sourceClipRelation.Add(hurt, hurtClips);
        sourceClipRelation.Add(flare, flareClips);
        sourceClipRelation.Add(hoof, hoofClips);
        sourceClipRelation.Add(damage, damageClips);
        sourceClipRelation.Add(death, deathClips);
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
           
        toPlay.clip = sourceClipRelation[toPlay][Random.Range(0, sourceClipRelation[toPlay].Length)];
        toPlay.pitch = Random.Range(low, high);
        toPlay.Play();

    }
}
