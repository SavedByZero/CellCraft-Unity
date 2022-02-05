using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGameObject : MovieClip, ICellGameObject
{
	private InfoBubble c_bubble;

	//need:  Locator,
	public float x { get; set; }
	public float y { get; set; }
	public bool dying { get; set; }
	private float radius; //what is your bounding circle, for collision or selection
	public float radius2;

	public Locator loc_bubble;  

	private Point pt_bubble;
	private Coroutine _doAnimRoutine;
	private Coroutine _doMovePointRoutine;
	private Coroutine _doMoveGobjRoutine;

	public MovieClip clip;
	public MovieClip anim;

	protected float lastDist2; //the distance to the obj last frame

	//protected var p_world:World;   //TODO
	//protected var p_engine:Engine;  //TODO

	protected bool isDamaged = false;
	protected int damageLevel = 0;
	protected bool showSubtleDamage = false; //flash intermediate damage anims?

	protected bool has_health = false; //either has health or doesn't
	protected float health; //how much health does it have?
	protected float maxHealth;
	protected int level = 0; //does it have an upgrade level?
	protected int maxLevel = 3;


	public bool anim_wiggle = true; //trivial animation, stuff we can turn off
	public bool anim_vital = false; //are we doing a vital animation? (ie, something whose timing matters?)

	public const int ANIM_GROW = 0;
	public const int ANIM_GROW_2 = 1;


	public const int ANIM_THREAD = 10;
	public const int ANIM_DIE = 12;
	public const int ANIM_DOCK = 13;
	public const int ANIM_PROCESS = 14;
	public const int ANIM_PROCESS_INPLACE = 15;
	public const int ANIM_HARDKILL = 20;
	public const int ANIM_ADHERE = 26;
	public const int ANIM_DIGEST = 27;
	public const int ANIM_DIGEST_START = 28;
	public const int ANIM_RECYCLE = 29;
	public const int ANIM_DIVIDE = 30;
	public const int ANIM_POP = 31;
	public const int ANIM_MERGE = 32;
	public const int ANIM_FUSE = 33;
	public const int ANIM_DAMAGE1 = 34;
	public const int ANIM_DAMAGE2 = 35;
	public const int ANIM_BUD = 36;
	public const int ANIM_LAND = 37;
	public const int ANIM_FADE = 38;
	public const int ANIM_EXIT = 39;
	public const int ANIM_VIRUS_GROW = 40;
	public const int ANIM_VIRUS_INFEST = 41;
	public const int ANIM_INVADE = 42;

	public const int ANIM_PLOP = 50;

	private bool has_io = false;

	protected Point pt_dest; //destination point
	protected CellGameObject go_dest; //destination game object


	private int move_mode;
	public bool isMoving = false; //for read only - dont get smart ideas

	private float move_dist; //how far to move
	public Vector2 v_move; //my movement vector
	private int move_count = 0; //
	protected int MAX_COUNT = 15; //how long between recalcs

	public static int FLOAT = 0;        //float to destination
	public static int WAYPOINT = 1; //follow waypoints
	public static int EDGE = 2;         //float to edge of destination


	//private var deadTimer:Timer;  //TODO

	protected float speed = 1;


	private static int boundX = 640;
	private static int boundY = 480;

	protected int grid_x = 0;
	protected int grid_y = 0;
	protected static float grid_w = 0;
	protected static float grid_h = 0;
	protected static float span_w = 0;
	protected static float span_h = 0;
	protected static ObjectGrid p_grid;  

	public static float cent_x = 0;
	public static float cent_y = 0;

	protected static float BOUNDARY_W = 1000;
	protected static float BOUNDARY_H = 1000;

	protected static float BOUNDARY_R = 1000;
	protected static float BOUNDARY_R2 = 1000 * 1000;

	protected GameDataObject gdata;

	protected bool snapToObject = true;

	public CellGameObject()
	{
		autoRadius();
		createInfoLoc();
		//makeGameDataObject();
	}

	private void createInfoLoc()
	{
		if (loc_bubble)
		{   //replace the locator with a data point signifying where the bubble goes
			pt_bubble = new Point(loc_bubble.transform.position.x, loc_bubble.transform.position.y);
			loc_bubble.transform.SetParent(null);
			loc_bubble = null;
		}
		else
		{               //just guess

			pt_bubble = new Point(0, 0);
		}
	}

	public bool hasIO()
	{
		return has_io;
	}

	public void instantSetHealth(int i)
	{
		health = i;
		if (health <= 0)
		{
			health = 0;
			onDamageKill();
		}
		else if (health < maxHealth * 0.25)
		{
			if (damageLevel != 2)
			{
				heavyDamageClip();
			}
		}
		else if (health < maxHealth * 0.5)
		{
			if (damageLevel != 1)
			{
				lightDamageClip();
			}
		}
		else
		{
			if (damageLevel != 0)
			{
				showNoDamage();
			}
		}
	}

	public virtual void takeDamage(float n)
	{
		health -= n;
		if (health <= 0)
		{
			health = 0;
			onDamageKill();
		}
		else if (health < maxHealth * 0.25)
		{
			if (damageLevel != 2)
			{
				showHeavyDamage();
			}
		}
		else if (health < maxHealth * 0.5)
		{
			if (damageLevel != 1)
			{
				showLightDamage();
			}
		}
		else
		{
			if (damageLevel != 0)
			{
				showNoDamage();
			}
		}
	}

	protected void bumpBubble()
	{
		if (c_bubble != null)
		{
			c_bubble.transform.SetSiblingIndex(c_bubble.transform.parent.childCount - 1);
			//setChildIndex(c_bubble, numChildren - 1);
		}
	}

	protected void showNoDamage()
	{
		isDamaged = false;
		damageLevel = 0;
		if (clip)
		{
			clip.GotoAndStop("normal");
		}
		bumpBubble();
	}

	protected void showLightDamage()
	{
		if (showSubtleDamage)
		{
			playAnim("damage_1");
		}

		lightDamageClip();
	}

	protected void showHeavyDamage()
	{
		if (showSubtleDamage)
		{
			playAnim("damage_2");
		}

		heavyDamageClip();
	}

	protected void lightDamageClip()
	{
		isDamaged = true;
		damageLevel = 1;
		if (clip)
		{
			clip.GotoAndStop("damage_1");
		}
		bumpBubble();
	}

	protected virtual void heavyDamageClip()
	{
		isDamaged = true;
		damageLevel = 2;
		if (clip)
		{
			clip.GotoAndStop("damage_2");
		}

		bumpBubble();
	}

	protected void onDamageKill()
	{
		if (!dying)
		{
			hideBubble();
			onDeath();
			dying = true;
			//trace("GameObject.onDamageKill() " + this + " " + name);
		}
	}

	public int getDamageLevel()
	{
		return damageLevel;
	}

	public int getHealth()
	{
		return (int)health;
	}

	public int getMaxHealth()
	{
		return (int)maxHealth;
	}
	public void setSpeed(float n)
	{
		speed = n;
	}

	public void setLevel(int l)
	{
		level = l;
	}

	public int getLevel()
	{
		return level;
	}

	public int getMaxLevel()
	{
		return maxLevel;
	}

	public void giveHealth(uint amt)
	{
		health += amt;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	public void giveMaxHealth()
	{
		health = maxHealth;
	}

	public void setMaxHealth(int m, bool fillUp)
	{
		if (m < maxHealth)
		{
			if (health > m)
			{ //fill DOWN if health was above new maximum
				health = m;
			}
		}
		maxHealth = m;
		if (fillUp)
		{
			health = maxHealth;
		}
		has_health = true;
	}

	/*   //TODO
	public void setWorld(w:World)
	{
		p_world = w;
	}
	//TODO
	public void setEngine(e:Engine)
	{
		p_engine = e;
	}*/

	public void destruct()
	{
		//p_world = null;  //TODO
		//p_engine = null;  //TODO
		if (c_bubble != null)
		{
			c_bubble.transform.SetParent(null);
			c_bubble = null;
		}

	}

	protected virtual void autoRadius()
	{
		float r = MaxWidth;
		if (MaxHeight > r) r = MaxHeight;
		setRadius(r / 2);
	}

	public void setRadius(float r)
	{
		radius = r;
		radius2 = radius * radius;
	}

	public float getRadius()
	{
		return radius;
	}

	public virtual float getCircleVolume()
	{
		return Mathf.PI * (radius * radius);
	}

	public float getSphereVolume()
	{
		return (4 / 3) * Mathf.PI * (radius * radius * radius);
	}

	public float getRadius2()
	{
		return radius * radius;
	}

	/**
		 * Handles pausing and returning to the correct animation. True pauses the animation, False returns to its
		 * natural state
		 * @param	yes
		 */

	public void pauseAnimate(bool yes)
	{
		if (!yes)
		{
			//trace("UNPAUSE!");
			if (anim_wiggle)
			{
				wiggle(true);
			}
			else
			{
				//wiggle(false);
			}
		}
		else
		{
			//trace("PAUSE!");
			wiggle(false);
		}
	}

	protected void wiggle(bool yes)
	{
		//cacheAsBitmap = !yes;  //TODO - what would even be a Unity equivalent for this?
		if (clip)
		{
			if (yes)
			{
				if (clip.SubClip != null)
					clip.SubClip.Play();
			}
			else
			{
				if (clip.SubClip != null)
					clip.SubClip.Stop();
			}
		}
	}

	public void animateOn()
	{ //just turn on wiggling
		anim_wiggle = true;
		wiggle(true);
	}

	public void animateOff()
	{ //just turn off wiggling
		anim_wiggle = false;
		wiggle(false);
	}

	public void playAnim(string label)
	{
		//trace("GameObject.playAnim() label = " + label + "me=" + name);
		if (!dying)
		{   //you are not allowed to start an animation while dying

			GotoAndStop(label);
			if (anim != null)
				anim.Stop();

			anim_vital = true;
			_doAnimRoutine = StartCoroutine(doAnimRoutine());

			if (clip != null)
				clip.gameObject.SetActive(false);

			//if we are playing a death animation, that's the end of me
			if (label == "die" || label == "recycle")
			{
				killMe();
			}
		}
	}

	IEnumerator doAnimRoutine()
    {
		while (true)
        {
			yield return new WaitForSeconds(0.016f);
			doAnim();
        }
    }

	protected void doAnim()//(e:RunFrameEvent)
	{
		if (anim != null)
		{
			anim.GotoAndStop(anim.CurrentSpriteIndex + 1);
		}
	}

	public virtual void startGetEaten()
	{
		hideBubble();

	}

	public void onDeath()
	{
		cancelMove();
		playAnim("recycle");
		isDamaged = true;

	}

	public void kill()
	{
		cancelMove();
		playAnim("die");
	}

	public virtual void onAnimFinish(int i, bool stop = true)
	{
		if (!dying)
		{
			if (stop)
			{
				hardRevertAnim();
			}
		}
	}

	protected void hardRevertAnim()
	{
		GotoAndStop(1);
		clip.gameObject.SetActive(true);
		if (clip.SubClip != null)
			clip.SubClip.GotoAndPlay(1);
		else
		{
			//trace("GameObject.onAnimFinish() NO clip.clip!" + this.name + " " + this);
		}
		anim_vital = false;
		//removeEventListener(RunFrameEvent.RUNFRAME, doAnim);
		StopCoroutine(_doAnimRoutine);
	}

	public virtual bool doAction(int i, object parms = null) 
	{
			//trace("Performing action (" + i +")!");
			return true;
	}

	public void calcMovement()
	{
		Vector2 v = new Vector2();
		if (pt_dest != null)
		{
			v.x = pt_dest.x - x;
			v.y = pt_dest.y - y;
		}
		else if (go_dest)
		{
			v.x = go_dest.x - x;
			v.y = go_dest.y - y;
		}
		//move_dist = v.length;
		v.Normalize();
		v *= speed;
		v_move = new Vector2(v.x, v.y);
	}

	public bool getIsMoving() 
	{
			return isMoving;
	}

	public virtual void moveToObject(CellGameObject o, int i, bool free = false) 
	{
		if (!dying)
		{
			go_dest = o;

			pt_dest = null;
			move_mode = i;
			isMoving = true;
			calcMovement();
			//removeEventListener(RunFrameEvent.RUNFRAME, doMoveToPoint);
			if (_doMovePointRoutine != null)
				StopCoroutine(_doMovePointRoutine);
			_doMoveGobjRoutine = StartCoroutine(doMoveToGobjRt());
			//addEventListener(RunFrameEvent.RUNFRAME, doMoveToGobj, false, 0, true);

		}

	}

	IEnumerator doMovePointRt()
    {
		while (true)
        {
			yield return new WaitForSeconds(0.016f);
			doMoveToPoint();
        }
    }
		
	IEnumerator doMoveToGobjRt()
    {
		while (true)
        {
			yield return new WaitForSeconds(0.016f);
			doMoveToGobj();
        }
    }

	public virtual void moveToPoint(Point p, int i, bool free = false)
	{
		pt_dest = p;
		go_dest = null;
		move_mode = i;
		isMoving = true;
		calcMovement();
		//removeEventListener(RunFrameEvent.RUNFRAME, doMoveToGobj);
		if (_doMoveGobjRoutine != null)
			StopCoroutine(_doMoveGobjRoutine);
		//addEventListener(RunFrameEvent.RUNFRAME, doMoveToPoint, false, 0, true);
		_doMovePointRoutine = StartCoroutine(doMovePointRt());

	}

	protected virtual void onArrivePoint()
	{
		isMoving = false;
	}

	protected virtual void onArriveObj()
	{
		isMoving = false;
	}

	protected virtual void doMoveToPoint()
	{
		x += v_move.x;
		y += v_move.y;
		move_count++;
		if (move_count > MAX_COUNT)
		{
			move_count = 0;
			calcMovement();
		}
		float x1 = x;
		float y1 = y;
		float x2 = pt_dest.x;
		float y2 = pt_dest.y;
		float dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
		lastDist2 = dist2;
		if (dist2 <= speed * 2)
		{
			arrivePoint();
		}
		updateLoc();

	}

	public void cancelMove()
	{
		cancelMoveObject();
		cancelMovePoint();
	}

	protected void cancelMovePoint()
	{
		if (pt_dest != null)
		{
			pt_dest.x = x;
			pt_dest.y = y;
			arrivePoint(true);
		}
	}

	protected void arrivePoint(bool wasCancel = false)
	{
		if (!wasCancel)
		{
			onArrivePoint();
			if (pt_dest != null)
			{
				x = pt_dest.x;
				y = pt_dest.y;
			}
		}
		isMoving = false;
		/*if (this is Virus) {
			if (Virus(this).entering == false) {
				trace("Virus.arrivePoint leaving()");
			}
			//trace("Virus has arrived");
		}*/
		StopCoroutine(_doMovePointRoutine);
		//removeEventListener(RunFrameEvent.RUNFRAME, doMoveToPoint);
	}

	public void pushVector(float d2, Vector2 v)
	{
		float dist = Mathf.Sqrt(d2);
		x += v.x * dist;
		y += v.y * dist;
		updateLoc();
	}

	public virtual void push(float xx, float yy)
	{
		x += xx;
		y += yy;
		updateLoc();
	}

	protected virtual void doMoveToGobj()
	{
		if (go_dest == null || go_dest.dying)
		{
			stopWhatYouWereDoing(true);
		}
		else if (go_dest)
		{
			x += v_move.x;
			y += v_move.y;
			move_count++;
			if (move_count > MAX_COUNT)
			{
				move_count = 0;
				calcMovement();
			}
			float x1 = x;
			float y1 = y;
			float x2 = go_dest.x;
			float y2 = go_dest.y;
			float dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
			lastDist2 = dist2;
			if (move_mode == FLOAT)
			{
				if (dist2 <= radius2)
				{
					arriveObject();
				}
			}
			else if (move_mode == EDGE)
			{
				if (dist2 <= radius2 + go_dest.getRadius2())
				{
					arriveObject();
				}
			}
			updateLoc();
		}
	}

	protected void stopWhatYouWereDoing(bool isObj)
	{
		if (isObj)
		{
			cancelMoveObject();
		}
		else
		{
			cancelMovePoint();
		}
		//define rest per subclass
	}

	protected void cancelMoveObject()
	{
		arriveObject(true);
	}

	protected void arriveObject(bool wasCancel = false)
	{
		if (!wasCancel)
		{
			if (move_mode == FLOAT)
			{
				if (go_dest)
				{
					if (snapToObject)
					{
						x = go_dest.x;
						y = go_dest.y;
					}
				}
			}
			onArriveObj();
		}
		isMoving = false;
		if (_doMoveGobjRoutine != null)
			StopCoroutine(_doMoveGobjRoutine);
		//removeEventListener(RunFrameEvent.RUNFRAME, doMoveToGobj);
	}

	protected virtual void killMe()
	{
		dying = true;
		StartCoroutine(onDeadTimer(1.5f));
		//addEventListener(TimerEvent.TIMER, onDeadTimer, false, 0, true);
		//deadTimer.start();
		//defined per subclass
	}

	protected IEnumerator onDeadTimer(float delay)
	{
		yield return new WaitForSeconds(delay);

		//hard KILL ME
	}

	public static void setBoundaryBox(float w, float h)
	{
		BOUNDARY_W = w;
		BOUNDARY_H = h;
	}

	public static void setBoundaryRadius(float r)
	{
		//trace("Gameobject.setBoundaryRadius(" + r + ")");
		BOUNDARY_R = r;
		BOUNDARY_R2 = r * r;
	}

	public static void setCentLoc(float x, float y)
	{
		cent_x = x;
		cent_y = y;
	}

	/*                                          //TODO
	public static void setGrid(g:ObjectGrid)
	{
		grid_w = g.getCellW();
		grid_h = g.getCellH();
		span_w = g.getSpanW();
		span_h = g.getSpanH();
		//trace("gridsize = ("+grid_w+","+grid_h+")");
		p_grid = g;
	}*/

	public virtual void putInGrid()
	{
		float xx = x - cent_x + span_w / 2;  //place in the grid at half the grid width from the center, half the grid height from the center??
		float yy = y - cent_y + span_h / 2;
		gdata.x = xx;                         //record the position of this object here. 
		gdata.y = yy;
		grid_x = (int)(xx / grid_w);          //  I don't entirely understand this formula or how it's trying to position the thing (yet)
		grid_y = (int)(yy / grid_h);          //
		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x >= grid_w) grid_x = (int)grid_w - 1;
		if (grid_y >= grid_h) grid_y = (int)grid_h - 1;


		//p_grid.putIn(grid_x, grid_y, gdata);  //TODO
	}

	public void place(float xx, float yy)
	{
		x = xx;
		y = yy;
		updateLoc();
	}


	public void makeGameDataObject()
	{
		gdata = new GameDataObject();
		gdata.setThing(x, y, getRadius(), this, this.GetType());
	}

	public GameDataObject getGameDataObject() 
	{
		return gdata;
	}

	public virtual void updateLoc()
	{
		/*var xx:Number = x + span_w / 2;
		var yy:Number = y + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		var old_x:int = grid_x;
		var old_y:int = grid_y;
		grid_x = int(x/grid_w);
		grid_y = int(y/grid_h);
		if((old_x != grid_x) || (old_y != grid_y)){
			p_grid.takeOut(old_x,old_y,this);
			p_grid.putIn(grid_x,grid_y,this);
		}*/
	}

	public virtual void matchZoom(float n) 
	{
	
	this.transform.localScale = new Vector3(1 / n, 1 / n, 1 / n);
	}

	public virtual void updateBubbleZoom(float n)
	{
		if (c_bubble)
		{
			c_bubble.matchZoom(n);
		}
	}

	protected void hideBubble()
	{
		if (c_bubble)
		{
			c_bubble.gameObject.SetActive(false);
		}
	}

	protected void showBubble(string s)
	{
		//trace("GameObject.showBubble() " + s);
		if (!c_bubble)
		{
			c_bubble = new InfoBubble();
			c_bubble.transform.SetParent(this.transform, false);//addChild(c_bubble);
																//c_bubble.x = pt_bubble.x;
																//c_bubble.y = pt_bubble.y;
			c_bubble.transform.position = new Vector2(pt_bubble.x, pt_bubble.y);
			//c_bubble.matchZoom(World.getZoom()); //TODO
		}
		else
		{
			c_bubble.transform.SetSiblingIndex(this.transform.childCount - 1);
			//setChildIndex(c_bubble, numChildren - 1);
		}
		if (!c_bubble.gameObject.activeSelf)
        {
			c_bubble.gameObject.SetActive(true);
        }
		c_bubble.setIcon(s);
	}



}