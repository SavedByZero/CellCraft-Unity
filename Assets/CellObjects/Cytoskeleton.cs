using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Cytoskeleton : CellObject
{
	private bool skeletonReady = false;
		
	private List<Microtubule> list_tubes;	   //tubes that hold something
	private List<Microtubule> list_tubes_blank; //tubes that are just structural (like for ppoding)
		
	private List<GravPoint> list_grav;			//grav points that hold something
	private List<GravPoint> list_grav_warble;			//grav points that hold something
	private List<GravPoint> list_grav_blank;		//grav points that are just structural (like for ppoding)
	//private var list_grav_warble:Vector.<GravPoint>;
	//
	private List<float> list_circ;
		
	private Centrosome p_centrosome;
	private Nucleus p_nucleus;
	private Membrane p_membrane;
		
	public const float PPOD_RADIUS = 1.50f;
	public const float PPOD_RADIUS2 = PPOD_RADIUS* PPOD_RADIUS;
	public const float GRAV_RADIUS = 1.00f;
	public const float GRAV_RADIUS2 = GRAV_RADIUS* GRAV_RADIUS;
	public const float PPOD_SPEED = 12;
	public static bool SHOW = false;
		
	public static float MEM_FRAC = 0.7f;
	public static float MEM_WARBLE_FRAC = 1-(0.6f);
	public static float MEM_WARBLECIRC_FRAC = 0.7f * 0.8f;
	public static int WARBLE_POINTS = 6;
	public GameObject Microtubule_Prefab;
		
	public bool isPPoding = false;
		
	public float cent_radius = 1.00f;

	private Coroutine _runRoutine;

	public override void Start()
	{
		base.Start();
		this.gameObject.SetActive(false);
		canSelect = false;
		singleSelect = true;
		Microtubule.PPOD_R2 = PPOD_RADIUS2;
	}

	public override void init()
	{
		base.init();
		list_tubes = new List<Microtubule>();
		list_tubes_blank = new List<Microtubule>();
		list_grav = new List<GravPoint>();
		list_grav_blank = new List<GravPoint>();
		list_grav_warble = new List<GravPoint>();
		list_circ = new List<float>();
		//list_circ.length = WARBLE_POINTS * 2;  //TODO:?
		//list_grav_warble.length = WARBLE_POINTS;  //TODO:?
		for (int i = 0; i < WARBLE_POINTS; i++) {
			list_grav_warble.Add(new GravPoint(new Point(0,0), null, 1));
		}

		_runRoutine = StartCoroutine(run());
	}

	public void show()
	{
		this.gameObject.SetActive(true);
		SHOW = true;
	}

	public void hide()
	{
		this.gameObject.SetActive(false);
		SHOW = false;
	}

	public void setMembrane(Membrane m)
	{
		p_membrane = m;
	}

	public void setNucleus(Nucleus n)
	{
		p_nucleus = n;
	}

	public void setCent(Centrosome c)
	{
		p_centrosome = c;
		x = p_centrosome.x;
		y = p_centrosome.y; //line them up*/
	}

	public override void doCellMove(float xx, float yy)
	{
		foreach(Microtubule t in list_tubes) {
			t.doCellMove(xx, yy);
		}
		recordGravityPoints();
	}

	public void ppodContract(Microtubule m, float xx, float yy)
	{
		//x -= xx;
		//y -= yy;
		/*p_centrosome.x = x;
		p_centrosome.y = y;*/
		foreach(Microtubule t in list_tubes) {
			t.ppodContract(xx, yy);
		}

		//DO NOT PPOD CONTRACT PPOD TUBES!
		recordGravityPoints();
	}

	public void updateTube()
	{
		//trace("Cytoskeleton.updateTube()");
		recordGravityPoints();
	}

	public void finishTube(bool isBlank = true)
	{
		//trace("Cytoskeleton.finishTube()!");
		bool bool_ = true;
		List<Microtubule> theList = (isBlank ? list_tubes_blank : list_tubes);
		
		int length = theList.Count;

		for (int i = 0; i < length; i++) 
		{
			if (theList[i].amReady == false)
			{
				bool_ = false;
				//trace("Cytsokeleton.finishTube()! list_tubes[" + i + "].amReady= " + list_tubes[i].amReady);
			}
			else
			{
				//tubeList[i].showDebugShape();
			}
		}
		skeletonReady = bool_;
		//trace("Cytoskeleton.finishTube() skeletonReady = " + skeletonReady);
		if (skeletonReady)
		{
			//hookUpMicrotubules();
			newWarblePoints();
			recordGravityPoints();
			//trace("Cytoskeleton.finishTube()");
			p_membrane.onSkeletonReady();
		}
	}

	public void makeNewTube(CellObject c)
	{
		Point p = new Point(c.x - x, c.y - y);
		Microtubule mt = makeMicrotubule(p);
		mt.setObject(c);
		updateAll();
	}

	public void makeTubes(List<CellObject> list)
	{
		//trace("LIST = " + list);
		int length = list.Count;
		for (int i = 0; i < length; i++) 
		{
			CellObject org = list[i];
			Point p = new Point(org.x - x, org.y - y);
			Microtubule mt = makeMicrotubule(p);
			mt.setObject(org);
		}

	}

	public void updateAll()
	{
		recordGravityPoints();
	}

	public void onStartPPod()
	{
		isPPoding = true;
		p_membrane.onStartPPod();
	}

	public void onCancelPPod()
	{
		isPPoding = false;
		p_membrane.onFinishPPod();
	}

	public void onFinishPPod()
	{
		isPPoding = false;
		p_membrane.onFinishPPod();
	}

	public void cancelPseudopod()
	{
		foreach(Microtubule m in list_tubes_blank) {
			m.cancelPPod();
		}
	}

	public void tryPseudopod(float x2, float y2, float cost)
	{
		//p_engine.onPseudopod();  //TODO

		float dx = x2 - p_centrosome.x;
		float dy = y2 - p_centrosome.y;
		float d2 = (dx * dx) + (dy * dy);

		bool overShot = false;

		
		if (d2 > WizardOfOz.LENS_RADIUS2)
		{ //test to see if the ppod is beyond our range
			overShot = true;
		}

		Vector2 v = new Vector2(dx, dy); //get a vector from the point to the centrosome
		v.Normalize();                         //make it a unit vector
		v *= ((cent_radius - (PPOD_RADIUS))); //find the point right near the edge of the membrane

		Point p = new Point(p_centrosome.x + v.x, p_centrosome.y + v.y);
		Microtubule m = makePPodMicrotubule(p);

		if (overShot)
		{  //if we overshot somehow, make us stop short of escaping the lens
			Vector2 v2 = new Vector2(dx, dy); //get a vector from the centrosome to the point
				v2.Normalize();                           //make it a unit vector
			v2 *= (WizardOfOz.LENS_RADIUS);     //multiply by the lens radius
			x2 = p_centrosome.x + v2.x;               //this is our new ppod point
			y2 = p_centrosome.y + v2.y;
		}

		m.setObjectSelf(); //the microtubule's cellObject is itself!
		m.setSpeed(PPOD_SPEED);
		m.ppodTo(x2, y2);

		SfxManager.Play(SFX.SFXDrain);
		p_cell.spendATP(cost, p, 1, 0, false);
		//recordGravityPoints();
	}

	private Microtubule makePPodMicrotubule(Point p) 
	{
		GameObject mo = Instantiate(Microtubule_Prefab) as GameObject;
		Microtubule temp = mo.GetComponent<Microtubule>();
		temp.setPoints(new Point(0, 0), p);
			temp.setSkeleton(this);
	temp.isBlank = true;
		temp.transform.SetParent(this.transform, false); //was addchild
			list_tubes_blank.Add(temp);
			
			temp.instantGrow();
			
			return temp;
		}

	private Microtubule makeMicrotubule(Point p)
	{
		GameObject mo = Instantiate(Microtubule_Prefab) as GameObject;
		Microtubule temp = mo.GetComponent<Microtubule>();
		temp.setPoints(new Point(0, 0), p);
		temp.setSkeleton(this);
		temp.isBlank = false;
			temp.gameObject.transform.SetParent(this.transform, false);//addChild(temp);
			list_tubes.Add(temp);

		temp.instantGrow();

		return temp;
	}

	public void removeTube(Microtubule m, bool isBlank = false) 
	{

		int i = 0;
		int length;
		if (!isBlank)
		{
			length = list_tubes.Count;
			for (i = 0; i < length; i++)
			{
				if (list_tubes[i] == m)
				{
					list_tubes[i].destruct();
					m.transform.SetParent(null);
					list_tubes.RemoveAt(i);
					break;
				}
			}
		}
		else
		{
			//trace("Cytoskeleton.removeTube() " + m.name);
			//trace("Cytoskeleton.removeTube() " + list_tubes_blank);
			length = list_tubes_blank.Count;
			for (i = 0; i < length; i++)
			{
				if (list_tubes_blank[i] == m)
				{
					list_tubes_blank[i].destruct();
					m.transform.SetParent(null);// removeChild(m);
					list_tubes_blank.RemoveAt(i);
					break;
				}
			}
			//trace("Cytoskeleton.removeTube() after " + list_tubes_blank);
		}

		recordGravityPoints();

	}

	public void clearTubes()
	{
		int length = list_tubes.Count - 1;
		for (int i = length; i >= 0; i--) 
		{
			list_tubes[i].transform.SetParent(null);//removeChild(list_tubes[i]);
			list_tubes[i].destruct();
			list_tubes[i] = null;
		}
		list_tubes = null;
		length = list_tubes_blank.Count - 1;
		for (int i = length; i >= 0; i--)
		{
			//removeChild(list_tubes_blank[i]);
			list_tubes_blank[i].transform.SetParent(null);
			list_tubes_blank[i].destruct();
			list_tubes_blank[i] = null;
		}
	}

	public List<Microtubule> getTubes() 
	{
			return list_tubes;
	}

	public List<Microtubule> getBlankTubes() 
	{
		return list_tubes_blank;
	}

	private void hookUpMicrotubules()
	{

	}

	private void clearGravPoints()
	{
		int i = 0;
		if (list_grav != null)
		{
			int length = list_grav.Count;
			for (i = 0; i < length; i++)
			{
				list_grav[i].destruct();
				list_grav[i] = null;
			}
			list_grav = new List<GravPoint>();
		}
		if (list_grav_blank != null)
		{
			int length = list_grav_blank.Count;
			for (i = 0; i < length; i++)
			{
				list_grav_blank[i].destruct();
				list_grav_blank[i] = null;
			}
			list_grav_blank = new List<GravPoint>();
		}

	}

	public void updateGravityPoints(bool doWarble = false)
	{
		int length = list_tubes.Count;

		for (int j = 0; j < length; j++) 
		{          //just do the regular organelles
			Point p = list_tubes[j].getTerminus();  //we don't need a new gravity point, just a regular point
			try
			{
				list_grav[j].x = p.x;                   //update the existing gravPoint's location only	
				list_grav[j].y = p.y;
			}
			catch (Exception e) {
				//trace(e + "list_tubes.length = " + list_tubes.length + " list_grav.length = " + list_grav.length);
			}
			}
			float r = p_membrane.getRadius();
			int count = 0;
			foreach(Microtubule t in list_tubes_blank) {
				GravPoint g = t.getGravPoint();
				list_grav_blank[count].x = g.x;
				list_grav_blank[count].y = g.y;
				count++;
			}
			if (doWarble)
			{
				getWarblePoints();
			}
			p_membrane.updateGravPoints(list_grav, list_grav_blank);
		}


	private void recordGravityPoints()
	{
		//trace("Cytoskeleton.recordGravityPoints()");
		clearGravPoints();

		int length = list_tubes.Count;
		int j = 0;
		float r = GRAV_RADIUS;
		float rr = p_membrane.getRadius();
		float frac = MEM_FRAC;
		cent_radius = rr * frac;
		for (j = 0; j < length; j++)
		{

			Microtubule m;
			GravPoint gg;
			if (list_tubes[j].p_obj)
			{
				GravPoint g = list_tubes[j].getGravPoint();
				gg = g.copy();
				if (list_tubes[j].hasCentrosome)
				{           //Centrosome is a special case

					gg.radius = cent_radius;
					gg.radius2 = cent_radius * cent_radius;
					updateCentralGrav();
				}
				else
				{
					gg.radius = GRAV_RADIUS;
					gg.radius2 = GRAV_RADIUS2;
				}
				list_grav.Add(gg);
			}
		}


		length = list_tubes_blank.Count;
		for (j = 0; j < length; j++)
		{
			GravPoint ggg = list_tubes_blank[j].getGravPoint();
			GravPoint gg = ggg.copy();                //OMG DON'T you forget to clone data objects : BUGZORS!
			gg.radius = PPOD_RADIUS;
			gg.radius2 = PPOD_RADIUS2;
			list_grav_blank.Add(gg);
		}

		//getWarblePoints();
		updateGravityPoints(true);

		//p_membrane.updateGravPoints(list_grav,list_grav_blank);
	}

	public void updateCentralGrav()
	{
		//CENTRAL_GRAV_RADIUS = cent_radius;
		BasicUnit.updateCGravR2(cent_radius * cent_radius);
	}

	public void updateWarbleCircle()
	{
		//circ:Vector.<Number> = new Vector.<Number>();

		int MAX = WARBLE_POINTS;

		float rr = p_membrane.getRadius() * MEM_WARBLECIRC_FRAC;

		int count = 0;
		for (int i = 0; i < MAX; i++)
		{
			float xloc = ((Mathf.Cos(i / MAX * Mathf.PI * 2)) * rr);
			float yloc = ((Mathf.Sin(i / MAX * Mathf.PI * 2)) * rr);
			list_circ.Add(xloc);//[count] = xloc;// .push(xloc);
			list_circ.Add(yloc);//[count + 1] = yloc;// .push(yloc);
			count += 2;
			//list_grav.push(new GravPoint(new Point(xloc, yloc), null, r2));
		}

	}

	private void getWarblePoints()
	{
		//trace("Cytoskeleton.getWarblePoints() " + list_grav_warble.length);
		if (list_grav_warble != null)
		{
			foreach(GravPoint g in list_grav_warble) {
				if (g != null)
				{
					GravPoint gg = g.copy();
					gg.x += p_centrosome.x;
					gg.y += p_centrosome.y;
					list_grav.Add(gg);
				}
			}
		}
		//trace("Cytoskeleton.getWarblePoints after = " + list_grav);
	}

	public List<GravPoint> newWarblePoints() 
	{
			float rr = p_membrane.getRadius();
			//var frac:Number = MEM_FRAC;
			float r2 = (WARBLE_POINTS* (MEM_WARBLE_FRAC)* (rr)) / (WARBLE_POINTS);
			return makeWarblePoints(r2);

	//trace("Cytoskeleton.newWarblePoints() " + list_grav_warble.length);
	}

	public List<GravPoint> makeWarblePoints(float r2) 
	{
		if (list_circ != null)
		{
			float xo = 0;// p_centrosome.x;
			float yo = 0;// p_centrosome.y;
			updateWarbleCircle();

			float theR = r2;
			int count = 0;
			int length = list_circ.Count;
			for (int i = 0; i < length; i += 2) 
			{
				if (count < list_grav_warble.Count)
				{
					list_grav_warble[count].x = list_circ[i] + xo;
					list_grav_warble[count].y = list_circ[i + 1] + yo;
					list_grav_warble[count].radius = theR;
					list_grav_warble[count].radius2 = theR;
					list_grav_warble[count].p_obj = null;
					count++;
				}
				//trace("Cytoskeleton.makeWarblePoints() list_grav_warble[" + count + "]=" + list_grav_warble[count]);
				//list_grav_warble.push(new GravPoint(new Point(list_circ[i]+xo, list_circ[i + 1]+yo), null, theR));
			}
		}
		
		return new List<GravPoint>(list_grav_warble); //.concat();
		
	//trace("Cytoskeleton.makeWarblePoints()  " + list_circ + "," + list_grav_warble);
	}

	IEnumerator run() 
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
	foreach(Microtubule m in list_tubes) {
				m.growBitHelper();
			}
			foreach (Microtubule mm in list_tubes_blank) {
				mm.growBitHelper();
			}

		}
	}
		



}

