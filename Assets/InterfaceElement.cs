using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceElement : MovieClip
{
    //protected var p_master:Interface; //TODO

    //Stuff to help the tooltip work
    //private var ttTimer:Timer;  //TODO
	protected string ttString = "nothing";

	//tooltip offsets
	private float tt_xoff = 0;
	private float tt_yoff = 0;
		
		//a sub element is an interface element inside of another interface element
	private bool subElement = false;
		
		//if we are a subinterfaceelement
	private float parent_xoff = 0;
	private float parent_yoff = 0;
		
	private int shoveH = 0;
	private int shoveV = 1;
		
	protected bool tt_onClick = false;
		
	public GameObject c_black;
	public bool isBlackedOut = true;

	public InterfaceElement()
	{
		//ttTimer = new Timer(500, 0);  //TODO

		unBlackOut();
		setInterface();

	}

	public void blackOut()
	{
		if (!isBlackedOut)
		{
			isBlackedOut = true;
			//removeEventListener(MouseEvent.MOUSE_OVER, overWait);  //?
			//removeEventListener(MouseEvent.MOUSE_OUT, doOut);  //?
			//removeEventListener(MouseEvent.CLICK, clickTT);  //?
			if (c_black != null)
				c_black.SetActive(true);
			
		}
		//visible = false;
	}

	public void unBlackOut()
	{
		if (isBlackedOut)
		{
			isBlackedOut = false;
			//addEventListener(MouseEvent.MOUSE_OVER, overWait);  //?
			//addEventListener(MouseEvent.MOUSE_OUT, doOut);  //?
			//addEventListener(MouseEvent.CLICK, clickTT);  //?
			if (c_black)
				c_black.SetActive(false);
		}
		//visible = true;
	}
	/*
	public function getMaster():Interface{   //TODO
			return p_master;
		}

	public function setMaster(i:Interface){   //TODO
		p_master = i;
		//trace(this + "received master: " + master);
	}*/

	public void setInterface()
	{
		GameObject parentObj = this.transform.parent.gameObject;
		/*if (parent is Interface)
			p_master = Interface(parent);  //TODO
		else*/ if (parentObj.GetComponent<InterfaceElement>() != null)
		{
			subElement = true;
			parent_xoff = parentObj.transform.position.x;
			parent_yoff = parentObj.transform.position.y;
			//trace(this + "waiting for master");
		}
	}

	public void setTTString(string s)
	{
		ttString = s;
	}

	public void setTToff(float xx, float yy)
	{
		tt_xoff = xx;
		tt_yoff = yy;
	}

	protected void clickTT()//(m:MouseEvent)
	{
		if (tt_onClick)
		{
			showTooltip();
		}
	}

	private void overWait()//(m:MouseEvent)
	{
		//ttTimer.reset();  //TODO
		//ttTimer.start();  //TODO
	}

	public void tt_shove(int xx, int yy)
	{ //this element walled on the right
		if (xx == 1 || xx == 0 || xx == -1)
			shoveH = xx;
		else
			Debug.LogError("xx = " + xx + ": tt_shove(xx,yy) only accepts 1,0,-1 as input!");

		if (yy == 1 || yy == 0 || yy == -1)
			shoveV = yy;
		else
			Debug.LogError("yy = " + yy + ":tt_shove(xx,yy) only accepts 1,0,-1 as input!");
	}

	private void showTooltip()
	{
		//p_master.showTooltip(ttString, x + tt_xoff + parent_xoff, y + tt_yoff + parent_yoff, shoveH, shoveV);  //TODO
	}

	private void overTime()//(t:TimerEvent)
	{
		//trace("master = " + master);
		if (ttString != "nothing")
		{
			showTooltip();
		}
		//ttTimer.removeEventListener(TimerEvent.TIMER, overTime);  //TODO
	}

	private void doOut()//(m:MouseEvent)
	{
		resetTimer();
		/*if (p_master)  //TODO
		{
			p_master.hideTooltip(ttString);
			onHideTooltip();
		}*/
	}

	protected void onHideTooltip()
	{
		//per subclass define
	}

	private void resetTimer()
	{
		//ttTimer.addEventListener(TimerEvent.TIMER, overTime);  //TODO
		//ttTimer.reset();  //TODO
	}

	public void glow()
    {
		//TODO - a programmatic glow seemed to be applied to the "Cross" Symbol here.  Something will have to replace this xml.
		//
		/*< Motion duration = "30" xmlns = "fl.motion.*" xmlns: geom = "flash.geom.*" xmlns: filters = "flash.filters.*" >
		 
			 < source >
		 
				 < Source frameRate = "30" x = "-4" y = "-3" scaleX = "1" scaleY = "1" rotation = "0" elementType = "sprite" symbolName = "Cross" class="InterfaceElement">
			<dimensions>
				<geom:Rectangle left = "-75" top="-75" width="150" height="150"/>
			</dimensions>
			<transformationPoint>
				<geom:Point x = "0.5" y="0.5"/>
			</transformationPoint>
		</Source>
	</source>

	<Keyframe index = "0" tweenSnap="true" tweenSync="true">
		<tweens>
			<SimpleEase ease = "0.97" />
		</ tweens >
		< filters >
			< filters:GlowFilter blurX = "0" blurY="0" color="0xFFFFFF" alpha="1" strength="1" quality="1" inner="false" knockout="false"/>
			<filters:GlowFilter blurX = "0" blurY="0" color="0x00FF00" alpha="1" strength="1" quality="1" inner="false" knockout="false"/>
		</filters>
	</Keyframe>

	<Keyframe index = "14" tweenSnap="true" tweenSync="true">
		<tweens>
			<SimpleEase ease = "-1" />
		</ tweens >
		< filters >
			< filters:GlowFilter blurX = "10" blurY="10" color="0xFFFFFF" alpha="1" strength="4" quality="1" inner="false" knockout="false"/>
			<filters:GlowFilter blurX = "10" blurY="10" color="0x00FF00" alpha="1" strength="2" quality="1" inner="false" knockout="false"/>
		</filters>
	</Keyframe>

	<Keyframe index = "29" >
		< filters >
			< filters:GlowFilter blurX = "0" blurY="0" color="0xFFFFFF" alpha="1" strength="1" quality="1" inner="false" knockout="false"/>
			<filters:GlowFilter blurX = "0" blurY="0" color="0x00FF00" alpha="1" strength="1" quality="1" inner="false" knockout="false"/>
		</filters>
	</Keyframe>
</Motion>;*/
    }





}
