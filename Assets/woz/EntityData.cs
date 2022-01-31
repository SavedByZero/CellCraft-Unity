using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData 
{
	public bool is_thing; //is this a specific thing, or not?
		
		public Vector2 loc;	//where is this located?
		public string name;		//what is the name of this?
		public string type;		//what is this?
		public int count;		//how many are there
		public float aggro;	//if it's hostile, how hostile?
		public float spawn;	//does it spawn? how often?
		public bool Boolean = false; //is it activated?
				
		public EntityData()
		{
			loc = new Vector2();
		}
}
