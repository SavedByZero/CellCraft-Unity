using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthGem : MovieClip
{
	public Text c_text;  //TODO: Verdana
	
		
	public HealthGem()
	{
		amount = 100;
	}


	public int amount
	{
		set {
			c_text.text = value.ToString();
			GotoAndStop(value + 1);
		}
		
	}
}
