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
        switch (actionName)
        {
            case "ribosome":
                for(int i=0; i < 5; i++)
                    MyCell.generateRibosome();
                break;
        }
    }

    private void passUpCosts(string costString)
    {
        //pass up to HUD messenger
        onPassUpCosts?.Invoke(costString);
    }

    public void ReceiveCO(CellObject co, int type)
    {
        List<string> actionNames = new List<string>();
        
        switch (type)
        {
            case Selectable.NUCLEUS:
                actionNames.Add("ribosome");
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
