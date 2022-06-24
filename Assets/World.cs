using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	private const float OFFSET_X = 120;//subtract 120 from right
	private const float OFFSET_Y = 40; //add 40 from top
		
		//
		
	private float scrollX = 0;
	private float scrollY = 0;
	private Point scrollPoint;
	private Point old_scrollPoint;
	private static float scale = 1;
	public float _scale = 1;
		
	//pointers:
		
	private Director p_director;
	private Engine p_engine;
	private CellGameInterface p_interface;
		
	//selectables:
		
	private List<Selectable> selectList;
		
	//children:

	private Graphics c_zoomShape;
	private Graphics c_zoomShape2;
	private Graphics c_selectShape;
//	private Cross c_cross;  //TODO
		
	//everything between this comment and c_objectLayer is a child of c_objectLayer
	private Cell c_cell;
	private WorldCanvas c_canvas;
	
	/*private var c_mask:WorldMask;     //TODO
	private var c_maskBlur:WorldBlur;*/
		
	//everything below this comment is a direct child of world (this)

	private GameObject c_objectLayer;
	private Terrain c_terrain;
		
	private float lvlWidth = 0;
	private float lvlHeight = 0;
		
	private float startX = 0;
	private float startY = 0;
		
	private bool animationOn = true;
	private bool doFollowCell = true;
		
	private int bkgIndex = -1;
		
	public static float LENS_SIZE = 100;
	public static float LENS_R2 = 100*100;
		
	public static float BOUNDARY_R2 = 1000;
	public static float BOUNDARY_W = 1000;
	public static float BOUNDARY_H = 1000;
		//public static var BOUNDARY_
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void init()
	{

		//testing:
		//c_cross = new LittleCross();


		scrollPoint = new Point(0, 0);
		old_scrollPoint = new Point(0, 0);
		this.transform.localPosition = new Vector3(Director.STAGEWIDTH/2, Director.STAGEHEIGHT/2);
		//y = Director.STAGEHEIGHT / 2; //put the "center of the world" at the center of the screen
		//x = Director.STAGEWIDTH / 2;

		if (bkgIndex == -1)
		{
			Debug.LogError("Background index not set!");
		}
		makeTerrain(bkgIndex, lvlWidth, lvlHeight);
		makeObjectLayer();
		makeMask();
		makeCell();

		goStartPoint();

		//addEventListener(MouseEvent.MOUSE_DOWN, mouseDown);
		//addEventListener(RunFrameEvent.RUNFRAME, run);

		
	}

	public void goStartPoint()
	{
		c_cell.moveCellTo(startX, startY);//(startX * Terrain.SCALE_MULT, startY * Terrain.SCALE_MULT); //move to the start location //TODO
		centerOnCell();
	}

	public void setStartPoint(float xx, float yy)
	{
		startX = xx;
		startY = yy;
	}

	public void setLevel(string s, float w, float h)
	{
		s = s.ToLower();
		if (s == "petridish")
		{
			bkgIndex = Terrain.PETRI_DISH;
		}
		else if (s == "monstermouth")
		{
			bkgIndex = Terrain.MONSTER_MOUTH;
		}
		else if (s == "petridish_silver")
		{
			bkgIndex = Terrain.PETRI_DISH_SILVER;
		}
		else if (s == "petridish_gold")
		{
			bkgIndex = Terrain.PETRI_DISH_GOLD;
		}
		else if (s == "petridish_green")
		{
			bkgIndex = Terrain.PETRI_DISH_GREEN;
		}
		lvlWidth = w;
		lvlHeight = h;
		Debug.Log("World.setLevel : size=(" + lvlWidth + "," + lvlHeight + ")");
	}

	public void destruct()
	{
		p_director = null;
		p_engine = null;
		p_interface = null;

		if (c_objectLayer)
		{
			if (c_cell)
			{
				c_cell.destruct();
				//c_objectLayer.removeChild(c_cell);
				c_cell = null;
			}
			if (c_canvas)
			{
				c_canvas.destruct();
				//c_objectLayer.removeChild(c_canvas);
				c_canvas = null;
			}
			//removeChild(c_objectLayer);
			c_terrain.destruct();
			//removeChild(c_terrain);
			c_terrain = null;
			c_objectLayer = null;
		}
	}

	public void setDirector(Director d)
	{
		p_director = d;
	}

	public void setEngine(Engine e)
	{
		p_engine = e;
	}

	public void setInterface(CellGameInterface i)
	{
		p_interface = i;
	}

	private void makeTerrain(int i, float w, float h)
	{

		//c_terrain = new Terrain(i, OFFSET_X, OFFSET_Y, w, h);  //TODO: initiate this the right way. 

		c_terrain.transform.position = new Vector3(-this.transform.position.x, -this.transform.position.y, this.transform.position.z);
		//c_terrain.x = -x;   //PHYSICALLY offset the CANVAS so it takes up the entire window
		//c_terrain.y = -y;

		//c_terrain.center(0, 0) //CENTER the drawing location by this
			//addChild(c_terrain);
	}

	private void makeObjectLayer()
	{
		//c_objectLayer = new Sprite();  //
		//addChild(c_objectLayer);      //TODO: Instantiate this properly
		
	
		c_objectLayer.transform.localPosition = new Vector3(-OFFSET_X / 2, OFFSET_Y / 2);// = ;

	}

	private void makeMask()
	{
		//c_mask = new WorldMask();   //TODO all this?
		//c_maskBlur = new WorldBlur();
		//addChild(c_mask);
		//addChild(c_maskBlur);
		//mask = c_mask;
	}



	private void run()//(e:RunFrameEvent)
	{
		//c_cell.dispatchEvent(e);  //TODO
		//c_canvas.dispatchEvent(e);  //TODO
	}


	public void mouseDown()
	{

		//p_engine.onWorldMouseDown();  //TODO
	}

	public void updateMaskSize(float r)
	{
		LENS_SIZE = r;
		LENS_R2 = r * r;
		/*c_mask.setSize(r);       //TODO
		c_maskBlur.setSize(r);
		if (p_engine)
		{
			p_engine.onUpdateMaskSize(r);
		}*/
	}

	public float getMaskSize() 
	{
		return 1;// c_mask.getSize();  //TODO
	}

