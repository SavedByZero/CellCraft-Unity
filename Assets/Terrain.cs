using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Terrain : CellGameObject
{
	//This class looks like it defines the type of floor the cell is on -- for instance, petri dishes. 
	//-32 x 32, -32 x 32
    public const int PETRI_DISH = 0;
	public const int PETRI_DISH_GOLD = 1;
	public  const int PETRI_DISH_SILVER = 2;
	public const int PETRI_DISH_GREEN = 3;


	public  const int ROBOT_BOX = 10;
	public  const int ROBOT_BOX_BROKEN = 11;
	public const int MONSTER_MOUTH = 12;
	public GameObject[] Skins;
	private GameObject _currentSkin;
	public GameObject mask;
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
		SwitchSkin(0);
		GameObject.FindObjectOfType<Muscle>().onMovingTowards += ShiftDirection;
	}

	void SwitchSkin(int index)
    {
		for(int i=0; i < Skins.Length; i++)
        {
			Skins[i].SetActive(i == index);
			if (i==index)
            {
				_currentSkin = Skins[i];
            }
        }
    }

	public void ShiftDirection(float cellDirX, float cellDirY)
    {
		Vector3 dir = new Vector3(cellDirX, cellDirY);
		this.transform.DOBlendableLocalMoveBy(dir, 3f).SetDelay(0f);//this.transform.position += dir;
		Camera.main.transform.DOBlendableLocalMoveBy(dir, 3f).SetDelay(0f);//Camera.main.transform.position += dir;
		_currentSkin.transform.DOBlendableLocalMoveBy(dir, 3f).SetDelay(0f);//_currentSkin.transform.position += dir;

		//mask.transform.position -= dir;
		
    }

	/*
	 *  This essentially draws the terrain, using the offsets as stage offsets. Originally it made a bitmap from bitmap data 
	 *  in a variable called data_terrain. 
	 * */
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

	/*
	 * This should create the terrain sprite based on the type.  
	 * */
	private void makeSprite()
    {


    }

	/*
	 *  This clears the sprite by setting it to null, originally. 
	 * */
	private void clearSprite()
    {

    }

	/*
	 * Originally, this created the bitmap image from bitmap data, divided the levelScaleW and levelScaleH by c_sprite's width and height, 
	 * scaled the matrix by those two levelScaleW and H variables, populated c_sprite with the bitmap, added the bitmap, set it to position 
	 * 0, offsetY. 
	 * Interestingly, it uses data_terrain both for bmp_terrain and c_sprite. 
	 * Summary: it draws the terrain onto the sprite and places it. 
	 */
	private void MakeBMP()
    {

    }

	/*
	 * this changed the bitmap terrain scroll rect to show a new part of the image data based on the scrollX and scrollY values. 
	 * Summary: I think it just scrolls the graphic data with your movement?. 
	 * */
	private void updateScroll()
    {
		
    }

	/*
	 * This updates the bitmap to reflect new scrolling and scaling -- the zoom change functions use this.  
	 */
	private void updateBMP()
    {
		/*
		 * matrix.identity();	
			matrix.scale(levelScaleW, levelScaleH); //accomodate the level size
			matrix.scale(scale, scale);			  //accomodate the zoom
			offset();							  
			matrix.translate(scrollX, scrollY);	  //accomodate the scroll
					
			data_terrain.fillRect(data_terrain.rect, 0x000000);
			data_terrain.draw(c_sprite, matrix);
		 */
	}

	/*
	 * This resets scale to old_scale and called updateBMP(). Not sure why. 
	 */
	public void oldZoom()
    {

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

	/**
		 * Move the terrain to the center of the screen, assuming it is at position 0
		 */
	public void offset()
    {

    }


	// Update is called once per frame
	void Update()
    {
        
    }
}
