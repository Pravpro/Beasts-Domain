using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.Assertions;
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
        public AudioMixerGroup arena;
        public AudioMixerGroup training;
        public AudioMixerGroup environment;
        public AudioMixerGroup fire;
        public AudioMixerGroup geyser;
        public AudioMixerGroup master;
        public AudioMixerGroup music;
        public AudioMixerGroup player;
        public AudioMixerGroup rocks;
        public AudioMixerGroup sfx;
        public AudioMixerGroup homebase;
        public AudioMixerGroup trees;
        public AudioMixerGroup weapons;
    }

    [System.Serializable]
    public class Snapshots
    {
        public AudioMixerSnapshot homebase;
        public AudioMixerSnapshot training;
        public AudioMixerSnapshot pauseArena;
        public AudioMixerSnapshot pauseHomeBase;
        public AudioMixerSnapshot pauseTraining;
        public AudioMixerSnapshot arena;
        public AudioMixerSnapshot alleys;
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
            public AudioClip[] walking;
            [Range(0f, 1f)] public float walkingVol;
            public AudioClip[] running;
            [Range(0f, 1f)] public float runningVol;
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
            public AudioClip[] roar1;
            [Range(0f, 1f)] public float roar1Vol;
            public AudioClip[] roar2;
            [Range(0f, 1f)] public float roar2Vol;
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
        public AudioClip[] bossStinger;
        [Range(0f, 1f)] public float bossStingerVol;
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

    public enum SnapshotState
    {
        Homebase = 0,
        Training = 1,
        Arena = 2,
        Alleys = 3,
        PauseHombase = 4,
        PauseTraining = 5,
        PauseArena = 6,
        ResumeHomebase = 7,
        ResumeTraining = 8,
        ResumeArena = 9
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
    [HideInInspector] public AudioSource walking; //pitch: 0.8, 1.2
    [HideInInspector] public AudioSource running; //pitch: 0.8, 1.2
    [HideInInspector] public AudioSource slingshotDraw;
    [HideInInspector] public AudioSource slingshotRelease;
    [HideInInspector] public AudioSource rock; //pitch: 0.6, 1.3
    [HideInInspector] public AudioSource spell;
    [HideInInspector] public AudioSource moodboard;
    [HideInInspector] public AudioSource strings;
    [HideInInspector] public AudioSource bossStrings;
    [HideInInspector] public AudioSource bossStinger;
    [HideInInspector] public AudioSource boss1;
    [HideInInspector] public AudioSource boss2;
    [HideInInspector] public AudioSource homebase;
    [HideInInspector] public AudioSource roar1;
    [HideInInspector] public AudioSource roar2;
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
        landing = AddAudio(false, sfxClips.player.landingVol, mixerGroups.action); // Params: 1.loop 2.Volume 3.Mixer Group
        jumping = AddAudio(false, sfxClips.player.jumpingVol, mixerGroups.action);
        stamina = AddAudio(false, sfxClips.player.staminaVol, mixerGroups.action);
        walking = AddAudio(false, sfxClips.player.walkingVol, mixerGroups.action);
        running = AddAudio(false, sfxClips.player.runningVol, mixerGroups.action);
        damage = AddAudio(false, sfxClips.player.damageVol, mixerGroups.action);
        death = AddAudio(false, sfxClips.player.deathVol, mixerGroups.action);
        boulder = AddAudio(false, sfxClips.player.boulderVol, mixerGroups.action);
        slingshotDraw = AddAudio(false, sfxClips.player.slingshotDrawVol, mixerGroups.weapons);
        slingshotRelease = AddAudio(false, sfxClips.player.slingshotReleaseVol, mixerGroups.weapons);
        rock = AddAudio(false, sfxClips.player.rockVol, mixerGroups.weapons);
        spell = AddAudio(false, sfxClips.player.spellVol, mixerGroups.weapons);
        // SFX: Beast sources
        roar1 = AddAudio(false, sfxClips.beast.roar1Vol, mixerGroups.beast);
        roar2 = AddAudio(false, sfxClips.beast.roar2Vol, mixerGroups.beast);
        hurt = AddAudio(false, sfxClips.beast.hurtVol, mixerGroups.beast);
        flare = AddAudio(false, sfxClips.beast.flareVol, mixerGroups.beast);
        hoof = AddAudio(false, sfxClips.beast.hoofVol, mixerGroups.beast);
        // Music sources
        moodboard = AddAudio(true, musicClips.moodboardVol, mixerGroups.training);
        strings = AddAudio(true, musicClips.stringsVol, mixerGroups.arena);
        bossStrings = AddAudio(true, musicClips.bossstringsVol, mixerGroups.arena);
        bossStinger = AddAudio(false, musicClips.bossstringsVol, mixerGroups.beast);
        boss1 = AddAudio(true, musicClips.boss1Vol, mixerGroups.arena);
        boss2 = AddAudio(true, musicClips.boss2Vol, mixerGroups.arena);
        homebase = AddAudio(true, musicClips.homebaseVol, mixerGroups.homebase);
        // Environment sources
        tree = AddAudio(false, environmentClips.treeVol, mixerGroups.trees);
        birds = AddAudio(true, environmentClips.birdsVol, mixerGroups.birds);
        fire = AddAudio(true, environmentClips.fireVol, mixerGroups.fire);
        geyser = AddAudio(false, environmentClips.geyserVol, mixerGroups.geyser);
        rockSlide = AddAudio(false, environmentClips.rockSlideVol, mixerGroups.rocks);


        sourceClipRelation.Add(landing, sfxClips.player.landing);
        sourceClipRelation.Add(jumping, sfxClips.player.jumping);
        sourceClipRelation.Add(stamina, sfxClips.player.stamina);
        sourceClipRelation.Add(walking, sfxClips.player.walking);
        sourceClipRelation.Add(running, sfxClips.player.running);
        sourceClipRelation.Add(damage, sfxClips.player.damage);
        sourceClipRelation.Add(death, sfxClips.player.death);
        sourceClipRelation.Add(boulder, sfxClips.player.boulder);
        sourceClipRelation.Add(slingshotDraw, sfxClips.player.slingshotDraw);
        sourceClipRelation.Add(slingshotRelease, sfxClips.player.slingshotRelease);
        sourceClipRelation.Add(rock, sfxClips.player.rock);
        sourceClipRelation.Add(spell, sfxClips.player.spell);
        sourceClipRelation.Add(roar1, sfxClips.beast.roar1);
        sourceClipRelation.Add(roar2, sfxClips.beast.roar1);
        sourceClipRelation.Add(hurt, sfxClips.beast.hurt);
        sourceClipRelation.Add(flare, sfxClips.beast.flare);
        sourceClipRelation.Add(hoof, sfxClips.beast.hoof);
        sourceClipRelation.Add(moodboard, musicClips.moodboard);
        sourceClipRelation.Add(strings, musicClips.strings);
        sourceClipRelation.Add(bossStrings, musicClips.bossstrings);
        sourceClipRelation.Add(bossStinger, musicClips.bossStinger);
        sourceClipRelation.Add(boss1, musicClips.boss1);
        sourceClipRelation.Add(boss2, musicClips.boss2);
        sourceClipRelation.Add(homebase, musicClips.homebase);
        sourceClipRelation.Add(tree, environmentClips.tree);
        sourceClipRelation.Add(birds, environmentClips.birds);
        sourceClipRelation.Add(fire, environmentClips.fire);
        sourceClipRelation.Add(geyser, environmentClips.geyser);
        sourceClipRelation.Add(rockSlide, environmentClips.rockSlide);

        
    }



    AudioSource AddAudio(bool loop, float vol, AudioMixerGroup output)
    {
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.volume = vol;
        newAudio.outputAudioMixerGroup = output;
        return newAudio;
    }

    public void Play(AudioSource toPlay)
    {
        toPlay.clip = sourceClipRelation[toPlay][Random.Range(0, sourceClipRelation[toPlay].Length)];
        toPlay.Play();

    }

    public void Play(AudioSource toPlay, float[] pitch )
    {
        Assert.AreEqual(2, pitch.Length);
        toPlay.pitch = Random.Range(pitch[0], pitch[1]);
        Play(toPlay);

    }

    public void Play(AudioSource toPlay, float blend )
    {
        toPlay.spatialBlend = blend;
        Play(toPlay);
    }

    public void Play(AudioSource toPlay, float blend, float[] pitch)
    {
        toPlay.spatialBlend = blend;
        Play(toPlay, pitch);
    }

    public AudioSource Localize(GameObject target, AudioSource source)
    {
        AudioSource newAudio = target.AddComponent<AudioSource>();
        newAudio.loop = source.loop;
        newAudio.volume = source.volume;
        newAudio.outputAudioMixerGroup = source.outputAudioMixerGroup;
        sourceClipRelation.Add(newAudio, sourceClipRelation[source]);
        Destroy(source);
        return newAudio;
    }

    public void PlayPhaseOne()
    {
        strings.Stop();
        Play(roar1);
        Play(bossStinger);
        boss1.clip = sourceClipRelation[boss1][Random.Range(0, sourceClipRelation[boss1].Length)];
        boss1.PlayDelayed(2.5f);
        bossStrings.clip = sourceClipRelation[bossStrings][Random.Range(0, sourceClipRelation[bossStrings].Length)];
        bossStrings.PlayDelayed(2.5f);
    }

    public void PlayPhaseTwo()
    {
        boss1.Stop();
        bossStrings.Stop();

        Play(roar2);
        boss2.clip = sourceClipRelation[boss2][Random.Range(0, sourceClipRelation[boss2].Length)];
        boss2.PlayDelayed(5.5f);
        bossStrings.clip = sourceClipRelation[bossStrings][Random.Range(0, sourceClipRelation[bossStrings].Length)];
        bossStrings.PlayDelayed(5.5f);
    }

    public void SetVolume(SnapshotState state)
    {
        Debug.Log(state);
        switch (state)
        {
            case SnapshotState.Homebase:
                snapshots.homebase.TransitionTo(2f);
                break;
            case SnapshotState.Training:
                snapshots.training.TransitionTo(2f);
                break;
            case SnapshotState.Arena:
                snapshots.arena.TransitionTo(2f);
                break;
            case SnapshotState.Alleys:
                snapshots.alleys.TransitionTo(2f);
                break;
            case SnapshotState.PauseHombase:
                snapshots.pauseHomeBase.TransitionTo(0f);
                break;
            case SnapshotState.PauseTraining:
                snapshots.pauseTraining.TransitionTo(0f);
                break;
            case SnapshotState.PauseArena:
                snapshots.pauseArena.TransitionTo(0f);
                break;
            case SnapshotState.ResumeHomebase:
                snapshots.homebase.TransitionTo(0f);
                break;
            case SnapshotState.ResumeTraining:
                snapshots.training.TransitionTo(0f);
                break;
            case SnapshotState.ResumeArena:
                snapshots.arena.TransitionTo(0f);
                break;
            
        }
    }
}

