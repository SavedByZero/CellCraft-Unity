using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostEntry : MonoBehaviour
{
    // Start is called before the first frame update
    public void SetCost(string name, string amount)
    {
        GetComponentInChildren<MovieClip>().GotoAndStop(name);
        GetComponentInChildren<Text>().text = amount; 
    }
}
