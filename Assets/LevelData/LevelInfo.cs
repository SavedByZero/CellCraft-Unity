using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System;


public class LevelInfo 
{
   
    [XmlElement("level")]
    public Level CurrentLevel;
    [XmlElement("size")]
    public Size CurrentSize;
    [XmlElement("start")]
    public Start StartPos;
    [XmlElement("background")]
    public Background Backdrop;
    [XmlElement("resources")]
    public Resources CurrentResources;
    [XmlElement("organelles")]
    public Organelles CurrentOrganelles;
    [XmlElement("membrane")]
    public Membrane MembraneCount;

    public LevelInfo()
    {

    }

    public override string ToString()
    {
        return "Level: " + CurrentOrganelles.Mito +"," + MembraneCount.Nodes;
    }


    public struct Level
    {
        [XmlAttribute("index")]
        public int Index;
        [XmlAttribute("title")]
        public string Title;
    }

    public struct Size
    {
        [XmlAttribute("width")]
        public float Width;
        [XmlAttribute("height")]
        public float Height;
    }

    public struct Start
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
    }

    public struct Background
    {
        [XmlAttribute("name")]
        public string Name;
    }

    //<resources atp="1000" na="0" aa="0" fa="0" g="0"/>
    public struct Resources
    {
        [XmlAttribute("atp")]
        public float atp;
        [XmlAttribute("na")]
        public float na;
        [XmlAttribute("aa")]
        public float aa;
        [XmlAttribute("fa")]
        public float fa;
        [XmlAttribute("g")]
        public float g;

    }

    //<organelles mito="0" chloro="0" ribo="0" lyso="0" slicer="0" perox="0"/>
    public struct Organelles
    {
        [XmlAttribute("mito")]
        public int Mito;
        [XmlAttribute("chloro")]
        public int Chloro;
        [XmlAttribute("ribo")]
        public int Ribo;
        [XmlAttribute("lyso")]
        public int Lyso;
        [XmlAttribute("slicer")]
        public int Slicer;
        [XmlAttribute("perox")]
        public int Perox;
    }

    public struct Membrane
    {
        [XmlAttribute("nodes")]
        public int Nodes;
    }
}




