using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


public class Engine : MonoBehaviour
{
	public delegate void ATPChanged(float atp);
	public ATPChanged onATPChanged;
	public delegate void NAChanged(float na);
	public ATPChanged onNAChanged;
	public delegate void AAChanged(float aa);
	public ATPChanged onAAChanged;
	public delegate void FAChanged(float fa);
	public ATPChanged onFAChanged;
	public delegate void GChanged(float g);
	public ATPChanged onGChanged;


	private int _defensins_produced;
	private int _defensins_ordered;
	private int _defensin_strength;
	public CellGameEvent EngineEvent = new CellGameEvent();
	public MessageEvent EngineMessageEvent = new MessageEvent();
	public ResourceBar Glucose_ResourceBar;
	private bool[] spend_checker = new bool[] { true, true, true, true, true };
	public float r_atp
    {
		get
        {
			return r_atp_;
        }

		set
        {
			r_atp_ = value;
			onATPChanged?.Invoke(r_atp_);
        }
    }

	public float r_na
    {
		get
        {
			return r_na_;
        }

		set
        {
			r_na_ = value;
			onNAChanged?.Invoke(r_na_);
			
        }
    }

	public float r_aa
    {
		get
        {
			return r_aa_;
        }

		set
        {
			r_aa_ = value;
			onAAChanged?.Invoke(r_aa_);
        }
    }

	public float r_fa
    {
		get
		{
			return r_fa_;
		}

		set
		{
			r_fa_ = value;
			onFAChanged?.Invoke(r_fa_);
		}
	}

	public float r_g
    {
		get
		{
			return r_g_;
		}

		set
		{
			r_g_ = value;
			onGChanged?.Invoke(r_g_);
		}
	}


	private float r_atp_ = 100;
	private float r_na_ = 0;
	private float r_aa_ = 0;
	private float r_fa_ = 0;
	private float r_g_ = 0;

	private float r_max_atp = 10000;
	private float r_max_na = 1000;
	private float r_max_aa = 1000;
	private float r_max_fa = 1000;
	private float r_max_g = 1000;

	public const int SELECT_ONE = 1;
	public const int SELECT_MANY = 2;
	public const int SELECT_NONE = 0;

	bool dirty_basicUnit;

    void Start()
    {
		//r_atp = 100;
		if (Glucose_ResourceBar != null)
			Glucose_ResourceBar.SetMax(r_max_g);
	}

    public void changeZoom(float n)
	{
		//if (c_world) //TODO
			//c_world.changeZoom(n);  //TODO
	}

	public void oneLessDefensin(float n)
	{
		_defensins_produced -= (int)n;
		_defensins_ordered -= (int)n;
		dirty_basicUnit = true;
	}

	public void setDefensinStrength(float n)
	{
		_defensin_strength = (int)n;
		dirty_basicUnit = true;
	}

	public void finishDefensin()
	{
		_defensins_produced++;
		dirty_basicUnit = true;
	}

	public List<float> getSpiralPoints(Point p, float length, float rad)
	{
			List<float> list = new List<float>();
		list.Add(p.x); list.Add(p.y); //TODO: why was this originally list.add(p.x,p.y)???  //first, stick the first point in the center;
			if (length > 1) 
			{
				bool done = false;
				float currCirc = 1; //start on the 1st circle
				while (length > 0) 
				{
					float r = (rad* (currCirc* 2)) + rad; //one for the one in the middle, plus 2 for each ring
					int dots = (int) ((Mathf.PI*2* r) / (2 * rad)); //how many can fit
				
					/*List<float> circPoints = circlePointsOffset(r, dots, p.x, p.y);   //TODO
					for each(float n in circPoints)
					{
						list.Add(n);
					}*/
	
	currCirc++;
					length -= dots; //when this reaches 0 or less, we're done
				}
			}
			return list;
		}

