using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wiggler : MonoBehaviour
{
    private Vector3 _originalPos;

    // Start is called before the first frame update
    void Start()
    {
        _originalPos = this.transform.position;
        StartCoroutine(Wiggle());
    }

    // Update is called once per frame
    IEnumerator Wiggle()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            this.transform.DOLocalMove(_originalPos + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f)), 1);
        }
       
        
        //this.transform.position = _originalPos + , 0);
    }
}
