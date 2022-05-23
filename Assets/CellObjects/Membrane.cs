using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using Unity.VectorGraphics;

public class Membrane : CellObject
{
	private List<MembraneNode> list_nodes = new List<MembraneNode>();
	private List<GravPoint> list_grav;
	private List<BasicUnit> list_basicUnits;
	private List<Virus> list_viruses;
	private List<GravPoint> list_grav_blank;
	public SoftBody Skin;
	public GameObject GraphicsPrefab;
		
		
	private Centrosome p_cent;
	private Cytoskeleton p_skeleton;

    public Coroutine _waitForReadyRoutine { get; private set; }
	private Coroutine _doMouseMoveRoutine;

    public bool skeletonReady = false;
		
		public GameObject membraneSprite;
		public GameObject iconSprite;
		
		public Graphics shape_spring;  //was: shape
	//Scene gg = new Scene();
		public Graphics shape_gap;
		public Graphics shape_cyto;
		public Graphics shape_outline;
		
		public Targetter targetter;
		
		public Graphics shape_debug;
		
		public static float cyto_volume = 0;
		
		public static float MIN_STRETCH = 0.35f;
		public static float MAX_STRETCH = 1 - MIN_STRETCH;
		public static float ACC_STRETCH = MAX_STRETCH*0.5f;
		public static float MIN_COLOR_STRETCH = MIN_STRETCH*2;
		public static float AVG_STRETCH = 1;
		
		public static float MAX_DEFENSIN_STRENGTH = 0.5f;
		
		private bool DEBUG = true;
		public static bool SHOW_GRAVPOINTS = false;
		public static bool SHOW_NODES = false;
		private const bool SHOWLINES = true;

		public const float DRAW_CURVES = 0;
		public const float DRAW_LINES = 1;
		
		public static float DRAW_QUALITY = DRAW_CURVES; //draw curves
		
		
		
		public const float SPRING_THICK_ = .35f;
		public const float GAP_THICK_ = .15f;
		public const float OUTLINE_THICK_ = .40f;
		
		public static float SPRING_THICK = SPRING_THICK_;
		public static float GAP_THICK = GAP_THICK_;
		public static float OUTLINE_THICK = OUTLINE_THICK_;
		
		private const float SPRING_MIN = .13f;
		private const float GAP_MIN = .04f;
		private const float OUTLINE_MIN = .15f;
		
		private const float HEALTH_PER_NODE = 10;

		private int HEALTH_COUNT = 60; //+1 health every 2 seconds
		private int healthCounter = 60;//HEALTH_COUNT;
		
		private float D_NODECLICK = 50;
		private float D2_NODECLICK = 50 * 50;//D_NODECLICK * D_NODECLICK;
		private float D_MNODECLICK = 50 * .75f;
		private float D2_MNODECLICK = (50 * .75f) * (50*.75f);//D_MNODECLICK* D_MNODECLICK;

		private float D_BASIC_UNIT_COLLIDE = .70f;
	private float D2_BASIC_UNIT_COLLIDE = 4.9f;//D_BASIC_UNIT_COLLIDE* D_BASIC_UNIT_COLLIDE;

	private const float OBJ_FUDGE = 1; //the "effective" size of an object gravpoint radius for a hard collision

		private int waitRecycleCounter = 0;
		private bool waitForRecycle = false;
		
		private int waitCounter = 0;
		
		private bool isDragging = false; 
		private bool isPPodCursor = false;
		private bool isMouseOver = false; 
		private bool isMouseDown = false; //did we click the mouse down on this object
		private float mouseDown_x = 0;
		private float mouseDown_y = 0;
		
		private bool isPPoding = false;
		
		private float d2_mouse = 0;
		private const float D2_PPOD = 1.00f; //100 pixels
		public const float PPOD_ANGLE = 90; //+- 30 degrees
		
		private float worldScale = 1;
		
		//private Engine p_engine;
		private ObjectGrid p_objectGrid;
		
		private bool dummy_flag = false;
		private bool endDummy = false;
		
		private Point dummy_lastPoint;
		private Point dummy_nextPoint;
		
		private Point dummy_avgPoint;
		private Point dummy_midPoint;
		private Point dummy_midPoint2;
		private int dummyCounter = 0;
		
		/*private var ph_balance:Number = 7.5;	  //the real number
		private var ph_balance_show:Number = 7.5; //the number we show - add a delay for animation*/
		
		private Color cyto_col;
		private Color spring_col;
		private Color gap_col;
		
		private Color health_col;
		private Color health_col2;

	public const float STARTING_RADIUS = 2.5f;//4.00f;
	public const int STARTING_NODES = 15;//15;
		public const int MAX_NODES = 25;

	public static int CURR_NODES = 15;//15;
		
		private ObjectGrid p_cgrid;
		//private var p_virusGrid:ObjectGrid;
		
		private float D2_NODERADIUS = .10f;//
		
		private float defensins=0; 				//how many defensins do we have?
		public static float defensin_strength=0; //chance of killing incoming viruses
		private const float MAX_DEFENSIN_PER_NODE = 1; //you need X defensins per node for maximum strength
		
		
		private Vector2 penetrate_unit_vector;
		private Vector2 penetrate_vector; //used for pushing goodiegems away
		
		//private var NODES_PER_SHIELD:int = 5;
		//private var shieldCounter:int = NODES_PER_SHIELD;
		
		private int wait_cent_count = 0;
		private int WAIT_CENT_TIME = 30;
		private int WAIT_CENT_TIME_SHORT = 5;
		private Coroutine _churnRoutine;
		private Coroutine _waitForRecycleReadyRoutine;
		
		public bool acceptingVesicles = true;
    private Coroutine _waitShortForCentPullRoutine;
    private Coroutine _waitForCentPullRoutine;
	


	//new stuff from the replacement class
	public GameObject Rim;
	private Graphics m_rim;
	public GameObject NodePrefab;
	public Transform Anchor;
	private List<MembraneNode> _nodePool;
	public Muscle Mover;

	private void Awake()
	{
		_nodePool = new List<MembraneNode>();
		Mover.onFinishStretching += moverFinishedStretching;
		//shape_cyto = gameObject.AddComponent<Graphics>();
		//m_rim = Rim.AddComponent<Graphics>();
		//m_rim.TextureName = "MembraneOuter";
		//m_rim.Pattern = true;
		//shape_cyto.PixelsPerUnit = 1;
		//m_rim.PixelsPerUnit = 1;
		//shape_cyto.SortOrder = 2;


	}