/**
 * When the cell moves, this function is called to keep the screen centered on it.
 * @param	xx the x position of the centrosome
 * @param	yy the y position of the centrosome
 */

	public void onCellMove(float xx = 0, float yy = 0) 
	{ //please, please send me the centrosome's position
		if (doFollowCell)
		{
			//centerOnCell(), inlined for speed
			scrollPoint.x = -xx;
			scrollPoint.y = -yy;
			setScroll(scrollPoint.x * scale, scrollPoint.y * scale);
		}

		
		Point p = new Point(xx, yy);
		p = reverseTransformPoint(p);

	}

	private void updateMask2(float xx, float yy)
	{
		Point p = new Point(xx, yy);
		p = reverseTransformPoint(p);
		/*c_mask.x = p.x - x;    //TODO
		c_mask.y = p.y - y;*/
	}

	private void updateMaskScale()
	{
		/*c_mask.scaleX = scale;   //TODO
		c_mask.scaleY = scale;*/
	}

	private void updateMask()
	{
		Point p = new Point(c_cell.c_centrosome.x, c_cell.c_centrosome.y);
		p = reverseTransformPoint(p);
		
		/*c_mask.x = p.x - x;
		c_mask.y = p.y - y;     //TODO
		*/
		
	}

	private void makeCanvas()
	{
		c_canvas = new WorldCanvas();

		//c_objectLayer.addChild(c_canvas);

		/*c_canvas.setDirector(p_director);   //TODO
		c_canvas.setCell(c_cell);

		c_cell.setCanvas(c_canvas);
		p_engine.receiveCanvas(c_canvas);

		c_canvas.setEngine(p_engine);
		c_canvas.setWorld(this);
		c_canvas.init();*/
	}

	private void makeCell()
	{
		//IMPORTANT : the cell must ALWAYS remain at position 0,0. It's organelles and contents can move
		//wherever the heck they want, but the "bucket" containing all of its bits, c_cell, must as a data
		//object stay at 0,0. If this ever changes, that offset has to be taken into account when selecting, etc.
		c_cell = new Cell();

		//c_objectLayer.addChild(c_cell);
		c_cell.setDirector(p_director);
		/*p_engine.receiveCell(c_cell); //set the engine's cell pointer  //TODO all this 
		 
		c_cell.setEngine(p_engine);
		c_cell.setWorld(this);
		c_cell.setInterface(p_interface);*/

		makeCanvas(); //have to do this after the cell is made but before it's initialized

		c_cell.init();
		//get the selectables within the cell and add them to the selectList
		makeSelectList();
	}

	public static float getZoom() 
	{
			return scale;
	}

	public float getScale()
	{
		return scale;
	}

	public Cell getCell()
	{
		return c_cell;
	}

	public void updateSelectList()
	{
		makeSelectList();
	}

	private void makeSelectList()
	{
		selectList = new List<Selectable>(c_cell.getSelectables());
		//selectList = list.concat();
	}

	public void changeZoom(float n)
	{
		float old_scale = scale;

		float xx = scrollX;
		float yy = scrollY;
		c_objectLayer.transform.localScale = new Vector3(n, n, n);
	
		c_terrain.changeZoom(n);
		scale = n;
		_scale = n;
		c_cell.onZoomChange(n);
		//c_canvas.onZoomChange(n);  //TODO
		centerOnPoint(-scrollPoint.x, -scrollPoint.y);
		updateMaskScale();
	}

	public void updateScroll()
	{
		setScroll(scrollPoint.x * scale, scrollPoint.y * scale);
		updateMask();
	}

	public Point getScroll() 
	{
		return new Point(scrollPoint.x, scrollPoint.y);
	}

	public void setScroll(float xx, float yy) 
	{
		float old_scrollX = c_objectLayer.transform.localPosition.x;
		float old_scrollY = c_objectLayer.transform.localPosition.y;
		c_objectLayer.transform.localPosition = new Vector3(xx - OFFSET_X / 2, yy + OFFSET_Y / 2);

		if (!checkBoundX())
		{
			c_objectLayer.transform.localPosition = new Vector3(old_scrollX, c_objectLayer.transform.localPosition.y, 0);
			scrollPoint.x = old_scrollPoint.x;
		}
		else
		{
			//c_terrain.setScrollX(xx);  //TODO
		}

		if (!checkBoundY())
		{
			c_objectLayer.transform.localPosition = new Vector3(c_objectLayer.transform.localPosition.x, old_scrollY, 0);
			scrollPoint.y = old_scrollPoint.y;
		}
		else
		{
			//c_terrain.setScrollY(yy);  //TODO
		}

		scrollX = c_objectLayer.transform.localPosition.x + OFFSET_X / 2;
		scrollY = c_objectLayer.transform.localPosition.y - OFFSET_Y / 2;
	}

	public void doScroll(float sx, float sy) 
	{
		old_scrollPoint.x = scrollPoint.x;
		old_scrollPoint.y = scrollPoint.y;
		scrollPoint.x -= sx / scale;
		scrollPoint.y -= sy / scale;

		setScroll(scrollPoint.x * scale, scrollPoint.y * scale);
		updateMask();
		//updateScroll();
	}

	public void setBoundaryCircle(float r)
	{
		r *= Terrain.SCALE_MULT;
		BOUNDARY_R2 = r * r;
		CellGameObject.setBoundaryRadius(r);
	}

	public void setBoundaryBox(float w, float h)
	{
		w *= Terrain.SCALE_MULT;
		h *= Terrain.SCALE_MULT;
		BOUNDARY_W = w;
		BOUNDARY_H = h;
		CellGameObject.setBoundaryBox(w, h);
		//throw new Error("Boundary box not implemented!");
	}

	public bool checkBoundX()
	{
			float tw = /*(c_terrain.getWidth()) //TODO */ 640 * Terrain.SCALE_MULT;
			float wo = (OFFSET_X)* Terrain.SCALE_MULT;
	//tw *= .75;
			float ssx = scrollPoint.x;//(c_objectLayer.x + OFFSET_X / 2);
			if (ssx > (tw) / 2) { //Div by 4: divide by 2 for half the distance of the thing. divide by 2 again because you only need to scroll half to see the edge
				return false;
			}else if (ssx< -(tw) / 2) {
				return false;
			}
			return true;
	}

	public bool checkBoundY()
	{
		float th = /*(c_terrain.getHeight())  //TODO */ 480 * Terrain.SCALE_MULT;
		float ssy = scrollPoint.y;// c_objectLayer.y - OFFSET_Y / 2;
		float ho = (OFFSET_Y) * Terrain.SCALE_MULT;
		if (ssy > (th) / 2)
		{
			return false;
		}
		else if (ssy < -(th) / 2)
		{
			return false;
		}
		return true;
	}

	public bool checkBounds(float sx, float sy)
	{

		float tw = 640 * scale;//c_terrain.getWidth()* scale;  //TODO
		float th = 480 * scale;//c_terrain.getHeight()* scale;  //TODO
		tw *= 17 / 14;
			
		tw /= scale;
		th /= scale;
		float ssx = (c_objectLayer.transform.localPosition.x + OFFSET_X / 2) / scale;
		float ssy = (c_objectLayer.transform.localPosition.y - OFFSET_Y / 2) / scale;
			
		if (ssx > tw/2) {
			return false;
		}else if (ssx< -tw/2) {
			return false;
		}

		if (ssy > th / 2)
		{
			return false;
		}
		else if (ssy < -th / 2)
		{
			return false;
		}
		return true;
	}

	public Point getCenteredPoint()
	{
		//unpack setScroll(xx,yy);
		float xx = scrollX;
		float yy = scrollY;

		//unpack (scrollPoint.x * scale, scrollPoint.y * scale) = (xx,yy)
		xx /= scale;
		yy /= scale;

		//unpack scrollPoint.x = -xx, scrollPoint.y = -yy

		xx *= -1;
		yy *= -1;

		return new Point(xx, yy);
	}

	public void centerOnPoint(float xx, float yy) 
	{
		scrollPoint.x = -xx;
		scrollPoint.y = -yy;

		setScroll(scrollPoint.x * scale, scrollPoint.y * scale);
		updateMask();
		//updateScroll();
	}

	public void followCell(bool yes)
	{
		//needs to be optimized perhaps
		doFollowCell = yes;
	}

	public void centerOnCell()
	{
		centerOnPoint(c_cell.c_centrosome.x, c_cell.c_centrosome.y);
	}

	public void pauseAnimate(bool yes)
	{
		c_cell.pauseAnimate(yes);
		c_canvas.pauseAnimate(yes);
	}

	public void setAnimation(bool yes)
	{
		if (yes)
		{
			c_cell.animateOn();
		}
		else
		{
			c_cell.animateOff();
		}
		animationOn = yes;
	}

	/**
		 * Takes a point in world space and transforms it into stage space
		 * @param	p the Point in world space
		 * @return the Point in stage space
		 */

	public Point reverseTransformPoint(Point p) 
	{
			//do the opposite of transformPoint()
			p.x *= scale;
			p.y *= scale;
			
			p.x += c_objectLayer.transform.localPosition.x;
			p.y += c_objectLayer.transform.localPosition.y;
			
			p.x += this.transform.localPosition.x;
			p.y += this.transform.localPosition.y;
			return p;
		}

	/**
	 * Takes a point in stage space ( (0,0) to (640,480) ) and transforms it into world space
	 * @param	p the Point in stage space
	 * @return  the Point in world space
	 */

	public Point transformPoint(Point p)
	{
		//First, we have to line up the origins

		//Subtract the worlds own offset first
		p.x -= this.transform.localPosition.x;
		p.y -= this.transform.localPosition.y;

		//Subtract the objectLayer's own offset next
		p.x -= c_objectLayer.transform.localPosition.x;
		p.y -= c_objectLayer.transform.localPosition.y;

		//Now that origins are lined up, scale the point to the right location
		p.x /= scale;
		p.y /= scale;

		return p;
	}

	public void darkenAll()
	{
		foreach (Selectable item in selectList) 
		{
			if (!item.isSelected())
			{
				item.darken();
			}
		}
	}

	public void undarkenAll()
	{
		foreach(Selectable item in selectList) 
		{
			item.undarken();
		}
	}



	public void selectStuff(Point p, float radius)
	{
		p = transformPoint(p);
		radius /= scale;
		float radius2 = radius * radius;
		foreach(Selectable item in selectList) 
		{
			if (item.getCanSelect() && !item.getSingleSelect())
			{
				float r2 = item.getRadius();
				float dist2 = FastMath.getDist2(p.x, p.y, item.x, item.y); 

				if (dist2 < radius2 + r2)
				{
					//p_engine.selectMany(item);  //TODO
				}
			}
		}
		//p_engine.finishSelectMany();  //TODO
	}








}
