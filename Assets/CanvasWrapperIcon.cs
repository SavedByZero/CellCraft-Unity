using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasWrapperIcon : MovieClip
{
	//public var clip:MovieClip;
		
	

	public float getRadius() 
	{
			if (MaxWidth > MaxHeight) 
		{
				return MaxWidth/2;
			}
		else
			{
				return MaxHeight / 2;
			}
	}
}