	public override void Start()
	{
		base.Start();
		//m_rim.LineWidth = .4f;
		//cyto_col = FastMath.ConvertFromUint(0x44AAFF);
		//m_rim.SetLineColor(Color.clear);//(FastMath.ConvertFromUint(0x0066FF));
		//spring_col = FastMath.ConvertFromUint(0x0066FF); //dark blue
		//gap_col = FastMath.ConvertFromUint(0x99CCFF);  //light blue 
		
		defensin_strength = 0;
		canSelect = false;
		//singleSelect = true;
		text_title = "Membrane";
		text_description = "";
		text_id = "membrane";
		num_id = Selectable.MEMBRANE;
		bestColors = new bool[]{ false, false, true};
		//buttonMode = false;  //TODO
		updatePH(7.5f);
		CURR_NODES = 15;
	

		has_health = true;
		setMaxHealth(1000, true);
		if (p_cgrid == null)
			p_cgrid = GameObject.FindObjectOfType<ObjectGrid>();
		//instantSetHealth(10);

		//shape_cyto.SetFillColor(FastMath.ConvertFromUint(0x44aaff));
	}

	public override void destruct()
	{
		StopCoroutine(_churnRoutine);


		
		//membraneSprite.removeChild(shape_spring);
		//membraneSprite.removeChild(shape_gap);
		//membraneSprite.removeChild(shape_outline);
		//removeChild(membraneSprite);
		//removeChild(shape_cyto);

		destructNodes();
		//p_cent = null;
		//p_cell = null;
		p_skeleton = null;
		//p_engine = null;
		p_objectGrid = null;
		base.destruct();
	}

	public override void init()
	{
		base.init();
		//list_shields = new Vector.<ShieldIcon>();
		//list_nodes = new List< MembraneNode > ();
		list_viruses = new List<Virus> ();

		//iconSprite = new Sprite();  //TODO

		makeNodes(STARTING_RADIUS, STARTING_NODES);
		tempTurnOffCentPullShort();
		//MembraneNode.turnOffCentPull();

		/*if (testNodeLinks())
		{
			activateNodes();
		}*/
		

		

		//shape_cyto = (Instantiate(GraphicsPrefab) as GameObject).GetComponent<Graphics>();//new Graphics();
		//shape_spring = (Instantiate(GraphicsPrefab) as GameObject).GetComponent<Graphics>();
		//shape_node = new Shape();

		//shape_gap = (Instantiate(GraphicsPrefab) as GameObject).GetComponent<Graphics>();
		//shape_outline = (Instantiate(GraphicsPrefab) as GameObject).GetComponent<Graphics>();
		//shape_debug = (Instantiate(GraphicsPrefab) as GameObject).GetComponent<Graphics>();

		
		//shape_spring.SetFillColor(spring_col);
		//shape_spring.SetLineColor(spring_col.r, spring_col.g, spring_col.b);
		//shape_gap.SetFillColor(gap_col);
		//shape_gap.SetLineColor(gap_col.r, gap_col.g, gap_col.b);


		//shape_cyto.transform.SetParent(membraneSprite.transform);
		//shape_cyto.name = "Cytoplasm";

		//shape_outline.transform.SetParent(membraneSprite.transform);
		//shape_outline.name = "Outline";
		//shape_spring.transform.SetParent(membraneSprite.transform);
		//shape_spring.name = "Spring";
		//shape_gap.transform.SetParent(membraneSprite.transform);
		//shape_gap.name = "Gap";
		//shape_debug.transform.SetParent(this.transform);
		targetter.transform.SetParent(this.transform);
		/*   //TODO: figure out how this is done in Unity 

		//membraneSprite.addEventListener(MouseEvent.ROLL_OVER, doOver, false, 0, true);
		//membraneSprite.addEventListener(MouseEvent.ROLL_OUT, doOut, false, 0, true);
		//membraneSprite.addEventListener(Event.MOUSE_LEAVE, doOut, false, 0, true);
		//membraneSprite.addEventListener(MouseEvent.MOUSE_DOWN, doMouseDown, false, 0, true);

		membraneSprite.addEventListener(MouseEvent.CLICK, click, false, 0, true);

		 */
		_churnRoutine = StartCoroutine(churn());
		//addEventListener(RunFrameEvent.FAUXFRAME, justDraw, false, 0, true);

	
		
		targetter.gameObject.SetActive(false);

		dummy_avgPoint = new Point(0,0);
		dummy_midPoint = new Point(0,0);
		dummy_midPoint2 = new Point(0,0);
		dummy_lastPoint = new Point(0,0);
		dummy_nextPoint = new Point(0,0);


		//hideShields();

		p_cell.onMembraneUpdate();
	}

	public float getDefensins() 
	{
			return defensins;
	}

	public float getDefensinStrength()
	{
		return defensin_strength;
	}


	public void removeDefensin(float n)
	{
		defensins -= n;
		calcDefensinStrength();
		p_engine.oneLessDefensin(n);
	}

	private void calcDefensinStrength()
	{
		defensin_strength = (defensins / (list_nodes.Count * MAX_DEFENSIN_PER_NODE));
		if (defensin_strength > MAX_DEFENSIN_STRENGTH)
		{
			defensin_strength = MAX_DEFENSIN_STRENGTH;
		}
		p_engine.setDefensinStrength(defensin_strength);
	}

	public void addDefensin(float n)
	{
		defensins += n;
		calcDefensinStrength();
		p_engine.finishDefensin();
		//p_cell.onFinishDefensin();  //TODO

	}

	private void destructNodes()
	{
		int i = 0;
		foreach(MembraneNode m in list_nodes) {
			m.destruct();
			//removeChild(m);
			list_nodes[i] = null;
			i++;
		}
		list_nodes = null;
	}

	public void setObjectGrid(ObjectGrid og)
	{
		p_objectGrid = og;

		MembraneNode.setGrid(og);
	}

	public void setCanvasGrid(ObjectGrid og)
	{
		p_cgrid = og;
	}

	public void updateGrid()
	{
		foreach(MembraneNode m in list_nodes) {
			m.placeInGrid();
		}

		int length = list_viruses.Count;
		for (int i = 0; i < length; i++ ) {
			if (list_viruses[i])
			{
				if (!list_viruses[i].dying && !list_viruses[i].isDoomed)
				{
					list_viruses[i].placeInGrid();
				}
				else
				{
					list_viruses[i].clearGrid();
					list_viruses[i] = null;
					list_viruses.RemoveAt(i);
					i--;
					length--;
				}
			}
			else
			{
				list_viruses.RemoveAt(i);
				i--;
				length--;
			}
		}
		//length = list_
	}

	public List<MembraneNode> getClosestNodes(float x, float y, float r2) 
	{
			List<MembraneNode> v = new List<MembraneNode>();
			foreach(MembraneNode m in list_nodes)
			{
				float x2 = m.x;
				float y2 = m.y;
				float dist2 = (((x - x2) * (x - x2)) + ((y - y2) * (y - y2)));
				if (dist2 < r2)
				{
					
					v.Add(m);
				}
			}
			return v;
	}

	public void setSkeleton(Cytoskeleton s) 
	{
		p_skeleton = s;
	}

	public void setCent(Centrosome c) 
	{
		p_cent = c;
	}

