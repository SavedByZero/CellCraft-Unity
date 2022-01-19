using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class MusicManager
{

    static MusicManager _instance;
    public static MusicManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MusicManager();
            }

            return _instance;
        }
    }

    public static Music CurrentMusic { get; set; }

    private Dictionary<Music, AudioClip> AudioClips { get; set; }
    //private Dictionary<Music, float[]> StartTimes;
    private AudioSource channel;
    private GameObject audioSourcePrefab;
    private float Vol = 0;
    private float VolumeMultiplier = .5f; //50% of max volume
    private bool isMuted = false;

    public MusicManager()
    {
        AudioClips = new Dictionary<Music, AudioClip>();
        audioSourcePrefab = Resources.Load<GameObject>("AudioChannel");
        channel = GameObject.Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
        channel.loop = true;

        /*StartTimes = new Dictionary<Music, float[]>
        {
           { Music.ForestZone_BGM,    new float[] { 0, 104, 164 } },
           { Music.MountainZone_BGM,  new float[] { 0, 78 , 183 } },
           { Music.TundraZone_BGM,    new float[] { 0, 113, 205 } },
           { Music.DesertZone_BGM,    new float[] { 4, 93 , 4 } },
           { Music.SwampZone_BGM,     new float[] { 0, 106, 176 } },
           { Music.GameHub_BGM,       new float[] { 0, 0, 0 } },
           { Music.MonitorTone,       new float[] { 0, 0, 0 } },
        };*/

    }

    ////////////////////////////
    ///    AUDIO METHODS
    ///////////////////////////

    public static void Play(Music type)
    {
        var m = MusicManager.Instance;
        MusicManager.CurrentMusic = type;

        AudioClip clip;
        if (m.AudioClips.TryGetValue(type, out clip) == false)
        {
            clip = m.LoadResource(type);
            m.AudioClips.Add(type, clip);
        }

        //If no AudioSource loaded yet, loads one
        if (m.channel == null)
            m.channel = GameObject.Instantiate(m.audioSourcePrefab).GetComponent<AudioSource>();


        m.channel.clip = clip;

        //gets a random starting point (3 options) based on Type        
        //var start = m.StartTimes[type][(int)System.Math.Floor((double)Random.value * 3)];
        m.channel.time = 0;
        m.channel.Play();
        //m.channel.time = start;

        //Tweens music from 0 to volume
        if (m.channel.mute == false)
        {
           // DOTween.To(() => m.channel.volume, x => m.channel.volume, m.Vol, 1);
          //  m.channel.volume = 0;
           // m.channel.DOFade(m.VolumeMultiplier * GetVolume(), 2);
        }

        m.channel.mute = m.isMuted;
    }

    public static void Stop()
    {
        var m = MusicManager.Instance;
        m.channel.Stop();

        MusicManager.CurrentMusic = Music.None;
    }

    public static void SetVolume(float v, bool ignoreMultiplier = false)
    {
        var m = MusicManager.Instance;

        //If no AudioSource loaded yet, loads one
        if (m.channel == null)
            m.channel = GameObject.Instantiate(m.audioSourcePrefab).GetComponent<AudioSource>();

        m.Vol = v;

        if (ignoreMultiplier)
        {
            m.channel.volume = v;
        }
        else
        {
            m.channel.volume = m.VolumeMultiplier * v;
        }
    }

    public static float GetVolume()
    {
        var m = MusicManager.Instance;
        return m.Vol;
    }

    public static void SetMute(bool mute)
    {
        var m = MusicManager.Instance;
        m.channel.mute = mute;
        m.isMuted = mute;
       
    }


    public static bool getMute()
    {
        return MusicManager.Instance.isMuted;
    }

    ////////////////////////////
    ///    ASSETS
    ///////////////////////////

    private AudioClip LoadResource(Music type)
    {
        string loc = "Music/";
        switch (type)
        {
            case Music.MusicBattle:
                return Resources.Load<AudioClip>(loc + "MusicBattle");
             case Music.MusicBattleIntro:
                return Resources.Load<AudioClip>(loc + "MusicBattleIntro");
             case Music.MusicCalm:
                return Resources.Load<AudioClip>(loc + "MusicCalm");
             case Music.MusicCredits:
                return Resources.Load<AudioClip>(loc + "MusicCredits");
             case Music.MusicFinale:
                return Resources.Load<AudioClip>(loc + "MusicFinale");
             case Music.MusicScene:
                return Resources.Load<AudioClip>(loc + "MusicScene");
             case Music.MusicTitleLoop:
                return Resources.Load<AudioClip>(loc + "MusicTitleLoop");
            
           
        }
        return null;
    }


}

public enum Music
{
    None,
    MusicBattle,
    MusicBattleIntro,
    MusicCalm,
    MusicCredits,
    MusicFinale,
    MusicScene,
    MusicTitleLoop

}
