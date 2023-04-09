using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostPanel : MonoBehaviour
{
    CostEntry[] _entries;
    
    // Start is called before the first frame update
    void Start()
    {
        _entries = GetComponentsInChildren<CostEntry>(true);


    }

    void turnOff()
    {
        for(int i=0; i < _entries.Length; i++)
        {
            _entries[i].gameObject.SetActive(false);
        }
    }

    public void ReceiveCost(string costString)
    {
        //turnOff();
        string[] costs = costString.Split(',');
        for(int i=0; i < costs.Length; i++)
        {
            string[] costParts = costs[i].Split(':');
            _entries[i].SetCost(costParts[0], costParts[1]);
            _entries[i].gameObject.SetActive(true);
        }
    }
}