	/*
	public bool testNodeLinks()
	{
		bool done = false;
		bool pass = false;
		int i = 0;
		MembraneNode theNode;
		MembraneNode firstNode;
		firstNode = list_nodes[0];			
		while (!done)
		{
			theNode = list_nodes[i];		//get the next node (starts at 0)
			if(i<list_nodes.Count-1)
			{	//if not at the end yet
				if (theNode.p_next == list_nodes[i + 1]) 
				{	//if the next node is ALSO the next one in the list
						theNode = theNode.p_next;				//we're good, look at the next node
				}
			}
			else
			{
				if (theNode.p_next == firstNode)
				{ //if the next one is the first one
					done = true;                    //we are done
					pass = true;                    //we have passed
				}
				else
				{
					done = true;                    //we have failed
					pass = false;
				}
			}
		i++;	//increase the counter by one
		}
		return pass;
	}*/
		
	public float getCircum()
	{
		return (list_nodes.Count * MembraneNode.D_NODEREST);
	}

	public override float getRadius()
	{
		return (list_nodes.Count * MembraneNode.D_NODEREST) / (Mathf.PI * 2);
	}

	
	private void activateNodes()
	{
		//var length:int = list_nodes.length;
		Skin.PrepareNodes();
		float distX = list_nodes[0].x - list_nodes[0].p_next.x;
		float distY = list_nodes[0].y - list_nodes[0].p_next.y;
		MembraneNode.D2_NODEREST = (distX * distX) + (distY * distY);
		distX = x - list_nodes[0].p_cent.x;
		distY = y - list_nodes[0].p_cent.y;
		MembraneNode.D2_CENTREST = (distX * distX) + (distY * distY);
		MembraneNode.D_NODEREST = Mathf.Sqrt(MembraneNode.D2_NODEREST);
		MembraneNode.D_CENTREST = Mathf.Sqrt(MembraneNode.D2_CENTREST);

	}

	/**
	 * Returns the index of the least stretched node
	 * @return
	 */

	public int getMinStretchIndex() 
	{
		float bestStretch = 0;
		int bestNode = 0;
		for (int j = 0; j<list_nodes.Count; j++) 
		{
			if (list_nodes[j].stretch > bestStretch) 
			{
				bestStretch = list_nodes[j].stretch;
				bestNode = j;
			}
		}
		return bestNode;
	}
		
		/**
		 * Returns the index of the most stretched node
		 * @return
		 */
		
	public int getMaxStretchIndex()
	{
		int length = list_nodes.Count;
		float bestStretch = 1;
		int bestNode = 0;
		for (int j = 0; j < length; j++) 
		{
			if (list_nodes[j].stretch < bestStretch)
			{
				bestStretch = list_nodes[j].stretch;
				bestNode = j;
			}
		}
		return bestNode;
	}

	public bool canTakeMembrane() 
	{
			return (!waitForRecycle);
		}

/**
 * Finds the least stretched node and removes it
 */

	public void removeMembraneNodes(int max, bool doUpdate = true) 
	{
		waitForRecycle = true;
		waitRecycleCounter = 0;
		_waitForRecycleReadyRoutine = StartCoroutine(waitForRecycleReady());
		for (int i = 0; i < max; i++) 
		{
			if (doUpdate && i == max - 1)
			{       //only do the update on the last one, if we're doing updates at all
				removeMembraneNode(true);
			}
			else
			{
				removeMembraneNode(false);
			}
		}
	}


	private void removeMembraneNode(bool doUpdate = true) 
	{
		int bestNode = getMinStretchIndex();
		deleteMembraneNode(bestNode, doUpdate);
		if (doUpdate)
		{
			onChangeMembrane(true);
		}
		updateGrid();
	}

	private void onChangeMembrane(bool hardUpdate = false)
	{
		p_skeleton.newWarblePoints();

		p_skeleton.updateAll();
		p_cell.onMembraneUpdate(hardUpdate);
		calcDefensinStrength();
	}

	private void onChangeNodes(int i = 0)
	{
		CURR_NODES = list_nodes.Count;
		int listlength = list_nodes.Count;
		float springDist = MembraneNode.D_NODEREST;
		float circumference = springDist * (listlength + 2); //+2 to fudge
		float radius = circumference / (Mathf.PI*2);
		//MembraneNode.getSprings(springDist, radius);
		updateBasicUnitCollideDist();

		updateMaxHealth(i);

	}

