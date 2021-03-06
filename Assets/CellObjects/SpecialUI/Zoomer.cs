using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomer : MySlider
{
    public GameObject plus;  //button
    public GameObject minus;//:SimpleButton;
	private const float zRange = .02f;
	private float oldZoom = 0;
	private float zoomScale = 0.5f;
	//public GameObject SceneParent;
	//public GameObject CellMembraneHolder;

	public override void init()
	{
		base.init();
		//minus.addEventListener(MouseEvent.CLICK, doMinus);
		//plus.addEventListener(MouseEvent.CLICK, doPlus);
		setValue(0.5f);
		updateMe();
	}

	public void doPlus()
	{
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, handle.transform.localPosition.y + step, handle.transform.localPosition.z);
		if (handle.transform.localPosition.y > max)
			handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, max, handle.transform.localPosition.z);

		updateMe(1);
	}

	public void doMinus()
	{
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, handle.transform.localPosition.y - step, handle.transform.localPosition.z);
		if (handle.transform.localPosition.y < min)
			handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, min, handle.transform.localPosition.z);

		updateMe(-1);
	}

	public void unTakeOver()
	{
		//var t:ColorTransform = this.transform.colorTransform;
		//t.redOffset = 0;
		//this.transform.colorTransform = t;
	}

	public void takeOver()
	{
		//var t:ColorTransform = this.transform.colorTransform;
		//t.redOffset = 255;
		//this.transform.colorTransform = t;
	}

	public void wheelZoom(int i)
	{
		int j = 0;
		if (i > 0)
		{
			for (j = 0; j < i; j++)
			{
				doPlus();
			}
		}
		else if (i < 0)
		{
			for (j = 0; j > i; j--)
			{
				doMinus();
			}
		}
	}

	/**
		 * Give me the scale you want in the game, I'll get the zoom
		 * @param	zoom
		 */

	public void setZoomScale(float zs)
	{
		//if you want .25 scale, that's going to be when the val is .75
		//that is 1 - zoomScale gives us .75
		zoomScale = zs;
		float val = (1 - zoomScale);
		if (val < 0.1) val = 0.1f;
		if (val > 1) val = 1;
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, val * range,handle.transform.localPosition.z);

		updateMe();
	}

	protected override void updateMe(float dir = 0)
	{
		base.updateMe();
		float zoom = dir;// (dir > 0 ? 0.1f : -0.1f);
		//float zoom = (zRange - (_value * zRange) + 0.1f); //
		Debug.Log("value zoom " + zoom);
		//if (SceneParent != null)
		{
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + zoom);
			//SceneParent.transform.localScale = new Vector3(SceneParent.transform.localScale.x + zoom, SceneParent.transform.localScale.y + zoom, 1);
			//CellMembraneHolder.transform.localScale = new Vector3(CellMembraneHolder.transform.localScale.x + zoom, CellMembraneHolder.transform.localScale.y + zoom, 1);
			//p_master.changeZoom(zoom); //TODO //do this to get the right values
		}
		oldZoom = zoom;

		float val = handle.transform.localPosition.y / range;
		val = 1 - val;
		zoomScale = val;
	}


	public float getZoomScale() 
	{
		return (zoomScale); 
	}
		

}
