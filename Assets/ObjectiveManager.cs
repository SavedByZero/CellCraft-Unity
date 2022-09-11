using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ObjectiveManager : MonoBehaviour
{
    public string[] ObjectiveDescriptions;
    private Objective[] _objectives;
    
    public Text Header;
    public Text CurrentDescriptionField;
    public MovieClip BookClip;
    public delegate void CompleteObjective(GameObjective objective);
    public CompleteObjective onCompleteObjective;
    private GameObjective _currentObjective;
    public Glass ObjectiveGlass;
    private static ObjectiveManager _om;
    private bool _ready = true;
    // Start is called before the first frame update
    void Start()
    {
        onCompleteObjective += recordObjective;
        MusicManager.Play(Music.MusicCalm);
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

        //setNextObjective();
        CurrentDescriptionField.text = _objectives[(int)_currentObjective].Description;
        _om = this;
        DontDestroyOnLoad(this);
    }

    public static ObjectiveManager GetInstance()
    {
        return _om;
    }

    void setNextObjective()
    {
        int index = (int)++_currentObjective;
        CurrentDescriptionField.text = _objectives[index].Description;
        Header.gameObject.SetActive(true);
        _ready = true;
    }

    private void recordObjective(GameObjective objective)
    {
        if (objective == _currentObjective && _ready)
        {
            Header.gameObject.SetActive(false);
            _ready = false; //to avoid bugs where spamming the same objective complets it multiple times during the transition.
            CurrentDescriptionField.text = _objectives[(int)_currentObjective].SuccessText;
            CurrentDescriptionField.DOColor(Color.white, 0.5f);
            CurrentDescriptionField.transform.DOScale(1.5f, 1.5f).SetLoops(2,LoopType.Yoyo);///DOPunchScale(Vector3.one * 1.25f, 1,5);
            StartCoroutine(transition());
        }
        //setNextObjective();
        
    }

    IEnumerator transition()
    {
        BookClip.Play();
        ObjectiveGlass.Animate(new Color(0.2f,.8f,1f));
        yield return new WaitForSeconds(3);
        CurrentDescriptionField.DOColor(Color.black, 0.5f);
        //CurrentDescriptionField.text = ""
        setNextObjective();
    }

    public enum GameObjective
    {
        MakeAPseudoPod,
        FindGlucose
    }
}