	private IEnumerator waitShortForCentPull()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			wait_cent_count++;
			if (wait_cent_count > WAIT_CENT_TIME_SHORT)
			{
				wait_cent_count = 0;
				StopCoroutine(_waitShortForCentPullRoutine);
				//MembraneNode.turnOnCentPull();
				readyForVesicle();
			}
		}
	}

	private IEnumerator waitForCentPull()
	{
		while (true)
		{
			wait_cent_count++;
			if (wait_cent_count > WAIT_CENT_TIME)
			{
				wait_cent_count = 0;
				StopCoroutine(_waitForCentPullRoutine);
				//MembraneNode.turnOnCentPull();
				readyForVesicle();
			}
		}
	}

	/**
		 * Finds the most stretched node and adds an extra node there
		 */
	public void addMembraneNode(bool doUpdate = true)
	{
		cancelPseudopod();
		tempTurnOffCentPull();

		int bestNode = getMaxStretchIndex();

		insertMembraneNode(bestNode, doUpdate);

		if (doUpdate)
		{
			onChangeMembrane(true);
		}

		float length = list_nodes.Count;
		foreach(MembraneNode m in list_nodes) 
		{
			Vector2 v = new Vector2(m.x - cent_x, m.y - cent_y);
			v.Normalize();
			m.x += v.x * length; //extra "shove" to avoid crap
			m.y += v.y * length;

		}
		updateGrid();
	}

	public void acceptVesicle()
	{
		acceptingVesicles = false;
	}

	public void readyForVesicle()
	{
		acceptingVesicles = true;
	}

	public void tempTurnOffCentPullShort()
	{
		_waitShortForCentPullRoutine = StartCoroutine(waitShortForCentPull());
		//addEventListener(RunFrameEvent.RUNFRAME, waitShortForCentPull, false, 0, true);
		//MembraneNode.turnOffCentPull();
	}

	public void tempTurnOffCentPull()
	{
		_waitForCentPullRoutine = StartCoroutine(waitForCentPull());
		//addEventListener(RunFrameEvent.RUNFRAME, waitForCentPull, false, 0, true);
		//MembraneNode.turnOffCentPull();
	}

	/*
	 * This deletes and reshuffles all the positions 
	 * */
	public void deleteMembraneNode(int i, bool doUpdate = true)
	{

		tempTurnOffCentPull();

		MembraneNode theNode = list_nodes[i];


		//MembraneNode prev = list_nodes[i].p_prev;
		//MembraneNode next = list_nodes[i].p_next;

		//prev.p_next = next; //unhook it from the list, and hook up its pointers to eachother
		//next.p_prev = prev;

		//theNode.destruct(); //kill the node




		list_nodes[i].gameObject.SetActive(false);// = null;
		list_nodes.RemoveAt(i); //remove it from the list

		//What will need to be done here as a replacement:  
		//-deactivate all the nodes 
		//-reactivate n-1 (new number) of nodes from the pool as if they were just made
		//then: do everything below 

		int listlength = list_nodes.Count;

		for (int j = 0; j < listlength; j++) { //reorder everybody
			list_nodes[j].index = j;
		}

		float length = list_nodes.Count;
		foreach(MembraneNode m in list_nodes) {

			//Vector2 v = new Vector2(m.x - cent_x, m.y - cent_y);
			//v.Normalize();
			//m.x += v.x * length; //extra "shove" to avoid crap
			//m.y += v.y * length;

		}

		if (doUpdate)
			onChangeNodes(-1);
	}

	public MembraneNode insertMembraneNode(int i, bool doUpdate = true) 
	{ //insert a new basal node after i
			//trace("Membrane.insertMembraneNode(" + i + ")");
			
		//WEIRD BUG: if the vesicle comes from the right, we get i = -1, throwing an out of range error
		if (i< 0) {
			i = 0;
		}

		/*MembraneNode newNode = interpolate(list_nodes[i], list_nodes[i].p_next, 0.5f);

		MembraneNode old = list_nodes[i];
		MembraneNode oldNext = list_nodes[i].p_next;

		
		newNode.p_prev = (old);
		newNode.p_next = (oldNext);

		
		old.p_next = (newNode);
		oldNext.p_prev = (newNode);

		newNode.p_cent = (p_cent);
		*/

		//This code was trying to insert a new node at position X.  It would interpolate the position between x and x+1, then shift everything accordingly.
		//we will need to:
		//-record the spot of node i. 
		//find some way to make sure the new node is at that spot, in the new deactivate / reactivate system.  Maybe.


		MembraneNode newNode = null;  //make it!

		float listlength = list_nodes.Count;
		
		list_nodes.Insert(i,newNode); //add it to the list



		for (int j = 0; j < listlength; j++) { //reorder everybody
			list_nodes[j].index = j;
		}

		if (doUpdate)
			onChangeNodes(1);

		return newNode;
					//updateVolume();
					//hookUpMicrotubules(p_skeleton.getTubes());
			
	}

	public override float getCircleVolume() 
	{
		float length = list_nodes.Count;
		float rad = (length* MembraneNode.D_NODEREST)/2;
		cyto_volume = Mathf.PI* (rad* rad); // area, really, but whatever
		return cyto_volume;
	}

	public void updateBasicUnitCollideDist()
	{
		D2_BASIC_UNIT_COLLIDE = MembraneNode.D2_NODEREST;
		D_BASIC_UNIT_COLLIDE = Mathf.Sqrt(D2_BASIC_UNIT_COLLIDE);
	}

	public void onSkeletonReady()
	{
		if (!skeletonReady)
		{
			_waitForReadyRoutine = StartCoroutine(waitForReady());
			//addEventListener(RunFrameEvent.RUNFRAME, waitForReady, false, 0, true);
			skeletonReady = true;
			giveMaxHealth();
		}
	}

	public IEnumerator waitForRecycleReady() 
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			waitRecycleCounter++;
			if (waitRecycleCounter > 30)
			{
				waitRecycleCounter = 0;
				StopCoroutine(_waitForRecycleReadyRoutine);
				//removeEventListener(RunFrameEvent.RUNFRAME, waitForRecycleReady);
				waitForRecycle = false;
			}
		}
	}

	public IEnumerator waitForReady()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			waitCounter++;
			if (waitCounter > 3)
			{
				StopCoroutine(_waitForReadyRoutine);
				//removeEventListener(RunFrameEvent.RUNFRAME, waitForReady);
				skeletonReady = true;
			}
		}
	}

	public void updateViruses(List<Virus> list)
	{
		list_viruses = new List<Virus>(list);//.concat();
	}

	public void updateBasicUnits(List<BasicUnit> list)
	{
		list_basicUnits = new List<BasicUnit>(list);
	}

	public void updateGravPoints(List<GravPoint> list, List<GravPoint> list_blank=null)
	{

		list_grav = new List<GravPoint>(list);

		if (list_blank != null)
		{
			list_grav_blank = new List<GravPoint>(list_blank);
		}
	}



	public List<object> getPopPoints()
	{
		List<Point> points = new List<Point>();
		List<float> radii = new List<float>();
		GravPoint g;
		int i;
		float centrad = p_skeleton.cent_radius;
		float splashNum = 20;
		float splashSize = (centrad * 4) / splashNum;
		List<float> v = p_engine.getSpiralPoints(new Point(cent_x, cent_y), splashNum, splashSize);
		int vlength = v.Count;
		float scale;
		for (i = 0; i < vlength; i += 2)
		{
			points.Add(new Point(v[i], v[i + 1]));
			scale = UnityEngine.Random.Range(1f,2f); //between 1 & 2
			radii.Add(splashSize * scale);
		}

		foreach(MembraneNode n in list_nodes) {
			if (i % 2 == 0)
			{
				points.Add(new Point(n.x, n.y));
				scale = UnityEngine.Random.Range(0.5f,1.5f); //between 0.5 & 1.5
				radii.Add(35 * scale); //magic number!
			}
		}
		
		points.Reverse(); //so it will look prettier
		radii.Reverse();
		return new List<object> { points, radii };

	}

	private MembraneNode fetchNodeFromPool()
    {
		for(int i=0; i < _nodePool.Count; i++)
        {
			if (!_nodePool[i].gameObject.activeSelf)
			{
				_nodePool[i].gameObject.SetActive(true);
				return _nodePool[i];
			}
        }

		GameObject n = Instantiate(NodePrefab) as GameObject;
		MembraneNode node = n.GetComponent<MembraneNode>();
		_nodePool.Add(node);
		return node;
    }

	private void makeNodes(float radius, int max)
	{

		List<float> v = FastMath.circlePoints(radius, max);
		int length = v.Count;
		float rot = 0;
		for (int i = 0; i < length; i += 2)
		{
			rot = 90 + ((i / 2) * (360 / max));
			MembraneNode node = fetchNodeFromPool();
			node.p_cent = p_cent;
			node.p_membrane = this;
			
			//MembraneNode fn = node.GetComponent<MembraneNode>();
			SpringJoint2D sj2d = node.GetComponent<SpringJoint2D>();
			//sj2d.connectedBody = Anchor.GetComponent<Rigidbody2D>();
			Rigidbody2D rb = node.GetComponent<Rigidbody2D>();
			node.transform.localPosition = (new Vector3(v[i], v[i + 1]));
			
			//rb.MoveRotation(rot);// = new Vector3(0, 0, rot);
		
			//node.transform.SetParent(this.transform);
			node.init();
			
			//sj2d.autoConfigureDistance = false;
			node.index = list_nodes.Count;//max - 1;
			list_nodes.Add(node);
			if (i > 0)
            {
				node.p_prev = list_nodes[list_nodes.Count-2];
				list_nodes[list_nodes.Count-2].p_next = node;
            }
			if (i == length-2)
            {
				node.p_next = list_nodes[0];
				list_nodes[0].p_prev = node;
            }
			//AddToMembraneSkin(node.gameObject);
			//makeNode(v[i], v[i + 1], rot, i / 2, max - 1); //do max-1 so that it knows that that's the last index in the list
		}
		for (int i = list_nodes.Count-1; i >= 0; i--)
		{
			AddToMembraneSkin(list_nodes[i].gameObject);  //This needs to be done backwards so the sprite shape circle winds correctly. 
		}
		activateNodes();
		
		//list_nodes = new List<MembraneNode>(GetComponentsInChildren<MembraneNode>());
		/*List<float> v = FastMath.circlePoints(radius, max);
		int length = v.Count;
		float rot = 0;
		for (int i = 0; i < length; i += 2) 
		{
			rot = 90 + ((i / 2) * (360 / max));
			makeNode(v[i], v[i + 1], rot, i / 2, max - 1); //do max-1 so that it knows that that's the last index in the list
		}*/

	}

	private void AddToMembraneSkin(GameObject node)
    {
		Skin.AddNode(node);
		
    }

	private MembraneNode createNode(float xx, float yy, float r, int i) 
	{
			MembraneNode n = new MembraneNode();
			n.x = xx;
			n.y = yy;
			//n.rotation = r;
			n.p_cent = p_cent;
			n.p_membrane = this;
			n.index = i;
			n.init();
		
			/*if(shieldCounter >= NODES_PER_SHIELD){
				createShield();
				shieldCounter = 0;
			}
			shieldCounter++;*/
			
			return n;
	}

	private void createShield()
	{
		/*var s:ShieldIcon = new ShieldIcon();
		list_shields.push(s);
		iconSprite.addChild(s);
		trace("Membrane.createShield()! list_shields.length = " + list_shields.length);
		if (shieldsOn) {  
			showShields();					   //match the numbers
			s.scaleX = list_shields[0].scaleX; //match the scale
			s.scaleY = list_shields[1].scaleY;
		}*/
	}

	private void makeNode(float xx, float yy, float r, int i, int max)
	{
		/*Debug.Log("makin node at " + xx + "," + yy);
		MembraneNode n = createNode(xx, yy, r, i);
		list_nodes.Add(n);
		linkNode(i, max);*/
	}

	private void linkNode(int i, int max)
	{
		/*if (i > 0)
		{ //don't link the first one
			list_nodes[i].p_prev = list_nodes[i - 1];
			list_nodes[i - 1].p_next = list_nodes[i];
		}

		if (i == max)
		{//once we arrive at the last one, link the first one
			list_nodes[i].p_next = (list_nodes[0]);
			list_nodes[0].p_prev = (list_nodes[i]);
		}*/
	}

	void OnMouseClick()
	{
		//trace("Membrane : Clicked at" + m.localX + "," + m.localY + "VS" + mouseX + "," + mouseY);
		if (isMouseDown)
		{
			//TODO: this may need to be reworked with the other "click" substitutes for the base classes
			isMouseDown = false;
		}
	}

	private IEnumerator churn()
	{
		while (true)
		{
			
			updateHealth();
			Skin.UpdateVertices();
			//Redraw();
			updateNodes();
			yield return new WaitForEndOfFrame();
		}
	}

	public void takeDamageAt(float xx, float yy, float n)
	{
		base.takeDamage(n);
		p_cell.makeStarburst(xx, yy); onHealthChange();
		//trace("Membrane.takeDamageAt(" + n + ") health=" + health + "/" + maxHealth);
	}

	protected override void onDamageKill()
	{
		//override damagekill for the membrane
		if (!p_cell.isCellDying)
		{
			p_cell.startNecrosis(); //we are TOAST!
		}
	}

	public void onHealthChange()
	{
		p_cell.onMembraneHealthChange((int)health);
	}

	public void updateMaxHealth(int changeNodes = 0, bool fillUp = false)
	{
		int listlength = list_nodes.Count;
		int extra = listlength - STARTING_NODES;

		if (changeNodes > 0)
		{
			if (extra > 0)
			{
				setMaxHealth(100 + (extra * (int)HEALTH_PER_NODE), fillUp);
				if (changeNodes > 0 && changeNodes <= extra)
				{
					giveHealth((uint)(changeNodes * HEALTH_PER_NODE));
				}
			}
		}
		else if (changeNodes < 0)
		{
			if (extra >= 0)
			{
				setMaxHealth(100 + (extra * (int)HEALTH_PER_NODE), false);
			}
		}


		onHealthChange();
		//setMaxHealth(listlength * HEALTH_PER_NODE,true);

	}

	public override void giveHealth(uint amt)
	{
		health += amt;
		if (health > maxHealth)
		{
			health = maxHealth;
		}
	}

	public void updateHealth()
	{
		healthCounter++;
		if (healthCounter > HEALTH_COUNT)
		{
			healthCounter = 0;

			if (health < maxHealth)
			{
				if (p_cell.spendATP(Costs.FIX_MEMBRANE) > 0)
				{
					giveHealth(1);
				}
			}

			float frac = (float)(health) / (float)(maxHealth);
			//Color springColor = FastMath.ConvertFromUint(spring_col);
			health_col = Color.Lerp(Color.red, spring_col ,frac); //the dark blue
			health_col2 = Color.Lerp(FastMath.ConvertFromUint(0xFFCC99), gap_col, frac);  //the light blue 
			onHealthChange();
		}
	}

	public void onStartPPod()
	{
		isPPoding = true;
	}

	public void onFinishPPod()
	{
		isPPoding = false;
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, null, "pseudopod_finish", 1);  //TODO
	}

	private void cancelPseudopod()
	{
		p_skeleton.cancelPseudopod();
	}

	public override void doCellMove(float xx, float yy)
	{
		foreach(MembraneNode m in list_nodes) 
		{
			m.doCellMove(xx, yy);
		}
	}

	private void tryPseudopod(float xx, float yy)
	{
		float dx = cent_x - xx;   //  this distance between the centrosome and the mouse click
		float dy = cent_y - yy;   //
		float d2 = (dx * dx) + (dy * dy);  //the square of that.
		d2 /= (Costs.MOVE_DISTANCE2/100);   //divide the square by the cost / 100.  This part is really just to figure the cost of the move by distance.

		float cost = Costs.PSEUDOPOD[0] * d2;               
		//if (p_engine.canAfford((int)cost, 0, 0, 0, 0))//TODO
		{
			p_skeleton.cancelPseudopod(); //only 1 ppod at once!
			if (p_skeleton.tryPseudopod(xx, yy, cost))         //pass the mouse click coords to p_skeleton.tryPseudopod.
            {
				
				
				
			}
		}
		/*else  //TODO
		{
			p_engine.showImmediateAlert(Messages.A_NO_AFFORD_PPOD);
		}*/  

	}

	public void StretchMembrane(float xx, float yy)
    {
		//Bookmark: the new way of moving the membrane
		Mover.Stretch(xx, yy);
	}

	/**
		 * Give me your position, I'll return the node where (mnode.pos+mnode.p_next.pos)/2 is closest to it
		 * @param	xx
		 * @param	yy
		 * @return
		 */

	public MembraneNode findClosestMembraneHalf(float xx, float yy) 
	{
			MembraneNode bestM = null;
			float bestD2 = 10000000000000000;
			int length = list_nodes.Count;
			for (int i = 0; i<length; i++) {
				
				float xpos = (list_nodes[i].x + list_nodes[i].p_next.x) / 2;
				float ypos = (list_nodes[i].y + list_nodes[i].p_next.y) / 2;
				
				float xdist = (xpos - xx);
				float ydist = (ypos - yy);
				
				float dist2 = (xdist* xdist) + (ydist* ydist);
				
				if (dist2<bestD2) {
					bestD2 = dist2;
					bestM = list_nodes[i];
				}
			}
			return bestM;
		}
		
		/**
		 * Give me your position, I'll return the closest node to it
		 * @param	xx
		 * @param	yy
		 * @return
		 */
		
		public MembraneNode findClosestMembraneNode(float xx, float yy)
		{
			MembraneNode bestM = null;
			float bestD2 = 10000000000000000;
			int length = list_nodes.Count;
			for (int i = 0; i < length; i++) 
			{
				//var xdist:Number = (list_nodes[i].x+cent_x - xx);
				float xdist = (list_nodes[i].x - xx);
				//var ydist:Number = (list_nodes[i].y+cent_y - yy);
				float ydist = (list_nodes[i].y - yy);
				float dist2 = (xdist * xdist) + (ydist * ydist);
				if (dist2 < bestD2)
				{
					bestD2 = dist2;
					bestM = list_nodes[i];
				}
			}
			return bestM;
		}

		private bool quickCircSeg(Vector2 c, float r, Vector2 p1, Vector2 p2, MembraneNode m)
		{
			
			Vector2 dir = p2 - p1;
			Vector2 diff = c - p1;
			float frac = Vector2.Dot(diff, dir);// diff.dotOf(dir);
		float mag = Vector2.Dot(dir, dir);// dir.dotOf(dir);
			float t = frac / mag;
			if (t< 0) t = 0;
			if (t > 1) t = 1;
			Vector2 pushBit = dir * t;
			Vector2 closest = p1 + pushBit;
			Vector2 d = c - closest;
		float distsqr = Vector2.Dot(d, d);//.dotOf(d);
			
			if (distsqr<(r* r)) {
				return true;
			}
			return false;
		}
		
	private float quickCircSeg2(Vector2 c, float r, Vector2 p1, Vector2 p2, MembraneNode m)
	{

		Vector2 dir = p2 - p1;
		Vector2 diff = c - p1;
		float frac = Vector2.Dot(diff, dir);// diff.dotOf(dir);
		float mag = Vector2.Dot(dir, dir);// dir.dotOf(dir);
		float t = frac / mag;
		if (t < 0) t = 0;
		if (t > 1) t = 1;
		Vector2 pushBit = dir * t;
		Vector2 closest = p1 + pushBit;
		Vector2 d = c - closest;

		penetrate_vector = new Vector2(d.x, d.y); // d.Copy();
		penetrate_vector *= (0.5f);
		penetrate_unit_vector = penetrate_vector.normalized;
		//penetrate_vector.normalize();
		float distsqr = Vector2.Dot(d, d);// d.dotOf(d);
		float pen2 = (distsqr - (r * r));
		/*if (debug) {
			trace("Membrane.quickCircSeg2() penetrate_vector = " + penetrate_vector);
		}*/
		return pen2;
	}

	private bool intersectCircleSegment(CellObject obj, Vector2 c, float r, Vector2 p1, Vector2 p2, MembraneNode m) 
	{
		Vector2 dir = p2 - p1;
		Vector2 diff = c - p1;
		float frac = Vector2.Dot(diff, dir);// diff.dotOf(dir);
		float mag = Vector2.Dot(dir, dir);// dir.dotOf(dir);
		float t = frac / mag;
		if (t< 0) t = 0;
			if (t > 1) t = 1;
		Vector2 pushBit = dir * t;
		Vector2 closest = p1 + pushBit;
		Vector2 d = c - closest;
		float distsqr = Vector2.Dot(d, d);// d.dotOf(d);
		Debug.Log("intersectcs " + r + "," + distsqr);
		if (distsqr<(r* r)) {
			float penetrate = r - d.magnitude;//  d.length;
			//penetrate /= 100f; //TODO: I added this as a damper for Unity's coordinate system. Make sure it works okay.
				Vector2 unitd = d.normalized;
				Vector2 pushd = unitd *= (penetrate);

			//Bookmark: this used to move the cell membrane nodes to make the pseudopod 
			Debug.Log("push direction for membrane: " + -pushd);

			/*m.push(-pushd.x, -pushd.y);     //Old Way
			m.p_next.push(-pushd.x, -pushd.y);
			*/
			
			

			if (obj) {
					if (!(obj is Microtubule) && !(obj is Centrosome) && !(obj is Nucleus)) { //if its an object
						obj.push(pushd.x, pushd.y);	
						obj.cancelMove();
					}
				}
				return true;
			}
			return false;
	}

	private void moverFinishedStretching()
    {

    }

	/**
		 * This function keeps everything happy. It is big, it is ugly, but for the most part, it works.
		 * @param	i
		 */

	public void collisionTest(int i)
	{
		MembraneNode m1 = list_nodes[i];
		MembraneNode m2 = list_nodes[i].p_next;
		Vector2 p1 = new Vector2(m1.x, m1.y);
		Vector2 p2 = new Vector2(m2.x, m2.y);

		bool yes = false;
		float dist;
		float dist2;

		//First, check the canvas neighbors

		List<GameDataObject> neighbors = p_cgrid.getNeighbors((int)m1.grid_x, (int)m1.grid_y);
			foreach(GameDataObject gdo in neighbors) {
			CanvasObject cv = (gdo.ptr as CanvasObject);
			if (cv != null)
			{
				if (cv is GoodieGem)
				{
					dist2 = quickCircSeg2(new Vector2(cv.x, cv.y), cv.getRadius(), p1, p2, m1);
					if (dist2 < 0)
					{
						(cv as GoodieGem).onTouchCell2(dist2, penetrate_unit_vector);
					}
				}
				else if (quickCircSeg(new Vector2(cv.x, cv.y), cv.getRadius(), p1, p2, m1))
				{
					if (cv is CanvasWrapperObject)
					{
						/*if (CanvasWrapperObject(cv).c_cellObj is BigVesicle)  //TODO
						{
							mergeVesicle(CanvasWrapperObject(cv), m1, m2);
						}*/ 
					}
					cv.onTouchCell();
				}
			}
		}

		//Next, test the cell neighbors

		List<GameDataObject> vNeighbors = p_grid.getNeighbors((int)m1.grid_x, (int)m1.grid_y);
		foreach(GameDataObject gdo in vNeighbors)
		{
			ICellGameObject go = (ICellGameObject)(gdo.ptr);
			CellObject c;
			if (go != null)
			{
				if (go is Virus)
				{
					Virus v = (Virus)go;

					//Check to see if the virus is INSIDE the cell and NOT trying to escape
					//If so, we need to make its collision behaving!
					if (v.position_state == Virus.POS_INSIDE_CELL && v.motivation_state != Virus.MOT_ESCAPING_CELL)
					{
						//THIS IS AN UGLY HACK BUT IT MAKES THINGS WORK:
						dist2 = quickCircSeg2(new Vector2(v.x, v.y), v.getRadius(), p1, p2, m1);
						if (dist2 < 0)
						{

							float ddx = v.x - cent_x;
							float ddy = v.y - cent_y;

							float dmx = (m1.x + m2.x) / 2 - cent_x;
							float dmy = (m1.y + m2.y) / 2 - cent_y;

							float dd2 = ddx * ddx + ddy * ddy;
							float dm2 = dmx * dmx + dmy * dmy;

							//CHECK TO SEE IF I'M ACTUALLY INSIDE MEMBRANE. 
							//If so, push to keep me there.
							//If not, let me in!!!!
							//Bookmark
							if (dd2 < dm2)
							{
								v.push(penetrate_vector.x, penetrate_vector.y); //push away from membrane
								if (m1.state_ppod)
								{               //push away from pseudopoding membrane
									v.push(m1.xdist, m1.ydist);
								}
								if (m2.state_ppod)
								{
									v.push(m2.xdist, m1.ydist);
								}
							}
						}
					}
					else if (v.position_state != Virus.POS_INSIDE_CELL)
					{
						if (quickCircSeg(new Vector2(v.x, v.y), v.getRadius(), p1, p2, m1))
						{
							//This is the normal case, if it's not inside & not escaping, check to see if it's touching the cell membrane
							v.onTouchCell();
						}
					}
				}
				else if (go is HardPoint)
				{
					c = (CellObject)(go);
					if (intersectCircleSegment(null, new Vector2(c.x, c.y), c.getRadius(), p1, p2, m1))
					{
						yes = true;
					}
				}
				else if (go is CellObject)
				{
					c = (CellObject)(go);
					bool doTest = false;
					if (c is BasicUnit)
					{
						if (c.doesCollide && c.might_collide)
						{
							doTest = true;
						}
					}
					else if (c.doesCollide)
					{
						doTest = true;
					}

					if (doTest)
					{
						bool showLyso = false;
						if (c is Lysosome)
						{
							showLyso = true;
						}
						dist2 = quickCircSeg2(new Vector2(c.x, c.y), c.getRadius(), p1, p2, m1);
						if (dist2 < 0)
						{
							if (c.isMoving)
							{
								c.cancelMove();
							}
							c.push(penetrate_vector.x, penetrate_vector.y);
							if (m1.state_ppod)
							{
								c.push(m1.xdist, m1.ydist);
							}
							if (m2.state_ppod)
							{
								c.push(m2.xdist, m1.ydist);
							}
						}
					}
				}
			}
		}

		/*foreach(GravPoint gg in list_grav_blank) 
		{
			//Debug.Log("grav");
			CellObject c;
			if (gg != null)
			{
				c = gg.p_obj;
				
				if (intersectCircleSegment(c, new Vector2(gg.x, gg.y), gg.radius, p1, p2, m1))
				{
					m1.state_ppod = true; //if I'm touching a ppod ball, I'm ppoding!
					m1.p_prev.state_ppod = true;
					m1.p_next.state_ppod = true;
					yes = true;
				}
				else
				{
					m1.state_ppod = false;
				}
			}
	}*/

		if (yes)
		{
			m1.has_collided = true;
		}
		else
		{
			m1.has_collided = false;
		}
	}

	public void mergeVesicle(CanvasWrapperObject cw, MembraneNode m1, MembraneNode m2)
	{
		string content = cw.content;
		float xx = cw.x;
		float yy = cw.y;
		float rad = cw.getRadius();
		MembraneNode m = findClosestMembraneHalf(xx, yy);
		Point p = new Point((m.x + m.p_next.x) / 2, (m.y + m.p_next.y) / 2);
		Vector2 v = new Vector2(xx - p.x, yy - p.y); //vector from vesicle center to node

		//since the vesicle is TOUCHING the cell, then the magnitude of v is ALWAYS very close to the radius of the vesicle



		//multiplying the vector by 2 will get the diameter
		
		Vector2 vCent = new Vector2(cw.x - cent_x, cw.y - cent_y); //vector from vesicle center to the centrosome
		vCent.Normalize();              //unit vector in the direction of the centrosome
		cancelPseudopod();
		/*if (m1.state_ppod || m2.state_ppod) {
			vCent.multiply(-rad*5)
		}else{*/
		vCent *= (-rad * 3);           //shove it towards the centrosome
											//}
											//throw new Error("Let's see!");
		p_cell.makeVesicleContent(content, cw.x + vCent.x, cw.y + vCent.y); //make the thing, shoved slightly towards the centrosome

		//p_cell.killCanvasObject(cw);
		//removeMembraneNodes(2);
	}

	public void updatePH(float ph)
	{
		//ph_balance = ph;
		//updateColors();
	}

	private void updateNodes()
	{
		int i = 0;

		//MembraneNode.tug_x = 0;
		//MembraneNode.tug_y = 0;

		int length = list_nodes.Count;
		float avg_stretch = 0;
		for (i = 0; i < length; i++)
		{
			
			if (skeletonReady)
			{
				
				collisionTest(i);
				avg_stretch += list_nodes[i].stretch;
			}
		}
		AVG_STRETCH = avg_stretch / length;

		//drawAllNodes(true);

		//p_skeleton.x += MembraneNode.tug_x;//TODO: figure out how the skeleton needs to move in this new reality, with no tug value
		//p_skeleton.y += MembraneNode.tug_y;//
	}

	public void drawAllNodes(bool doTug = false)
	{
		
		
	}

	public override void updateBubbleZoom(float n)
	{
		base.updateBubbleZoom(n);

		if (SPRING_THICK_ * n < SPRING_MIN)
			SPRING_THICK = SPRING_MIN / n;
		else
			SPRING_THICK = SPRING_THICK_;


		if (GAP_THICK_ * n < GAP_MIN)
			GAP_THICK = GAP_MIN / n;
		else
			GAP_THICK = GAP_THICK_;

		if (OUTLINE_THICK_ * n < OUTLINE_MIN)
			OUTLINE_THICK = OUTLINE_MIN / n;
		else
			OUTLINE_THICK = OUTLINE_THICK_;

	}

	public void drawMembrane(MembraneNode m)
	{
		
		

		
		

	}

	public void drawCentralSprings(MembraneNode m)
	{
		
	}

	public override void setEngine(Engine e)
	{
		p_engine = e;
	}

	public void clearMouse()
	{
		if (isMouseDown)
		{
			isMouseDown = false;
			hideCursor();
			hidePPodCursor();
			StopCoroutine(_doMouseMoveRoutine);
			//removeEventListener(RunFrameEvent.RUNFRAME, doMouseMove);
		}
	}

	private void doMouseUp()
	{
		Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
		if (isMouseDown)
		{
			float xd = mouse.x - mouseDown_x;
			float yd = mouse.y - mouseDown_y;
			d2_mouse = (xd * xd) + (yd * yd);
			if (d2_mouse > D2_PPOD)
			{
				Debug.Log("trying pseudopod");
				tryPseudopod(mouse.x, mouse.y);
				hidePPodCursor();
			}
		}
		isMouseDown = false;
		hideCursor(); //go back from the bleb cursor to nothing
		StopCoroutine(_doMouseMoveRoutine);
	}

	private void hideTargetter()
	{
		targetter.gameObject.SetActive(false);
	}

	public void onCellMove(float xx, float yy)
	{
		//this.transform.localPosition -= new Vector3(xx, yy, 0);
		targetter.transform.localPosition = new Vector3(targetter.transform.localPosition.x - xx,targetter.transform.localPosition.y - yy, targetter.transform.position.z);
		mouseDown_x -= xx;
		mouseDown_y -= yy;
	}

	private void showTargetter()
	{


		List <MembraneNode> m = getClosestNodes(mouseDown_x, mouseDown_y, D2_PPOD);
		if (m != null)
		{
			//BUG ALERT
			//BUG ALERT: This assumes that the membrane is at 0,0 without scale!
			worldScale = 1;//p_engine.getWorldScale(); //TODO
			targetter.transform.localScale = new Vector3(1 / worldScale, 1 / worldScale, 1); //There's going to be bugs unless you update this whenever you change scale!

			targetter.gameObject.SetActive(true);
			targetter.transform.localPosition = new Vector3(mouseDown_x, mouseDown_y, targetter.transform.position.z);
			if (p_engine != null)
				p_engine.setCursorArrowPoint(mouseDown_x, mouseDown_y);
			if (m.Count > 1)
			{
				MembraneNode mm = m[m.Count-1];
				m.RemoveAt(m.Count - 1);
				if (p_engine != null)
					p_engine.setCursorArrowRotation(mm.transform.eulerAngles.z);
			}
		}
	}


	private new void OnMouseDown()
	{
		base.OnMouseDown();
		Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
		isMouseDown = true;
		mouseDown_x = mouse.x;
		mouseDown_y = mouse.y;
		//p_cell.dispatchEvent(m);
		Debug.Log("mouse down");
		_doMouseMoveRoutine = StartCoroutine(doMouseMove());
		
		//m.stopPropagation(); //keep it from going to the cell

		//p_engine.dispatchEvent(m); //TODO  //send it directly to the engine
								   //m.stopImmediatePropagation();
								   //trace("Membrane.doMouseDown()! isMouseDown = " + isMouseDown);
	}

	private IEnumerator doMouseMove()
	{
		while (true)
		{
			//Debug.Log("doMouseMove " + isMouseDown);
			yield return new WaitForEndOfFrame();
			if (isMouseDown)
			{
				/*if (Director.IS_MOUSE_DOWN == false)
				{ //CHEAP HACK : "Release Outside" event - just check every frame against global
					doMouseUp();
				}
				else*/ //TODO: uncomment when this is better understood. 
				{
					Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,0));
					
					float xd  = mouse.x - mouseDown_x;
					float yd = mouse.y - mouseDown_y;
					d2_mouse = (xd * xd) + (yd * yd);
					
					if (d2_mouse >= D2_PPOD)
					{
						showPPodCursor();
						
					}
					else
					{
						hidePPodCursor();
						
					}
				}
			}
		}
	}

	private void showPPodCursor()
	{
		if (!isPPodCursor)
		{
			if (p_engine != null)
				p_engine.showCursor((int)CellAction.PSEUDOPOD);
			isPPodCursor = true;
			showTargetter();
		}
	}

	private void hidePPodCursor()
	{
		if (isPPodCursor)
		{
			if (p_engine != null)
			{
				p_engine.endCursorArrow();
				p_engine.lastCursor();
			}
			isPPodCursor = false;
			hideTargetter();
		}
	}

    private void OnMouseUp()
    {
		doMouseUp();
    }

    void OnMouseOver()
	{
		if (!isMouseDown)
		{
			Debug.Log("over");
			showCursor();
		}
		else
        {
			//tryPseudopod(1, 1);
        }
	}

	private void OnMouseExit()
	{
		if (!isMouseDown)
		{
			Debug.Log("exit");
			hideCursor();
		}
		
	}

	private void showCursor()
	{
		if (!isMouseOver)
		{
			if (p_engine != null)
				p_engine.showCursor((int)CellAction.PSEUDOPOD_PREPARE);
			isMouseOver = true;
		}
	}

	private void hideCursor()
	{
		if (isMouseOver)
		{
			if (p_engine != null)
				p_engine.lastCursor();
			isMouseOver = false;
		}
	}

	public void Redraw()
	{
		 
		shape_cyto.Begin();
		m_rim.Begin();
		
		shape_cyto.MoveTo(list_nodes[0].transform.localPosition.x, list_nodes[0].transform.localPosition.y);
		m_rim.MoveTo(list_nodes[0].transform.localPosition.x, list_nodes[0].transform.localPosition.y);
		int i;
		Vector3 sum = new Vector3();
		float maxY = 0;
		float minY = 0;
		float maxX = 0;
		float minX = 0;
		for (i = 0; i < list_nodes.Count - 1; i++)
		{
			Vector2 p0 = list_nodes[i].transform.localPosition;
			Vector2 p3 = list_nodes[i + 1].transform.localPosition;
			Vector2 distToNext = (p3 - p0);
			Vector2 distFromNext = (p0 - p3);
			//distToNext = FastMath.rotateVector(LerpVal * Mathf.PI / 180, distToNext);
			//distFromNext = FastMath.rotateVector(LerpVal * Mathf.PI / 180, distFromNext);

			m_rim.BezierCurveTo(p0.x, p0.y, p0.x + distToNext.x, p0.y + distToNext.y, p3.x, p3.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
			shape_cyto.LineTo(p0.x, p0.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);
			shape_cyto.LineTo(p3.x, p3.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);
										   //m_Cytoplasm.LineTo(dist.x, dist.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);

		}
		Vector2 p0l = list_nodes[i].transform.localPosition;
		Vector2 p3l = list_nodes[0].transform.localPosition;
		Vector2 distToNextl = (p3l - p0l) / 2;
		Vector2 distFromNextl = (p0l - p3l) / 2;

		m_rim.BezierCurveTo(p0l.x + distToNextl.x, p0l.y + distToNextl.y, p3l.x + distFromNextl.x, p3l.y + distFromNextl.y, p3l.x, p3l.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
		shape_cyto.LineTo(p3l.x, p3l.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);

		m_rim.Stroke();

		shape_cyto.Fill();
		m_rim.End();
		shape_cyto.End();
		

	}



























}

