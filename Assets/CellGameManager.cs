using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CellGameManager : MonoBehaviour
{
    public GameObject InfoBubblePrefab;
    public RawImage Fader;
    private int _startHealth = 100;
    public int StartHealth
    {
        get
        {
            return _startHealth;
        }

        set
        {
            _startHealth = value;
        }
       
    }
    private List<GameObject> _infoBubbles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        Kickoff(); 
    }
    
    public void Kickoff()
    {
        CellObject[] cell_objects = GetComponentsInChildren<CellObject>();
        for (int i = 0; i < cell_objects.Length; i++)
        {
            //TODO: make sure these are set when individually activated
            //if (cell_objects[i].gameObject.activeSelf)
            if (cell_objects[i].getHealth() < StartHealth)
                cell_objects[i].instantSetHealth(StartHealth);
        }
        if (Fader != null)
        {
            Fader.DOFade(0, 1);
        }
    }

    public void EndLevel()
    {
        if (Fader != null)
        {
            Fader.DOFade(0, 1).OnComplete(new TweenCallback(delegate { 
                //load level select 
            }));
        }
    }

    public GameObject FetchInfoBubble()
    {
        for(int i=0; i < _infoBubbles.Count; i++)
        {
            if (!_infoBubbles[i].activeSelf)
            {
                _infoBubbles[i].SetActive(true);
                return _infoBubbles[i];
            }
        }

        GameObject bubble = Instantiate(InfoBubblePrefab) as GameObject;
        _infoBubbles.Add(bubble);
        return bubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
