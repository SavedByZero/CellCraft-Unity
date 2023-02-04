using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;


public class LevelThings 
{
    [XmlArray("goodies"), XmlArrayItem("thing")]
    public Thing[] GoodieThings;

    [XmlArray("waves"), XmlArrayItem("wave")]
    public Wave[] Waves;

    public override string ToString()
    {
        return "goodiethings: " + GoodieThings + ", waves: " + Waves;
    }
}


//<thing name="Glucose Pool" type="glucose" x="100" y="100" count="50" spawn="1" active="false" />
public struct Thing
{
    [XmlAttribute("name")]
    public string Name;
    [XmlAttribute("type")]
    public string Type;
    [XmlAttribute("x")]
    public float X;
    [XmlAttribute("y")]
    public float Y;
    [XmlAttribute("count")]
    public int Count;
    [XmlAttribute("Spawn")]
    public int Spawn;
    [XmlAttribute("active")]
    public string Active;
}

//<wave id="wave_0_a" type="virus_injector" count="30" vesicle="0.5" spread="2" delay="30" sleep_seconds="15"/>
public struct Wave
{
    [XmlAttribute("id")]
    public string Id;
    [XmlAttribute("type")]
    public string Type;
    [XmlAttribute("count")]
    public int Count;
    [XmlAttribute("vesicle")]
    public float Vesicle;
    [XmlAttribute("spread")]
    public int Spread;
    [XmlAttribute("delay")]
    public float Delay;
    [XmlAttribute("sleep_seconds")]
    public float SleepSeconds;
}
