using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class Virus : CellObject
{
	public string type;
	public bool atCell = false; //at the membrane
	public bool inCell = false; //inside the cell
		
	public int tauntCount = 0;
	public int TAUNT_TIME = 15;
		
	public const int POS_OUTSIDE_CELL = 0;
	public const int POS_INSIDE_CELL = 1;
	public const int POS_TOUCHING_CELL = 2;
		
	public const int MOT_INVADING_CELL = 0;
	public const int MOT_INJECTING_CELL = 1;
	public const int MOT_ESCAPING_CELL = 2;
	public const int MOT_INFESTING_CELL = 3;
		
	public const int CON_MOVE_TO_RIBOSOME = 0;
	public const int CON_MOVE_TO_MEMBRANE = 1;
	public const int CON_MOVE_TO_NUCLEUS = 2;
	public const int CON_MOVE_TO_EXIT = 3;
		
	public const int DYING = 2;
		
	public int position_state = POS_OUTSIDE_CELL;
	public int motivation_state = -1;
    protected Coroutine _tryEnter;
    public int condition_state = -1;
		
	public MembraneNode mnode;
	public Ribosome rib;
	public Nucleus nuc;
	public Point nuc_node;
		
	public bool entering = false; //are we entering or exiting the membrane?
	public bool leaving = false; //are we leaving the gameplay zone?
	public int spawnCount = 1;
	public int rnaCount = 1;
	public WorldCanvas p_canvas;
	public string wave_id;
	protected float normal_speed;
	protected float escape_speed;
	protected float inside_speed;
		
	public bool toCent = false;
		
	int absorbCount = 0;
	const int ABSORB_TIME = 10;
		
	//public static var pv_grid:ObjectGrid;
		
	public bool isNeutralized = false;
		
	protected int DMG_PIERCE_MEMBRANE = 1;
	public const float INJECT_DISTANCE = 30;
		
	public int newnode_count = 0;
	public int NEWNODE_TIME = 5;
		
	private List<Lysosome> list_lyso;
		
	private int waitEatCounter = 0;
	private int WAIT_EAT_TIME = 30;
		
	private bool shieldBlocked = false;
	
	private bool vesicleGrow = false;
	private bool vesicleShrink = false;
	public bool doesVesicle = false;
	public bool hasVesicle = false;
	private Graphics c_ves_shape;
	private float ves_size = 0;
	private float ves_max_size = 0;
	private float ves_size_grow = 0.5f;
		
	private uint cyto_col = 0x44AAFF;
	private uint spring_col = 0x0066FF;
	private uint gap_col = 0x99CCFF;

	private Coroutine _updateVesicleRoutine;
	protected Coroutine _clingCellRoutine;
    private Coroutine _checkAbsorbRoutine;
    protected Coroutine _tauntCellRoutine;
    private Coroutine _waitForEatRoutine;

    public Virus()
	{
		singleSelect = false;
		canSelect = false;
		text_title = "Virus";
		text_description = "Oh noes! A virus!";
		text_id = "virus";
		wave_id = "";
		num_id = Selectable.VIRUS;
		setMaxHealth(10, true);
		normal_speed = speed;
		escape_speed = speed * 4;
		inside_speed = speed / 2;
		setRadius(25); //MAGIC NUMBER ALERT
		makeGameDataObject();
		//giveVesicle();
	}

	public void setVesicle(bool b)
	{
		doesVesicle = b;
		if (doesVesicle)
		{
			if (position_state == POS_OUTSIDE_CELL)
			{ //grow a vesicle if you're outside the cell. Otherwise wait until you touch membrane
				giveVesicle();
			}
		}
	}

	public void shrinkVesicle()
	{
		//trace("Virus.shrinkVesicle()");
		vesicleGrow = false;
		vesicleShrink = true;
		_updateVesicleRoutine = StartCoroutine(updateVesicle());
	}

	public void removeVesicle()
	{
		hasVesicle = false;
	}

	public void giveVesicle(bool maxSize = false)
	{
		hasVesicle = true;
		doesVesicle = true;
		c_ves_shape = new Graphics();
		//addChild(c_ves_shape);  //TODO: ?
		//setChildIndex(c_ves_shape, 0);//put it at the bottom   //TODO:?
		ves_max_size = getRadius() / 2;
		if (maxSize)
		{
			ves_size = ves_max_size;
			drawVesicle();
		}
		else
		{
			vesicleGrow = true;
			_updateVesicleRoutine = StartCoroutine(updateVesicle());
		}
		//updateVesicle(null);

	}

	public override void updateBubbleZoom(float n)
	{
		base.updateBubbleZoom(n);
		if (hasVesicle)
		{
			drawVesicle();
		}
	}

	private void clearVesicle()
	{
		c_ves_shape.End();
		c_ves_shape.enabled = false; //.visible = false;   //TODO:  May need rethinking
	
	}

	public IEnumerator updateVesicle()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (vesicleGrow && ves_size < ves_max_size)
			{
				ves_size += ves_size_grow;
				if (ves_size >= ves_max_size)
				{
					ves_size = ves_max_size;
					vesicleGrow = false;
					StopCoroutine(_updateVesicleRoutine);
				}
			}
			if (vesicleShrink && ves_size > 0)
			{
				ves_size -= ves_size_grow;
				if (ves_size <= 0)
				{
					ves_size = 0;
					vesicleShrink = false;
					StopCoroutine(_updateVesicleRoutine);
					clearVesicle();
				}
			}
			drawVesicle();
		}
	}

	public void drawVesicle()
	{
		if (ves_size > 0)
		{
			c_ves_shape.Begin();
			c_ves_shape.SetFillColor(FastMath.ConvertFromUint(cyto_col));
			c_ves_shape.lineStyle(Membrane.OUTLINE_THICK / 3, Color.black);
			c_ves_shape.Circle(0, 0, ves_size);
			c_ves_shape.Fill();
			c_ves_shape.Stroke();
			//c_ves_shape.graphics.endFill();
			
			c_ves_shape.lineStyle(Membrane.SPRING_THICK / 4, FastMath.ConvertFromUint(spring_col));
			c_ves_shape.Circle(0, 0, ves_size);
			c_ves_shape.Fill();
			c_ves_shape.Stroke();
			//shape.graphics.drawEllipse(x-size/2,y-size/2,size, size);
			c_ves_shape.lineStyle(Membrane.GAP_THICK / 6, FastMath.ConvertFromUint(gap_col));
			c_ves_shape.Circle(0, 0, ves_size);
			//shape.graphics.drawEllipse(x-size/2,y-size/2,size, size);

		}
	}

	public void addLyso(Lysosome l)
	{
		if (list_lyso == null)
		{
			list_lyso = new List<Lysosome>();
		}
		bool isInList = false;
		foreach(Lysosome ll in list_lyso) {
			if (l == ll)
			{
				isInList = true;
			}
		}
		if (!isInList)
		{
			list_lyso.Add(l);
		}
	}

	public override void destruct()
	{
		StopCoroutine(_clingCellRoutine);
		
		if (list_lyso != null)
		{
			//trace("Virus(" + this.name + ").destruct()");
			releaseLyso();
			while (list_lyso.Count > 0) {
				list_lyso.RemoveAt(0);
			}
			list_lyso = null;
		}
		clearGrid();
		base.destruct();
	}

	public void dismissAllLysosExcept(Lysosome l)
	{
		//trace("Virus(" + this.name + ").dismissAllLysosExcept(" + l.name + ")");
		if (list_lyso != null)
		{
			int length = list_lyso.Count;
			for (int i = length - 1; i >= 0; i--) {
				if (list_lyso[i] != l)
				{
					list_lyso[i].releaseByVirus(this);
					list_lyso[i] = null;
					list_lyso.RemoveAt(i);
				}
			}
		}
	}



	protected void releaseLyso()
	{
		if (list_lyso != null)
		{
			int length = list_lyso.Count;
			for (int i = length - 1; i >= 0; i--) {
				list_lyso[i].releaseByVirus(this);
				list_lyso[i] = null;
				list_lyso.RemoveAt(i);
			}
		}
	}

	public void setCanvas(WorldCanvas c)
	{
		p_canvas = c;
	}

	protected virtual void touchingCell()
	{
		releaseLyso();
		position_state = POS_TOUCHING_CELL;
		//this.transform.colorTransform = new ColorTransform(0.75,0.75,0.75,1, 0,0,0,0);
	}

	protected virtual void insideCell()
	{
		speed = inside_speed;
		position_state = POS_INSIDE_CELL;
		//this.transform.colorTransform = new ColorTransform(1,1,1,1, 0,0,0,0);
	}

	protected virtual void outsideCell()
	{
		position_state = POS_OUTSIDE_CELL;

		//this.transform.colorTransform = new ColorTransform(0.5, 0.5, 0.5, 1, 0, 0, 0, 0);
	}

	public void setup(bool doEscape = false)
	{
		if (doEscape)
		{
			//entering = false;

			condition_state = CON_MOVE_TO_MEMBRANE;
			onBornInCell();
			insideCell();
			motivation_state = MOT_ESCAPING_CELL;

			StopCoroutine(_tryEnter);
			playAnim("grow");

		}
		else
		{

			condition_state = CON_MOVE_TO_MEMBRANE;
			outsideCell();
			whatsMyMotivation();

			entering = true;
			_tryEnter = StartCoroutine(tryEnter());
			enterMembrane();
		}
	}

	protected virtual void onBornInCell()
	{
		//define per subclass
	}

	public void onGrow()
	{
		if (motivation_state == MOT_ESCAPING_CELL && position_state == POS_INSIDE_CELL)
		{
			//inCell = true;
			p_cell.onVirusSpawn(wave_id, 1);
			exitMembrane();
		}
	}

	protected virtual IEnumerator tauntCell()
	{
		yield return new WaitForEndOfFrame();

		//define per subclass
	}

	public override void releaseFromLysosome(Lysosome l)
	{
		//trace("Virus.RELEASE FROM LYSOSOME");
	}



	public override void targetForEating(Lysosome l = null)
	{
		addLyso(l);
		//super.targetForEating();
		//don't make it doomed just yet!
	}

	protected virtual void onInvade(bool doDamage = true)
	{
		//trace("Virus.onInvade()");
		mnode = null;
		cancelMove(); //just to be sure
		if (!hasVesicle)
		{
			if (!isNeutralized && doDamage)
			{
				p_cell.c_membrane.takeDamageAt(x, y, DMG_PIERCE_MEMBRANE);
				p_cell.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "virus_entry_wound", wave_id, 1);
			}
		}
		else
		{
			removeVesicle();
		}

		Vector2 v = new Vector2(x - cent_x, y - cent_y);
		v.Normalize();
		v *= (-INJECT_DISTANCE);
		x += v.x;
		y += v.y;

		StopCoroutine(_clingCellRoutine);
	

		entering = false;
		StopCoroutine(_tryEnter);

		leaving = false;

		insideCell();
		//atCell = false;
		//inCell = true;

		condition_state = CON_MOVE_TO_RIBOSOME;
		rib = p_cell.findClosestRibosome(x, y, true);
		if (rib)
		{
			//trace("Virus.onInvade() rib = " + rib);
			moveToObject(rib, FLOAT, true);
		}
		else
		{
			//trace("Virus.onInvade() rib is NULL!");
		}
	}

	public void onExit()
	{
		//trace("Virus.onExit() EXIT!");

		condition_state = CON_MOVE_TO_EXIT;

		speed = escape_speed;
		StopCoroutine(_clingCellRoutine);

		if (!isNeutralized)
		{
			p_cell.c_membrane.takeDamageAt(x, y, DMG_PIERCE_MEMBRANE); //damage the membrane
			p_cell.onVirusEscape(wave_id, 1);
			p_cell.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "virus_exit_wound", wave_id, 1);
		}

		outsideCell();


		StopCoroutine(_tryEnter);

		mnode = null;
		Vector2 v = new Vector2(x - cent_x, y - cent_y);
		v.Normalize();

		//v *= (p_canvas.getBoundary() * 1.5f); //leave the screen   //TODO
		_checkAbsorbRoutine = StartCoroutine(checkAbsorb());
		moveToPoint(new Point(x + v.x, y + v.y), CellGameObject.FLOAT, true);
	}

	public void neutralize()
	{

		isNeutralized = true;
		if (position_state != POS_TOUCHING_CELL)
		{
			onExit();
		}
	}

	private IEnumerator checkAbsorb()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			absorbCount++;
			if (absorbCount > ABSORB_TIME)
			{
				absorbCount = 0;
				if (p_canvas.checkAbsorbCellObject(this, isNeutralized))
				{

					p_cell.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "virus_escape", wave_id, 1);
					cancelMove();
					StopCoroutine(_checkAbsorbRoutine);
					p_cell.killVirus(this);
				}
			}
		}
	}

	protected virtual void whatsMyMotivation()
	{
		//define per subclass
	}

	public void exitMembrane()
	{
		//entering = false;

		StopCoroutine(_tryEnter);
		mnode = p_cell.c_membrane.findClosestMembraneHalf(x, y);
		Vector2 v = new Vector2((mnode.x + mnode.p_next.x) / 2, (mnode.y + mnode.p_next.y) / 2);
		v *= (2);
		moveToPoint(new Point(v.x, v.y), CellGameObject.FLOAT, true);
		//speed = escape_speed;
	}

	public void enterMembrane()
	{
		if (motivation_state != MOT_ESCAPING_CELL)
		{
			mnode = p_cell.c_membrane.findClosestMembraneHalf(x, y);
			moveToPoint(new Point((mnode.x + mnode.p_next.x) / 2, (mnode.y + mnode.p_next.y) / 2), CellGameObject.FLOAT, true);
		}
	}

	protected IEnumerator tryEnter()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (condition_state == CON_MOVE_TO_MEMBRANE && position_state == POS_OUTSIDE_CELL)
			{
				newnode_count++;
				if (newnode_count > NEWNODE_TIME)
				{
					newnode_count = 0;
					enterMembrane();
				}
			}
		}
	}

	public override void calcMovement()
	{


		if (mnode != null)
		{ //if mnode is defined it's assumed we're moving towards it
			pt_dest.x = (mnode.x + mnode.p_next.x) / 2;
			pt_dest.y = (mnode.y + mnode.p_next.y) / 2;
			Vector2 v = new Vector2(pt_dest.x - x, pt_dest.y - y);
			float ang = (FastMath.toRotation(v) / (Mathf.PI * 2)) * 360;//(v.toRotation() * 180) / Math.PI;

			if (condition_state == CON_MOVE_TO_MEMBRANE)
			{
				this.transform.eulerAngles = new Vector3(0,0,ang - 90);
			}
			else
			{
				this.transform.eulerAngles = new Vector3(0, 0, ang + 90);
			}
		}
		base.calcMovement();
	}

	protected override void onArriveObj()
	{
		base.onArriveObj();
		if (!isDoomed)
		{
			if (position_state == POS_INSIDE_CELL && rib)
			{ //we're looking for a ribosome
				doRibosomeThing();
			}
		}
	}

	protected IEnumerator clingCell()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (mnode != null)
			{
				x = (mnode.x + mnode.p_next.x) / 2;
				y = (mnode.y + mnode.p_next.y) / 2;
			}
		}
	}

	public virtual void onTouchCell()
	{

		if (condition_state == CON_MOVE_TO_MEMBRANE && position_state == POS_INSIDE_CELL)
		{ //if we're exiting the cell
			if (!isDoomed)
			{
				touchingCell();
				mnode = p_cell.c_membrane.findClosestMembraneHalf(x, y);
				_clingCellRoutine = StartCoroutine(clingCell());
				arrivePoint();
				playAnim("exit");
				if (doesVesicle)
				{
					giveVesicle();
				}
			}
		}

		if (condition_state == CON_MOVE_TO_MEMBRANE && position_state == POS_OUTSIDE_CELL)
		{//if we're entering the cell
			if (checkDefensin())
			{
				if (!isDoomed)
				{
					touchingCell();
					mnode = p_cell.c_membrane.findClosestMembraneHalf(x, y);
					_clingCellRoutine = StartCoroutine(clingCell());
					onTouchCellAnim();
					arrivePoint();
				}
			}
		}
	}

	protected virtual void onTouchCellAnim()
	{
		playAnim("land");
	}

	public override void startGetEaten()
	{
		
		StopCoroutine(_tauntCellRoutine);
		playAnim("eaten");
		if (hasVesicle)
		{
			shrinkVesicle();
		}
		_waitForEatRoutine = StartCoroutine(waitForEat());
		base.startGetEaten();
	}

	private IEnumerator waitForEat()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			waitEatCounter++;
			if (waitEatCounter > WAIT_EAT_TIME)
			{
				waitEatCounter = 0;
				onDeath();
			}
		}
	}

	public override void playAnim(string label)
	{

		base.playAnim(label);
		if (!dying)
		{   //you are not allowed to start an animation while dying
			//if we are playing a death animation, that's the end of me
			if (label == "fade")
			{
				//trace("Virus(" + this.name + " playAnim(\"fade\")");
				releaseLyso();
				dying = true; //HACK to keep things working smoothly
			}
		}
	}

	protected override void arrivePoint(bool wasCancel = false)
	{
		base.arrivePoint(wasCancel);
		if (!wasCancel)
		{
			if (!entering && !leaving)
			{       //HACKITY HACK
				if (mnode != null)
				{
					float x1 = x;
					float x2 = (mnode.x + mnode.p_next.x) / 2;
					float y1 = y;
					float y2 = (mnode.y + mnode.p_next.y) / 2;
					float dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
					if (dist2 <= radius2 * 2)
					{
						onTouchCell();
					}
					else
					{
						exitMembrane();
					}
				}
				//var dx:Number = x1*x
			}
		}
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		//trace("CellObject.onAnimFinish() " + i + "me = " + name);
		//trace("Virus.onAnimFinish(" + i + ")");
		base.onAnimFinish(i, stop);
		switch (i)
		{
			case ANIM_GROW: onGrow(); break;
			case ANIM_LAND: onLand(); break;
			case ANIM_INVADE: onInvade(); break;
			case ANIM_RECYCLE:
			case ANIM_FADE:
			case ANIM_DIE: onDie(); break;
			case ANIM_EXIT: onExit(); base.onAnimFinish(i, stop); break;
		}

	}

	protected void onDie()
	{
		if (p_cell)
		{
			p_cell.killVirus(this);
		}
		//trace("Virus(" + this.name + ").onDie()");
		releaseLyso();
	}

	protected bool checkDefensin() 
	{
			if (hasVesicle) { //vesicles are immune to defensins
				shrinkVesicle(); //shrink the vesicle away
				return true;
			}
		if (!shieldBlocked)
		{
			if (!isNeutralized)
			{
				float chance = UnityEngine.Random.Range(0f,1f);
				if (chance < Membrane.defensin_strength)
				{
					playAnim("fade");
					shieldBlocked = true;
					p_cell.showShieldBlock(x, y);
					//trace("Virus.onLand() blocked by defensin! chance=" + chance + " defensin=" + Membrane.defensin_strength);
					return false;
				}
				else
				{
					return true;
					//trace("Virus.onLand() NOT blocked by defensin! chance=" + chance + " defensin=" + Membrane.defensin_strength);
				}

			}
		}
		else
		{
			return false;
		}
		return true;
		}
		
		protected void onLand()
		{
			//trace("Virus.onLand!");
			if (!isNeutralized)
			{
				doLandThing();
			}
			else
			{
				//trace("Virus.onLand() neutralized =" + isNeutralized + " get Out of here!");
				onExit();
			}

		}

	protected virtual void doRibosomeThing()
	{
		//define per subclass
	}

	protected virtual void doLandThing()
	{
		//define per subclass
	}

	public override void getPpodContract(float xx, float yy)
	{
		if (position_state == POS_TOUCHING_CELL || position_state == POS_INSIDE_CELL)
		{
			base.getPpodContract(xx, yy);
		}
		else
		{
			//donothing
		}
	}

	public override void updateLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		updateGridLoc(xx, yy);
	}

	public override void doCellMove(float xx, float yy)
	{
		if (position_state == POS_TOUCHING_CELL || position_state == POS_INSIDE_CELL)
		{
			base.doCellMove(xx, yy);
		}
		else
		{
			//do nothing if not inside the cell
		}
	}

















}

