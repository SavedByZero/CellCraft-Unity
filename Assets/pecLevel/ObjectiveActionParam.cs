using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveActionParam 
{
    public string name;
    public string val;

    public ObjectiveActionParam() { }
    public ObjectiveActionParam (string name, string val)
    {
        this.name = name;
        this.val = val;
    }

    public override string ToString()
    {
        return this.name + ":" + this.val;
    }
}
