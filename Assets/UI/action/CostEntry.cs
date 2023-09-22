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
        Text textfield = GetComponentInChildren<Text>();
        textfield.color = (amount[0] == '-') ? Color.green : Color.black;
        amount = amount.Replace('-', '+');
        textfield.text = amount; 
    }
}
