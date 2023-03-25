using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    private List<string> _ObjectiveDescriptions = new List<string>();
    private Objective[] _objectives;
    
    public Text Header;
    public Text CurrentDescriptionField;
    public MovieClip BookClip;
    public delegate void CompleteObjective(string objective);
    public CompleteObjective onCompleteObjective;
    private int _currentObjective;
    private string _nextUp;
    private List<CellGameObjective> _cellGameObjectives = new List<CellGameObjective>();
    public Glass ObjectiveGlass;
    private static ObjectiveManager _om;
 
    private bool _ready = true;
    // Start is called before the first frame update
    void Start()
    {
        onCompleteObjective += recordObjective;
        MusicManager.Play(Music.MusicCalm);
       
        _om = this;
        DontDestroyOnLoad(this);
    }

    public void ParseObjectives()
    {
        _objectives = new Objective[_ObjectiveDescriptions.Count];
        for (int i = 0; i < _ObjectiveDescriptions.Count; i++)
        {
            string[] parts = _ObjectiveDescriptions[i].Split('|');
            _objectives[i] = new Objective();
            _objectives[i].Description = parts[0];
            _objectives[i].SuccessText = parts[1];
            _objectives[i].Id = i;
            _objectives[i].Complete = false;
        }

        //setNextObjective();
        CurrentDescriptionField.text = _objectives[(int)_currentObjective].Description;
    }

    public void SetFirstLevelObjective()
    {
        if (_cellGameObjectives[_currentObjective].active)
            processActions(_cellGameObjectives[_currentObjective]);
    }

    void processActions(CellGameObjective cgo)
    {
        ActionIomplementer ai = GetComponent<ActionIomplementer>();
        for(int i=0; i < cgo.actions.Count; i++)
        {
            if (cgo.actions[i].type == "activate_objective")
            {
                //process trigger for next one
                _nextUp = cgo.actions[i].paramList[0].val;
            }
            else
                ai.ReceiveAction(cgo.actions[i]);
            //make an action implementer class?
            Debug.Log("current objective action firing: " + cgo.actions[i]);
        }
    }

    public void AddLevelObjective(CellGameObjective cgo)
    {
        _cellGameObjectives.Add(cgo);
        _ObjectiveDescriptions.Add(cgo.Data_name+"|Objective Complete!");
        
      
        //lo.
    }

    public static ObjectiveManager GetInstance()
    {
        return _om;
    }

    void setNextObjective()
    {
        for(int i=0; i < _cellGameObjectives.Count; i++)
        {
            if (_cellGameObjectives[i].id == _nextUp)
            {
                //_nextUp = "";
                _currentObjective = i;
                break;
            }
        }
        //int index = (int)++_currentObjective;
        CurrentDescriptionField.text = _objectives[_currentObjective].Description;
        Header.gameObject.SetActive(true);
      
        if (_cellGameObjectives[_currentObjective].active)
        {
            processActions(_cellGameObjectives[_currentObjective]);
            recordObjective(_cellGameObjectives[_currentObjective].id);
        }
    }

    private void recordObjective(string objective)
    {
        if (_cellGameObjectives.Count == 0)
            return;

        if (objective == _cellGameObjectives[_currentObjective].id && _ready)
        {
            Header.gameObject.SetActive(false);
           // _ready = false; //to avoid bugs where spamming the same objective complets it multiple times during the transition.
            CurrentDescriptionField.text = _objectives[_currentObjective].Description;
            CurrentDescriptionField.DOColor(Color.white, 0.5f);
            CurrentDescriptionField.transform.DOScale(1.5f, 1.5f).SetLoops(2,LoopType.Yoyo);///DOPunchScale(Vector3.one * 1.25f, 1,5);
            StartCoroutine(transition());
        }
        //setNextObjective();
        
    }

    IEnumerator transition()
    {
        _ready = false;
        BookClip.Play();
        ObjectiveGlass.Animate(new Color(0.2f,.8f,1f));
        yield return new WaitForSeconds(3);
        CurrentDescriptionField.DOColor(Color.black, 0.5f);
        //If the objective was NOT set to activate on kickoff, do it now. 
        if (_cellGameObjectives[_currentObjective].active == false)
            processActions(_cellGameObjectives[_currentObjective]);
        _ready = true;
        setNextObjective();
    }

}
