using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CellGameManager : MonoBehaviour
{
    public GameObject InfoBubblePrefab;
    public RawImage Fader;
    public int StartHealth = 100;
    private List<GameObject> _infoBubbles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        CellObject[] cell_objects = GetComponentsInChildren<CellObject>();
        for(int i=0; i < cell_objects.Length; i++)
        {
            cell_objects[i].instantSetHealth(StartHealth);
        }
        if (Fader != null)
        {
            Fader.DOFade(0, 1);
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
