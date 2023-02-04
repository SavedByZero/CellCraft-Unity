using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public CellcraftLevel Level;
    private Terrain _terrain;
    private Cell _cell;
    private Engine _engine;
    // Start is called before the first frame update
    void Start()
    {
        _engine = GetComponentInChildren<Engine>();
        _terrain = GetComponentInChildren<Terrain>();
        _cell = GetComponentInChildren<Cell>();
        LoadLevel();
        populateInfo();
    }

    public void LoadLevel(string xml_filename = "level_00")
    {
        TextAsset _xml = Resources.Load(xml_filename) as TextAsset;
        XmlSerializer levelSerializer = new XmlSerializer(typeof(CellcraftLevel));
        StringReader reader = new StringReader(_xml.ToString());
        Level = levelSerializer.Deserialize(reader) as CellcraftLevel;
       // Debug.Log("level data: " + level);
        //  StartCoroutine(afterLoad(levelInfo));


    }

    void populateInfo()
    {
        _terrain.SwitchSkin(Level.Info.Backdrop.Name);
        int atp = (int)Level.Info.CurrentResources.atp;
        int na = (int)Level.Info.CurrentResources.na;
        int aa = (int)Level.Info.CurrentResources.aa;
        int fa = (int)Level.Info.CurrentResources.fa;
        int g = (int)Level.Info.CurrentResources.g;
        _cell.setResources(atp, na,aa,fa, g);
        _engine.r_atp = atp;
        _engine.r_na = na;
        _engine.r_aa = aa;
        _engine.r_fa = fa;
        _engine.r_g = g;

        int mito = (int)Level.Info.CurrentOrganelles.Mito;
        int chloro = (int)Level.Info.CurrentOrganelles.Chloro;
        int ribo = (int)Level.Info.CurrentOrganelles.Ribo;
        int lyso = (int)Level.Info.CurrentOrganelles.Lyso;
        int slicer = (int)Level.Info.CurrentOrganelles.Slicer;
        int perox = (int)Level.Info.CurrentOrganelles.Perox;
        _cell.Init(mito, chloro, ribo, lyso, slicer, perox);
        
       
        
        
        //resources, organelles, membrane nodes?
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
