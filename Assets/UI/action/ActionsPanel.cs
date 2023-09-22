using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsPanel : MonoBehaviour
{
    public GameObject Content;
    Button[] _actionButtons;
    COAction[] _actions;
    public delegate void PassUpCosts(string costString);
    public PassUpCosts onPassUpCosts;
    public Cell MyCell;
    private int _requiredATP;
    private int _requiredNA;
    private int _requiredAA;
    private int _requiredFA;
    private int _requiredG;
    private CellObject _co;
    // Start is called before the first frame update

    private void Awake()
    {
        _actionButtons = GetComponentsInChildren<Button>(true);
        _actions = GetComponentsInChildren<COAction>(true);
        for(int i=0; i < _actions.Length; i++)
        {
            _actions[i].onSendCostString += passUpCosts;
            _actions[i].onDoAction += doAction;
        }
    }

    private void doAction(string actionName)
    {
        Engine engine = GameObject.FindObjectOfType<Engine>();
        bool canAfford = engine.canAfford(_requiredATP, _requiredNA, _requiredAA, _requiredFA, _requiredG);
        if (!canAfford)
            return;
        engine.r_atp -= _requiredATP;
        engine.r_na -= _requiredNA;
        engine.r_aa -= _requiredAA;
        engine.r_fa -= _requiredFA;
        engine.r_g -= _requiredG;
        switch (actionName) //Bookmark: execute action panel functionality to organelles here
        {
            case "ribosome":
                for (int i = 0; i < 5; i++)
                {
                   
                    MyCell.generateRibosome();
                  
                        //engine.
                    
                   
                }
                break;
            case "slicer":
                for (int i = 0; i < 5; i++)
                {

                    MyCell.generateSlicer(_requiredNA);

                    //engine.


                }
                break;
            case "recycle":
                //Recycle organelle
                MyCell.startRecycle(_co as Selectable); //Michael: add true as a second parameter to recycle em all
                break;
        }
    }

    private void passUpCosts(string costString)
    {
        string[] costs = costString.Split(',');
        for (int i = 0; i < costs.Length; i++)
        {
           
            string[] costParts = costs[i].Split(':');
            int parsedCost = int.Parse(costParts[1]);
            switch (costParts[0])
            {
                case "atp":
                    _requiredATP = parsedCost;
                    break;
                case "na":
                    _requiredNA = parsedCost;
                    break;
                case "fa":
                    _requiredFA = parsedCost;
                    break;
                case "aa":
                    _requiredAA = parsedCost;
                    break;
                case "g":
                    _requiredG = parsedCost;
                    break;
            }
          

   
            //_entries[i].SetCost(costParts[0], costParts[1]);
            //_entries[i].gameObject.SetActive(true);
        }
        //pass up to HUD messenger
        onPassUpCosts?.Invoke(costString);
    }

    public void ReceiveCO(CellObject co, int type)
    {
        _co = co;
        List<string> actionNames = new List<string>();
        //Bookmark: add action panel functionality to organelles here
        switch (type)
        {
            case Selectable.NUCLEUS:
                actionNames.Add("ribosome");
                actionNames.Add("slicer");
                break;
            case Selectable.SLICER_ENZYME: //TODO: save organelle variable here so it can be recycled
                actionNames.Add("recycle");
             break;
        }

        Content.SetActive(true);
        turnOffButtons();

        for(int i=0; i < actionNames.Count; i++)
        {
            _actions[i].DefineAction(actionNames[i]);
            _actionButtons[i].gameObject.SetActive(true);
        }
    }

    void turnOffButtons()
    {
        for(int i=0; i < _actionButtons.Length; i++)
        {
            _actionButtons[i].gameObject.SetActive(false);
        }
    }
}
