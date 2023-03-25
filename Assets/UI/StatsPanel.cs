using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    public ResourceBar Health;
    public ResourceBar Infest;
    public GameObject Contents;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReceiveCO(CellObject co)
    {
        Contents.gameObject.SetActive(true);
        Health.SetMax(co.getMaxHealth());
        Health.Set(co.getHealth());
        Infest.SetMax(co.getMaxInfest());
        Infest.Set(co.getInfest());
    }
}
