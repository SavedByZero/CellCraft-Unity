using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.Xml;

public class LevelObjective 
{
    //<objective id="load" active="true" sound="0" trigger="true">
    [XmlAttribute("id")]
    public string Id;
    [XmlAttribute("active")]
    public string Active;
    [XmlAttribute("sound")]
    public float Sound;
    [XmlAttribute("trigger")]
    public bool Trigger;
    [XmlElement("data")]
    public Data LData;
    [XmlElement("pre_action")]
    public Action PreAction;
    [XmlElement("action")]
    public Action LAction;

    public override string ToString()
    {
        return "id : " + Id + ", action: " + LAction.Set_Radicals.Value;
    }
    //<data name="Game Loads" hidden="true" type="game_load" targetType="null" targetNum="0" progress="0" targetCondition="true" delay="0" />
    public struct Data
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("hidden")]
        public bool Hidden;
        [XmlAttribute("type")]
        public string Type;
        [XmlAttribute("targetType")]
        public string TargetType;
        [XmlAttribute("targetNum")]
        public int TargetNum;
        [XmlAttribute("progress")]
        public float Progress;
        [XmlAttribute("targetCondition")]
        public string TargetCondition;
        [XmlAttribute("delay")]
        public float Delay;

    }

    /*
     * <action>
	
				<set_sunlight value="1"/>
				<set_toxinlevel value="0"/>
				<set_cyto_process value="true"/>
				<set_radicals value="false"/>
			</action>
    <action>
				
				<set_sunlight value="1"/>
				<set_toxinlevel value="0"/>
				<set_cyto_process value="false"/>
				<set_radicals value="false"/>
			</action>

    <action>
				<showTutorial id="Organelles">
					<tutorial title="Okay, so we've got the beginnings of a decent cell here." slide="talk_spike_normal"/>
					<tutorial title="But it needs some more ORGANELLES. Let's start by adding a NUCLEUS." slide="talk_spike_normal"/>
					<tutorial title="Organelles are like the cell's organs" slide="about_organelles"/>
				</showTutorial>
				<show_newthing id="nucleus"/>
				<activate_objective id="get_nucleus"/>
			</action>
     * */
  
    public struct Action 
    {
        [XmlElement("hide_organelle")]
        public HideOrganelle[] Hide_Organelle;
        [XmlElement("hide_resource")]
        public HideResource[] Hide_Resource;
        [XmlElement("hide_interface")]
        public HideInterface[] Hide_Interface;
        [XmlElement("wipe_organelle_act")]
        public WipeOrganelleAct[] Wipe_Organelle_Act;
        [XmlElement("set_sunlight")]
        public SetSunlight Set_Sunlight;
        [XmlElement("set_toxinlevel")]
        public SetToxinLevel Set_ToxinLevel;
        [XmlElement("set_cyto_process")]
        public SetCytoProcess Set_Cytoprocess;
        [XmlElement("set_radicals")]
        public SetRadicals Set_Radicals;
        [XmlElement("showTutorial")]
        public ShowTutorial Show_Tutorial;
        [XmlElement("show_newthing")]
        public ShowNewThing Show_Newthing;
        [XmlElement("activate_objective")]
        public ActivateObjective Activate_Objective;
        [XmlElement("plop_organelle")]
        public PlopOrganelle Plop_Organelle;
        [XmlElement("activate_stuff")]
        public ActivateStuff Activate_stuff;
        [XmlElement("spawn_object")]
        public SpawnObject Spawn_Object;
        [XmlElement("set_arrow_show")]
        public SetArrowShow Set_Arrow_Show;
        [XmlElement("discovery")]
        public Discovery Discovery;
        [XmlElement("show_resource")]
        public ShowResource Show_Resource;
        [XmlElement("throw_flag")]
        public ThrowFlag Throw_Flag;
        [XmlElement("add_organelle_act")]
        public AddOrganelleAct Add_Organelle_Act;
        [XmlElement("set_zoom")]
        public SetZoom Set_Zoom;
        [XmlElement("set_scroll_to")]
        public SetScrollTo Set_Scroll_To;
        [XmlElement("show_interface")]
        public ShowInterface Show_Interface;
        [XmlElement("send_wave")]
        public SendWave Send_Wave;
        [XmlElement("finish_level")]
        public FinishLevel Finish_Level;

    }

    public struct FinishLevel
    {
        [XmlAttribute("value")]
        public string Value;
    }

    public struct ShowInterface
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct SetScrollTo
    {
        [XmlAttribute("target")]
        public string Target;
        [XmlAttribute("time")]
        public float Time;
    }

    public struct SetZoom
    {
        [XmlAttribute("value")]
        public float Value;
        [XmlAttribute("time")]
        public float Time;
    }

    public struct AddOrganelleAct 
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("value")]
        public string Value;
    }


    public struct ThrowFlag
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct ShowResource
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct Discovery
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct SetArrowShow
    {

    }

    public struct ActivateStuff
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct SpawnObject
    {
        [XmlAttribute("id")]
        public string Id;
    }
    public struct PlopOrganelle
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct ActivateObjective
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct ShowNewThing
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct HideOrganelle
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct HideResource
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct HideInterface
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct SendWave
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct WipeOrganelleAct
    {
        [XmlAttribute("id")]
        public string Id;
    }

    public struct SetSunlight
    {
        [XmlAttribute("value")]
        public float Value;
    }

    public struct SetToxinLevel
    {
        [XmlAttribute("value")]
        public float Value;
    }

    public struct SetCytoProcess
    {
        [XmlAttribute("value")]
        public bool Value;
    }

    public struct SetRadicals
    {
        [XmlAttribute("value")]
        public bool Value;
    }

    public struct ShowTutorial
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlElement("tutorial")]
        public Tutorial Tutorial;
    }

    public struct Tutorial
    {
        [XmlAttribute("title")]
        public string Title;
        [XmlAttribute("slide")]
        public string Slide;
    }





}
