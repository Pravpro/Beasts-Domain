using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;


public class AudioManagerMain : MonoBehaviour
{
    private Dictionary<AudioSource, AudioClip[]> sourceClipRelation = new Dictionary<AudioSource, AudioClip[]>();
    private Dictionary<AudioSource, AudioMixerGroup> sourceGroupRelation = new Dictionary<AudioSource, AudioMixerGroup>();


    [System.Serializable]
    public class MixerGroups
    {
        public AudioMixerGroup action;
        public AudioMixerGroup beast;
        public AudioMixerGroup birds;
        public AudioMixerGroup bossFight;
        public AudioMixerGroup choir;
        public AudioMixerGroup environment;
        public AudioMixerGroup fire;
        public AudioMixerGroup geyser;
        public AudioMixerGroup master;
        public AudioMixerGroup music;
        public AudioMixerGroup player;
        public AudioMixerGroup rocks;
        public AudioMixerGroup sfx;
        public AudioMixerGroup themes;
        public AudioMixerGroup trees;
        public AudioMixerGroup weapons;
    }

    [System.Serializable]
    public class Snapshots
    {
        public AudioMixerSnapshot homebase;
        public AudioMixerSnapshot moodboard;
        public AudioMixerSnapshot strings;
        public AudioMixerSnapshot pause;
        public AudioMixerSnapshot boss;
    }

    [System.Serializable]
    public class SfxClips
    {
        public AudioClip[] landingClips;
        public AudioClip[] jumpingClips;
        public AudioClip[] staminaClips;
        public AudioClip[] footstepClips;
        public AudioClip[] slingshotDrawClips;
        public AudioClip[] slingshotReleaseClips;
        public AudioClip[] rockClips;
        public AudioClip[] spellClips;
        public AudioClip[] damageClips;
        public AudioClip[] deathClips;
        public AudioClip[] treeClips;
        public AudioClip[] birdsClips;
        public AudioClip[] fireClips;
        public AudioClip[] geyserClips;
        public AudioClip[] boulderClips;
        public AudioClip[] rockSlideClips;
    }

    [System.Serializable]
    public class MusicClips
    {
        public AudioClip[] stringsClips;
        public AudioClip[] bossstringsClips;
        public AudioClip[] boss1Clips;
        public AudioClip[] boss2Clips;
        public AudioClip[] moodboardClips;
        public AudioClip[] homebaseClips;
    }

    [System.Serializable]
    public class MonsterClips
    {
        public AudioClip[] phase1Clips;
        public AudioClip[] phase2Clips;
        public AudioClip[] hurtClips;
        public AudioClip[] flareClips;
        public AudioClip[] hoofClips;
    }

    [Space(6)]
    public MixerGroups mixerGroups;
    [Space(6)]
    public Snapshots snapshots;

    [Header("Clips")]
    public SfxClips sfxClips;
    public MusicClips musicClips;
    public MonsterClips monsterClips;
    

    [HideInInspector] public AudioSource landing; // pitch: 0.7, 1.3
    [HideInInspector] public AudioSource jumping; // pitch: 1.0, 1.1
    [HideInInspector] public AudioSource stamina;
    [HideInInspector] public AudioSource footstep; //pitch: 0.8, 1.2
    [HideInInspector] public AudioSource slingshotDraw;
    [HideInInspector] public AudioSource slingshotRelease;
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
    [HideInInspector] public AudioSource birds; //pitch: 0.9, 1.1
    [HideInInspector] public AudioSource fire;
    [HideInInspector] public AudioSource geyser;
    [HideInInspector] public AudioSource boulder;
    [HideInInspector] public AudioSource rockSlide;

    void Awake()
    {
        // Params: 1.Loop 2.Play on Awake 3.Volume
        landing = AddAudio(false, false, 0.6f);
        landing.outputAudioMixerGroup = mixerGroups.action;

        jumping = AddAudio(false, false, 0.7f);
        jumping.outputAudioMixerGroup = mixerGroups.action;

        stamina = AddAudio(false, false, 0.7f);
        footstep = AddAudio(false, false, 0.4f);
        slingshotDraw = AddAudio(false, false, 0.7f);
        slingshotRelease = AddAudio(false, false, 0.7f);
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
        birds = AddAudio(false, true, 0.3f);
        fire = AddAudio(false, true, 0.4f);
        geyser = AddAudio(false, false, 0.8f);
        boulder = AddAudio(true, false, 0.4f);
        rockSlide = AddAudio(false, false, 0.4f);

        sourceClipRelation.Add(landing, sfxClips.landingClips);
        sourceClipRelation.Add(jumping, sfxClips.jumpingClips);
        sourceClipRelation.Add(stamina, sfxClips.staminaClips);
        sourceClipRelation.Add(footstep, sfxClips.footstepClips);
        sourceClipRelation.Add(slingshotDraw, sfxClips.slingshotDrawClips);
        sourceClipRelation.Add(slingshotRelease, sfxClips.slingshotReleaseClips);
        sourceClipRelation.Add(rock, sfxClips.rockClips);
        sourceClipRelation.Add(spell, sfxClips.spellClips);
        sourceClipRelation.Add(moodboard, musicClips.moodboardClips);
        sourceClipRelation.Add(strings, musicClips.stringsClips);
        sourceClipRelation.Add(bossstrings, musicClips.bossstringsClips);
        sourceClipRelation.Add(boss1, musicClips.boss1Clips);
        sourceClipRelation.Add(boss2, musicClips.boss2Clips);
        sourceClipRelation.Add(homebase, musicClips.homebaseClips);
        sourceClipRelation.Add(phase1, monsterClips.phase1Clips);
        sourceClipRelation.Add(phase2, monsterClips.phase1Clips);
        sourceClipRelation.Add(hurt, monsterClips.hurtClips);
        sourceClipRelation.Add(flare, monsterClips.flareClips);
        sourceClipRelation.Add(hoof, monsterClips.hoofClips);
        sourceClipRelation.Add(damage, sfxClips.damageClips);
        sourceClipRelation.Add(death, sfxClips.deathClips);
        sourceClipRelation.Add(tree, sfxClips.treeClips);
        sourceClipRelation.Add(birds, sfxClips.birdsClips);
        sourceClipRelation.Add(fire, sfxClips.fireClips);
        sourceClipRelation.Add(geyser, sfxClips.geyserClips);
        sourceClipRelation.Add(boulder, sfxClips.boulderClips);
        sourceClipRelation.Add(rockSlide, sfxClips.rockSlideClips);

        
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

    public void HomebaseVolume()
    {
        snapshots.homebase.TransitionTo(2f);
    }

    public void MoodboardVolume()
    {
        snapshots.moodboard.TransitionTo(2f);
    }

    public void StringsVolume()
    {
        snapshots.strings.TransitionTo(2f);
    }

    public void BossVolume()
    {
        snapshots.boss.TransitionTo(0f);
    }

    public void PauseVolume()
    {
        snapshots.pause.TransitionTo(0f);
    }
}

