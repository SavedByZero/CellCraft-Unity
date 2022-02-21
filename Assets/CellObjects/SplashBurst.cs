using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SplashBurst : CellObject
{
	public int iteration = 1;
		
	public float popRadius;
		
	private const float SPEED = 2;
	private const float DIST = 40;
		

		
	public SplashBurst()
	{
	
	}

	public void startPopping()
	{
		//var p:Point = new Point(x + (v.x * DIST * iteration), y + (v.y * DIST * iteration));
		//var p:Point = new Point(x + ((v.x) * (DIST)), y + ((v.y) * (DIST)));
		//moveToPoint(p, FLOAT, true);
		int i = (int)Mathf.Floor(UnityEngine.Random.Range(0f,1f) * 3);
		switch (i)
		{
			case 0: playAnim("pop"); break;
			case 1: playAnim("pop1"); break;
			case 2: playAnim("pop2"); break;
			default: playAnim("pop"); break;
		}
		//playAnim("pop");
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		switch (i)
		{
			case ANIM_POP: destroy(); break;
		}
	}

	private void destroy()
	{
		p_cell.killSplashBurst(this);
	}

	protected override void arrivePoint(bool wasCancel = false)
	{
		//trace("PopVesicle.arrivePoint() " + this.name);
		base.arrivePoint(wasCancel);

		//p_cell.popVesicle(this);

	}

}

