using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxPauseCommand 
{
		public static int ZOOM = 0;
		public static int SCROLL_TO = 1;
		
		
		public int id;
		
		public float x;
		public float y;
		public float ox;
		public float oy;
		public float dx;
		public float dy;
		
		public int time;
		public int elapsed = 0;
		
		public float value;
		public float ovalue;
		public float dvalue;
		
		public  FauxPauseCommand(int i, object param, int t)
		{
			id = i;
			time = t;
			switch (id)
			{
				case 1:
					Point p = param as Point;
					x = p.x; y = p.y; break;
				case 0: 
					value = (float)param;
					break;
			}
		}

	public void calcZoom(float currZoom)
	{
		ovalue = currZoom;
		dvalue = (value - currZoom) / time;
	}

	public void calcScrollTo(float currX, float currY)
	{
		ox = currX;
		oy = currY;
		dx = (x - ox) / time;
		dy = (y - oy) / time;
	}
}
