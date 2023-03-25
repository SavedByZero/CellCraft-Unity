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
    private GoodieManager _GM;
    private List<CellGameObjective> _cellGameObjectives = new List<CellGameObjective> ();
    // Start is called before the first frame update
    void Start()
    {
        _engine = GetComponentInChildren<Engine>();
        _terrain = GetComponentInChildren<Terrain>();
        _cell = GetComponentInChildren<Cell>();
        _GM = GetComponentInChildren<GoodieManager>();
        LoadLevel();
        populateInfo();
        populateStuff();
        populateTriggers();
        _cell.setCytoProcess(true);
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

    void populateStuff()
    {
        Stuff[] goodies = Level.LevelStuff.GoodieStuff;
        for(int i=0; i < goodies.Length; i++)
        {
            _GM.PlaceGoodie(goodies[i]);
        }
    }


    void populateThings()
    {

    }

    ObjectiveAction addAndDefineAction(string aname, CellGameObjective obj)
    {
        obj.actions.Add(new ObjectiveAction());
        ObjectiveAction action = obj.actions[obj.actions.Count - 1];
        action.type = aname;
        action.paramList = new List<ObjectiveActionParam>();
        return action;
    }

    void populateTriggers()
    {
        ObjectiveManager om = GetComponent<ObjectiveManager>();
        for (int i=0; i <  Level.LevelObjectives.Length; i++)
        {
           

            LevelObjective objective = Level.LevelObjectives[i];
            CellGameObjective obj = new CellGameObjective();
            obj.Data_name = objective.LData.Name;
            obj.Data_delay = objective.LData.Delay;
            obj.active = (objective.Active == "true");
            obj.id = objective.Id;
            
            //obj.actions[0].paramList = new List<ObjectiveActionParam>();
            if (objective.LAction.Spawn_Object.Id != null)
            {
               

                ObjectiveAction action = addAndDefineAction("spawn_object", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Spawn_Object.Id));
                action.paramList.Add(new ObjectiveActionParam("loc_id", objective.LAction.Spawn_Object.LocId));
                action.paramList.Add(new ObjectiveActionParam("move_type", objective.LAction.Spawn_Object.MoveType));
            }
            if (objective.LAction.Wipe_Organelle_Act != null && objective.LAction.Wipe_Organelle_Act.Length > 0)
            {
                
                ObjectiveAction action = addAndDefineAction("wipe_organelle_act", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Wipe_Organelle_Act[0].Id));  //TODO: this is an array 

                //add wipe organelle action
            }
            if (objective.LAction.Hide_Organelle != null && objective.LAction.Hide_Organelle.Length > 0)
            {
                for(int j=0; j < objective.LAction.Hide_Organelle.Length; j++)
                {
                   
                    ObjectiveAction action = addAndDefineAction("hide_organelle", obj);
                    action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Hide_Organelle[j].Id));
                }
                
                
                  
            }
            if (objective.LAction.Set_Sunlight.Value > 0)
            {
                //add set sunlight action
                ObjectiveAction action = addAndDefineAction("set_sunlight", obj);
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Set_Sunlight.Value.ToString()));
            }
            if (objective.LAction.Set_ToxinLevel.Value > 0)
            {
                ObjectiveAction action = addAndDefineAction("set_toxinlevel", obj);
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Set_ToxinLevel.Value.ToString()));
            }
            if (objective.LAction.Set_Cytoprocess.Value)
            {
                ObjectiveAction action = addAndDefineAction("set_cyto_process", obj);
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Set_Cytoprocess.Value.ToString()));
            }
            if (objective.LAction.Set_Radicals.Value )
            {
                ObjectiveAction action = addAndDefineAction("set_radicals", obj);
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Set_Radicals.Value.ToString()));
            }
            if (objective.LAction.Show_Newthing.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("show_newthing", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Show_Newthing.Id.ToString()));
            }
            if (objective.LAction.Activate_Objective.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("activate_objective", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Activate_Objective.Id.ToString()));
            }
            if (objective.LAction.Plop_Organelle.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("plop_organelle", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Plop_Organelle.Id.ToString()));
            }
            if (objective.LAction.Activate_stuff.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("activate_stuff", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Activate_stuff.Id.ToString()));
            }
            if (objective.LAction.Discovery.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("discovery", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Discovery.Id.ToString()));
            }
            if (objective.LAction.Show_Tutorial.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("show_tutorial", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Show_Tutorial.Id.ToString()));
                action.paramList.Add(new ObjectiveActionParam("slides", objective.LAction.Show_Tutorial.Slides));
                

            }
            if (objective.LAction.Show_Resource.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("show_resource", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Show_Resource.Id.ToString()));
            }
            if (objective.LAction.Throw_Flag.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("throw_flag", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Throw_Flag.Id.ToString()));
            }
            if (objective.LAction.Add_Organelle_Act.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("add_organelle_act", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Add_Organelle_Act.Id.ToString()));
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Add_Organelle_Act.Value.ToString()));
            }
            if (objective.LAction.Finish_Level.Value != null)
            {
                ObjectiveAction action = addAndDefineAction("finish_level", obj);
                action.paramList.Add(new ObjectiveActionParam("value", objective.LAction.Finish_Level.Value.ToString()));
            }
            if (objective.LAction.Send_Wave.Id != null)
            {
                ObjectiveAction action = addAndDefineAction("send_wave", obj);
                action.paramList.Add(new ObjectiveActionParam("id", objective.LAction.Send_Wave.Id.ToString()));
            }

           
           
            
            //call 'doObjAction' method in Engine at some point with each action  
            _cellGameObjectives.Add(obj);
            om.AddLevelObjective(obj);

        }
        om.ParseObjectives();
        om.SetFirstLevelObjective();

    }

    void addAction(LevelObjective obj)
    {

    }
}
