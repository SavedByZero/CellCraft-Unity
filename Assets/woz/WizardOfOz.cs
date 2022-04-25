using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardOfOz : MonoBehaviour
{
    public static float LENS_RADIUS = 10;
		public static float LENS_RADIUS2 = 10000;
    private float lensRadius = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void updateLensSize(float r)
    {
        lensRadius = r;
        LENS_RADIUS = r;
        LENS_RADIUS2 = r * r;
        //visible_area = r * r * Mathf.PI; //TODO
        //p_canvas.updateLensSize(r);  //TODO
    }
}
