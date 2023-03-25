using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : CellGameObject
{
	public const int NOTHING = -1;
	public const int CENTROSOME = 0;
	public const int CYTOSKELETON = 1;
	public const int MEMBRANE = 2;
	public const int NUCLEUS = 3;
	public const int _ER = 4;
	public const int GOLGI = 5;
	public const int CHLOROPLAST = 6;
	public const int MITOCHONDRION = 7;
	public const int SLICER_ENZYME = 8;
	public const int RIBOSOME = 9;
	public const int VESICLE = 10;
	public const int PEROXISOME = 11;
	public const int LYSOSOME = 12;
	public const int BIGVESICLE = 13;
	public const int PROTEIN_GLOB = 14;
	public const int GLYCOGEN = 15;
	public const int VIRUS = 16;
	public const int VIRUS_INJECTOR = 17;
	public const int VIRUS_INVADER = 18;
	public const int VIRUS_INFESTER = 19;
	public const int DEFENSIN = 30;
	public const int FREE_RADICAL = 40;
	public const int DNAREPAIR = 50;
	public const int HARDPOINT = 70;
	public const int TOXIN = 90;
	public const int TOXIN_PARTICLE = 91;

	public bool isDoomed = false; //if you're doomed, you are GOING to die and can't go anywhere
	protected bool does_recycle = false;
	public bool isRecycling = false;

	protected bool canSelect = true;
	protected bool singleSelect = false; //is this thing selectable with click only, or the grow circle?
	protected string text_title = "UNKNOWN";
	protected string text_description = "No Data";
	public string text_id = "unknown";   //public for speed reading purposes
	public int num_id = -1;
	protected bool[] bestColors = {false, false, false};
		
	protected bool selected;
		
	protected float infestation = 0;
	protected float max_infestation = 100;
	protected bool hasInfestation = false;
		
	protected int infest_count = 0;
	protected int INFEST_TIME = 30; 
	protected float INFEST_MULT = 5;
	protected float infest_rate = 0.0f;
		
	protected List<CellAction> list_actions;
		
	public bool recycleOfMany = false;

	protected bool _mDown;

	public override void Start()
	{
		base.Start();
		//addEventListener(MouseEvent.MOUSE_DOWN, mouseDown); //TODO
		//addEventListener(MouseEvent.CLICK, click);  //TODO
		list_actions = new List<CellAction> ();
		setupActions();
		//buttonMode = true;
	}

	public void setActions(List<CellAction> v)
	{
		list_actions = v;
	}

	public void setupActions()
	{
		list_actions = new List<CellAction>(){CellAction.MOVE, CellAction.REPAIR, CellAction.DIVIDE, CellAction.RECYCLE, CellAction.POP};

		//list_actions = Vector.<int>([Act.MOVE, Act.RECYCLE, Act.POP, Act.REPAIR, Act.DIVIDE]);
	}

	public float getInfest() {
			return infestation;
		}

	public float getMaxInfest()
	{
		return max_infestation;
	}

	public string getTextTitle()
	{
		return text_title;
	}

	public string getTextDescription()
	{
		return text_description;
	}

	public string getTextID()
	{
		return text_id;
	}

	public int getNumID()
	{
		return num_id;
	}

	public void setCanSelect(bool yes) {
		canSelect = yes;
	}

	public bool getCanSelect()
	{
		return canSelect;
	}

	public bool getSingleSelect()
	{
		return singleSelect;
	}

	public void makeDoomed()
	{
		isDoomed = true;
		doomline();
	}

	public virtual void releaseFromLysosome(Lysosome l)
	{

	}

	public virtual void targetForEating(Lysosome l = null)
	{
		makeDoomed();
	}

	public override void startGetEaten()
	{
		if (!isDoomed)
		{
			cancelMove();
			makeDoomed();
		}
		base.startGetEaten();
	}

	public void makeSelected(bool yes)
	{
		if (canSelect)
		{
			if (yes)
			{
				selected = true;
				outline();
				//hilight(0.5);
			}
			else
			{
				selected = false;
				unOutline();
				//unHilight();
			}
		}
	}

	public bool isSelected()
	{
			return selected;
	}

	public List<CellAction> getActionList()
	{
		if (!isDoomed)
		{
			return list_actions;
		}
		else
		{
			return getDoomList();
		}
	}

	public List<CellAction> getDoomList() 
	{
		List<CellAction> v = new List<CellAction>();
		v.Add(CellAction.CANCEL_RECYCLE);
		return v;
	}

	public void darken()
	{
		darklight(0.25f);
	}

	public void undarken()
	{
		darklight(0);
	}

	protected void darklight(float amount)
	{
		//var c:ColorTransform = this.transform.colorTransform;
		SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
		Color c = sr.color;
		c = new Color(1 - amount, 1 - amount, 1 - amount);
		

		sr.color = c;
	}

	protected void doomline()
	{
		//I don't know what to do with this
		showBubble("x");
	}

	protected void outline()
	{
		//TODO: get an outline component ready for this 

		/*var f:GlowFilter = new GlowFilter(0xFF9900, 1, 3, 3, 3);
		var f1:GlowFilter = new GlowFilter(0xFFFF00, 1, 3, 3, 3);
		var f2:GlowFilter = new GlowFilter(0xFFFFFF, 1, 5, 5, 5);
		this.filters = [f2, f1, f];*/
	}

	protected void unOutline()
	{
		//TODO: remove outline component, when it is ready
		/*
		this.filters = [];
		if (isDoomed)
		{
			doomline();
		}*/

	}

	protected void hilight(float amount)
	{
		SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
		Color c = sr.color;


		//var c:ColorTransform = this.transform.colorTransform;
		if (!bestColors[0])
		{
			//c.redMultiplier = 1 - amount;
			c.r = 1 - amount;
		}
		//c.redOffset = 255 * amount;

		if (!bestColors[1])
		{
			c.g = 1 - amount;
			//c.greenMultiplier = 1 - amount;
		}
		//c.greenOffset = 255 * amount;

		if (!bestColors[2])
		{
			c.b = 1 - amount;
			//c.blueMultiplier = 1 - amount;
		}
		//c.blueOffset = 255 * amount;

		//this.transform.colorTransform = c;
	}

	protected void unHilight()
	{
		SpriteRenderer sr = this.gameObject.GetComponent<SpriteRenderer>();
		sr.color = Color.white;

		
	}

	public virtual void OnMouseDown()
	{
		if (canSelect)
		{
			_mDown = true;
			if (singleSelect)
			{
				_mDown = false;
				//m.stopPropagation(); //kill the click
			}
			else
			{

			}
		}
	}

	public virtual void OnMouseUp()
	{
		if (canSelect && _mDown)
		{
			if (singleSelect)
			{
				Debug.Log("click");
			}
		}
		_mDown = false;
	}

	public static int[] countSelectables(List<Selectable> vc) 
	{
			int r = 0; //ribosomes
			int l = 0; //lysosomes
			int p = 0; //peroxisomes
			int v = 0; //vesicles
			int se = 0; //slier enzymes
			
			int n = 0; //number of things
			
			for (int i=0; i < vc.Count; i++)
			{
				Selectable item = vc[i];
				switch (item.getNumID())
				{
					case RIBOSOME: r++; break;
					case LYSOSOME: l++; break;
					case PEROXISOME: p++; break;
					case VESICLE: v++; break;
					case SLICER_ENZYME: se++; break;
				}
			}
			if (r > 0) n++;
			if (l > 0) n++;
			if (p > 0) n++;
			if (v > 0) n++;
			if (se > 0) n++;
			return new int[]{ n, r, l, p, v, se};
		}



		public virtual bool tryRecycle(bool oneOfMany = false)
		{
			if (oneOfMany)
			{
				recycleOfMany = true;
			}
			if (does_recycle && !isRecycling)
			{
				doRecycle();
				return true;
			}
			return false;
		}

		public virtual void doRecycle()
		{
			cancelMove();
			isRecycling = true;
			playAnim("recycle");
		}



	public override void moveToObject(CellGameObject o, int i, bool free = false)
	{
		if (!isDoomed)
		{
			base.moveToObject(o, i, free);
		}
	}

	public virtual void externalMoveToPoint(Point p, int i) {
		moveToPoint(p, i);
	}

	public override void moveToPoint(Point p, int i, bool free = false)
	{
		if (!isDoomed)
		{
			base.moveToPoint(p, i, free);
		}
	}
		


}
