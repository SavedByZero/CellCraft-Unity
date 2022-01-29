using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasObject : CellGameObject
{
	//private static var p_cgrid:ObjectGrid; //TODO
		//protected var p_canvas:WorldCanvas;  //TODO
		
	public static float LENS_RADIUS = 1000;
	public static float LENS_RADIUS2 = 1000 * 1000;

	public CanvasObject()
	{
		speed = 2;
		makeGameDataObject();
	}

	/*public override function destruct()
	{
		//super.destruct();
		//p_canvas = null;
	}*/

	public override void putInGrid()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;
		gdata.x = xx;
		gdata.y = yy;
		
		//This seems to be some kind of scaling formula for grid positioning?
		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);

		//I think the sectio below is just boundary checking?
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;
		
		//p_cgrid.putIn(grid_x, grid_y, gdata);  //TODO
	}


	public void setCanvas()//(c:WorldCanvas)  //TODO
	{
		//p_canvas = c;  //TODO
	}

	public void resetLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;
		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);
		//p_cgrid.takeOut(grid_x, grid_y, gdata); //TODO
		//p_cgrid.putIn(grid_x, grid_y, gdata);  //TODO
	}

	public override void updateLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		int old_x = grid_x;
		int old_y = grid_y;
		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;
		if ((old_x != grid_x) || (old_y != grid_y))
		{
			//p_cgrid.takeOut(old_x, old_y, gdata); //TODO
			//p_cgrid.putIn(grid_x, grid_y, gdata);  //TODO
		}
	}


	public static void setCanvasGrid() //(ObjectGrid g)  //TODO
   	{
		//all of the new settings will have been set in the superclass, GameObject,
		//when setGrid() was called to set the p_grid, which has identical properties
		
		//p_cgrid = g;  //TODO

		//all we need now is the pointer
	}

	public virtual void onTouchCell()
	{
		
		//p_cgrid.takeOut(grid_x, grid_y, gdata);  //TODO

		//what now
	}

	public virtual void onTouchcell2(float n)
	{
		//define per subclass
	}


}