	public void notifyOHandler(string mainType, string evType, string targType, float targNum)
	{

		//e.
		//var e:EngineEvent = new EngineEvent(mainType, evType, targType, targNum);  //TODO
		//oHandler.dispatchEvent(e);  //TODO

	}

	public bool canAfford(int atp, int na, int aa, int fa, int g) 
	{
			clearSpendCheck(new int[] { atp, na, aa, fa, g });
			if (r_atp >= atp) {
				spend_checker[0] = true;
			}
			if (r_na >= na)
			{
				spend_checker[1] = true;
			}
			if (r_aa >= aa)
			{
				spend_checker[2] = true;
			}
			if (r_fa >= fa)
			{
				spend_checker[3] = true;
			}
			if (r_g >= g)
			{
				spend_checker[4] = true;
			}
			if (spend_checker[0] && spend_checker[1] && spend_checker[2] && spend_checker[3] && spend_checker[4])
			{
				return true;
			}
			return false;
	}

	private void clearSpendCheck(int[] a = null)
	{
		if (a != null)
		{
			for (int i = 0; i < a.Length; i++) {
				if (a[i] > 0)
					spend_checker[i] = false;
				else
					spend_checker[i] = true;
			}
		}
		else
		{
			spend_checker = new bool[]{ false, false, false, false, false};
		}
	}

	public void GainATP(float i)
    {
		r_atp += i;
		EngineEvent?.Invoke("atp_gain", i);
    }

	public void GainGlucose(float i)
    {
		r_g += i;
		EngineEvent?.Invoke("glucose_gain", i);
		if (Glucose_ResourceBar != null)
			Glucose_ResourceBar.Set(i);
    }

	public bool spendATP(float i) 
	{
			clearSpendCheck(new float[]{i,0,0,0,0});
			if (r_atp >= i) 
			{
					spend_checker[0] = true;
					spend_checker[1] = true;
					spend_checker[2] = true;
					spend_checker[3] = true;
					spend_checker[4] = true;
					r_atp -= i;
			//dirty_resource = true;
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
			Debug.Log("got atp " + i);
			EngineEvent?.Invoke("atp_loss",i);
					return true;
					
			
			}
		return false;
	}

	private void clearSpendCheck(float[] a = null)
	{
		if (a != null)
		{
			for (int i = 0; i < a.Length; i++) {
				if (a[i] > 0)
					spend_checker[i] = false;
				else
					spend_checker[i] = true;
			}
		}
		else
		{
			spend_checker = new bool[] { false, false, false, false, false };
		}
	}

	public bool produce(float[] a) 
	{
			r_atp += a[0];
			r_na += a[1];
			r_aa += a[2];
			r_fa += a[3];
			r_g += a[4];
			checkMaxResource();
	/*notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
	notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "na", r_na);
	notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "aa", r_aa);
	notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "fa", r_fa);
	notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "g", r_g);*/
	//dirty_resource = true;
			return true;
		}

	public void zeroResources()
	{
		r_atp = 0;
		r_na = 0;
		r_aa = 0;
		r_fa = 0;
		r_g = 0;
		/*notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "na", r_na);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "aa", r_aa);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "fa", r_fa);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "g", r_g);*/
		//dirty_resource = true;
	}

	public bool loseResources(float[] a)
	{
		r_atp -= a[0];
		r_na -= a[1];
		r_aa -= a[2];
		r_fa -= a[3];
		r_g -= a[4];
		if (r_atp < 0) r_atp = 0;
		if (r_na < 0) r_na = 0;
		if (r_aa < 0) r_aa = 0;
		if (r_fa < 0) r_fa = 0;
		if (r_g < 0) r_g = 0;

		/*notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "na", r_na);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "aa", r_aa);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "fa", r_fa);
		notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "g", r_g);
		dirty_resource = true;*/
		return true;
	}

	private void checkMaxResource()
	{
		if (r_atp > r_max_atp)
			r_atp = r_max_atp;

		if (r_aa > r_max_aa)
			r_aa = r_max_aa;

		if (r_na > r_max_na)
			r_na = r_max_na;

		if (r_fa > r_max_fa)
			r_fa = r_max_fa;

		if (r_g > r_max_g * 2)
			r_g = r_max_g;
	}

