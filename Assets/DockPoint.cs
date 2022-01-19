using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockPoint : Point
{
	
	public bool busy = false;
		public int index = 0;
		public float BUSY_TIME = 30*5; //5 seconds
		public float busy_count = 0;
			
		public DockPoint(float xPos  = 0, float yPos = 0) : base(xPos, yPos)
		{

		}

	public void makeBusy()
	{
		busy = true;
		busy_count = 0;
	}

	public void unBusy()
	{
		busy = false;
		busy_count = 0;
	}

	public void setBusyTime(int i)
	{
		BUSY_TIME = i;
	}

	public void busyCount()
	{
		if (busy)
		{
			busy_count++;
			if (busy_count > BUSY_TIME)
			{
				busy = false;
				busy_count = 0;
			}
		}
	}

	public DockPoint copy() 
	{
			DockPoint d = new DockPoint();
		d.x = x;
			d.y = y;
			d.busy = busy;
			d.index = index;
			return d;
	}
}
