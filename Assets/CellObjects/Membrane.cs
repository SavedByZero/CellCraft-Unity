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
		private List<MembraneNode> list_nodes;
		private List<GravPoint> list_grav;
		private List<BasicUnit> list_basicUnits;
		private List<Virus> list_viruses;
		private List<GravPoint> list_grav_blank;
		

		
		private Centrosome p_cent;
		private Cytoskeleton p_skeleton;
		public bool skeletonReady = false;
		
		public GameObject membraneSprite;
		public GameObject iconSprite;
		
		public Shape shape_spring;  //was: shape
	//Scene gg = new Scene();
		public Shape shape_gap;
		public Shape shape_cyto;
		public Shape shape_outline;
		
		public Targetter targetter;
		
		public Shape shape_debug;
		
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
		
		
		
		public const float SPRING_THICK_ = 35;
		public const float GAP_THICK_ = 15;
		public const float OUTLINE_THICK_ = 40;
		
		public static float SPRING_THICK = SPRING_THICK_;
		public static float GAP_THICK = GAP_THICK_;
		public static float OUTLINE_THICK = OUTLINE_THICK_;
		
		private const float SPRING_MIN = 13;
		private const float GAP_MIN = 4;
		private const float OUTLINE_MIN = 15;
		
		private const float HEALTH_PER_NODE = 10;

		private int HEALTH_COUNT = 60; //+1 health every 2 seconds
		private int healthCounter = 60;//HEALTH_COUNT;
		
		private float D_NODECLICK = 50;
		private float D2_NODECLICK = 50 * 50;//D_NODECLICK * D_NODECLICK;
		private float D_MNODECLICK = 50 * .75f;
		private float D2_MNODECLICK = (50 * .75f) * (50*.75f);//D_MNODECLICK* D_MNODECLICK;

		private float D_BASIC_UNIT_COLLIDE = 70;
	private float D2_BASIC_UNIT_COLLIDE = 70 * 70;//D_BASIC_UNIT_COLLIDE* D_BASIC_UNIT_COLLIDE;

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
		private const float D2_PPOD = 100 * 100; //100 pixels
		public const float PPOD_ANGLE = 90; //+- 30 degrees
		
		private float worldScale = 1;
		
		//private var p_engine:Engine;
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
		
		private uint cyto_col = 0x44AAFF;
		private uint spring_col = 0x0066FF;
		private uint gap_col = 0x99CCFF;
		
		private uint health_col;
		private uint health_col2;
		
		public const float STARTING_RADIUS = 400;
		public const int STARTING_NODES = 15;
		public const int MAX_NODES = 25;
		
		public static int CURR_NODES = 15;
		
		private ObjectGrid p_cgrid;
		//private var p_virusGrid:ObjectGrid;
		
		private float D2_NODERADIUS = 10;
		
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
		
		public bool acceptingVesicles = true;
		public Membrane()
		{
		
		}

}

