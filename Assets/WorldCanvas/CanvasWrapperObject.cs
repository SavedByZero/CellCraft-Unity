using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
	 * This class is used to put a CellObject in the Canvas. You can move it around like any canvas object, but it will
	 * take its childed cellobject with it, which always stays at 0,0 relative to this, which is its parent.
	 * @author Lars A. Doucet
	 */
public class CanvasWrapperObject : CanvasObject
{
	//public var c_cellObj:CellObject;  //TODO
	//private var p_cell:Cell;   //TODO
	
	public CanvasWrapperIcon c_icon;
	public string content = "";
	private float maxSpeed = 10;
	private float minSpeed = 1;

	/*
	public void setCell(Cell c)  //TODO
	{
		p_cell = c;
	}*/


	/*
	public void setCellObj(CellObject c)  //TODO
	{
		c_cellObj = c;
		c_cellObj.x = 0;
		c_cellObj.y = 0;
		c_cellObj.setCanSelect(false);
		addChild(c_cellObj);
	}*/

	public override void matchZoom(float Number)
	{

		//c_cellObj.onCanvasWrapperUpdate(); //TODO
	}

	public void makeVesicleObjectFromId(string id)
	{
		Debug.Log("CanvasWrapperObject.makeVesicleObjectFromID(" + id + ")!");
		content = id;
		if (c_icon)
		{
			c_icon.GotoAndStop(id);
		}
		else
		{
			//TODO: make some kind of object pooling thing here to replace the commented out code below. 

			//c_icon = new CanvasWrapperIcon();
			//c_icon.x = 0;
			//c_icon.y = 0;
			//addChild(c_icon);
			//c_icon.gotoAndStop(id);
		}
		float radius = c_icon.getRadius() * 1.25f;
		setRadius(radius);

		/*BigVesicle v = p_cell.export_makeBigVesicle(radius);   //TODO
		setCellObj(v);*/
		c_icon.transform.SetParent(this.transform, false);
		c_icon.transform.SetSiblingIndex(this.gameObject.transform.childCount - 1);  //put icon back on top
	}

	protected override void doMoveToGobj()
	{

		base.doMoveToGobj();
		if (lastDist2 < LENS_RADIUS2 * 1.1)
		{ //slow down as we approach the cell
			speed = minSpeed;
		}
		else
		{
			speed = maxSpeed;               //speed up if we're far away
		}
	}

	public override void onTouchCell()
	{
		Debug.Log("CanvasWrapperObject.onTouchCell() " + this);
		if (!dying)
		{
			base.onTouchCell();
			//p_canvas.onTouchCanvasWrapper(this);  //TODO
			dying = true;
		}
	}




}
