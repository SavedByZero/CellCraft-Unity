using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGameManager : MonoBehaviour
{
    public GameObject InfoBubblePrefab;
    private List<GameObject> _infoBubbles = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
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
