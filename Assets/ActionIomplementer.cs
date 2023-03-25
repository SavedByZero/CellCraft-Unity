using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionIomplementer : MonoBehaviour
{
    public SpriteRenderer CentrosomeRenderer;
    public SpriteRenderer NucleusRenderer;
    public SpriteRenderer GolgiRenderer;
    public SpriteRenderer ERRenderer;
    private Engine _engine;
    // Start is called before the first frame update
    void Start()
    {
        _engine = GameObject.FindObjectOfType<Engine>();
    }

    public void ReceiveAction(ObjectiveAction action )
    {
        Debug.Log("received action: " + action);
        switch(action.type)
        {
            case "show_newthing":
                Debug.Log("showing new thing: " + action);
                showThing(action.paramList[0].val);
                break;
            case "hide_organelle":
                if (action.paramList[0].val == "centrosome")
                    CentrosomeRenderer.color = new Color(1, 1, 1, 0);
                else if (action.paramList[0].val == "nucleus")
                {
                    NucleusRenderer.color = new Color(1, 1, 1, 0);
                    NucleusRenderer.gameObject.SetActive(false);
                }
                else if (action.paramList[0].val == "golgi")
                    GolgiRenderer.color = new Color(1, 1, 1, 0);
                else if (action.paramList[0].val == "er")
                    ERRenderer.color = new Color(1, 1, 1, 0);  
                break;
            case "spawn_object":
                //ves_mitochondrion
                _engine.spawnObject(action.paramList[0].val, action.paramList[1].val, action.paramList[2].val);
                break;
            case "show_tutorial":
                Debug.Log("to parse tutorial slides");
                if (action.paramList.Count > 1)
                {
                    GameObject.FindObjectOfType<Tutorial>(true).showSlides(action.paramList[1].val);
                    
                }
                

                break;
            case "activate_objective":
                //pass the key back to the objective manager as the next one to trigger, once this one is complete.
                break;
            case "finish_level":
                
                break;

        }
        //map the action to a real function 
    }

    void showThing(string name)
    {
        AppearanceController ac = GameObject.FindObjectOfType<AppearanceController>(true);
        ac.SetTargetByName(name);
        ac.gameObject.SetActive(true);
        ac.ShowUp();
    }
}
