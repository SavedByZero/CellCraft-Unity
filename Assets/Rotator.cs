using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.eulerAngles.z > -90)
        {
            this.transform.eulerAngles = new Vector3(0, 0, this.transform.eulerAngles.z + Time.deltaTime);
        }
    }
}
