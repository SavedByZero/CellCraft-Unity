using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SfxManager
{
    const int MAX_ChannelS = 6;

    static SfxManager _instance;
    public static SfxManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SfxManager();
            }

            return _instance;
        }
    }

    private Dictionary<SFX, AudioClip> AudioClips { get; set; }

    private List<AudioSource> channels;
    private GameObject audioSourcePrefab;
    private bool isMuted = false;

    public SfxManager()
    {
        audioSourcePrefab = Resources.Load<GameObject>("AudioChannel");
        AudioClips = new Dictionary<SFX, AudioClip>();
        channels = new List<AudioSource>();
        getOpenChannel();
    }


    ////////////////////////////
    ///    AUDIO METHODS
    ///////////////////////////

    public static void Play(SFX s)
    {
        var m = SfxManager.Instance;

        AudioClip clip;
        if (m.AudioClips.TryGetValue(s, out clip) == false)
        {
            clip = m.LoadResource(s);
            m.AudioClips.Add(s, clip);
        }

        var c = m.getOpenChannel();
        c.clip = clip;
        c.Play();
        c.mute = m.isMuted;
    }

    public static void StopAll()
    {
        var m = SfxManager.Instance;
        m.channels.ForEach(c => c.Stop());
    }

    public static void SetVolume(float v)
    {
        var m = SfxManager.Instance;
        m.channels.ForEach(c => c.volume = v);
    }

    public static float GetVolume()
    {
        var m = SfxManager.Instance;
        return m.channels.First().volume;
    }

    public static void SetMute(bool mute)
    {
        var m = SfxManager.Instance;
        m.channels.ForEach(c => c.mute = mute);
        m.isMuted = mute;
        //DataManager.GetModel<SavedSettingsModel>().SfxMute = mute;
    }

    public static bool getMute()
    {
        return SfxManager.Instance.isMuted;
    }

    ////////////////////////////
    ///    ChannelS
    ///////////////////////////
    //  Each AudioSource Prefab is a sound channel.
    //  If all channels are busy, it opens a new channel.

    private AudioSource getOpenChannel()
    {
        var openChannel = channels.FirstOrDefault(c => c.isPlaying == false);
        if (openChannel == null)
        {
            //if max channels reached, just pass the first one to be overwritten
            if (channels.Count >= MAX_ChannelS) return channels.First();

            //creates a new channel if all channels are busy
            var audioSource = GameObject.Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
            channels.Add(audioSource);
            openChannel = audioSource;
        }

        return openChannel;
    }

    public static List<AudioSource> GetChannels()
    {
        return SfxManager.Instance.channels;
    }


    ////////////////////////////
    ///    ASSETS
    ///////////////////////////

    private AudioClip LoadResource(SFX s)
    {
        string loc = "SFX/";
        switch (s)
        {
            case SFX.SFXAbsorb:
                return Resources.Load<AudioClip>(loc + "SFXAbsorb");
          
            case SFX.SFXBattleHit:
                return Resources.Load<AudioClip>(loc + "SFXBattleHit");
           case SFX.SFXBlipSteps:
                return Resources.Load<AudioClip>(loc + "SFXBlipSteps");
           case SFX.SFXBlock:
                return Resources.Load<AudioClip>(loc + "SFXBlock");
           case SFX.SFXClickForNext:
                return Resources.Load<AudioClip>(loc + "SFXClickForNext");
           case SFX.SFXCoin:
                return Resources.Load<AudioClip>(loc + "SFXCoin");
           case SFX.SFXDiscovery:
                return Resources.Load<AudioClip>(loc + "SFXDiscovery");
           case SFX.SFXDiscoveryTwinkle:
                return Resources.Load<AudioClip>(loc + "SFXDiscoveryTwinkle");
           case SFX.SFXDivide:
                return Resources.Load<AudioClip>(loc + "SFXDivide");
           case SFX.SFXDrain:
                return Resources.Load<AudioClip>(loc + "SFXDrain");
            case SFX.SFXDropFall:
                return Resources.Load<AudioClip>(loc + "SFXDropFall");
            case SFX.SFXFanfare:
                return Resources.Load<AudioClip>(loc + "SFXFanfare");
            case SFX.SFXHeal:
                return Resources.Load<AudioClip>(loc + "SFXHeal");
            case SFX.SFXHurt:
                return Resources.Load<AudioClip>(loc + "SFXHurt");
            case SFX.SFXMudSlide:
                return Resources.Load<AudioClip>(loc + "SFXMudSlide");
            case SFX.SFXMudStep:
                return Resources.Load<AudioClip>(loc + "SFXMudStep");
            case SFX.SFXMultiDrain:
                return Resources.Load<AudioClip>(loc + "SFXMultiDrain");
            case SFX.SFXNewAction:
                return Resources.Load<AudioClip>(loc + "SFXNewAction");
            case SFX.SFXPageTurn:
                return Resources.Load<AudioClip>(loc + "SFXPageTurn");
            case SFX.SFXPop1:
                return Resources.Load<AudioClip>(loc + "SFXPop1");
           case SFX.SFXPop2:
                return Resources.Load<AudioClip>(loc + "SFXPop2");
           case SFX.SFXRadicalRise:
                return Resources.Load<AudioClip>(loc + "SFXRadicalRise");
           case SFX.SFXRepairRise:
                return Resources.Load<AudioClip>(loc + "SFXRepairRise");
           case SFX.SFXShield:
                return Resources.Load<AudioClip>(loc + "SFXShield");
           case SFX.SFXSlicerRise:
                return Resources.Load<AudioClip>(loc + "SFXSlicerRise");
           case SFX.SFXSplish:
                return Resources.Load<AudioClip>(loc + "SFXSplish");
           case SFX.SFXTitleOpenerHit:
                return Resources.Load<AudioClip>(loc + "SFXTitleOpenerHit");
           case SFX.SFXTitleOpenerShort:
                return Resources.Load<AudioClip>(loc + "SFXTitleOpenerShort");
           case SFX.SFXToxin:
                return Resources.Load<AudioClip>(loc + "SFXToxin");
           case SFX.SFXVirusInfect:
                return Resources.Load<AudioClip>(loc + "SFXVirusInfect");
           case SFX.SFXVirusRise:
                return Resources.Load<AudioClip>(loc + "SFXVirusRise");
           case SFX.SFXWriting:
                return Resources.Load<AudioClip>(loc + "SFXWriting");
           case SFX.SFXZap:
                return Resources.Load<AudioClip>(loc + "SFXZap");
           case SFX.SFXZlap:
                return Resources.Load<AudioClip>(loc + "SFXZlap");
          
           
        }
        return null;
    }


}

public enum SFX
{
    SFXAbsorb,
    SFXBattleHit,
    SFXBlipSteps,
    SFXBlock,
    SFXClickForNext,
    SFXCoin,
    SFXDiscovery,
    SFXDiscoveryTwinkle,
    SFXDiscoveryTwinkleLow,
    SFXDivide,
    SFXDrain,
    SFXDropFall,
    SFXFanfare,
    SFXHeal,
    SFXHurt,
    SFXMudSlide,
    SFXMudStep,
    SFXMultiDrain,
    SFXNewAction,
    SFXPageTurn,
    SFXPop1,
    SFXPop2,
    SFXRadicalRise,
    SFXRepairRise,
    SFXShield,
    SFXSlicerRise,
    SFXSplish,
    SFXTitleOpenerHit,
    SFXTitleOpenerShort,
    SFXToxin,
    SFXVirusInfect,
    SFXVirusRise,
    SFXWriting,
    SFXZap,
    SFXZlap


}
