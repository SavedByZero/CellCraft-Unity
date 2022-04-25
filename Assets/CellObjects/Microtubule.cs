using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Microtubule : CellObject
{
	public Point origin;
	public Point terminus;
	public Point finishPoint = new Point(0,0);
		
	private float distToTarget;
		
	public CellObject p_obj;
		
		//private var speed:Number = 4;
	private float xSpeed = 0;
	private float ySpeed = 0;
		
	private float angle = 0;
		
	public float xLoc = 0;
	public float yLoc = 0;
		
	private float counter = 0;
		
		public bool amReady = false;
		public bool hasCentrosome = false; //is this the one with the centrosome?
		public bool isBlank = false;
		
		//public var list_bits:Vector.<TubeBit>;
		//public var list_grav:Vector.<GravPoint>;
		public int count_bit = 0;
		//public var actinList:Vector.<ActinRoad>;
		
		private int turnBit = 0;
		private int delCounter = 0;
		private int blurMax = 1;
		
		private bool blurringOn = false;
		public bool isPpoding = false;
		public bool isPpodContracting = false;
		
		private GameObject actinTarget;
		private int currActin = 0;
		
		public static float endTubeRadius = .75f;
		private Sprite debugShape;  //was: true
		public bool debugging = true;
		
		public Cytoskeleton p_skeleton;
		public bool dirty_grav = false; //do I have new grav data since last time I gave it?

	public static float PPOD_R2 = 1.5f * 1.5f; //TODO: I modified this because the initial value seemed way too high and the skeleton set it lower. Keep an eye out for issues here. //10.00f*10.00f;
		public static float LENS_R2 = 10.00f*10.00f;
		
		//public var isShrunk:Boolean = false;
		
		private int instant_grow_count = 0;
		private const int INSTANT_GROW_FAILSAFE = 10;
	private Coroutine _GrowBitRoutine;

    public override void Start()
    {
        base.Start();
		canSelect = false;
		singleSelect = true;
		finishPoint = new Point(0, 0);
		//cacheAsBitmap = true;
		speed = 17;
	}

	public void destruct()
	{
		p_obj = null;
		p_skeleton = null;
		removeAllBits();
		base.destruct();
		origin = null;
		terminus = null;
		xLoc = 0;
		yLoc = 0;
	}

	public void setPoints(Point o, Point t)
	{
		origin = o;
		terminus = t;
		x = origin.x;
		y = origin.y;
	}

	public void showDebugShape()
	{
		/*
		debugShape = new Shape();
		debugShape.graphics.clear();
		var tb = getLastBit();
		debugShape.graphics.moveTo(0, 0);
		debugShape.graphics.lineStyle(2, 0xFFFFFF);
		debugShape.graphics.drawCircle(tb.x, tb.y, endTubeRadius);
		addChild(debugShape);*/
	}

	public void setSkeleton(Cytoskeleton c)
	{
		p_skeleton = c;
	}

	public void setObjectSelf()
	{
		p_obj = this;
		hasCentrosome = false;
		isBlank = true;
	}

	public void setObject(CellObject c)
	{
		p_obj = c;
		c.setTube(this);
		if (c.getNumID() == Selectable.CENTROSOME)
		{
			hasCentrosome = true;
		}
		isBlank = false;
	}

	public CellObject getObject() 
	{
			return p_obj;
	}

	public void growActin(GameObject t)
	{
		//actinTarget = t;
		//trace("GROW ACTIN!");
		Point p = new Point(t.transform.position.x, t.transform.position.y);
		makeActin(p);
		//addEventListener(Event.ENTER_FRAME, growActinBit);
	}

	public Point getTerminus() 
	{
			return new Point(terminus.x, terminus.y);
	}


	public GravPoint getGravPoint()
	{
		if (isPpoding)
		{
			return new GravPoint(new Point(xLoc, yLoc), this);
		}
		else if (isPpodContracting)
		{
			return new GravPoint(new Point(origin.x, origin.y), this);
		}
		return new GravPoint(new Point(xLoc, yLoc), p_obj);
	}

	public CellObject getObj()
	{
		return p_obj;
	}

	public List<GravPoint> getGravPoints()
	{
			List<GravPoint> list = new List<GravPoint>();
			
			//if(isPpoding)
			
			list.Add(new GravPoint(new Point(origin.x, origin.y),this));
			list.Add(new GravPoint(new Point(xLoc, yLoc),this));
			
			return list;
	}

	public float lastX()
	{
		return terminus.x;
	}

	public float lastY()
	{
		return terminus.y;
	}

	public TubeBit getLastBit()
	{
		//return list_bits[list_bits.length - 1];
		return null;
	}

	void makeActin(Point p)
	{ 
	}

	public void followObj()
	{
		
		terminus = new Point(p_obj.x - p_skeleton.x, p_obj.y - p_skeleton.y);
		calcTrajectory(terminus);
		p_skeleton.updateGravityPoints();
		//instantFollow();
	}

	public void instantGrow()
	{
		xLoc = x;
		yLoc = y;
		calcTrajectory(terminus);
		instantGrowBit();
	}

	public override void cancelMove()
	{

		//removeEventListener(RunFrameEvent.RUNFRAME, growBit);
		if (_GrowBitRoutine != null)
			StopCoroutine(_GrowBitRoutine);
		isMoving = false;
		terminus.x = xLoc;
		terminus.y = yLoc;
		onFinish();
	}

	public void ppodTo(float xx, float yy)
	{
		isPpoding = true;
		p_skeleton.onStartPPod();
				origin.x = terminus.x;
		origin.y = terminus.y;
		xLoc = terminus.x;
		yLoc = terminus.y;
		terminus.x = xx;
		terminus.y = yy;
		calcTrajectory(terminus);
		isMoving = true;
		_GrowBitRoutine = StartCoroutine(growBit());
	}

	public void grow()
	{
		xLoc = x;
		yLoc = y;
		calcTrajectory(terminus);
		isMoving = true;
		_GrowBitRoutine = StartCoroutine(growBit());
	}

	private void calcTrajectory(Point p)
	{


		Vector2 dist = new Vector2(xLoc - p.x, yLoc - p.y);
		dist.Normalize();

		angle = FastMath.toRotation(dist);
		angle *= (180 / Mathf.PI);
		angle -= 90;

		dist *= speed;
		xSpeed = -dist.x;
		ySpeed = -dist.y;
	}

	public override void doCellMove(float xx, float yy)
	{
		terminus.x += xx;
		terminus.y += yy;
		xLoc += xx;
		yLoc += yy;
		origin.x += xx;
		origin.y += yy;
		//don't mess with your objects


	}

	public void ppodContract(float xx, float yy)
	{
		
		terminus.x -= xx; //move my endpoint
		terminus.y -= yy;
		xLoc -= xx;       //move my currpoint
		yLoc -= yy;
		origin.x -= xx;   //move my startpoint
		origin.y -= yy;
		if (p_obj != this)
		{ 
			if (p_obj.num_id == Selectable.CENTROSOME)
			{
				p_obj.getPpodContract(xx, yy);
			}
		}
	}

	private void contract()
	{

		float tempX = terminus.x;
		float tempY = terminus.y;
		terminus.x = origin.x;
		terminus.y = origin.y;
		origin.x = tempX;
		origin.y = tempY;
		xLoc = origin.x;
		yLoc = origin.y;
		calcTrajectory(terminus);
		_GrowBitRoutine = StartCoroutine(growBit());
	
	}

	private void instantFollow()
	{
		countBits();
		float bitsNeeded = Mathf.Ceil(distToTarget / speed);
		int diff = (int)bitsNeeded - count_bit;
		if (diff < 0)
		{ //we need less bits
			removeBits(-diff);
		}
		else if (diff > 0)
		{
			addBits(diff);
		}
		reorientBits();
	}

	public void instantGrowBit()
	{
		amReady = false;
		//while (!amReady)
		{
			_GrowBitRoutine = StartCoroutine(growBit());
		}
	}

	private void addBits(int i)
	{
		for (int j = 0; j < i; j++){
			
			count_bit++;
		}
	}

	private void removeAllBits()
	{
		
	}

	private void removeBits(int i)
	{
		
	}

	private void reorientBits()
	{
		
	}

	public void cancelPPod()
	{
		if (isPpoding)
		{
			isPpoding = false;
			StopCoroutine(_GrowBitRoutine);
			isMoving = false;
			xSpeed = 0;
			ySpeed = 0;
			p_skeleton.onCancelPPod();
			p_skeleton.removeTube(this, true);

		}
	}

	private void onFinish()
	{
		xSpeed = 0;
		ySpeed = 0;
		finishPoint.x = xLoc; //where our last bit was placed
		finishPoint.y = yLoc; //
		xLoc = terminus.x;    //where we were trying to go
		yLoc = terminus.y;
		if (_GrowBitRoutine != null)
			StopCoroutine(_GrowBitRoutine);
		isMoving = false;

		amReady = true;
		//list_grav.push(new GravPoint(new Point(xLoc, yLoc), this));

		p_skeleton.finishTube(isBlank);
		if (isPpoding)
		{
			isPpoding = false;
			p_skeleton.onFinishPPod();
			SfxManager.Play(SFX.SFXMudStep);
			SfxManager.Play(SFX.SFXMudSlide);

			isPpodContracting = true;
			contract();
		}
		else if (isPpodContracting)
		{
			isPpodContracting = false;
			p_skeleton.removeTube(this, true);
		}
	}

	public IEnumerator growBit()
	{
		while (!amReady)
		{
			
			growBitHelper();
			yield return new WaitForEndOfFrame();
		}
	}

	public void growBitHelper()
    {
		float oldLocX = xLoc;
		float oldLocY = yLoc;

		xLoc += xSpeed;
		yLoc += ySpeed;

		float d2 = (xLoc * xLoc) + (yLoc * yLoc);
		d2 += PPOD_R2 * 4;
		bool outside = false;
		if (d2 > BOUNDARY_R2)
		{
			Vector2 cent_v = new Vector2(cent_x - 0, cent_y - 0);
			Vector2 cent_v_n = cent_v.normalized;
			Vector2 dir_v = new Vector2(terminus.x, terminus.y);
			Vector2 dir_v_n = dir_v.normalized;
			float angle = FastMath.angleTo(dir_v, cent_v);
			Debug.Log("Microtubule.growBit OUTSIDE()! \n cent_v = " + cent_v + " dir_v = " + dir_v);
			Debug.Log("Microtubule.growBit OUTSIDE()! \n cent_v_n = " + cent_v_n + " dir_v_n = " + dir_v_n);
			outside = true;
		}

		if (outside)
		{//we're outside the boundary
			xLoc = oldLocX;
			yLoc = oldLocY;
			if (isPpoding)
			{

				//trace("Microtubule.growBit() FAIL boundary check! loc=(" + xLoc + "," + yLoc + ") bound_r2=" + BOUNDARY_R2 + " d2=" + d2);
				cancelPPod();
				//onFinish(); //force the ppod to end, because we're at the end of the thing
				//end ppod
			}
			amReady = true; // This is a temp fix - should we keep it?
		}
		else
		{                   //we're inside the boundary
			dirty_grav = true;

			if (isPpodContracting)
			{
				p_skeleton.ppodContract(this, xSpeed, ySpeed);
			}

			counter++;

			if (counter * speed > 50)
			{
				calcTrajectory(terminus);
				counter = 0;
			}

			p_skeleton.updateTube();

			if (terminus != null)
			{
				float absX = (xLoc - terminus.x);
				float absY = (yLoc - terminus.y);
				absX = absX < 0 ? -absX : absX;
				absY = absY < 0 ? -absY : absY;

				if ((absX < speed * 1.5) && (absY < speed * 1.5))
				{
					onFinish();
				}
				if (count_bit > 100)
				{
					amReady = true;
					//trace("Microtubule.growBit() : too many bits!");
					Debug.LogError("Microtubule.growBit(): WAAAAY TOO MANY BITS!");
				}
			}
			else
			{
				//trace("Microtubule.growBit() : no terminus!");'
				amReady = true;
			}
		}
	}

	public void startDecay()
	{
		
	}



	public Point getTubePoint(int i) 
	{
			Point p = new Point(0,0);
			
			return p;
	}

	private void countBits()
	{
		//count_bit = list_bits.length;
	}

	private int getBitCount()
	{
		return count_bit;
	}

	public void deleteBackBits() 
	{
		
	}


}

