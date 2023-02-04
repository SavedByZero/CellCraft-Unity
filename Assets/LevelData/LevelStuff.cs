using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


public class LevelStuff 
{
    [XmlArray("goodies"), XmlArrayItem("stuff")]
    public Stuff[] GoodieStuff;

    public override string ToString()
    {
        return "stuff : " + GoodieStuff;
    }

}

//<stuff name="na_batch" type="na" count="50" spawn="0.05" active="false" />
public struct Stuff
{
    [XmlAttribute("name")]
    public string Name;
    [XmlAttribute("type")]
    public string Type;
    [XmlAttribute("count")]
    public int Count;
    [XmlAttribute("spawn")]
    public float Spawn;
    [XmlAttribute("active")]
    public string Active;

}
