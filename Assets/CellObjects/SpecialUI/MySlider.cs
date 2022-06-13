using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySlider : InterfaceElement
{
    public GameObject handle;

	protected float _value;
	protected float old_value;
		
	protected float max = 48;
	protected float min = -48;
	protected float range;
	protected float step;
	protected bool dragging = false;
    private bool _enterFrame;

    // Start is called before the first frame update
    public void Start()
    {
        range = max - min;
        step = range / 50;
    }

	/**
		 * For your slider to properly work, it must manually have its init() function called! 
		 * This is to avoid runtime errors when the items are created via code and not having
		 * been placed on stage natively
		 */

	public virtual void init()
	{
		//handle.addEventListener(MouseEvent.MOUSE_DOWN, grabHandle);
		//this.stage.addEventListener(MouseEvent.MOUSE_UP, dropHandle);
		setValue(0);
	}

	public void setValue(float v)
	{
		old_value = _value;
		_value = v;
		if (_value > 1)
			_value = 1;
		if (_value < 0)
			_value = 0;
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, _value * range,handle.transform.localPosition.z);
		updateMe();
	}

	//doesn't call certain callbacks to avoid recursive loops
	protected void hardSetValue(float v)
	{
		old_value = _value;
		_value = v;
		if (_value > 1)
			_value = 1;
		if (_value < 0)
			_value = 0;
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, _value * range, handle.transform.localPosition.z);
	}


	// Update is called once per frame
	protected virtual void updateMe()
    {
		old_value = _value;
		Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		mouse *= 100;
		Debug.Log("mouse " + mouse);
		_value = (mouse.y <= max && mouse.y >= min) ? mouse.y : (mouse.y > max ? max : min);//handle.transform.localPosition.y / range;
	}

    private void Update()
    {
        if (_enterFrame)
        {
			updateMe();
			handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, _value, handle.transform.localPosition.z);
        }
    }

    public void grabHandle()
	{
		//handle.startDrag(false, new Rectangle(handle.x, 0, 0, range));
		
		_enterFrame = true;
		dragging = true;
	}

	public void dropHandle()
	{
		//handle.stopDrag();
		
		updateMe();
		_enterFrame = false;
		dragging = false;
	}


	//to be called externally only in order to set the handle, don't call any zoom functions!
	public void oldValue()
	{
		_value = old_value;
		handle.transform.localPosition = new Vector3(handle.transform.localPosition.x, _value * range, handle.transform.localPosition.z);
		//update();
	}

}
