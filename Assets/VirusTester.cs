using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusTester : MonoBehaviour
{
    public Cell TestCell;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
       
        yield return new WaitForSeconds(3);
        TestCell.spawnRibosomeVirus(TestCell.GetComponentInChildren<Ribosome>(), Selectable.VIRUS_INFESTER, 1, "virus_infester");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