	public bool spend(float[] a) 
	{
		clearSpendCheck(a);
		if (r_atp >= a[0]) {
			spend_checker[0] = true;
		}
		if (r_na >= a[1])
		{
			spend_checker[1] = true;
		}
		if (r_aa >= a[2])
		{
			spend_checker[2] = true;
		}
		if (r_fa >= a[3])
		{
			spend_checker[3] = true;
		}
		if (r_g >= a[4])
		{
			spend_checker[4] = true;
		}
		if (spend_checker[0] && spend_checker[1] && spend_checker[2] && spend_checker[3] && spend_checker[4])
		{
			r_atp -= a[0];
			r_na -= a[1];
			r_aa -= a[2];
			r_fa -= a[3];
			r_g -= a[4];
			/*notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
			notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "na", r_na);
			notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "aa", r_aa);
			notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "fa", r_fa);
			notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "g", r_g);
			dirty_resource = true;*/
			return true;
		}
		return false;
	}
		
	public void getRewards(float[] a) 
	{
		//p_cell.rewardProduce(a, 1, new Point(p_cell.c_centrosome.x, p_cell.c_centrosome.y));  //TODO
	}

	private void income(float[] a) 
	{

		if ((int)(a[0]) > 0)
		{
			r_atp += a[0];
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "atp", r_atp);
		}

		if (a[1] > 0)
		{
			r_na += a[1];
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "na", r_na);
		}

		if (a[2] > 0)
		{
			r_aa += a[2];
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "aa", r_aa);
		}

		if (a[3] > 0)
		{
			r_fa += a[3];
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "fa", r_fa);
		}

		if (a[4] > 0)
		{
			r_g += a[4];
			//notifyOHandler(EngineEvent.RESOURCE_CHANGE, "null", "g", r_g);
		}
		checkMaxResource();

	//dirty_resource = true;
	}



