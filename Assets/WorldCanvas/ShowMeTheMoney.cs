using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShowMeTheMoney : CanvasObject
{
	public MovieClip icon;
	public Text text;
		
	private Point trueLoc;
	private Point trueLocTxt;
	private Point trueLocIcon;
		
	private string type;
	private float amount;
		
	private float zoom = 1;

	private float offset = 0;
	private float OFFSET_AMOUNT = 30;
		
	private int zoomOffX = 0;
		
	private const float ZOOM_OUT_MAX = 0.2f; //from 0-this, show nothing
	private const float ZOOM_OUT_HIGH = 0.3f; //from MAX-this, show tiny 
	private const float ZOOM_OUT_LOW = 0.4f; //from HIGH-this, show mid 
	private const float ZOOM_OUT_MIN = 1.0f; //from LOW-this+, show normal 
		
	private const float RISE_AMOUNT = 1; //how many frames to rise for
	private float riseSpeed = 1;		//how many screen-space pixels to rise each frame
	private float riseSign = 1;		//how many screen-space pixels to rise each frame
	private float riseCount = 0;
		
	private bool scaleMode = false;

	/**
		 * This class is the icon that shows up whenever you get or lose a resource
		 */

	public ShowMeTheMoney(string t, float a, float xx, float yy, float speed = 1, float off = 0, bool scMode = true)
	{
		trueLoc = new Point(xx, yy);
		trueLocTxt = new Point(text.transform.position.x, text.transform.position.y);
		//trueLocIcon = new Point(text.x,text.y);
		riseSpeed = speed;

		setType(t);
		setAmount(a);
		setScaleMode(scMode);

		offset = off;
		//x = xx;
		//y = yy - offset * (OFFSET_AMOUNT / zoom);
	}

	public void setScaleMode(bool mode)
	{
		scaleMode = mode;
		//matchZoom(zoom);
	}

	public void setType(string t)
	{
		type = t;
		icon.GotoAndStop(t);
	}

	public void setAmount(float a)
	{
		amount = a;
		string s = "";
		string s1 = "";
		uint c;
		if (amount > 0)
		{
			c = 0xFFFFFF;
			s = "+";
			riseSign = 1;
			setGlow(1);
			text.gameObject.SetActive(true);
		}
		else if (amount < 0)
		{
			c = 0xFF0000;

			s = "<font color=\"#FFFFFF\">";
			s1 = "</font>";
			riseSign = -1; //fall
			setGlow(-1);
			text.gameObject.SetActive(true);
		}
		else if (amount == 0)
		{
			riseSign = 1;
			//setGlow(1);
			text.gameObject.SetActive(false);
		}
		if (type != "fire")
		{
			text.fontStyle = FontStyle.Bold;
			text.text = a.ToString();//.htmlText = "<b>" + s + a.toFixed(0) + s1 + "</b>";
		}
		else
		{
			text.fontStyle = FontStyle.Bold;
			text.text = a.ToString(); //.htmlText = "<b>" + s + a.toFixed(2) + s1 + "</b>";
		}

		this.transform.DOBlendableMoveBy(Vector3.up * 30, 1);

		//addEventListener(RunFrameEvent.RUNFRAME, rise, false, 0, true);
	}


	/*public override void matchZoom(float n)
	{
		zoom = n;
		base.matchZoom(n);
		x = trueLoc.x;
		y = trueLoc.y - offset * (OFFSET_AMOUNT / zoom);

		if (scaleMode)
		{
			updateIcon();
			updateText();
		}
	}*/

	public void updateIcon()
	{
		icon.gameObject.SetActive(true);
		if (zoom <= ZOOM_OUT_MAX)
		{ //zoomed all the way out
			icon.gameObject.SetActive(false);
		}
		else if (zoom <= ZOOM_OUT_HIGH)
		{ //zoomed high
			icon.GotoAndStop(type + "_tiny");
		}
		else if (zoom <= ZOOM_OUT_LOW)
		{
			icon.GotoAndStop(type + "_");
		}
		else if (zoom <= ZOOM_OUT_MIN)
		{
			icon.GotoAndStop(type);
		}
	}

	public void setGlow(int i)
	{
		/*var glow:GlowFilter;  //TODO
		var glow2:GlowFilter;
		if (i == 1)
		{
			glow = new GlowFilter(0x000000, 1, 4, 4, 5);
			text.filters = [glow];
		}
		else
		{
			glow = new GlowFilter(0xFF0000, 1, 3, 3, 3);
			glow2 = new GlowFilter(0x990000, 1, 3, 3, 3);
			text.filters = [glow, glow2];
		}*/  
	}

	public void updateText()
	{
		text.gameObject.SetActive(true);
		if (zoom <= ZOOM_OUT_MAX)
		{ //zoomed all the way out
			zoomOffX = 0;
			text.gameObject.SetActive(false);
		}
		else if (zoom <= ZOOM_OUT_HIGH)
		{ //zoomed high
			zoomOffX = 10;
			if (amount < 0)
			{
				text.color = Color.white;
				text.fontStyle = FontStyle.Bold;
				text.text = "-";//.htmlText = "<b><font color=\"#FFFFFF\">-</b></font>";
			}
			else if (amount > 0)
			{
				text.text = "+";
				text.fontStyle = FontStyle.Bold;
				//text.htmlText = "<b>+</b>";
			}
			else
			{
				text.fontStyle = FontStyle.Normal;
				text.text = "";
			}
		}
		else if (zoom <= ZOOM_OUT_LOW)
		{
			zoomOffX = 5;
			setAmount(amount);
		}
		else if (zoom <= ZOOM_OUT_MIN)
		{
			zoomOffX = 0;
			setAmount(amount);
		}

		//text.x = trueLocTxt.x + zoomOffX;
	}

	private void terminate()
	{
		//p_canvas.removeMeTheMoney(this);  //TODO
	}

}
