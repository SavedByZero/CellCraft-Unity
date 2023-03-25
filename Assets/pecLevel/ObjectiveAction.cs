using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveAction 
{
    public string type;
    public List<ObjectiveActionParam> paramList = new List<ObjectiveActionParam>();

    public override string ToString()
    {
        return type + ": " + paramList.Count + "," + paramList[0].name + "=" + paramList[0].val;
    }
}
