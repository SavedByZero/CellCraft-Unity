using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataObject : MonoBehaviour
{
		public  float x;
		public  float y;
		public  float radius;
		public  float radius2;
		public  string id;
		public  object ptr; //pointer to whatever I am
		public  Type ptr_class; //what class is that thing
		
		public GameDataObject()
	{

	}


	public void setThing(float xx, float yy, float r, object thing, Type c)
	{
		x = xx;
		y = yy;
		radius = r;
		radius2 = r * r;
		ptr = thing;
		ptr_class = c;
	}
}
