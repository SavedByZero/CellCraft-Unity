using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuddleData : EntityData
{
	private string _goodieType;
	private float _radius;
	private float _area;
	private float _concentration;
	private float _amount;
   public PuddleData() : base()
    {

    }

	public string goodieType
	{
		set
        {
			string s = value.ToLower();
			_goodieType = s;
			if (s == "aa")
			{
				//_concentration = Cell.AA_CONCENTRATION;  //TODO
			}
			else if (s == "na")
			{
				//_concentration = Cell.NA_CONCENTRATION; //TODO
			}
			else if (s == "fa")
			{
				//_concentration = Cell.FA_CONCENTRATION;  //TODO
			}
			else if (s == "g")
			{
				//_concentration = Cell.G_CONCENTRATION;  //TODO
			}
			else
			{
				Debug.LogError("Unrecognized goodie type : " + s);
			}
		}
		
	}

	public float radius
	{
		set
        {
			_radius = value;
			_area = _radius * _radius * Mathf.PI;
			_amount = _concentration * _area;
		}
		
	}

	public float amount {
		get {
			return _amount;
		}
			
	}
}
