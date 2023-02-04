using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("cellcraft")]
public class CellcraftLevel 
{
    [XmlElement("levelInfo")]
    public LevelInfo Info;
    [XmlElement("levelStuff")]
    public LevelStuff LevelStuff;
    [XmlElement("levelThings")]
    public LevelThings LevelThings;
    [XmlArray("levelTriggers"), XmlArrayItem("objective")]
    public LevelObjective[] LevelObjectives;

    public override string ToString()
    {
        return "info: "+ Info.ToString() + "\n" + "level objectives: " + LevelObjectives[0];
    }
}
