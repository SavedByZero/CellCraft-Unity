using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : CellGameObject
{
    public const int PETRI_DISH = 0;
	public const int PETRI_DISH_GOLD = 1;
	public  const int PETRI_DISH_SILVER = 2;
	public const int PETRI_DISH_GREEN = 3;
	public  const int ROBOT_BOX = 10;
	public  const int ROBOT_BOX_BROKEN = 11;
	public const int MONSTER_MOUTH = 12;
	// Start is called before the first frame update

	private GameObject c_sprite;
	private GameObject bmp_terrain;
	//private var data_terrain:BitmapData;  //TODO?
		
	private Matrix4x4 matrix;

	private float scale = 1; //it's important to initialize this!
	private float zoom = 1;
	private float old_scale = 1;
	private float scrollX = 0;
	private float scrollY = 0;
	private float old_scrollX = 0;
	private float old_scrollY = 0;
	public const float SCALE_MULT = 20;
		
	private float offsetX = 0;
	private float offsetY = 0;
		
	private float levelScaleW = 1;
	private float levelScaleH = 1;
		
	private float t_width;
	private float t_height;
    void Start()
    {
		//initTerrain has to somehow be called with parameters

	}

	public void initTerrain(int i, float ox, float oy, float w, float h)
    {
		Debug.Log("new Terrain(" + i + "," + ox + "," + oy + "," + w + "," + h + ")");
		offsetX = ox;
		offsetY = oy;
		makeMatrix();
		//makeBMP(i, w, h);  //TODO
		t_width = w;
		t_height = h;
	}

	private void makeMatrix()
	{
		matrix = new Matrix4x4();
	
	}

	public void changeZoom(float n)
	{
		//assume we were at 1.2 and this is called with n=1.5
		//diff will = 0.3, we need to zoom up by 30%
		//diff will then = 1.3, representing 130%
		//the matrix will be scaled by 1.3, ie up by 30%

		//var diff:Number = n - scale;	//get the difference between the new zoom and the old zoom
		//diff += 1;
		old_scale = scale;
		zoom = n;
		scale = n * SCALE_MULT;                     //store the new zoom

		//matrix.scale(diff, diff);
		
		//updateBMP();  //TODO

	}


	// Update is called once per frame
	void Update()
    {
        
    }
}