public void showImmediateWarning(String s)
	{
		//c_interface.showEnemyWarning(s);  //TODO
	}

	public void showImmediateAlertCode(int i)
	{
		string s = "";
		switch (i)
		{
			case FailCode.AT_MAX: s = ("You already have the maximum amount"); break;
			case FailCode.CANT_AFFORD: s = getFailAffordString(); break;
			case FailCode.NO_NUCLEOLUS_PORE: s = ""; break;
			case FailCode.NO_NUCLEUS_PORE: s = ""; break;

			default: 
				s = ""; break;
		}
		if (s != "")
		{
			showImmediateAlert(s);
		}
	}

	private string getFailAffordString() 
	{
			string s = "Not enough ";
			/*List<string> list = new List<string>();  //TODO
			//trace("Engine.getFailAffordString() spend_checker = " + spend_checker);
			if (!spend_checker[0]) 
			{
				list.Add("ATP");
				notifyOHandler(EngineEvent.ENGINE_TRIGGER, "not_enough", "atp", r_atp);
			}
			if (!spend_checker[1])
			{
				list.Add("NA");
				notifyOHandler(EngineEvent.ENGINE_TRIGGER, "not_enough", "na", r_na);
			}
			if (!spend_checker[2])
			{
				list.Add("AA");
				notifyOHandler(EngineEvent.ENGINE_TRIGGER, "not_enough", "aa", r_aa);
			}
			if (!spend_checker[3])
			{
				list.Add("FA");
				notifyOHandler(EngineEvent.ENGINE_TRIGGER, "not_enough", "fa", r_fa);
			}
			if (!spend_checker[4])
			{
				list.Add("G");
				notifyOHandler(EngineEvent.ENGINE_TRIGGER, "not_enough", "g", r_g);
			}
			for (int i = 0; i < list.Count; i++ ) 
			{
				s += list[i];
				if (i < list.Count - 1)
				{
					s += ", ";
					if (i == list.Count - 2)
					{
						s += "and ";
					}
				}
				else if (i == list.Count - 1)
				{
					s += "!";
				}
			}
			if (list.Count == 1)
			{
				if (list[0] == "NA")
				{
					if (p_cell.countThing("rna") > 0)
					{            //If there's RNA strands, tell the user to wait
						s += " Wait for RNA strands to dissolve!";
						//Not enough NA! Wait for RNA strands to dissolve!
					}
					else if (theLevelNum > 1)
					{                   //If there's no RNA strands, & we've introduced recycling
						bool mitoTrue = false;
						bool chloroTrue = false;
						if (p_cell.countThing("mitochondrion") > 1)
						{
							mitoTrue = true;
						}
						if (p_cell.countThing("chloroplast") > 1)
						{
							chloroTrue = true;
						}
						if (mitoTrue || chloroTrue)
						{       //If there's more than 1 mito or chloro
							s += " Recycle a";
							if (mitoTrue)
							{
								s += " mitochondrion";
							}
							if (chloroTrue)
							{
								if (mitoTrue) { s += " or "; }
								s += " chloroplast";
							}
							s += "!";
							//Not enough NA! Recycle a mitochondrion!
							//Not enough NA! Recycle a chloroplast!
							//Not enough NA! Recycle a mitochondrion or chloroplast!
						}
					}
				}
				else if (list[0] == "AA")
				{
					//lvl.levelData.
					var checkBatch:Boolean = false;
					var checkRecycle:Boolean = false;
					if (checkLevelBatch("aa"))
					{
						checkBatch = true;
						s += " Go exploring";
					}


					if (theLevelNum > 1)
					{ //if we're not in one of the intro levels before recycling is allowed
						checkRecycle = true;
						if (checkBatch)
						{
							s += ", or r";
						}
						else
						{
							s += " R"
									}
						s += "ecycle something";
					}

					if (checkBatch || checkRecycle)
					{
						s += "!";
					}
				}
				else if (list[0] == "FA")
				{
					s += " Recycle something or make more with G!";
				}
			}*/
		return s;
		}
		
		public bool checkLevelBatch(String s)
		{
		return true; // d_woz.checkLevelBatch(s);  //TODO
		}

	public void showImmediateAlert(string s) 
	{

		EngineMessageEvent?.Invoke(s);
		/*if (s == alert_last)
		{
			if (!c_interface.c_messageBar.isShown())
			{
				c_interface.showAlert(s);
			}
		}
		else
		{
			alert_last = s;
			c_interface.showAlert(s); //just show it, don't store it or worry
		}*/
	}

	public float getWorldScale()
	{
		return 1;
		//	return c_world.getScale();
	}

	public void setCursorArrowRotation(float r)
	{
		//p_director.c_cursor.setArrowRotation(r);
	}

	public void setCursorArrowPoint(float xx, float yy)
	{
		Point p = new Point(xx, yy);
		//p = c_world.reverseTransformPoint(p);
		//p_director.c_cursor.setArrowPoint(p.x, p.y);
	}

	public void endCursorArrow()
	{
		//p_director.c_cursor.endArrow();
	}

	public void showCursor(int i)
	{
		/*int length = list_cursor.Count;
		var last:int = list_cursor[length - 1];
		if (last != i)
		{
			list_cursor.push(i);
			p_director.showCursor(i);
		}
		*/
	}

	public void lastCursor()
	{
		/*var length:int = list_cursor.length;
		if (length > 0)
		{
			var last:int = list_cursor[length - 1];
			if (last != Act.NOTHING)
			{
				list_cursor.pop();
				last = list_cursor[length - 2];
				p_director.showCursor(last);
			}
			else
			{
				p_director.showCursor(Act.NOTHING);
			}
		}
		else
		{ //if we somehow arrive at an empty list
			p_director.showCursor(Act.NOTHING);
		}
		*/
	}





}



