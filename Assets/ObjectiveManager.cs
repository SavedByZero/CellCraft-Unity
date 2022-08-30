using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public string[] ObjectiveDescriptions;
    private Objective[] _objectives;
    public Text CurrentDescriptionField;
    public delegate void CompleteObjective(GameObjective objective);
    public CompleteObjective onCompleteObjective;
    private GameObjective _currentObjective;
    // Start is called before the first frame update
    void Start()
    {
        onCompleteObjective += recordObjective;
        _objectives = new Objective[ObjectiveDescriptions.Length];
        for(int i=0; i < ObjectiveDescriptions.Length; i++)
        {
            string[] parts = ObjectiveDescriptions[i].Split('|');
            _objectives[i] = new Objective();
            _objectives[i].Description = parts[0];
            _objectives[i].SuccessText = parts[1];
            _objectives[i].Id = i;
            _objectives[i].Complete = false;
        }

        setNextObjective();

    }

    void setNextObjective()
    {
        int index = (int)_currentObjective++;
        CurrentDescriptionField.text = _objectives[index].Description;

    }

    private void recordObjective(GameObjective objective)
    {
        if (objective == _currentObjective)
        {
            CurrentDescriptionField.text = "";
        }
        //setNextObjective();
    }

    public enum GameObjective
    {
        None,
        MakeAPseudoPod,
        FindGlucose
    }
}
