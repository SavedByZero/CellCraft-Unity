using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellObject : Selectable
{
	protected Cell p_cell;
	protected Microtubule p_tube;  
		
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
	public  BigVesicle myVesicle; //the vesicle I'm in   
		
	private bool oldSelect = false;
		
	public bool isOutsideCell = false;
		
	public bool doesCollide = false; //does this collide with the membrane?
	public bool hardCollide = false; //does this stand rigidly against the membrane?

    private void Awake()
    {
		p_cell = GetComponentInParent<Cell>();
	}
    public CellObject()
	{
		setLevel(1);
		speed = 6;

	}

   

    public virtual void init()
	{
		
		getMyMoveCost();
		if (float.IsNaN(myMoveCost))
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

	public virtual void doCellMove(float xx, float yy)
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

	public virtual void getPpodContract(float xx, float yy)
	{
		x -= xx;
		y -= yy;
		if (isMoving)
		{
			if (pt_dest != null)
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
				return p_cell.spendATP(myMoveCostBySpeed) > 0;
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

	protected override void doMoveToGobj()
	{
		//if(checkMembrane()){
		if (checkSpend())
		{
			base.doMoveToGobj();
			distTravelled += speed;
			if (p_tube)
			{
				p_tube.followObj();
			}
		}
		else
		{ //Don't cancel the move if the player can't afford it, just wait
		  //cancelMoveObject();
		}
		//}
	}

	public override bool doAction(CellAction i, object parms = null)
	{
			//trace("CellObject.doAction() " + i);
			switch(i){
				case CellAction.DIVIDE:
					if (canDivide()) 
						return doDivide();
					else
						return false;
					break;
				case CellAction.RECYCLE:
					return tryRecycle();
					break;
				
			}
return false;
	}
		
	public void activate()
	{
		is_active = true;
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		//trace("CellObject.onAnimFinish() " + i + "me = " + name);
		switch (i)
		{
			case ANIM_BUD:
			case ANIM_GROW: activate(); break;

			case ANIM_RECYCLE: onRecycle(); break;
			case ANIM_DIVIDE: finishDivide(); break;
			case ANIM_DAMAGE1: hardRevertAnim(); break;
			case ANIM_DAMAGE2: hardRevertAnim(); break;
			case ANIM_PLOP: onPlop(); break;
		}
		base.onAnimFinish(i, stop);
	}

	public virtual void onHalfPlop()
	{
		p_cell.onHalfPlop(this);
	}

	protected virtual void onPlop()
	{
		p_cell.onPlop(this);
	}

	protected virtual void onRecycle()
	{
		p_cell.onRecycle(this, true, true);
	}

	public bool canDivide() 
	{ //override this to check if prerequisites have been met as well for divisible things
			return does_divide && !is_dividing;
	}

	public bool doDivide() 
	{
			is_dividing = true;
			playAnim("divide");

		bumpBubble();
			return true;
			//gotoAndStop("divide");
	}

	protected virtual void finishDivide()
	{

		is_dividing = false;
		//playAnim("normal");
		GotoAndStop("normal");
		//define per subclass
	}

	public virtual void inVesicle(BigVesicle v) 
	{
		isInVesicle = true;
		myVesicle = v;
	}

	public void outVesicle(bool unRecycle = false) 
	{
		isInVesicle = false;
		if (unRecycle && isDoomed)
		{
			unDoom();
		}
	}

	public virtual void onCanvasWrapperUpdate()
	{

	}

	public virtual void unDoom()
	{
		isDoomed = false;

		hideBubble();
		p_cell.unDoomCheck(this);
	}

	protected override void killMe()
	{
		base.killMe();
		if (selected)
		{
			p_cell.engineUpdateSelected();
		}

	}

	public void setPHDamage(float n, float mult = 1)
	{
		float damage = 0;
		if (n <= 7.5)
		{
			float diff = 7.5f - n;

			if (diff <= .25)
			{ //7.5-7.25
			  //no problem
			}
			else if (diff <= 1.0)
			{ //7.25-6.25
			  //lowered efficiency
			}
			else if (diff > 1.0)
			{
				diff -= 1.0f; //diff is now between 0 and 6.5, with 4.5 being super deadly
				diff /= 6.5f; //diff is now between 0 and 1
				damage = diff * Cell.MAX_ACID_DAMAGE;
			}
		}
		n = damage * mult;

		if (n < 0.001f)
		{
			StopAllCoroutines();
		}
		else
		{
			phDamage = n;
			if (phDamage < 1)
			{
				phDamage_time = (int)(1 / phDamage);
				phDamage_time *= 30;
				phDamage = 1;
			}
			else
			{
				phDamage_time = 30;
			}
			phDamage_counter = 0;
			//if(this is Nucleus)
			//	trace("CellObject.setPHDamage(" + n + ") phDamage_time = " + phDamage_time + " phDamage = " + phDamage);
			StartCoroutine(takePHDamage());
		}
	}

	protected virtual IEnumerator takePHDamage()
	{

		while (true)
        {
			yield return new WaitForSeconds(phDamage_time);
			phDamage_counter++;
        }
		
	}

	public override void takeDamage(float n, bool hardKill = false)
	{
		base.takeDamage(n);
		if (selected)
		{
			p_cell.updateSelected();
		}
	}

	public void updateGridLoc(float xx, float yy)
	{
		gdata.x = xx;
		gdata.y = yy;

		int old_x = grid_x;
		int old_y = grid_y;
		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;
		if ((old_x != grid_x) || (old_y != grid_y))
		{
			p_grid.takeOut(old_x, old_y, gdata);
			p_grid.putIn(grid_x, grid_y, gdata);
		}
	}

	public void clearGrid()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;

		int loc_x;
		int loc_y;
		for (int w = -1; w <= 1; w++) {
			for (int h = -1; h <= 1; h++) {
				loc_x = grid_x + w;
				loc_y = grid_y + h;
				if (loc_x < 0) loc_x = 0;
				if (loc_y < 0) loc_y = 0;
				if (loc_x >= grid_w) loc_x = (int)grid_w - 1;
				if (loc_y >= grid_h) loc_y = (int)grid_h - 1;
				p_grid.takeOut(loc_x, loc_y, gdata);
			}
		}

	}

	public void placeInGrid()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;

		p_grid.putIn(grid_x, grid_y, gdata);
	}



}
