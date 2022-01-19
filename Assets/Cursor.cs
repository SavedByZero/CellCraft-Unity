using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    public CellAction action = CellAction.NOTHING;
    public MovieClip Reticle;
    public MovieClip Icon;
    public Text ActionTextField;  //text
    public Text CostTextField;  //text_2
    public Minicost cost; //TODO
    //reticle: crosshairs, triangles, arrowHead, redArrowHead 
    // Start is called before the first frame update

    public bool isArrow = false;
		
	private float arrow_x; //the starting point of the arrow
	private float arrow_y;
	private float arrow_rotation = 0;
	//private var shape_arrow:Shape;  //TODO
	private static float ARROW_HEIGHT = 15;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
