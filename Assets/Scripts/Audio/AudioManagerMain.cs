﻿using System.Collections;
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
        [System.Serializable]
        public class PlayerClips
        {
            public AudioClip[] landing;
            [Range(0f, 1f)] public float landingVol;
            public AudioClip[] jumping;
            [Range(0f, 1f)] public float jumpingVol;
            public AudioClip[] stamina;
            [Range(0f, 1f)] public float staminaVol;
            public AudioClip[] footstep;
            [Range(0f, 1f)] public float footstepVol;
            public AudioClip[] damage;
            [Range(0f, 1f)] public float damageVol;
            public AudioClip[] death;
            [Range(0f, 1f)] public float deathVol;
            public AudioClip[] boulder;
            [Range(0f, 1f)] public float boulderVol;
            public AudioClip[] slingshotDraw;
            [Range(0f, 1f)] public float slingshotDrawVol;
            public AudioClip[] slingshotRelease;
            [Range(0f, 1f)] public float slingshotReleaseVol;
            public AudioClip[] rock;
            [Range(0f, 1f)] public float rockVol;
            public AudioClip[] spell;
            [Range(0f, 1f)] public float spellVol;
        }

        [System.Serializable]
        public class BeastClips
        {
            public AudioClip[] phase1;
            [Range(0f, 1f)] public float phase1Vol;
            public AudioClip[] phase2;
            [Range(0f, 1f)] public float phase2Vol;
            public AudioClip[] hurt;
            [Range(0f, 1f)] public float hurtVol;
            public AudioClip[] flare;
            [Range(0f, 1f)] public float flareVol;
            public AudioClip[] hoof;
            [Range(0f, 1f)] public float hoofVol;
        }

        public PlayerClips player;
        public BeastClips beast;
    }

    [System.Serializable]
    public class MusicClips
    {
        public AudioClip[] strings;
        [Range(0f, 1f)] public float stringsVol;
        public AudioClip[] bossstrings;
        [Range(0f, 1f)] public float bossstringsVol;
        public AudioClip[] boss1;
        [Range(0f, 1f)] public float boss1Vol;
        public AudioClip[] boss2;
        [Range(0f, 1f)] public float boss2Vol;
        public AudioClip[] moodboard;
        [Range(0f, 1f)] public float moodboardVol;
        public AudioClip[] homebase;
        [Range(0f, 1f)] public float homebaseVol;
    }

    [System.Serializable]
    public class EnvironmentClips
    {
        public AudioClip[] tree;
        [Range(0f, 1f)] public float treeVol;
        public AudioClip[] birds;
        [Range(0f, 1f)] public float birdsVol;
        public AudioClip[] fire;
        [Range(0f, 1f)] public float fireVol;
        public AudioClip[] geyser;
        [Range(0f, 1f)] public float geyserVol;
        public AudioClip[] rockSlide;
        [Range(0f, 1f)] public float rockSlideVol;
    }

    [Space(6)]
    public MixerGroups mixerGroups;
    [Space(6)]
    public Snapshots snapshots;

    [Header("Clips")]
    public SfxClips sfxClips;
    public MusicClips musicClips;
    public EnvironmentClips environmentClips;
    

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
        // SFX: Player sources
        landing = AddAudio(false, false, sfxClips.player.landingVol, mixerGroups.action); // Params: 1.Loop 2.Play on Awake 3.Volume 4. Mixer Group
        jumping = AddAudio(false, false, sfxClips.player.jumpingVol, mixerGroups.action);
        stamina = AddAudio(false, false, sfxClips.player.staminaVol, mixerGroups.action);
        footstep = AddAudio(false, false, sfxClips.player.footstepVol, mixerGroups.action);
        damage = AddAudio(false, false, sfxClips.player.damageVol, mixerGroups.action);
        death = AddAudio(false, false, sfxClips.player.deathVol, mixerGroups.action);
        boulder = AddAudio(true, false, sfxClips.player.boulderVol, mixerGroups.action);
        slingshotDraw = AddAudio(false, false, sfxClips.player.slingshotDrawVol, mixerGroups.weapons);
        slingshotRelease = AddAudio(false, false, sfxClips.player.slingshotReleaseVol, mixerGroups.weapons);
        rock = AddAudio(false, false, sfxClips.player.rockVol, mixerGroups.weapons);
        spell = AddAudio(false, false, sfxClips.player.spellVol, mixerGroups.weapons);
        // SFX: Beast sources
        phase1 = AddAudio(false, false, sfxClips.beast.phase1Vol, mixerGroups.beast);
        phase2 = AddAudio(false, false, sfxClips.beast.phase2Vol, mixerGroups.beast);
        hurt = AddAudio(false, false, sfxClips.beast.hurtVol, mixerGroups.beast);
        flare = AddAudio(false, false, sfxClips.beast.flareVol, mixerGroups.beast);
        hoof = AddAudio(false, false, sfxClips.beast.hoofVol, mixerGroups.beast);
        // Music sources
        moodboard = AddAudio(true, true, musicClips.moodboardVol, mixerGroups.choir);
        strings = AddAudio(true, true, musicClips.stringsVol, mixerGroups.themes);
        bossstrings = AddAudio(true, false, musicClips.bossstringsVol, mixerGroups.bossFight);
        boss1 = AddAudio(true, false, musicClips.boss1Vol, mixerGroups.bossFight);
        boss2 = AddAudio(true, false, musicClips.boss2Vol, mixerGroups.bossFight);
        homebase = AddAudio(true, true, musicClips.homebaseVol, mixerGroups.themes);
        // Environment sources
        tree = AddAudio(false, false, environmentClips.treeVol, mixerGroups.trees);
        birds = AddAudio(false, true, environmentClips.birdsVol, mixerGroups.birds);
        fire = AddAudio(false, true, environmentClips.fireVol, mixerGroups.fire);
        geyser = AddAudio(false, false, environmentClips.geyserVol, mixerGroups.geyser);
        rockSlide = AddAudio(false, false, environmentClips.rockSlideVol, mixerGroups.rocks);


        sourceClipRelation.Add(landing, sfxClips.player.landing);
        sourceClipRelation.Add(jumping, sfxClips.player.jumping);
        sourceClipRelation.Add(stamina, sfxClips.player.stamina);
        sourceClipRelation.Add(footstep, sfxClips.player.footstep);
        sourceClipRelation.Add(damage, sfxClips.player.damage);
        sourceClipRelation.Add(death, sfxClips.player.death);
        sourceClipRelation.Add(boulder, sfxClips.player.boulder);
        sourceClipRelation.Add(slingshotDraw, sfxClips.player.slingshotDraw);
        sourceClipRelation.Add(slingshotRelease, sfxClips.player.slingshotRelease);
        sourceClipRelation.Add(rock, sfxClips.player.rock);
        sourceClipRelation.Add(spell, sfxClips.player.spell);
        sourceClipRelation.Add(phase1, sfxClips.beast.phase1);
        sourceClipRelation.Add(phase2, sfxClips.beast.phase1);
        sourceClipRelation.Add(hurt, sfxClips.beast.hurt);
        sourceClipRelation.Add(flare, sfxClips.beast.flare);
        sourceClipRelation.Add(hoof, sfxClips.beast.hoof);
        sourceClipRelation.Add(moodboard, musicClips.moodboard);
        sourceClipRelation.Add(strings, musicClips.strings);
        sourceClipRelation.Add(bossstrings, musicClips.bossstrings);
        sourceClipRelation.Add(boss1, musicClips.boss1);
        sourceClipRelation.Add(boss2, musicClips.boss2);
        sourceClipRelation.Add(homebase, musicClips.homebase);
        sourceClipRelation.Add(tree, environmentClips.tree);
        sourceClipRelation.Add(birds, environmentClips.birds);
        sourceClipRelation.Add(fire, environmentClips.fire);
        sourceClipRelation.Add(geyser, environmentClips.geyser);
        sourceClipRelation.Add(rockSlide, environmentClips.rockSlide);

        
    }



    AudioSource AddAudio(bool loop, bool playAwake, float vol, AudioMixerGroup output)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        newAudio.outputAudioMixerGroup = output;
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

