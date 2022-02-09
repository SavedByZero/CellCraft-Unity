using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MembraneNode : ICellGameObject
{

	public float old_x = 0;
	public float old_y = 0;
	public float rotation = 0;
	
	public int index = 0;
		

	public Membrane p_membrane;
		
	public static float CLOSE_ENOUGH = 15;
	public static float NODE_PULL = 15;
		
	public static float NODE_PUSH = 6;

	public static float CENT_PULL = 10;
	public static float CENT_PULL_BASE = 10;
	public static float GRAV_PULL = 2;
		
	public static float FRICT = 0.5f;
	public static float GRAVITY = 40000;
		
	public static bool CENT_PULL_DIMINISHED = false;
	public bool stable = false;
	public bool state_ppod = false; //are we pseudopoding?
	public bool hasRNA = false;
	public List<RNA> list_rna;

	public bool state_tube = false; //do we care about tubes?
		
	public MembraneNode p_prev;
	public MembraneNode p_next;
	public MembraneNode p_cent;
		
	public Point pt_control_next;
	public Point pt_control_prev;
	public int controlSign = 1;
		
	public Vector2 u_norm; //unit vector, my normal
		
	public Vector2 u_next; //unit vector in dir of next node
	public Vector2 u_prev; //unit vector in dir of prev node
	public Vector2 u_cent; //unit vector in dir of centrosome
	public Vector2 u_tube; //unit vector in dir of tube
	public Vector2 u_grav; //unit vector in dir of gravity
		
	public Vector2 v_push; //the final push vector once all calcs have been done
	public Vector2 v_next;
	public Vector2 v_prev;
	public Vector2 v_cent; //final centrosome pull
	public Vector2 v_tube; //final tube pull 
	public Vector2 v_grav; //final gravity pull


	public Vector2 f_node;	//forces
	public Vector2 f_cent;
		
	public float dpull_node = 0;
	public float dpull_cent = 0;
	public float dpull_tube = 0;
	public float dpull_next = 0;
	public float dpull_prev = 0;

		
	public float foldRatio = 1;
	public float d2_next; //distance^2 to p_next
	public float d2_cent = 1;
		
	private float d_centx = 0;
	private float d_centy = 0;
		
	public static float D2_CENT = 1; //the GREATEST D2_CENT that anyone has
	public static float D2_CENT_NEW = 1; //for the next update
		
	public bool rest_node = false;
	public bool rest_cent = false;
	public bool rest_tube = false;
		
	public static float D_NODEREST;
	public static float D_CENTREST;
		
	public static float D2_NODEREST; //distance^2 between nodes that I will rest at
	public static float D2_NODEREST_OLD;
	public static float D2_CENTREST;
		
	public float d2_tubeRest;

	public CellObject p_org;
		
	public bool has_collided = false;
		
	public float stretch;
		
		
	private static float NODE_RADIUS = 50;
	private static ObjectGrid p_grid;

	private GameDataObject gdata;
	public bool isFolded = false;
	public float grid_x = 0;
	public float grid_y = 0;
		

		
	private static float span_w = 0;
	private static float span_h = 0;
	private static float grid_w = 0;
	private static float grid_h = 0;

		
	//a static floatiable across the class, whenever the node tugs on the centrosome, the centrosome must move
	//to preserve Newton's law of reaction. We build up this "tug" in a static floatiable, and then the membrane
	//uses it to move the whole cell once a frame
	public static float tug_x = 0; 
	public static float tug_y = 0;
		
	public float tug_prev_x = 0;
	public float tug_prev_y = 0;
	public float tug_next_x = 0;
	public float tug_next_y = 0;
		
	public float xdist = 0; //the distance I moved last
	public float ydist = 0;

    public float x { get { return 0; } set => _ = value; }
    public float y { get { return 0; } set => _ = value; }
    bool ICellGameObject.dying { get { return false; } set => _ = value; }

    public MembraneNode()
	{
		u_norm = new Vector2();
		u_cent = new Vector2();
		u_tube = new Vector2();
		u_grav = new Vector2();

		u_next = new Vector2();
		u_prev = new Vector2();

		v_next = new Vector2();
		v_prev = new Vector2();

		v_push = new Vector2();

		v_cent = new Vector2();
		v_grav = new Vector2();
		v_tube = new Vector2();

		f_node = new Vector2();
		f_cent = new Vector2();
		/*f_bleb = new Vector2D();
		f_ppod = new Vector2D();*/

		pt_control_next = new Point(x, y);
		pt_control_prev = new Point(x, y);


	}

	public void init()
	{
		makeGameDataObject();
		placeInGrid();
	}

	public static void turnOffCentPull()
	{
		if (!CENT_PULL_DIMINISHED)
		{
			CENT_PULL_BASE = CENT_PULL;
			CENT_PULL = 0;
			CENT_PULL_DIMINISHED = true;
		}
	}

	public static void turnOnCentPull()
	{
		if (CENT_PULL_DIMINISHED)
		{
			CENT_PULL = CENT_PULL_BASE;
			CENT_PULL_DIMINISHED = false;
		}
	}

	public float getRadius2() 
	{
			return NODE_RADIUS* NODE_RADIUS;
	}

	public void destruct()
	{
		p_cent = null;
		p_membrane = null;
		p_next = null;
		p_prev = null;
		p_org = null;

	}

	public void addRNA(RNA r)
	{
		if (list_rna != null )
		{

		}
		else
		{
			list_rna = new List<RNA> ();
			hasRNA = true;
		}
		list_rna.Add(r);
	}

	public void removeRNA(RNA r)
	{
		int length = list_rna.Count;
		for (int i = 0; i < length; i++) 
		{
			if (list_rna[i] == r)
			{
				list_rna[i] = null;
				list_rna.RemoveAt(i);
				i--;
				length--;
			}
		}
		if (length <= 0)
		{
			list_rna = null;
			hasRNA = false;
		}
	}

		public static void setGrid(ObjectGrid g)
		{
			grid_w = g.getCellW();
			grid_h = g.getCellH();
			span_w = g.getSpanW();
			span_h = g.getSpanH();
			p_grid = g;
		}

	public void updateStretch()
	{
		stretch = (D2_NODEREST) / d2_next;
	}

	public float getThick(float t)
		{
			//fadupinator
			//return t;
			
			float thick = stretch;
			thick *= t;
			if (thick > t)
				return t;
			if (thick < .1f)
				return .1f;
			return thick;
		}

	public static void getSprings(float nd, float cd) 
	{
		D2_NODEREST = nd * nd;
		D2_CENTREST = cd * cd;
		D_NODEREST = nd;
		D_CENTREST = cd;

	}

	public Color getColor()
	{
		//return 0x00066FF;
		float frac = (D2_NODEREST) / d2_next;
		if (frac > 1) frac = 1;
		if (frac < 0) frac = 0;

		//fadupinator
		//frac = 1;
		
		return Color.Lerp(Color.red, new Color(0, (float)66/255,1), frac);
	}

	public void makeGameDataObject()
	{
		gdata = new GameDataObject();
		gdata.setThing(x, y, NODE_RADIUS, this, this.GetType());
	}

	public GameDataObject getGameDataObject() 
	{
			return gdata;
		}

	public void makeSprings()
	{

	}


	public void addPushVector(Vector2 v, float mag) 
	{
		v *= (mag);
		v_push += (v);
	}

	public void updateDist()
	{

		float x1 = x;
		float y1 = y;
		float x2 = p_next.x;
		float y2 = p_next.y;
		d2_next = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))); //get dist2 to next node

		x2 = p_prev.x;
		y2 = p_prev.y;
		float d2_prev = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))); //get dist2 to prev node

		x2 = p_cent.x;
		y2 = p_cent.y;
		d_centx = x1 - x2;
		d_centy = y1 - y2;
		d2_cent = (((d_centx) * (d_centx)) + ((d_centy) * (d_centy))); //get dist2 to centrosome

		x1 = p_prev.x;
		y1 = p_prev.y;
		x2 = p_next.x;
		y2 = p_next.y;
		float d2_fold = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))); //get dist2 to fold

		if (d2_fold < MembraneNode.D2_NODEREST)
		{
			foldRatio = d2_fold / MembraneNode.D2_NODEREST;
		}
		else
		{
			foldRatio = 1;
		}

		if (index == 0)
		{
			D2_CENT = D2_CENT_NEW;
			D2_CENT_NEW = 0;
		}
		if (d2_cent > D2_CENT_NEW)
		{
			D2_CENT_NEW = d2_cent;
		}

		dpull_prev = D2_NODEREST - d2_prev; //positive : PUSH AWAY , negative PULL TOWARDS
		dpull_next = D2_NODEREST - d2_next;
		dpull_cent = d2_cent - D2_CENTREST;

		if (index == 0)
		{
			//trace("dpull p=" + dpull_prev + " n=" + dpull_next);
		}

		churn();
		//updateBbody();
	}

	public void churn()
	{
		old_x = x;
		old_y = y;
		u_next.Set(x - p_next.x, y - p_next.y);
		u_prev.Set(x - p_prev.x, y - p_prev.y);
		u_cent.Set(x - p_cent.x, y - p_cent.y);

		u_next.Normalize();
		u_prev.Normalize();
		u_cent.Normalize();

		pt_control_next.x = (x + p_next.x) / 2;
		pt_control_next.y = (y + p_next.y) / 2;
		pt_control_prev.x = (x + p_prev.x) / 2;
		pt_control_prev.y = (y + p_prev.y) / 2;

		Vector2 pushv = u_cent * 20; 
		Point push = new Point(pushv.x, pushv.y);
		float pushMult = 0;


		float d2_centRatio = D2_CENT / d2_cent;

		pushMult = (d2_centRatio) / (D2_CENT / p_prev.d2_cent);
		pushMult = pushMult - 1;
		pushMult *= 1;

		
		if (float.IsNaN(pushMult)) 
			pushMult = 0;

		pt_control_prev.x += push.x * (pushMult);// * foldRatio);
		pt_control_prev.y += push.y * (pushMult);// * foldRatio);

		pushMult = (d2_centRatio) / (D2_CENT / p_next.d2_cent);
		pushMult = pushMult - 1;
		pushMult *= 1;

		if (float.IsNaN(pushMult))
			pushMult = 0;

		pt_control_next.x += push.x * (pushMult);// * foldRatio);
		pt_control_next.y += push.y * (pushMult);// * foldRatio);



		float r = FastMath.toRotation(u_cent) * (180/Mathf.PI);
		r += 90;
		rotation = r;

		v_push.Set(0, 0); //clear the push vector			

		float nodeMult = NODE_PULL;
		float centMult = CENT_PULL;


		rest_node = true; //set this temporarily to true. If the test fails in either next two case, it is false;

		//inline abs functions for speed
		float absdpull_next = (dpull_next < 0) ? -dpull_next : dpull_next;
		float absdpull_prev = (dpull_prev < 0) ? -dpull_prev : dpull_prev;
		float absdpull_cent = (dpull_cent < 0) ? -dpull_cent : dpull_cent;


		if (absdpull_next > CLOSE_ENOUGH)
		{
			rest_node = false;
			dpull_next /= D2_NODEREST;
			dpull_next *= nodeMult;
			//addPushVector(u_next, dpull_next);
			v_next = u_next * (dpull_next);
		}
		if (absdpull_prev > CLOSE_ENOUGH)
		{
			rest_node = false;
			dpull_prev /= D2_NODEREST;
			dpull_prev *= nodeMult;
			//addPushVector(u_prev, dpull_prev);
			v_prev = u_prev * (dpull_prev);
		}
		if (absdpull_cent > CLOSE_ENOUGH)
		{
			v_cent = new Vector2(u_cent.x, u_cent.y);
			v_cent *= (-centMult);
		}


		updateStretch();

		//if (state_ppod) {
		if (hasRNA)
		{
			foreach(RNA rna in list_rna) {
				rna.mnodeMove(x - old_x, y - old_y);
			}
		}
		//}
	}

	public void tugNodes()
	{
	}

	public void doCellMove(float xx, float yy)
	{
		x += xx;
		y += yy;
	}

	public void doMove()
	{


		xdist = (v_next.x * FRICT) + (v_prev.x * FRICT) + (v_cent.x);
		ydist = (v_next.y * FRICT) + (v_prev.y * FRICT) + (v_cent.y);

		// Go ahead and add the distances
		x += xdist;
		y += ydist;



		updateLoc();

	}

	// Checks to see if n1 is more clockwise than n2.  Returns true is n1 is clockwise, false if counter clockwise.
	// We define clockwise as being 0 <= n1 - n2 <= PI)

	private bool checkClockwiseNodes(MembraneNode n1, MembraneNode n2, Centrosome cent)
	{
			float theta1 = Mathf.Atan2(n1.y-cent.y, n1.x-cent.x);
			float theta2 = Mathf.Atan2(n2.y-cent.y, n2.x-cent.x);
			float thetaDiff = boundTheta(theta1 - theta2); 
			if (thetaDiff > 0 && thetaDiff<Math.PI) // We're defining "clockwise" as being less than Pi clockwise around
				return true;
			else
				return false;
	}

	// Bound polar Theta value between 0 and 2Pi
	private float boundTheta(float n)
	{
		float Pi2 = 2 * Mathf.PI;
		if (n < 0)
			return (n + Pi2);
		else if (n > Pi2)
			return (n - Pi2);
		else
			return n;
	}



	public void placeInGrid()
	{
		float xx = x - p_cent.x + span_w / 2;
		float yy = y - p_cent.y + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);

		p_grid.putIn((int)grid_x, (int)grid_y, gdata);
	}

	private void updateLoc()
	{
		float xx = d_centx + span_w / 2;
		float yy = d_centy + span_h / 2;

		gdata.x = xx;
		gdata.y = yy;

		int old_x = (int)grid_x;
		int old_y = (int)grid_y;
		grid_x = (int)(xx / grid_w);
		grid_y = (int)(yy / grid_h);

		if (grid_x < 0) grid_x = 0;
		if (grid_y < 0) grid_y = 0;
		if (grid_x > grid_w) grid_x = grid_w - 1;
		if (grid_y > grid_h) grid_y = grid_h - 1;

		if ((old_x != grid_x) || (old_y != grid_y))
		{
			p_grid.takeOut(old_x, old_y, gdata);
			p_grid.putIn((int)grid_x, (int)grid_y, gdata);
		}
	}


	public void applyGravities()
	{
		v_grav.Set(0, 0); //clear gravity vector


	}

	public void applyGravity(float xx, float yy)
	{
		u_grav.Set(x - xx, y - yy);
		float x1 = x;
		float y1 = y;
		float x2 = xx;
		float y2 = yy;
		float dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));

		u_grav.Normalize();
		u_grav *= (GRAVITY / 2 / dist2);

		v_grav += (u_grav);
	}










}

