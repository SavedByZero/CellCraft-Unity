using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : Selectable
{
	//protected var p_cell:Cell;  //TODO
	//protected var p_tube:Microtubule;  //TODO
		
	public int plopDepth = 0;
		
	protected Point home;
		
	private float distTravelled = 0;
	private float moveCostDist = Costs.MOVE_DISTANCE;
	//private const oneMicron:Number = Costs.PIXEL_TO_MICRON;
	private float myMoveCost = 0;
	private float myMoveCostBySpeed = 0;
	private bool freeMove = false;
		
	public bool is_active = false;
	public bool is_basicUnit = false;
	public bool might_collide = false;
		
	protected bool is_dividing = false;
		
	protected bool does_divide = false;
	protected bool does_move = false;

		
	protected bool does_pop = false;
		
	public MovieClip clip_divide;
		
	private const float WORST_STRETCH = 0.35f;
		
	private float phDamage = 0;
	private int phDamage_time = 30;
	private int phDamage_counter = 0;
		
		
		
	public  bool isInVesicle = false;
	//public  BigVesicle myVesicle; //the vesicle I'm in   //TODO
		
	private bool oldSelect = false;
		
	public bool isOutsideCell = false;
		
	public bool doesCollide = false; //does this collide with the membrane?
	public bool hardCollide = false; //does this stand rigidly against the membrane?

	public CellObject()
	{
		setLevel(1);
		speed = 6;

	}

	public void init()
	{

		getMyMoveCost();
		if (isNaN(myMoveCost))
		{
			myMoveCost = 0;
		}
		myMoveCostBySpeed = myMoveCost * (speed / moveCostDist);
		//trace("mymovecostbyspeed = " + myMoveCostBySpeed);
	}

	private void receiveActionList()
	{
		List<CellAction> v = p_cell.getActionListFromEngine(this.num_id);
		if (v != null)
		{
			list_actions = v;
				
		}
	}

	public void playFallSound()
	{
		SfxManager.Play(SFX.SFXDropFall);
	}

	public void playSplishSound()
	{
		SfxManager.Play(SFX.SFXSplish);
	}

	public void setCell(Cell c)
	{
		p_cell = c;
		receiveActionList(); //do this here to avoid bugs sine receiveActionList requires access to the cell
	}

	public override void giveHealth(uint amt)
	{
		base.giveHealth(amt);
		if (selected)
		{
			if (p_cell.getEngineSelectCode() == Engine.SELECT_ONE)
			{ //SUPER DUPER HACK!
				p_cell.engineUpdateSelected();
			}
		}
	}

	public void setOutsideCell(bool b)
	{
		isOutsideCell = b;
	}

	protected override void heavyDamageClip()
	{
		base.heavyDamageClip();
		p_cell.checkScrewed(this);
	}

	public void setTube(Microtubule t)
	{
		p_tube = t;
	}

	public void hideOrganelle()
	{
		this.gameObject.SetActive(false);
		oldSelect = canSelect;
		canSelect = false;
	}

	public void showOrganelle()
	{
		this.gameObject.SetActive(false);
		canSelect = oldSelect;
	}

	new void OnMouseDown()
	{
		_mDown = true;
		if (singleSelect)
		{
			_mDown = false;
			return;
			//m.stopPropagation(); //kill the click
								 //p_cell.setSelectType(Selectable.NOTHING);
		}
		else
		{
			//trace(this + " mousedown");
			p_cell.setSelectType(num_id);
			//m.stopPropagation();
		}
	}

	new void OnMouseUp()
	{
		//trace("CellObject.click() this=" + this + " singleSelect=" + singleSelect);
		if (canSelect && _mDown == true)
		{
			if (singleSelect)
			{
				p_cell.selectOne(this, Input.mousePosition.x, Input.mousePosition.y);
			}
			else
			{
				p_cell.selectMany(this, true);
			}

			_mDown = false;
		}
	}

	public void doCellMove(float xx, float yy)
	{
		x += xx;
		y += yy;
		if (isMoving)
		{
			if (pt_dest != null)
			{
				pt_dest.x += xx;
				pt_dest.y += yy;
			}
		}
	}

	public void getPpodContract(float xx, float yy)
	{
		x -= xx;
		y -= yy;
		if (isMoving)
		{
			if (pt_dest)
			{
				pt_dest.x -= xx;
				pt_dest.y -= yy;
			}
			//calcMovement();
		}
	}


	protected void deployCytoplasm(float xx, float yy, float radius, float spread, bool free = true, bool instant = false)
	{
		Vector2 v = new Vector2(radius + Random.Range(0,1) * spread, 0); //Nucleus Radius = 75
		float radians = Random.Range(0,1) * (Mathf.PI * 2);
		float ca = Mathf.Cos(radians);
		float sa = Mathf.Sin(radians);
		float rx = x * ca - y * sa;
		float ry = x * sa + y * ca;
		v.x = rx;
		v.y = ry;

		
		Point p = new Point(v.x,v.y);
		p.x += xx;
		p.y += yy;

		if (!instant)
		{
			moveToPoint(p, FLOAT, free);
		}
		else
		{
			//trace("CellObject.deployCytoplasm() INSTANT");
			x = p.x;
			y = p.y;
			home = new Point(0,0);
			home.x = p.x - cent_x;
			home.y = p.y - cent_y;
		}
	}

	protected void getMyMoveCost()
	{
		float num = Costs.getMoveCostByString(text_id);
		myMoveCost = num;
	}

	protected bool checkMembrane() 
	{
			return true;
			
	}

	protected bool checkSpend()
	{
		bool okay = true;


		if (freeMove)
			return true;
		else
		{
			if (p_cell)
			{
				return p_cell.spendATP(myMoveCostBySpeed);
			}
			else
			{
				return false; //SOMETHING IS WRONG!
			}
		}

		//return okay;
	}

	protected void finishSpend()
	{

	}

	protected override void onArrivePoint()
	{
		base.onArrivePoint();
		finishSpend();
	}

	protected override void onArriveObj()
	{
		base.onArriveObj();

		finishSpend();
	}

	public override void externalMoveToPoint(Point p, int i)
	{
		moveToPoint(p, i);
	}

	public override void moveToPoint(Point p, int i, bool free = false)
	{
		freeMove = free;
		base.moveToPoint(p, i, free);
	}

	public override void moveToObject(CellGameObject o, int i, bool free = false)
	{
		freeMove = free;
		base.moveToObject(o, i, free);
	}

	public override void push(float xx, float yy)
	{
		base.push(xx, yy);
		if (p_tube)
		{
			p_tube.followObj();
		}

	}

	protected override void doMoveToPoint()
	{
		//if(checkMembrane()){
		if (checkSpend())
		{
			base.doMoveToPoint();
			distTravelled += speed;
			if (p_tube)
			{
				p_tube.followObj();
			}
		}
		else
		{ //Don't cancel the move if the player can't afford it, just wait
		  //cancelMovePoint();
		}
		//}
	}
}
