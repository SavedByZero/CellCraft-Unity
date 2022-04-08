using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class Cell : CellGameObject
{
	//values:

	private float volume;
		
	private float gravRadius = 150;
	private float gravRadius2 = 150 * 150;
	//resources:
		
	//THESE ARE NOT FOR REAL USE! THESE ARE JUST HANDY READ REFERENCES FOR SPEED!
	public float r_atp = 0;
	public float r_na = 0;
	public float r_aa = 0;
	public float r_fa = 0;
	public float r_g = 0;

	public float r_max_atp = 1;
	public float r_max_na = 1;
	public float r_max_aa = 1;
	public float r_max_fa = 1;
	public float r_max_g = 1;
		
	public float toxin_level = 0;
		
	private float ph_balance = 7.5f;
	private float cyto_volume = 10;
		
	//pointers:
	private Director p_director;

	private WorldCanvas p_canvas;

	//private Interface p_interface;  //TODO
	private WizardOfOz p_woz;
		
	//children:
	public Nucleus c_nucleus;
	public Centrosome c_centrosome;
	public ER c_er;
	public Membrane c_membrane;
	public Golgi c_golgi;
	public Cytoskeleton c_skeleton;
		
	public static int LYSOSOME_STARTED = 0;
	public static int LYSOSOME_FINISHED = 0;
	public static int LYSO_RNA_STARTED = 0;
	public static int LYSO_RNA_FINISHED = 0;
	public static int LYSO_RIB_STARTED = 0;
	public static int LYSO_RIB_FINISHED = 0;
	public static int LYSO_PRO_STARTED = 0;
	public static int LYSO_PRO_FINISHED = 0;
	public static int LYSO_VES_STARTED = 0;
	public static int LYSO_VES_FINISHED = 0;
	public static int LYSO_GOL_STARTED = 0;
	public static int LYSO_GOL_FINISHED = 0;
		
	public static bool RADICALS_ON = false;
		
	private bool spawning_engine_radicals = false;
	private int spawn_engine_radical_count = 0;
	private string spawn_engine_radical_origin;
	private string spawn_engine_radical_target;
	private int spawn_engine_radical_counter = 0;
	private int SPAWN_ENGINE_RADICAL_TIME = 30;

//consts:

	public const float DEFENSIN_AMOUNT = 1; //1 defensin per vesicle
	public const float TOXIN_AMOUNT = 2;
	//public const float ATP_CONCENTRATION = 0.000202636; //maximum amount of resource per square pixel
	public const float NA_CONCENTRATION = 0.000020264f;
	public const float AA_CONCENTRATION = 0.000050659f;
	public const float FA_CONCENTRATION = 0.0000101318f; //maximum of about 50 g
	public const float G_CONCENTRATION =  0.0000050659f; //maximum of about 25 G
		
	public const int MAX_NA = 100;
public const int MAX_AA = 200 * 10;// Costs.AAX;
	public const int MAX_FA = 100;
	public const int MAX_G = 50;
		
	public const float MAX_ACID_DAMAGE = 15; //maximum X damage per second at 0.0 pH

	//lists:
	public GameObject MRNA_Prefab;
	public GameObject RedRNA_Prefab;
	public GameObject EnzymeRNA_Prefab;
	public GameObject EvilDNA_Prefab;
	public GameObject DNARepairEnzyme_Prefab;
	public GameObject Lysosome_Prefab;
	public GameObject Ribosome_Prefab;
	public GameObject Peroxisome_Prefab;
	private List<RNA> list_rna;
	private List<Ribosome> list_ribo;
	private List<Lysosome> list_lyso;

	private List<Peroxisome> list_perox;
	private List<SlicerEnzyme> list_slicer;
	private List<DNARepairEnzyme> list_dnarepair;
	private List<FreeRadical> list_radical;
		
	private List<Virus> list_virus;
		
		
	private List<CellObject> list_junk; //protein globs, etc
		
	private List<Mitochondrion> list_mito;
	private List<Chloroplast> list_chlor;
		
	private List<BlankVesicle> list_ves;
	private List<BigVesicle> list_bigves;
		
	private List<HardPoint> list_hardpoint;
	//private var list_escortpoint:List<HardPoint>;
		
	private List<CellObject> list_running; //objects that need to run()
	private List<CellObject> list_selectable; //objects that can be selected

	public ObjectGrid c_objectGrid;
	public static bool SHOW_GRID = true;
		
	private bool dirty_selectList = false;
	private bool dirty_units = false;
		
	private const int GRID_W = 10;
	private const int GRID_H = 10;
		
	private int clearCount = 0;
	private int CLEAR_TIME = 30;
		
	private int PRODUCE_TIME = 60;
	private int produceCount = 0;
		
	private int NECROSIS_TIME = 120;
	private int NECROSIS_DMG = 6;
	private int necrosisCount = 0;
		
	public bool isCellDying = false;
		
	private int mitoCount = 0; //how many mitos do we have?
	private int chloroCount = 0; //how many mitos do we have?
	private bool canCytoProcess = true;
	public bool canFatProcess = true; //please don't change this externally, just use this for easy access
		
	public const int MAX_MITO = 10;
	public const int MAX_CHLORO = 10;
	public const int MAX_LYSO = 25;
	public const int MAX_PEROX = 10;
	public const int MAX_RIBO = 25;
	public const int MAX_SLICER = 50;
	public const int MAX_DNAREPAIR = 25;
	public const int MAX_DEFENSIN = 50;
	public const int MAX_TOXIN = 100;
	//public static const MAX_
		
	//Include the fastmath local function library
		
	private float lvl_sunlight = 1;
	private float lvl_toxins = 0;

	private Coroutine _runRoutine;
	private Coroutine _produceTickRoutine;
    private Coroutine _engineSpawnRadicalRoutine;
    private Coroutine _doNecrosisRoutine;

    public Cell()
	{
		//animateOff();
	}

    private void Start()
    {
		init(); //TODO: this is a placeholder to test the init method.
    }

    public void init()
	{
		setCentLoc(0, 0);
		makeObjectGrid();
		setChildren();
		makeLists();
		_runRoutine = StartCoroutine(run());

		
	}

	public override void destruct()
	{
		p_director = null;
		p_engine = null;
		//p_world = null;  //TODO
		p_canvas = null;
		//p_interface = null;  //TODO
		
		clearVector(list_ribo.Cast<MonoBehaviour>().ToList());
		clearVector(list_lyso.Cast<MonoBehaviour>().ToList());
		clearVector(list_perox.Cast<MonoBehaviour>().ToList());
		clearVector(list_slicer.Cast<MonoBehaviour>().ToList());
		clearVector(list_dnarepair.Cast<MonoBehaviour>().ToList());
		clearVector(list_mito.Cast<MonoBehaviour>().ToList());
		clearVector(list_chlor.Cast<MonoBehaviour>().ToList());
		clearVector(list_bigves.Cast<MonoBehaviour>().ToList());
		clearVector(list_ves.Cast<MonoBehaviour>().ToList());
		clearVector(list_rna.Cast<MonoBehaviour>().ToList());
		clearVector(list_hardpoint.Cast<MonoBehaviour>().ToList());
		clearVector(list_running.Cast<MonoBehaviour>().ToList());
		clearVector(list_selectable.Cast<MonoBehaviour>().ToList());
		list_ribo = null;
		list_lyso = null;
		list_perox = null;
		list_slicer = null;
		list_dnarepair = null;
		list_mito = null;
		list_chlor = null;
		list_bigves = null;
		list_ves = null;
		list_rna = null;
		list_hardpoint = null;
		list_running = null;
		list_selectable = null;
		base.destruct();
	}

	private void clearVector(List<MonoBehaviour> v)
	{
		foreach(MonoBehaviour g in v)
		{
			if (g.transform.parent == this.transform)
			{ //make sure that it exists and is my child

				GameObject.Destroy(g.gameObject);  //TODO: pool this eventually.  
			}
			//g = null; //TODO?
		}
	}

	public void OnMouseDown()
	{
		Debug.Log("mouse down");
		//p_engine.cellMouseDown();  //TODO

	}

	public void onZoomChange(float n)
	{
		foreach(CellGameObject g in list_running) {
			g.updateBubbleZoom(n);
		}
	}

	public void fauxRun()
	{
		c_membrane.drawAllNodes(); //update the membrane
		foreach(BigVesicle b in list_bigves) {
			b.updateBigVesicle();
		}
		foreach(Virus v in list_virus) {
			if (v.hasVesicle)
			{
				v.drawVesicle();
				//v.updateVesicle(null);
			}
		}
	}

	public IEnumerator run()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			int i = 0;
			foreach(CellGameObject g in list_running) {
				if (g != null)
				{
					g.RemoteRun();
					//figure out a way to call doAnim, doMoveToPoint
					
				}
				else
				{ //if we come across a null object, remove it from the list
				  //trace("spliced null object g=" + g + "from list_running");


					list_running.RemoveAt(i);  //TODO: I don't know if this will work with forward iteration 
				}
				i++;
			}
			//checkSituations();
			//produceTick();
			clearTick();
			flush();
		}
	}

	private IEnumerator produceTick()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			produceCount++;
			if (produceCount > PRODUCE_TIME)
			{
				produceCount = 0;
				if (mitoCount == 0)
				{  //anaerobic digestion
					if (spend(new float[] { 0, 0, 0, 0, 1 }, new Point(c_centrosome.x + 100, c_centrosome.y))) {
						produce(new float[] { 2, 0, 0, 0, 0 }, 1, new Point(c_centrosome.x + 100, c_centrosome.y));
					}
				}
				if (canFatProcess)
				{
					tryGtoFA();
				}
				else
				{
					burnExcessG();
				}
			}
		}
	}

	public void setResources(int atp, int na, int aa, int fa, int g, int max_atp, int max_na, int max_aa, int max_fa, int max_g)
	{
		r_max_atp = max_atp;
		r_max_na = max_na;
		r_max_aa = max_aa;
		r_max_fa = max_fa;
		r_max_g = max_g;
		r_atp = atp;
		r_aa = aa;
		r_na = na;
		r_g = g;
		r_fa = fa;
	}



	private void burnExcessG()
	{
		int diff = (int)(r_g - r_max_g);
		if (diff > 0)
		{
			spend(new float[] { 0, 0, 0, 0, 4 }, new Point(c_centrosome.x + 100, c_centrosome.y));
		}
	}

	private void tryGtoFA()
	{
		//var a:Array = p_engine.getResources();
		//trace("Cell.tryGtoFA() a = " + a + " r_max_g = " + r_max_g);
		if (r_g - r_max_g >= 4)
		{
			if (spend(new float[] { 0, 0, 0, 0, 4 }, new Point(c_centrosome.x + 100, c_centrosome.y))){
				produce(new float[] { 0, 0, 0, 1, 0 }, 1, new Point(c_centrosome.x + 100, c_centrosome.y));
			}
		}
	}

	private void clearTick()
	{
		clearCount++;
		if (clearCount > CLEAR_TIME)
		{
			clearCount = 0;
			c_objectGrid.clearGrid();
			c_membrane.updateGrid();
			updateGridThings();
			//c_cell.updateGrid();
		}
	}

	private void updateGridThings()
	{
		foreach(Mitochondrion m in list_mito) {
			if (!m.dying)
			{
				m.placeInGrid();
			}
		}
		foreach(Chloroplast c in list_chlor) {
			if (!c.dying)
			{
				c.placeInGrid();
			}
		}
		foreach(HardPoint h in list_hardpoint) {
			h.placeInGrid();
		}
		foreach(Peroxisome p in list_perox) {
			p.placeInGrid();
		}
		foreach(Lysosome l in list_lyso) {
			//if(!l.dying){
			l.placeInGrid();
			//}
		}
	}

	public void setSunlight(float n)
	{
		lvl_sunlight = n;
		foreach(Chloroplast c in list_chlor) {
			c.setSunlight(lvl_sunlight);
		}
	}

	public void setToxinLevel(float n)
	{
		lvl_toxins = n;
	}

	public void setFatProcess(bool yes)
	{
		canFatProcess = yes;
	}

	public void setCytoProcess(bool yes)
	{
		canCytoProcess = yes;
		if (yes)
		{
			_produceTickRoutine = StartCoroutine(produceTick());
			
		}
		else
		{
			StopCoroutine(_produceTickRoutine);
		}
	}

	public void flush()
	{
		if (dirty_selectList)
		{
			//trace("p_world = " + p_world);
			list_selectable.Sort(); // .sort(sortOnNumID); //sort it so the   //TODO: investigate this sorting request 
			bool hasMulti = false;
			foreach(Selectable s in list_selectable) {    //look for multiselectable units
				if (s.getCanSelect() && s.getSingleSelect() == false)
				{
					hasMulti = true;
					//trace("Cell.flush() : found multiselect unit: " + s);
					break;
				}
			}
			//TODO all below
			/*p_engine.setMultiSelectMode(hasMulti); //notify the engine so we can disable the multi-selecter if need be
			if (p_world)
			{
				p_world.updateSelectList();
			}*/  
			dirty_selectList = false;
		}
		if (dirty_units)
		{
			dirty_units = false;
			onPHChange(); //update new units
			onUnitChange();
		}
	}

	public void setChildren()
	{
		//try{
		c_centrosome.setCell(this);
		c_er.setCell(this);
		c_golgi.setCell(this);
		c_nucleus.setCell(this);

		setupMembrane();
		setupSkeleton();


	}

	private void setupMembrane()
	{
		c_membrane.setCell(this);
		c_membrane.setCent(c_centrosome);
		c_membrane.setSkeleton(c_skeleton);
		c_membrane.setObjectGrid(c_objectGrid);
		c_membrane.init();
		//c_membrane.setCanvasGrid(p_canvas.c_cgrid);  //TODO
		
		
		
		//p_engine.updateMaxResources();  //TODO
	}

	public void buyToxin()
	{
		float[] cost = Costs.getMAKE_TOXIN(1);
		spend(cost);
	//	p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_toxin", 1);   //TODO

		generateToxin((int)cost[1]);
	}

	public void buyDefensin()
	{
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_buy_defensin", Director.level, 1); }
		float[] cost = Costs.getMAKE_DEFENSIN(1);
		spend(cost);
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_defensin", 1);  //TODO
		generateDefensin((int)cost[1]);
	}

	public int sellDefensin() 
	{
		//cheap hack that finds the membrane in the lower right quadrant, where defensin vesicles
		//usually go to merge
		if(c_membrane.getDefensins() > 0){
			MembraneNode mnode = c_membrane.findClosestMembraneNode(c_golgi.x, c_golgi.y);
			Point p = new Point(mnode.x, mnode.y);
//end cheap hack
		int atpCost = (int)Costs.SELL_DEFENSIN[0];
			if (spendATP(atpCost, p) > 0) {
				c_membrane.removeDefensin(1);
				SfxManager.Play(SFX.SFXPop2);
				refundDefensin(p);
				//if(Director.STATS_ON){Log.LevelAverageMetric("cell_sell_defensin", Director.level, 1);}
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "take_defensin", 1); //TODO

		return FailCode.SUCCESS;
						}else
		{
			p_engine.showImmediateAlert(Messages.A_NO_AFFORD_RECYCLE);
			return FailCode.CANT_AFFORD;
		}
					}else
		{
			p_engine.showImmediateAlert("You don't have any defensins to recycle!");
		}
		return FailCode.TOTAL_FAIL;
	}

	public int sellMembrane() 
	{
			if (Membrane.CURR_NODES > Membrane.STARTING_NODES) {
				//refundMembrane();
				if (takeMembrane()) 
				{
					//cheap hack that finds the membrane in the lower left quadrant, where membrane vesicles
					//usually go to merge
					MembraneNode mnode = c_membrane.findClosestMembraneNode(cent_x - 50, cent_y - 50);
					Point p = new Point(mnode.x, mnode.y);
	//end cheap hack
				int atpCost = (int)Costs.SELL_MEMBRANE[0];
					if(spendATP(atpCost, p) > 0)
					{
						refundMembrane(p);
						//if(Director.STATS_ON){Log.LevelAverageMetric("cell_sell_membrane", Director.level, 1);}
	//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "take_membrane", 1); //TODO
			
					}else
					{
						//p_engine.showImmediateAlert(Messages.A_NO_AFFORD_RECYCLE);  //TODO
					}
	return FailCode.SUCCESS;
				}
			}else
			{
				p_engine.showImmediateAlert("Your membrane is at the minimum size already!");
			}
	return FailCode.TOTAL_FAIL;
		}
		
	public void buyMembrane()
	{
		if (Membrane.CURR_NODES < Membrane.MAX_NODES)
		{
			float ex = c_er.x + c_er.exit23.transform.position.x;
			float ey = c_er.y + c_er.exit23.transform.position.y;
			float[] cost = Costs.getMAKE_MEMBRANE(1);
			spend(cost);// , new Point(ex, ey), 1, 0, false, true);
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_buy_membrane", Director.level, 1); }
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_membrane", 1);  //TODO
			generateMembrane((int)cost[1]);
		}
		else
		{
			p_engine.showImmediateAlert("Can't expand! Membrane is at maximum size!");
		}
				//makeMembrane();
	}
		
	public void refundDefensin(Point p) 
	{
		float[] cost = new float[Costs.SELL_DEFENSIN.Length];
		Array.Copy(Costs.SELL_DEFENSIN, cost, Costs.SELL_DEFENSIN.Length);//<float[]>(new float[] { }); //Concat();
		cost[0] = 0;
		produce(cost, 1, p);
	}

	public void refundMembrane(Point p)
	{
		float[] cost = new float[Costs.SELL_MEMBRANE.Length];
		Array.Copy(Costs.SELL_MEMBRANE, cost, Costs.SELL_MEMBRANE.Length);// .Concat();
		//spendATP(cost[0]);
		cost[0] = 0;
		produce(cost, 1, p);
	}


	public bool takeMembrane() 
	{
		if(c_membrane.canTakeMembrane())
		{
			if(Membrane.CURR_NODES > Membrane.STARTING_NODES)
			{
					SfxManager.Play(SFX.SFXAbsorb);
					//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "shrink_membrane", "null", 1);  //TODO
					
					c_membrane.removeMembraneNodes(1,true);
					
					updateStartHardPoints();
	
				//p_engine.updateMaxResources(); //TODO
					return true;
				}else
				{
					p_engine.showImmediateAlert("Can't shrink! Membrane is at minimum size!");
					return false;
				}
			}
		else
		{
			Debug.Log("Cell.takeMembrane(): Hang on a sec, bro!");
			return false;
		}
	}

	public void makeMembrane(Point p = null)
	{
		if (Membrane.CURR_NODES < Membrane.MAX_NODES)
		{
			
			SfxManager.Play(SFX.SFXAbsorb);
			//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "expand_membrane", "null", 1);  //TODO
			
			c_membrane.addMembraneNode();
			updateStartHardPoints();
			
			//p_engine.updateMaxResources();  //TODO
		}
		else
		{
			p_engine.showImmediateAlert("Membrane at maximum size; vesicle recycled!");
			refundMembrane(p);
			c_membrane.readyForVesicle();
		}
	}

	public void updateMembrane()
	{
		c_skeleton.updateGravityPoints();
	}


	private void setupSkeleton()
	{
		c_skeleton.setCell(this);
		c_skeleton.setCent(c_centrosome);
		c_skeleton.setEngine(p_engine);
		c_skeleton.setNucleus(c_nucleus);
		c_skeleton.setMembrane(c_membrane);
		c_skeleton.init();
		c_skeleton.transform.SetSiblingIndex(c_centrosome.transform.GetSiblingIndex() - 1);
		//setChildIndex(c_skeleton, getChildIndex(c_centrosome) - 1);
	}


	public void onSkeletonReady()
	{
		c_membrane.onSkeletonReady();
	}

	public void makeMembraneVesicle(TransportVesicle t, float r)
	{
		BigVesicle v = makeBigVesicle();
		v.x = t.x;
		v.y = t.y;
		v.setProduct(t.getProduct());
		v.instantGrow(r);
		t.setBigVesicle(v);
		int ti = t.transform.GetSiblingIndex();
		int vi = v.transform.GetSiblingIndex();
		v.transform.SetSiblingIndex(ti);
		t.transform.SetSiblingIndex(vi);
		 //make v under t
	}

	private BigVesicle growBigVesicleFor(CellObject c)
	{
			BigVesicle v = makeBigVesicle();
	v.x = c.x;
			v.y = c.y;
			v.setPH(ph_balance);
			v.startDigestGrow(c);
		int ci = c.transform.GetSiblingIndex();
		int vi = v.transform.GetSiblingIndex();
		v.transform.SetSiblingIndex(ci);
		c.transform.SetSiblingIndex(vi);
		//make v under s
			return v;
		}

	public void onFinishDigestGrow(BigVesicle v) {

	}

	public int countThing(string str)
	{
		if (str == "lysososome") { return list_lyso.Count; }
		else if (str == "ribosome") { return list_ribo.Count; }
		else if (str == "slicer") { return list_slicer.Count; }
		else if (str == "peroxisome") { return list_perox.Count; }
		else if (str == "mitochondrion") { return list_mito.Count; }
		else if (str == "chloroplast") { return list_chlor.Count; }
		else if (str == "dnarepair") { return list_dnarepair.Count; }
		else if (str == "freeradical") { return list_radical.Count; }
		else if (str == "rna") { return checkRNACount(); }
		else if (str == "evil_rna") { return checkEvilRNACount(""); }
		return 0;
	}

	private void makeStartBigVesicle()
	{
		BigVesicle v = makeBigVesicle();
		v.x = -50;
		v.y = 200;
	}

	public SlicerEnzyme instantSlicer(float xx, float yy, bool notifyEngine = true) 
	{
			SlicerEnzyme s = makeSlicer(true);
			
			//if (notifyEngine)   //TODO
				//p_engine.startAndFinishSlicer();
			return s;
	}

	public Ribosome instantRibosome(float xx, float yy, bool notifyEngine = true)
	{
		Ribosome r = makeRibosome(true);
		

		//if (notifyEngine)    //TODO
			//p_engine.startAndFinishRibosome();
		return r;
	}

	private void makeStartHardPoints()
	{
		List<GravPoint> v = c_skeleton.newWarblePoints();
		int i = 0;
		foreach(GravPoint g in v) 
		{
			HardPoint h = makeHardPoint();
			h.x = g.x;
			h.y = g.y;
			h.setRadius(g.radius * 0.9f); //this is to avoid a rare bug where the cell membrane explodes on startup
			h.rememberRadius();
			if (i % 2 == 0)
			{
				h.warble_sign *= -1;
			}
			h.startWarble();
			h.updateLoc();
			i++;
		}
	}

	private void updateStartHardPoints()
	{

		List <GravPoint> v = c_skeleton.newWarblePoints();
		int i = 0;
		foreach(HardPoint h in list_hardpoint) {
			if (h.isWarble)
			{
				//h.setNewPos(v[i].x+cent_x, v[i].y+cent_y);
				h.x = v[i].x + cent_x;
				h.y = v[i].y + cent_y;
				h.setNewRadius(v[i].radius * 0.9f);
				h.resetWarble();
				//h.setRadius(v[i].radius * 0.9);
				//h.rememberRadius();
				h.updateLoc();
				i++;
			}
		}
	}

	private void makeStartSlicers(int n)
	{
		
		int i = 0;
		for (i = 0; i < n; i++)
		{
			SlicerEnzyme s = instantSlicer(0, 0);
			s.clip.GotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartLysosomes(int n)
	{
		List<float> c = FastMath.circlePointsOffset(230, n, 0, 0);
		int i = 0;
		for (i = 0; i < n; i++)
		{
			//p_engine.startAndFinishLysosome();  //TODO
			Lysosome l = instantLysosome(0, 0, false);
			l.deployGolgi(true);
			(l.clip as MovieClip).SubClip.GotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartPeroxisomes(int n)
	{
		List<float> c = FastMath.circlePointsOffset(230, n, 0, 0);
		int i = 0;
		for (i = 0; i < n; i++)
		{
			//p_engine.startAndFinishPeroxisome();  //TODO
			Peroxisome p = instantPeroxisome(0, 0, false);
			p.deployGolgi(true);
			p.isBusy = false; //HACK HACK HACK
			p.is_active = true;
			p.clip.SubClip.GotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartRibosomes(int n)
	{
		List<float> c = FastMath.circlePointsOffset(230, n, 0, 0);
		int i = 0;
		for (i = 0; i < n; i++)
		{
			Ribosome r = instantRibosome(0, 0);
			r.ribosomeDeploy(true);
			r.clip.SubClip.GotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartMitochondria(int n)
	{
		List<float> c = new List<float>() { -50, -200, -150, -200, 50, -200 };
		for (int i = 0; i < n; i++) 
		{
			Mitochondrion m = makeMitochondrion();
			m.x = c[i * 2];
			m.y = c[i * 2 + 1];// + c_nucleus.y ;
		}
	}

	private void makeStartChloroplasts(int n)
	{
		List<float> c = new List<float> { -50, 200, -150, 200, 50, 200 };
		for (int i = 0; i < n; i++) 
		{
			Chloroplast cl = makeChloroplast();
			cl.x = c[i * 2];// + c_nucleus.x;
			cl.y = c[i * 2 + 1];// + c_nucleus.y;
		}
	}

	private void makeSkeleton()
	{
		List< CellObject > list = new List<CellObject>();
		bool done = false;
		
		list.Add(c_centrosome);
		//list.push(c_er);
		c_skeleton.makeTubes(list);
		addRunning(c_skeleton);
	}

	private void makeTestMitochondria(int n)
	{
		List<float> c = FastMath.circlePointsOffset(300, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			Mitochondrion m = makeMitochondrion();
			m.x = c[i * 2];
			m.y = c[i * 2 + 1];
		}
	}

	private void makeTestLysosomes(int n)
	{
		List<float> c = FastMath.circlePointsOffset(250, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			instantLysosome(c[i * 2], c[i * 2 + 1]);
			//var l:Lysosome = growLysosome(c[i * 2],c[i * 2 + 1]);
			//l.is_active = true;
		}
	}

	private void makeTestChloroplasts(int n)
	{
		List<float> c = FastMath.circlePointsOffset(280, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			Chloroplast l =  makeChloroplast();
			l.x = c[i * 2];
			l.y = c[i * 2 + 1];
		}
	}

	private void makeTestRibosomes(int n)
	{
		List<float> c = FastMath.circlePointsOffset(230, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			Ribosome r = makeRibosome();
			r.x = c[i * 2];
			r.y = c[i * 2 + 1];
		}
	}

	public void setCanvas(WorldCanvas c)
	{
		p_canvas = c;
	}

	public void setDirector(Director d)
	{
		p_director = d;
	}

	public void receiveWoz(WizardOfOz w)
	{
		p_woz = w;
	}

	public override void setEngine(Engine e)
	{
		p_engine = e;
		c_membrane.setEngine(p_engine);
	}

	/*   //TODO
	public override void setWorld(World w)
	{
		p_world = w;
	}*/

	/*  //TODO
	public void setInterface(Interface i)
	{
		p_interface = i;
	}*/

	public override float getCircleVolume()
	{
			return c_membrane.getCircleVolume();
	}

/*************************/

	private void makeObjectGrid()
	{
		//trace("Cell.makeObjectGrid()");
		//c_objectGrid = new ObjectGrid();
		float size = Membrane.STARTING_RADIUS * 2;
		c_objectGrid.makeGrid(GRID_W, GRID_H, size, size);
		CellGameObject.setGrid(c_objectGrid);
	}
		
		public void makeLists()
	{
		list_chlor = new List< Chloroplast >();
		list_lyso = new List< Lysosome >();
		list_mito = new List< Mitochondrion >();
		list_virus = new List< Virus >();
		list_junk = new List< CellObject >();
		list_perox = new List< Peroxisome >();
		list_ribo = new List< Ribosome >();
		list_slicer = new List< SlicerEnzyme >();
		list_dnarepair = new List< DNARepairEnzyme >();
		list_radical = new List< FreeRadical >();
		list_bigves = new List< BigVesicle >();
		list_ves = new List< BlankVesicle >();
		list_rna = new List< RNA >();
		list_hardpoint = new List< HardPoint >();

		makeRunningList();
		makeSelectableList();

		/*makeStartSlicers(p_engine.lvl.levelData.start_slicers);  //TODO
		makeStartRibosomes(p_engine.lvl.levelData.start_ribos);
		makeStartMitochondria(p_engine.lvl.levelData.start_mitos);
		makeStartChloroplasts(p_engine.lvl.levelData.start_chloros);
		makeStartPeroxisomes(p_engine.lvl.levelData.start_peroxs);
		makeStartLysosomes(p_engine.lvl.levelData.start_lysos);*/
		makeStartHardPoints();

		
		makeSkeleton();


	}

	public void makeRunningList()
	{
		list_running = new List< CellObject >();

		addRunningList(list_chlor);
		addRunningList(list_lyso);
		addRunningList(list_mito);
		addRunningList(list_perox);
		addRunningList(list_ribo);
		addRunningList(list_slicer);
		addRunningList(list_radical);
		addRunningList(list_bigves);
		addRunningList(list_ves);
		addRunningList(list_hardpoint);
		//Other lists aren't included here, generally this is okay because these things (dna repair) etc aren't spawned
		//at the beginning of the game. If I ever decide to do that, I'll have to make sure to include them here
		//or starting units of that type won't work!

		addRunning(c_nucleus);
		addRunning(c_golgi);
		addRunning(c_er);
		addRunning(c_centrosome);
		addRunning(c_membrane);

	}

	public void makeSelectableList()
	{
		list_selectable = new List< CellObject >();

		addSelectableList(list_chlor);
		addSelectableList(list_lyso);
		addSelectableList(list_mito);
		addSelectableList(list_perox);
		addSelectableList(list_ribo);
		addSelectableList(list_bigves);
		addSelectableList(list_ves);

		addSelectable(c_nucleus);
		addSelectable(c_golgi);
		addSelectable(c_er);
		addSelectable(c_centrosome);
		addSelectable(c_membrane);
	}

	public void addRunningList(object v)
	{
		List<CellObject> list = v as List<CellObject>;
		foreach(CellObject i in list) {
			list_running.Add(i);
		}
	}

	public void addSelectableList(object v)
	{
		List<CellObject> list = v as List<CellObject>;
		foreach(CellObject i in list) 
				{
			list_selectable.Add(i);
		}
		dirty_selectList = true;
	}

	public int sortOnNumID(Selectable a, Selectable b) 
	{
			if (a.num_id > b.num_id) {
				return 1;
			}else if (a.num_id<b.num_id) {
				return -1;
			}else
		{
			return 0;
		}
	}
		
	public List<Selectable> getSelectables() 
	{
		List<Selectable> list = list_selectable.Cast<Selectable>().ToList();
		return list;
	}

	private void addSelectable(Selectable s) 
	{
		if (s)
		{
			list_selectable.Add(s as CellObject);
			dirty_selectList = true; //let the cell know it needs to flush the select list next frame
		}
	}

	private void addRunning(CellObject c) 
	{
		if (c)
		{
			list_running.Add(c);
		}
	}

	/*************************/

	public void updateSelected()
	{
		//p_engine.updateSelected(); //TODO

	}

	/**
		 * Called by the selected thing, causes the engine to select it and unselect all else
		 * @param	c
		 */

	public void selectOne(Selectable c, float xx, float yy)
	{
		/*
		//TODO
		if (p_engine.getSelectMode() == true)
		{
			p_engine.selectOne(c, xx, yy);
		}*/
	}

	/**
	 * Called by the selected thing, causes the engine to add this to the selected list
	 * @param	c
	 */

	public void selectMany(Selectable c, bool finish = false)
	{
		/*
		//TODO
		if (p_engine.getSelectMode() == true)
		{
			p_engine.selectMany(c, finish);
		}*/
	}


	public void setSelectType(int n)
	{
		//p_engine.setSelectType(n);  //TODO
	}

	/*************************/

	public override void pauseAnimate(bool yes)
	{
		foreach(CellObject c in list_running) {
			c.pauseAnimate(yes);
		}
	}

	public override void animateOn()
	{
		listsAnimate(true);
	}

	public override void animateOff()
	{
		listsAnimate(false);
	}

	private void listsAnimate(bool yes)
	{
		foreach(CellObject c in list_running) {
			if (yes)
			{
				c.animateOn();
			}
			else
			{
				c.animateOff();
			}
		}
	}

	public void onTopOf(CellObject thing1, CellObject thing2, bool superTop = false)
	{
		int t1=0, t2=0;
		try
		{
			t1 = thing1.transform.GetSiblingIndex();// getChildIndex(thing1);
			t2 = thing2.transform.GetSiblingIndex();//getChildIndex(thing2);
		}
		catch (System.Exception e) {
			Debug.LogError(e);
			return;
		}
		if (superTop)
		{ //make them at the top of EVERYTHING
			thing2.transform.SetSiblingIndex(this.transform.childCount - 1);
			//setChildIndex(thing2, numChildren - 1);
			int thi = thing1.transform.GetSiblingIndex();
			int th2i = thing2.transform.GetSiblingIndex();
			thing2.transform.SetSiblingIndex(thi);
			thing1.transform.SetSiblingIndex(th2i);
			
		}
		else
		{
			if (t1 < t2)
			{
				int th1i = thing1.transform.GetSiblingIndex();
				int th2i = thing2.transform.GetSiblingIndex();
				thing1.transform.SetSiblingIndex(th2i);
				thing2.transform.SetSiblingIndex(th1i);
			
			}
		}
		//setChildIndex(thing1, getChildIndex(thing2)+1);
		//put thing1 on top of thing2
	}

	public float spendATP(float i, Point p= null, float speed = 1, int offset= 0, bool scaleMode = true) 
	{
			if (p != null) {
				//p_canvas.justShowMeTheMoney("atp", -i, p.x, p.y, speed, offset, scaleMode);  //TODO
			}

		return 0;//p_engine.spendATP(i); //TODO
	}
		
	public void zeroResources()
	{
		//p_engine.zeroResources();  //TODO
	}

	public void loseResources(float[] a) {
		//p_engine.loseResources(a);  //TODO
	}



	/**
	 * 
	 * @param	a Array of resources to spend. [ATP,NA,AA,FA,G]
	 * @param	p Position to display. Won't show anything if not given.
	 * @param	speed How fast the displayed icon moves
	 * @param	offset How many icon heights to offset
	 * @param	scaleMode Whether it scales with zoom or not
	 * @return Whether the user could afford the spend
	 */

	public bool spend(float[] a, Point p= null,float speed = 1,int offset= 0, bool scaleMode = true,bool playSound = false)
	{
			int numNotZero = 0;
			if (p != null)
		{
			float[] b = a.Concat(null).ToArray();
			int length = b.Length;
		
			for (int i = 0; i < length; i++) 
			{
				if (b[i] > 0)
				{
					numNotZero++;
				}
				b[i] *= -1;
			}
			//trace("Cell.spend() : " + a + " numNotZero=" + numNotZero);
		}
			bool success = true; //p_engine.spend(a);  //TODO
		if (success)
		{
			if (playSound)
			{
				if (numNotZero > 1)
				{
						SfxManager.Play(SFX.SFXDrain);
						  //if we only spent 1, normal sound
				}
				else
				{
						SfxManager.Play(SFX.SFXMultiDrain);
					 //if we spent several, multiple drain sound
				}
			}
			if (p != null)
			{
				//p_canvas.justShowMeTheMoneyArray(b, p.x, p.y, speed, offset, scaleMode);  //TODO
			}
		}
		return success;
	}


	public bool rewardProduce(float[] a, float mult, Point p = null, float speed = 1, float offset = 0) 
	{
			int length = a.Length;
			for (int i = 0; i<length; i++) 
			{
				a[i] *= mult;
			}
		if (p!= null)
		{
			//p_canvas.justShowMeTheMoneyArray(a, p.x, p.y, speed, offset, false); //TODO
		}
		return true; // p_engine.produce(a);  //TODO
	}
		
	public void produceHeat(float amount, float mult, Point p = null, float speed = 1, float offset = 0) 
	{

		if (p != null)
		{
			//p_canvas.justShowMeTheMoney("fire", amount, p.x, p.y, speed, offset);  //TODO
		}
		//p_engine.changeTemperature(amount);  //TODO
	}

	public bool produce(float[] a, float mult, Point p = null,float speed = 1,float offset = 0)
	{
		int length = a.Length;
		for (int i = 0; i < length; i++) {
			a[i] *= mult;
		}
		if (p != null)
		{
			//p_canvas.justShowMeTheMoneyArray(a, p.x, p.y, speed, offset);  //TODO
		}
		return true;//p_engine.produce(a);   //TODO
	}

	public float getSunlight()
	{
		return lvl_sunlight;
	}

	/**Organelle making functions****/

	public void onHalfPlop(CellObject c)
	{
		c.transform.SetSiblingIndex(c.plopDepth);
		//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "halfplop_organelle", c.text_id, 1);  //TODO
	}

	public void onPlop(CellObject c)
	{
		//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "plop_organelle", c.text_id, 1);  //TODO
	}

	public void onRadicalHit(CellObject c)
	{
		//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "radical_damage", c.text_id, 1);  //TODO
	}

	public void setRadicals(Boolean b)
	{
		//trace("Cell.setRadicals(" + b + ")");
		foreach(Mitochondrion m in list_mito) {
			m.setRadicals(b);
		}
		foreach(Chloroplast c in list_chlor) {
			c.setRadicals(b);
		}
		RADICALS_ON = b;
	}

	public void notifyOHandler(string mainType, string evType, string targetType, float targNum)
	{
		//p_engine.notifyOHandler(mainType, evType, targetType, targNum);  //TODO
	}

	private void onKill(string str, int amount)
	{
		//p_engine.notifyOHandler(EngineEvent.THING_DESTROY, "null", str, 1);  //TODO
		//p_engine.notifyOHandler(EngineEvent.THING_CHANGE, "null", str, amount);  //TODO
	}

	private void onMake(string str, int amount)
	{
		dirty_units = true;
		//p_engine.notifyOHandler(EngineEvent.THING_CREATE, "null", str, 1);  //TODO
		//p_engine.notifyOHandler(EngineEvent.THING_CHANGE, "null", str, amount);  //TODO
	}

	public void plopOrganelle(string str)
	{
		//trace("Cell.plopOrganelle() " + str);

		CellObject c = hideOrganelle(str, false);
		c.plopDepth = c.transform.GetSiblingIndex();
		c.playAnim("plop");
		c.transform.SetSiblingIndex(this.transform.childCount - 1);
	}


	public CellObject hideOrganelle(string str, bool yes) 
	{
			CellObject toHide = null;
			if (str == "nucleus") {
				toHide = c_nucleus;
			}else if (str == "golgi")
			{
				toHide = c_golgi;
			}
			else if (str == "centrosome")
			{
				toHide = c_centrosome;
			}
			else if (str == "er")
			{
				toHide = c_er;
			}
			if (yes)
			{
				toHide.hideOrganelle();
			}
			else
			{
				toHide.showOrganelle();
			}
			return toHide;
	}
		
		
		public void onFinishDefensin()
		{
			onMake("defensin", (int)c_membrane.getDefensins());
		}

		public void makeToxin(float xx, float yy) 
		{
			toxin_level += TOXIN_AMOUNT;
			SfxManager.Play(SFX.SFXToxin);
		
			showToxinSpot(TOXIN_AMOUNT, xx, yy);
			onMake("toxin", (int)toxin_level);
			//p_engine.setToxinLevel(toxin_level / MAX_TOXIN);  //TODO
		}

		private Chloroplast makeChloroplast()
		{
			Chloroplast c = new Chloroplast();
			c.transform.SetParent(this.transform, false);
			c.setCell(this);
			list_chlor.Add(c);
			chloroCount = list_chlor.Count;
			//p_engine.setChloroCount(chloroCount);  //TODO
			addRunning(c);
			addSelectable(c);
			c.setSunlight(lvl_sunlight);
			onMake("chloroplast", list_chlor.Count);
			return c;
		}

	public void makeVesicleContent(string str, float xx, float yy)
	{
		SfxManager.Play(SFX.SFXAbsorb);
		

		if (str == "mitochondrion")
		{
			placeVesicleMito(xx, yy);
			//placeMitochondrion(xx, yy,true);
		}
		else if (str == "mitochondrion_dead")
		{
			placeVesicleMito(xx, yy, false);
			//placeMitochondrion(xx, yy, true, false);
		}
		else if (str == "protein_glob")
		{
			placeProteinGlob(xx, yy);
		}
		else if (str == "chloroplast")
		{
			placeVesicleChloro(xx, yy);
		}
		else if (str == "chloroplast_dead")
		{
			placeVesicleChloro(xx, yy, false);
		}
		updateMembrane();
	}



	public void placeProteinGlob(float xx, float yy)
	{
		ProteinGlob p = makeProteinGlob();
		p.x = xx;
		p.y = yy;
		makeEscortPoint(p);
		//c_skeleton.makeNewTube(p);
	}

	public void placeVesicleChloro(float xx, float yy, bool isAlive = true)
	{
		Chloroplast c = placeChloroplast(xx, yy, true, isAlive);
		c.setOutsideCell(true);
		makeEscortPoint(c);
	}

	public void placeVesicleMito(float xx, float yy, bool isAlive = true)
	{
		Mitochondrion m = placeMitochondrion(xx, yy, true, isAlive);
		m.setOutsideCell(true);
		makeEscortPoint(m);
		//updateMembrane();
	}

	public Chloroplast placeChloroplast(float xx, float yy, bool deploy = false, bool isAlive = true, bool nudge = true) 
	{
		Chloroplast c = makeChloroplast();
		c.x = xx;
		c.y = yy;
				
		c_skeleton.makeNewTube(c);
		if (nudge) {
			Vector2 v = new Vector2(xx - cent_x, yy - cent_y); //get the vector from the centrosome to this thing
	v.Normalize();											 //unit vector
			v *= (0.75f * c_skeleton.cent_radius);				 //multiply by the X * cell's min radius
			c.moveToPoint(new Point(cent_x + v.x, cent_y + v.y), FLOAT, true); //move to a comfortable spot in the cell
		}
		if (!isAlive)
		{
			c.instantSetHealth(10); //very low health
		}
		return c;
	}
		
	public Mitochondrion placeMitochondrion(float xx, float yy, bool deploy = false,bool isAlive = true, bool nudge = true)
	{
		Mitochondrion m = makeMitochondrion();
		m.x = xx;
		m.y = yy;
		c_skeleton.makeNewTube(m);
		//var r:Number = c_skeleton.cent_radius;
		if (nudge)
		{
			Vector2 v = new Vector2(xx - cent_x, yy - cent_y); //get the vector from the centrosome to this thing
			v.Normalize();                                           //unit vector
			v *= (0.75f * c_skeleton.cent_radius);               //multiply by the X * cell's min radius
			m.moveToPoint(new Point(cent_x + v.x, cent_y + v.y), FLOAT, true); //move to a comfortable spot in the cell
		}
		if (!isAlive)
		{
			m.instantSetHealth(10); //very low health
		}
		return m;
		//m.deployCytoplasm(cent_x + v.x,cent_y+v.y,
	}

	public List<BigVesicle> getVacList() 
	{
		return list_bigves.Cast<BigVesicle>().ToList();
	}

	public Virus export_makeVirus(string type) 
	{
			Virus v = null;
			if (type == "virus_injector") v = new VirusInjector();
			else if (type == "virus_invader") v = new VirusInvader();
			else if (type == "virus_infester") v = new VirusInfester();
			v.setCanvas(p_canvas);
			v.setCell(this);
			v.init();
			return v;
	}

	public void testPopVesicle()
	{
		//var v:PopVesicle = makePopVesicle(100,200,200);
	}


	public SplashBurst makeSplashBurst(float size, float xx, float yy, int iteration = 1, Vector2 vec = default(Vector2))
	{
		//trace("Cell.makeSplashBurst() size=" + size + " loc = ("+xx+","+yy+")");
		SplashBurst v = new SplashBurst();
		v.x = xx;
		v.y = yy;
		v.transform.localScale = new Vector3(size / v.MaxWidth, size / v.MaxHeight, 1);
		//v.width = size;  //TODO: figure this out.
		//v.height = size;
		v.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
		v.setCell(this);
		v.init();
		v.transform.SetParent(this.transform);
		addRunning(v);
		v.startPopping();
		v.transform.SetSiblingIndex(1);
		return v;

	}

	public void popSplashBurst(SplashBurst p)
	{
		/*//trace("Cell.popVesicle p=" + this + " radius=" + p.popRadius + " it = " + p.iteration);
		var times:int = 3;
		var rand:Number = Math.random();
		if (rand > 0.6) {
			times++;
		}


		var radius:Number = p.popRadius;
		var newRadius:Number = radius*1.5 / times;
		var v:Vector2D = new Vector2D(newRadius * 2, 0);
		v.rotateVector(Math.random() * Math.PI * 2);
		var angle:Number = Math.PI * 2 / times;

		//trace("Cell.popVesicle newRadius=" + newRadius + " times= " + times);

		if(p.iteration < 4){	//if it gets too recursive, don't make any new bubbles
			if(newRadius > 15){	//if it gets too small, don't make any new bubbles		
				for (var i:int = 0; i < times; i++) {
					var vnorm:Vector2D = v.getNormalized();
					var s:SplashBurst = makeSplashBurst(newRadius, v.x + p.x, v.y + p.y, p.iteration + 1, vnorm);
					//var pp:PopVesicle = makePopVesicle(newRadius,v.x+p.x,v.y+p.y,p.iteration+1,vnorm);
					v.rotateVector(angle);
				}
			}
		}

		killSplashBurst(p);*/
	}

	public BigVesicle export_makeBigVesicle(float size) 
	{
		BigVesicle v = new BigVesicle(size);
		v.setCell(this);
		return v;
	}

	private BigVesicle makeBigVesicle() 
	{
		BigVesicle v = new BigVesicle();
		v.transform.SetParent(this.transform);
		v.setCell(this);
		list_bigves.Add(v);
		addSelectable(v);
		addRunning(v);
	//c_membrane.updateVacList();
		onMake("big_vesicle", list_bigves.Count);
		return v;
	}

	public void spawnRibosomeDNARepair(Ribosome r, int count = 1)
	{
		DNARepairEnzyme d = makeDNARepair();
		float SIZE = 25;
		for (int i = 0; i < count; i++) {
			d.x = r.x + (UnityEngine.Random.Range(0f,1f) * SIZE) - SIZE / 2;
			d.y = r.y + (UnityEngine.Random.Range(0f,1f) * SIZE) - SIZE / 2;
		}
		d.init();
		//p_engine.finishDNARepair();  //TODO
	}

	public void spawnRibosomeSlicer(Ribosome r, int count = 1)
	{
		SlicerEnzyme s = makeSlicer();
		float SIZE = 25;
		for (int i = 0; i < count; i++) {
			s.x = r.x + (UnityEngine.Random.Range(0f,1f) * SIZE) - SIZE / 2;
			s.y = r.y + (UnityEngine.Random.Range(0f, 1f) * SIZE) - SIZE / 2;
		}
		s.init();
		//p_engine.finishSlicer();   //TODO
	}

	public void spawnRibosomeVirus(Ribosome r, int product, int count = 1, string wave_id = "", bool doesVesicle = false)
	{
		int success_count = 0;
		string v = "";
		string type;
		switch (product)
		{
			case Selectable.VIRUS_INJECTOR: v = "virus_injector"; type = "injector"; break;
			case Selectable.VIRUS_INVADER: v = "virus_invader"; type = "invader"; break;
			case Selectable.VIRUS_INFESTER: v = "virus_infester"; type = "infester"; break;
		}
		bool success = false;
		for (int i = 0; i < count; i++) 
		{
			float SIZE = 20;
			float xx = (UnityEngine.Random.Range(0f,1f) * SIZE) - (SIZE / 2);
			float yy = (UnityEngine.Random.Range(0f, 1f) * SIZE) - (SIZE / 2);
			Virus vi = makeVirus(v, r.x + xx, r.y + yy, true, true);
			if (vi)
			{
				vi.setVesicle(doesVesicle);
				vi.wave_id = wave_id;
				//trace("Cell.spawnRibosomeVirus() wave_id=" + vi.wave_id);
				success = true;
				success_count++;
			}
		}
		if (success)
		{ //only play the sound if we succeeded in making a virus
		  //p_engine.makeVirus(type, wave_id, success_count);  //TODO
			SfxManager.Play(SFX.SFXVirusRise);
			 //play the sound here so it's not played multiple times per spawn
		}
	}

	public void engineSpawnRadical(int count, string origin, string target, int delay)
	{
		spawn_engine_radical_count = count;
		spawn_engine_radical_origin = origin;
		spawn_engine_radical_target = target;
		SPAWN_ENGINE_RADICAL_TIME = delay;
		if (!spawning_engine_radicals)
		{
			_engineSpawnRadicalRoutine = StartCoroutine(onEngineSpawnRadical());
			
		}
	}

	private IEnumerator onEngineSpawnRadical()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			spawn_engine_radical_counter++;
			if (spawn_engine_radical_counter >= SPAWN_ENGINE_RADICAL_TIME)
			{
				spawn_engine_radical_counter = 0;
				StopCoroutine(_engineSpawnRadicalRoutine);
				CellObject maker = null;
				if (spawn_engine_radical_origin == "mitochondrion")
				{
					maker = getRandomMito();
				}
				else if (spawn_engine_radical_origin == "chloroplast")
				{
					maker = getRandomChloro();
				}
				if (maker != null)
				{
					for (int i = 0; i < spawn_engine_radical_count; i++) 
					{
						makeRadical(maker, false, spawn_engine_radical_target);
					}
				}
			}
		}
	}

	public FreeRadical makeRadical(CellObject maker, bool isInvincible = false, string targetStr= "") 
	{
		FreeRadical r = null;
		if (isInvincible) 
		{
			r = new InvincibleRadical();
		}else
		{
			r = new NormalRadical();
		}

		r.x = maker.x + maker.getRadius() * (-0.5f + UnityEngine.Random.Range(0f,1f));
		r.y = maker.y + maker.getRadius() * (-0.5f + UnityEngine.Random.Range(0f, 1f));
		r.setTargetStr(targetStr);
		r.setMaker(maker);
		r.transform.SetParent(this.transform);
		list_radical.Add(r);
		r.setCell(this);
		addRunning(r);
		addSelectable(r);
		onMake("radical", list_radical.Count);
		r.init();
		return r;	
	}
		
	public DNARepairEnzyme makeDNARepair()
	{
		GameObject dro = Instantiate(DNARepairEnzyme_Prefab) as GameObject;
		DNARepairEnzyme d = dro.GetComponentInChildren<DNARepairEnzyme>();
		
		d.transform.SetParent(this.transform);
		list_dnarepair.Add(d);
		d.setCell(this);
		addRunning(d);
		addSelectable(d);
		onMake("dnarepair", list_dnarepair.Count);
		d.init();
		return d;

	}

	public HardPoint makeEscortPoint(CellObject c)
	{
		HardPoint h = new HardPoint();
		//addChild(h);
		list_hardpoint.Add(h);
		h.setCell(this);
		addRunning(h);
		h.setEscort(c);
		h.init();
		return h;
	}

	public HardPoint makeHardPoint() 
	{
		HardPoint h = new HardPoint();
	//addChild(h); //LATER WE WILL COMMENT THIS OUT
		list_hardpoint.Add(h);
		h.setCell(this);
		addRunning(h);
		h.init();
		return h;
	}

	public SlicerEnzyme makeSlicer(bool instant_deploy = false)
	{
		SlicerEnzyme s = new SlicerEnzyme();
		if (instant_deploy)
		{
			s.play_init_sound = false;
			s.instant_deploy = true;
		}
			s.transform.SetParent(this.transform);
		list_slicer.Add(s);
		s.setCell(this);
		addRunning(s);
		addSelectable(s);
		onMake("slicer", list_slicer.Count);
		s.init();
		return s;
	}

	public int checkRNACount()
	{
		int count = 0;
		foreach(RNA r in list_rna){
			if (r is MRNA || r is EnzymeRNA)
			{
				count++;
			}
		}
		return count;
	}

	public int checkEvilRNACount(string wave) 
	{
		int count = 0;
		foreach(RNA r in list_rna)
		{
			if (wave != "")
			{
				if (r.getProductCreator() == wave)
				{
					count++;
				}
			}
			else
			{
				if (r is EvilRNA || r is EvilDNA)
				{
					count++;
				}
			}
		}
		//trace("Cell.checkEvilRNACount(" + wave + ") = " + count);
		return count;
	}

	//Possible shlemiel! Watch out for huge virus lists!
	public int checkVirusCount(string wave)
	{
		int count = 0;
		foreach(Virus v in list_virus) {
			if (v.wave_id == wave)
			{
				count++;
			}
		}
		//trace("Cell.checkVirusCount(" + wave + ") = " + count);
		return count;
	}

	public void makeVirusWave(WaveEntry w)
	{
		float r = CanvasObject.LENS_RADIUS;
		
		int RING_AMOUNT = 7;
		List<int> ring_list = new List<int>();

		int ring_count = 0;
		var ring_max = RING_AMOUNT;
		int count = 0;
		int i = 0;
		ring_list[0] = 0;

		while (count < w.count)
		{
			ring_list[ring_count]++;
			if (ring_list[ring_count] >= ring_max)
			{
				ring_list[ring_count + 1] = 0;
				ring_count++;
				ring_max *= 2;
			}
			count++;
		}
		count = 0;

		//circlePoints
		//var theVec:Vector2D
		List<List<float>> listPoints = new List<List<float>>();
		float mult = 0.1f;

		for (i = 0; i < ring_list.Count; i++)
		{
			listPoints.Add(FastMath.circlePointsOffset(CanvasObject.LENS_RADIUS * (1.1f + mult), ring_list[count], cent_x, cent_y));
			count++;
			mult += 0.2f;
			//v = circlePointsOffset(radius:Number, MAX:Number, xo:Number, yo:Number
		}

		var listCirclePoints = new List<float>();
		foreach(List<float> vvvv in listPoints) {
			//listCirclePoints.
			foreach(float n  in vvvv) {
				listCirclePoints.Add(n);
			}
			//listCirclePoints.push(vvvv.concat());// concat(vvvv);
		}

		int vesicle_count = (int) (w.count * w.vesicle);

		int success_count = 0;
		for (int ii = 0; ii < w.count; ii++) 
		{

			float jiggX = (UnityEngine.Random.Range(0f,1f) - 0.5f) * 150;
			float jiggY = (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 150;
			float theX = listCirclePoints[ii * 2] + jiggX;
			float theY = listCirclePoints[(ii * 2) + 1] + jiggY;

			Virus v = makeVirus(w.type, theX, theY);
			if (v)
			{
				success_count++;
			}

			v.wave_id = w.id;
			if (i < vesicle_count)
			{
				v.setVesicle(true);
			}
		}

		updateViruses();
		//p_engine.makeVirus(w.type, w.id, success_count);  //TODO
	}

	public Virus makeVirus(string type, float xx, float yy, bool doEscape = false, bool spendCost = false) 
	{
		Virus v = null;
		float[] cost;
		if (type == "virus_injector") 
		{
				v = new VirusInjector();

		}
		else if (type == "virus_invader")
		{
			v = new VirusInvader();

		}
		else if (type == "virus_infester")
		{
			v = new VirusInfester();

		}

		bool proceed = true;



		if (proceed)
		{
			v.transform.SetParent(this.transform);
			v.setCanvas(p_canvas);
			v.setCell(this);
			list_virus.Add(v);
			addRunning(v);
			onMake("type", list_virus.Count); //this needs to change when we implement more than 1 virus type
			v.x = xx;
			v.y = yy;
			v.setup(doEscape);
		}
		else
		{
			v.destruct();
			v = null;
		}
		return v;
	}

	public void fireToxinParticle(float xx, float yy)
	{
		ToxinParticle t = makeToxinParticle();
		//makeToxin(xx, yy);
		t.x = xx;
		t.y = yy;
		t.getOuttaHere();
	}

	public ToxinParticle makeToxinParticle() 
	{
		ToxinParticle t = new ToxinParticle();
		t.transform.SetParent(this.transform);
		t.setCell(this);
		t.setCanvas(p_canvas);
			list_junk.Add(t);
			addRunning(t);
		int count = 0;
		foreach(CellObject c in list_junk)
		{
			if (c is ToxinParticle)
			{
				count++;
			}
		}
	onMake("toxin_particle", count);
			return t;
	}

	private ProteinGlob makeProteinGlob()
	{
		ProteinGlob p = new ProteinGlob();
		p.transform.SetParent(this.transform);
		p.setCell(this);
		list_junk.Add(p);
		addRunning(p);
		int count = 0;
		foreach(CellObject c in list_junk) 
		{
			if (c is ProteinGlob)
			{
				count++;
			}
		}
		onMake("protein_glob", count);
		return p;
	}

	private Mitochondrion makeMitochondrion() 
	{
		Mitochondrion m = new Mitochondrion();
		m.transform.SetParent(this.transform);
		m.setCell(this);
		list_mito.Add(m);
		addSelectable(m);
		addRunning(m);
		onMake("mitochondrion", list_mito.Count);
		mitoCount = list_mito.Count;
		//p_engine.setMitoCount(mitoCount);  //TODO
		return m;
	}

	private TransportVesicle makeTransportVesicle(int product, float amount = 1)
	{
		TransportVesicle t = new TransportVesicle();
		t.setProduct(product, amount);
		t.transform.SetParent(this.transform);
		t.setCell(this);
		list_ves.Add(t);
		//addSelectable(t);
		addRunning(t);
		onMake("transport_vesicle", list_ves.Count); //need to count actual transport vesicles
		return t;
	}

	private Peroxisome makePeroxisome()
	{
		GameObject po = Instantiate(Peroxisome_Prefab) as GameObject;
		Peroxisome p = po.GetComponent<Peroxisome>();
		p.transform.SetParent(this.transform);
		p.setCell(this);
		list_perox.Add(p);
		addSelectable(p);
		addRunning(p);
		updateBasicUnits();
		onMake("peroxisome", list_perox.Count);
		return p;
	}

	private Lysosome makeLysosome() 
	{
		GameObject lo = Instantiate(Lysosome_Prefab) as GameObject;
		Lysosome l = lo.GetComponent<Lysosome>();
		l.transform.SetParent(this.transform);
		l.setCell(this);
		list_lyso.Add(l);
		addSelectable(l);
		addRunning(l);
		updateBasicUnits();
		onMake("lysosome", list_lyso.Count);
			return l;
	}

	/******/

	private void updateViruses()
	{
		c_membrane.updateViruses(list_virus);
	}

	private void updateBasicUnits()
	{
		List<BasicUnit> list = new List<BasicUnit>();
		foreach(SlicerEnzyme s in list_slicer) 
		{
			list.Add(s);
		}
		foreach(Peroxisome p in list_perox) {
			list.Add(p);
		}
		foreach(Lysosome l in list_lyso) {
			list.Add(l);
		}
		foreach(Ribosome r in list_ribo) {
			list.Add(r);
		}
		foreach(DNARepairEnzyme d in list_dnarepair) 
		{
			list.Add(d);
		}

		c_membrane.updateBasicUnits(list);
	}
		
		/******/
		
	public Point getGolgiLoc()
	{
		Point p = new Point(c_golgi.x, c_golgi.y);
		return p;
	}

	public EvilRNA generateEvilRNA(int i, int count, string wave_id= "", bool startImmediately = false, bool evilDNA = false, bool doesVesicle = false) 
	{
			
		EvilRNA r;  //was:  EvilRNA;
		if(evilDNA == false)
		{
		   GameObject ro = Instantiate(RedRNA_Prefab) as GameObject; // (i, count, wave_id);
			r = ro.GetComponent<EvilRNA>();
			r.InitRNA(i, count, wave_id);

		}else
		{
			GameObject ro = Instantiate(EvilDNA_Prefab) as GameObject;
			r = ro.GetComponentInChildren<EvilDNA>();
			(r as EvilDNA).InitEvilDNA(i, count, wave_id);
		}

		r.product_virus_vesicle = doesVesicle;

		r.transform.SetParent(this.transform, false);
		list_rna.Add(r);
		r.setCell(this);
		addRunning(r);
		if (startImmediately)
		{
			r.doesRotateUp = true;
			r.playAnim("fast_grow");
		}
		else
		{
			r.doesRotateUp = false;
			r.playAnim("grow");
		}
		bool wait;
		if (i != Selectable.VIRUS_INFESTER)
		{
			wait = !askForRibosome(r);
			if (wait)
			{
				r.waitForRibosome();
			}
		}
		else
		{
			//trace("Cell.generateEvilRNA is INFESTER");
			askForNucleusPore(r);
			//wait = !askForNucleusPore(r);
			//if (wait) {
			//	r.waitForNucleusPore();
			//}
		}
		return r;
	}

	public void onHealSomething(CellObject c, float n)
	{
		//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "heal_thing", c.text_id, n);  //TODO
	}

	public int getNucleusInfestation() 
	{
		int max = (int)c_nucleus.getMaxInfest();
		int h = (int)c_nucleus.getInfest();
		int d = max - h;
		return d;
	}

	public int getNucleusDamage()
	{
		int max = c_nucleus.getMaxHealth();
		int h = c_nucleus.getHealth();
		int d = max - h;
		return d;
	}

	public RNA generateInfestRNA(int i, string creator)
	{
		EvilRNA r = null;
		string virus_type = "";
		switch (i)
		{
			case Selectable.VIRUS_INFESTER:
				GameObject ro = Instantiate(RedRNA_Prefab) as GameObject;
				r = ro.GetComponentInChildren<RedRNA>();
				r.InitRNA(Selectable.VIRUS_INFESTER, VirusInfester.SPAWN_COUNT, creator);
				//r = new RedRNA(Selectable.VIRUS_INFESTER, VirusInfester.SPAWN_COUNT, creator);
				virus_type = "virus_infester";
				break;
		}
		r.setNAValue(0);
		r.transform.SetParent(this.transform);
		list_rna.Add(r);
		r.setCell(this);
		addRunning(r);
		//c_nucleus.getPore
		var pt = c_nucleus.getPoreLoc(0, true);
		r.x = pt.x;
		r.y = pt.y;
		r.playAnim("grow");
		bool wait = !askForRibosome(r);
		if (wait)
		{
			r.waitForRibosome();
		}
		//p_engine.onMakeEvilRNA(virus_type, creator, 1);  //TODO
		return r;
		
	}

	public RNA generateMRNA(int i, int na)
	{

		RNA r;
		if (i == Selectable.SLICER_ENZYME || i == Selectable.DNAREPAIR)
		{
			GameObject ro = Instantiate(EnzymeRNA_Prefab) as GameObject;
			r = ro.GetComponentInChildren<EnzymeRNA>(); //hack to get the animations right. We use an alternate RNA mc that has offset thread anims
														//you can use this for anything that generates proteins in place
			r.InitRNA(i);					
		}
		else
		{
			GameObject ro = Instantiate(MRNA_Prefab) as GameObject;
			r = ro.GetComponentInChildren<MRNA>();
			r.InitRNA(i);    //the normal MRNA. use this for something that docks with a ribosome and uses the ER
		}

		r.setNAValue(na);


		r.transform.SetParent(this.transform, false);
		list_rna.Add(r);
		r.setCell(this);
		addRunning(r);
		Point pt = c_nucleus.getPoreLoc(0, true);
		r.x = pt.x;
		r.y = pt.y;
		r.playAnim("grow");
		bool wait = !askForRibosome(r); //check for ribosomes
		if (wait)
		{
			r.waitForRibosome();
		}
		return r;
	}

	public bool tauntByRadical(FreeRadical r) 
	{
			//return peroxisomeEatSomething(r);
			Peroxisome p = findClosestPeroxisome(r.x, r.y);
			if (p) {
				p.setTargetRadical(r);
				return true;
			}
		return false;
	}
		
	public bool tauntByVirus(Virus v)
	{
		/*var l:Lysosome = findClosestLysosome(v.x, v.y);
		if (l) {
			l.
		}*/

		return lysosomeEatSomething(v);
	}

	public bool tauntByEvilRNA(EvilRNA e)
	{
		SlicerEnzyme s = findClosestSlicer(e.x, e.y);
		if (s)
		{
			s.targetEvilRNA(e);
			return true;
		}
		return false;
	}


	public Chloroplast getRandomChloro()
	{
		int length = list_chlor.Count;
		if (length > 0)
		{
			int m = (UnityEngine.Random.Range(0,2)) * length;
			return list_chlor[m];
		}
		return null;
	}

	public Mitochondrion getRandomMito() 
	{
		int length = list_mito.Count;
		if(length > 0)
		{
			int m = UnityEngine.Random.Range(0,2) * length;
				return list_mito[m];
		}
		return null;
	}
		
	public Lysosome getRandomLyso()
	{
		int length = list_lyso.Count;
		if (length > 0)
		{
			int m = UnityEngine.Random.Range(0,2) * length;
			return list_lyso[m];
		}
		return null;
	}

	public SlicerEnzyme getRandomSlicer()
	{
		int length = list_slicer.Count;
		if (length > 0)
		{
			int m = UnityEngine.Random.Range(0,2) * length;
			return list_slicer[m];
		}
		return null;
	}

	public Peroxisome getRandomPerox()
	{
		int length = list_perox.Count;
		if (length > 0)
		{
			int m = UnityEngine.Random.Range(0,2) * length;
			return list_perox[m];
		}
		return null;
	}

	private SlicerEnzyme findClosestSlicer(float xx, float yy) 
	{
			float bestD2 = 10000000000000000;
			int length = list_slicer.Count;
			SlicerEnzyme bestS = null;
			for (int i = 0; i<length; i++) 
			{
				SlicerEnzyme s = list_slicer[i];
				if(!s.hasRNA)
				{ //if it doesn't already have a target
					float dx = xx - s.x;
					float dy = yy - s.y;
					float d2 = (dx* dx) + (dy* dy);
					if (d2<bestD2) {
						bestD2 = d2;
						bestS = s;
					}
				}

			}
			//trace("Cell.findClosestSlicer(" + xx + "," + yy + ") = (" + bestS.x + "," + bestS.y + ")");
			return bestS;
	}

	public bool dockGolgiVesicle(BlankVesicle v) 
	{//attempt to dock the vesicle to the Golgi
			
		DockPoint pt = c_golgi.findDockingPoint();
		if (pt != null) 
		{
			v.setDockPoint(pt, (int)c_golgi.transform.position.x, (int)c_golgi.transform.position.y);
				
			return true;
		}
		return false;
	}
		
		public bool dockRibosomeER(Ribosome r)
		{ //attempt to dock the ribosome to the Rough ER
		  //LYSO_RIB_FINISHED++;
		  //trace("Cell.dockRibosomeER() : LYSO_RIB_FINISHED= " + LYSO_RIB_FINISHED);	
			DockPoint pt = c_er.findDockingPoint();
			if (pt != null)
			{
				r.setDockPoint(pt, (int)c_er.transform.position.x, (int)c_er.transform.position.y);
				//c_er.busyDockingPoint(pt.index);
				return true;
			}

			return false;
		}

	public bool askForGolgiExit(BlankVesicle v)
	{

		DockPoint exit = c_golgi.findExitPoint();
		if (exit != null)
		{
			//trace("found exit " + exit.index);
			//c_golgi.busyExitPoint(exit.index);
			v.setExit(exit, (int)c_golgi.transform.position.x, (int)c_golgi.transform.position.y);
			v.swimThroughGolgi();
			return true;
		}
		else
		{
			v.waitForExit();
		}
		return false;
	}

	public bool askForERExit(ProteinCloud p)
	{
		DockPoint exit = c_er.findExitPoint();
		if (exit != null)
		{
			//c_er.busyExitPoint(exit.index);
			p.setExit(exit, (int)c_er.transform.position.x, (int)c_er.transform.position.y);
			p.swimER();
		}
		else
		{
			p.waitForExit();
		}
		return false;
	}

	public List<object> getNucleusPore() 
	{
		return c_nucleus.getPorePoint();
	}

	public bool askForNucleusPore(RNA r)
	{
		List<object> a = c_nucleus.getPorePoint();
		if (a != null)
		{
			r.setNPore((a[0] as Point), c_nucleus, (int)(a[1]));
			return true;
		}
		return false;
	}

/**
 * Given an MRNA, tries to assign it to the closest ribosome
 * @param	r the MRNA
 * @return boolean : whether the function succeeded
 */

	public bool askForRibosome(RNA r)
	{
		Ribosome ri = findClosestRibosome(r.x, r.y);
		if (ri != null)
		{
			r.setRibosome(ri);
			return true;
		}
		else if (list_ribo.Count < 1)
		{
			//p_engine.showAlert(Messages.A_NO_RIBO_RNA);  //TODO
		}
		return false;
	}

	public void dismissLysosomes(BigVesicle b) 
	{
		int length = list_lyso.Count;
		for (int i = 0; i < length; i++) 
		{
			if (list_lyso[i].fuse_target == b)
			{
				list_lyso[i].dontFuseWithBigVesicle();
			}
		}
	}

	public int askForLysosomes(BigVesicle b, int howMany) 
	{
		//trace("Cell.askForLysosomes(): " + howMany);
		int gotMany = 0;
		for (int i = 0; i<howMany; i++) 
		{
			Lysosome ly = findClosestLysosome(b.x, b.y);
			if (ly != null) {			
				ly.fuseWithBigVesicle(b);
				gotMany++;
			}
		}
		if (gotMany < howMany)
		{
			//p_engine.showAlert(Messages.A_NO_LYSO_R);  //TODO
		}

		return howMany - gotMany;
			//return false;
	}
		
	public Peroxisome findClosestPeroxisome(float xx, float yy)
	{
		float dist2 = 1000000000;
		float bestDist = dist2;
		Peroxisome bestP = null;

		if (list_perox.Count > 0)
		{
			foreach(Peroxisome p in list_perox) 
			{
				if (p.is_active && !p.isBusy)
				{
					dist2 = FastMath.getDist2(xx, yy, p.x, p.y);
					if (dist2 < bestDist)
					{
						bestP = p;
						bestDist = dist2;
					}
				}
			}
		}
		return bestP;
	}

	public Lysosome findClosestLysosome(float xx, float yy)
	{
		float dist2 = 1000000000;
		float bestDist = dist2;
		Lysosome bestL = null;

		int length = list_lyso.Count;
		if (length > 0)
		{ //if there is at least one lysosome
			foreach(Lysosome ly in list_lyso) 
			{ //find the closest
				if (!ly.amEating)
				{
					if (!ly.isRecycling && !ly.isBusy)
					{
						dist2 = FastMath.getDist2(xx, yy, ly.x, ly.y);
						if (dist2 < bestDist)
						{
							bestL = ly;
							bestDist = dist2;
						}
					}
				}
			}
		}
		return bestL; //return the closest
	}

	public Ribosome findClosestRibosome(float xx, float yy, bool anyWillDo = false)
	{
		float dist2 = 1000000000;
		float bestDist = dist2;
		Ribosome bestR = null;

		//OPTIMIZE SHLEMIEL
		int length = list_ribo.Count;
		if (length > 0)
		{ //if there is at least one ribosome
			foreach(Ribosome ri in list_ribo) 
			{ //find the closest
				if (!ri.isBusy && ri.isReady() && !ri.isDoomed)
				{
					dist2 = FastMath.getDist2(xx, yy, ri.x, ri.y);
					if (dist2 < bestDist)
					{
						bestR = ri;
						bestDist = dist2;
					}
				}
			}
		}

		//if we couldn't find a free one, how bout we just give you anything, IF "anyWillDo" is true
		if (bestR == null && anyWillDo)
		{
			int i = (int)Mathf.Floor(UnityEngine.Random.Range(0f,1f) * (length - 1) );
			bestR = list_ribo[i];
		}

		return bestR; //return the closest
	}

	public bool freePore(int i = 0) 
	{
		return c_nucleus.freePore(i);
	}

	public void sendERProtein(Ribosome r, int i) 
	{
		ProteinCloud p = new ProteinCloud();
		p.x = r.x;
		p.y = r.y;
		p.setCell(this);
		p.setProduct(i);
		p.transform.SetParent(this.transform, false);
		addRunning(p);
		p.transform.SetSiblingIndex(c_er.transform.GetSiblingIndex() + 1);  //put the protein cloud just "above" the ER;

		askForERExit(p);
	}

	public TransportVesicle instantTransportVesicle(float xx, float yy, int product, float amount = 1, bool notifyEngine = true)
	{
		TransportVesicle t = makeTransportVesicle(product, amount);
		t.x = xx;
		t.y = yy;
		if (notifyEngine)
		{
			//p_engine.finishTransportVesicle();
		}
		return t;
	}

	public Peroxisome instantPeroxisome(float xx, float yy, bool notifyEngine = true)
	{
		Peroxisome p = makePeroxisome();
		p.x = xx;
		p.y = yy;

		//if (notifyEngine)
			//p_engine.finishPeroxisome();    //TODO
		return p;
	}

	public Lysosome instantLysosome(float xx, float yy, bool notifyEngine = true) 
	{
		Lysosome l = makeLysosome();
		l.x = xx;
		l.y = yy;
		onFinishLysosome(l);
		//if(notifyEngine)   //TODO
			//	p_engine.finishLysosome();
			return l;
	}

	public Lysosome budLysosome(float xx, float yy, float r)
	{
		Lysosome l = instantLysosome(xx, yy, false);
		
		l.transform.eulerAngles = new Vector3(0,0,r);
		l.bud();

		return l;
	}

	public void growLysosome(float xx, float yy) 
	{
		Lysosome l = instantLysosome(xx, yy);
		l.grow();
	}

	public void growPeroxisome(float xx, float yy) 
	{
		Peroxisome p = instantPeroxisome(xx, yy);
		p.grow();
	}

	public void growToxinVesicle(float xx, float yy, float amount = 1)
	{
		TransportVesicle t = instantTransportVesicle(xx, yy, Selectable.TOXIN, amount);
		t.grow();
	}

	public void growDefensinVesicle(float xx, float yy, float amount = 1)
	{
		
		TransportVesicle t = instantTransportVesicle(xx, yy, Selectable.DEFENSIN, amount);
		t.grow();
	}

	public void growFinalVesicle(Point p, int i)
	{
		//trace("GROW FINAL VESICLE! " + i);
		//LYSO_VES_FINISHED++;
		//trace("Cell.growFinalVesicle() : LYSO_VES_FINISHED= " + LYSO_VES_FINISHED);
		switch (i)
		{
			case Selectable.LYSOSOME: growLysosome(p.x, p.y); break;
			case Selectable.PEROXISOME: growPeroxisome(p.x, p.y); break;
			case Selectable.DEFENSIN: growDefensinVesicle(p.x, p.y, DEFENSIN_AMOUNT); break;
			case Selectable.TOXIN: growToxinVesicle(p.x, p.y, TOXIN_AMOUNT); break;
		}
	}

	public void growVesicle(ProteinCloud p, int i)
	{
		//LYSO_VES_STARTED++;
		//trace("Cell.growVesicle() : LYSO_VES_STARTED= " + LYSO_VES_STARTED);
		BlankVesicle v;
		switch (i)
		{

			case Selectable.PEROXISOME:
				//growFinalVesicle(
				growFinalVesicle(new Point(p.x, p.y), i);
				break;
			case Selectable.LYSOSOME:
			case Selectable.TOXIN:
			case Selectable.DEFENSIN:
				v = new PinkVesicle();
				v.x = p.x;
				v.y = p.y;
				v.setCell(this);
				v.setProduct(i);
				v.transform.SetParent(this.transform);
				list_ves.Add(v);
				addRunning(v);
				addSelectable(v);
				//swapChildren(v, r); //put it behind the Ribosome
				v.grow();
				break;
			case Selectable.MEMBRANE:
				growMembraneVesicle(p.x, p.y, 1);
				break;
		}
	}

	public void growMembraneVesicle(float xx, float yy, float amount = 1)
	{
		TransportVesicle t = instantTransportVesicle(xx, yy, Selectable.MEMBRANE, 1);
		t.growER();
	}

	public void generateVirusRNA(Virus v, int i, int rnaCount, int spawnCount, float xx, float yy, float r, bool startImmediately = false, bool evilDNA = false)
	{
		for (int j = 0; j < rnaCount; j++)
		{
			EvilRNA e = generateEvilRNA(i, spawnCount, v.wave_id, startImmediately, evilDNA, v.doesVesicle);
			e.x = xx;
			e.y = yy;
			if (v.mnode != null)
			{
				e.setMnode(c_membrane.findClosestMembraneHalf(e.x, e.y)); //to be SURE
																		  //e.setMnode(v.mnode);
			}
			else
			{
				//trace("Cell.generateVirusRNA() : no mnode!");
			}
			e.transform.eulerAngles = new Vector3(0,0,r);
		}
		if (!evilDNA)
		{
			//p_engine.onMakeEvilRNA(v.text_id, v.wave_id, rnaCount);  //TODO
		}
	}

	public void onVirusInfest(string s, int i)
	{
		//p_engine.onVirusInfest(s, i);  //TODO
	}

	public void onVirusEscape(string s, int i)
	{
		///p_engine.onVirusEscape(s, i);  //TODO
	}

	public void onVirusSpawn(string s, int i)
	{
		//p_engine.onVirusSpawn(s, i);  //TODO
	}

	public int getInfestWaveCount(string s) 
	{
		return 0;
		//return p_engine.getInfestWaveCount(s);  //TODO
	}

	public WaveEntry getWave(string s)
	{
		return null;
		//return p_engine.getWave(s);  //TODO
	}

	public void generateMembrane(int na) 
	{
		generateMRNA(Selectable.MEMBRANE, na);
	}

	public void generateToxin(int na) 
	{
		generateMRNA(Selectable.TOXIN, na);
	}

	public void generateDefensin(int na) 
	{
		generateMRNA(Selectable.DEFENSIN, na);
	}

	public bool generateLysosome(int na)
	{
		return generateMRNA(Selectable.LYSOSOME, na) != null;
	}

	public bool generatePeroxisome(int na)
	{
		return generateMRNA(Selectable.PEROXISOME, na) != null;
	}

	public bool generateDNARepair(int na)
	{
		return generateMRNA(Selectable.DNAREPAIR, na) != null;
	}

	public bool generateSlicer(int na) 
	{
		return generateMRNA(Selectable.SLICER_ENZYME, na) != null;
	}
	/**
	 * Starts the process of producing a ribosome
	 */

	public bool generateRibosome()
	{
		Ribosome r = makeRibosome(); //make the ribosome;
		Point p = c_nucleus.getPoreLoc(1); //get a nucleolus pore
		/*r.x = p.x + (Math.random()*30 - 15);
		r.y = p.y + (Math.random()*40 - 20);*/
		r.x = p.x;
		r.y = p.y - 10;
		r.playAnim("grow");
		//p_engine.plusRibosome()
		//p_engine.finishRibosome();  //TODO
		if (r) { return true; };
		return false;
	}

	/**
	 * Creates a Ribosome object
	 * @return a Ribosome object
	 */
	private Ribosome makeRibosome(bool instant_deploy = false)
	{
		GameObject ro = Instantiate(Ribosome_Prefab) as GameObject;
		Ribosome r = ro.GetComponent<Ribosome>();
		r.instant_deploy = instant_deploy;
		r.transform.SetParent(this.transform, false);
		r.setCell(this);
		list_ribo.Add(r);
		addSelectable(r);
		addRunning(r);
		updateBasicUnits();
		onMake("ribosome", list_ribo.Count);
		return r;
	}



	public void getPpodContract(float xx, float yy) 
	{
		int centnum = Selectable.CENTROSOME;
		foreach(CellObject c in list_running) 
		{
			if (c.num_id != centnum)
			{
				if (c != c_membrane && c != c_skeleton)
				{
					c.getPpodContract(xx, yy);
				}
			}
		}
		onCellMove(xx, yy);
	}

	public void moveCellTo(float xx, float yy)
	{
		float dx = xx - c_centrosome.x;
		float dy = yy - c_centrosome.y;
		moveCell(dx, dy);
	}

	public void moveCell(float xx, float yy)
	{
		//trace("Cell.moveCell(" + xx + "," + yy + ")");
		foreach(CellObject c in list_running) 
		{
			c.doCellMove(xx, yy);
		}
		onCellMove(xx, yy);
	}

	public void clearMouse()
	{
		if (c_membrane)
		{
			c_membrane.clearMouse();
		}
	}

	private void onCellMove(float xx, float yy)
	{
		c_membrane.onCellMove(xx, yy);

		//p_world.onCellMove(c_centrosome.x, c_centrosome.y);  //TODO
		//p_canvas.onCellMove(xx, yy);  //TODO
		
		updateObjectGridLoc();
		CellGameObject.setCentLoc(c_centrosome.x, c_centrosome.y);
		if (p_woz)
		{
			//p_woz.tickSpace();  //TODO
		}
		
	}

	/*****Organelle killing functions*****/

	public void killProteinCloud(ProteinCloud p)
	{
		int i = 0;
		foreach(CellGameObject g in list_running) {
			if (p == g)
			{
				list_running.RemoveAt(i);
			}
			i++;
		}
		p.transform.SetParent(null);
		p.destruct();
		p = null;
	}

	public void killBlankVesicle(BlankVesicle b)
	{
		int i = 0;
		foreach(BlankVesicle ves in list_ves) {
			if (ves == b)
			{
				list_ves.RemoveAt(i);
			}
			i++;
		}
		killRunning(b as CellGameObject);
		killSelectable(b as Selectable);
	}

	public bool sellSomething(int i) 
	{
			switch(i) {
				case Selectable.LYSOSOME: 
					return startRecycleLysosome();
					break;
				case Selectable.RIBOSOME: 
					return startRecycleRibosome();
					break;
				case Selectable.SLICER_ENZYME: 
					break;
				case Selectable.PEROXISOME: 
					break;
			}
		return false;
	}
		
	public bool startRecycleRibosome()
	{
		int length = list_ribo.Count;
		if (length > 0)
		{
			int j = length - 1;
			while (j >= 0)
			{
				if (!list_ribo[j].isBusy && !list_ribo[j].isRecycling)
				{
					return startRecycle(list_ribo[j]);
				}
				j--;
			}
		}
		return false;
	}

	public bool startRecycleLysosome() 
	{
		int length = list_lyso.Count;
			if (length > 0) 
			{
				int j = length-1;
				while (j >= 0) 
				{
					if(!list_lyso[j].isRecycling && list_lyso[j].is_active && !list_lyso[j].isBusy){
						return startRecycle(list_lyso[j]);
					}
					j--;
				}
			}
			

			return false;
	}
		
	public void neutralizeViruses()
	{
		foreach(Virus v in list_virus) 
		{
			v.neutralize();
		}
		//p_woz.neutralizeViruses();  //TODO
	}

	public void onMembraneHealthChange(int i) 
	{
		//p_interface.setMembraneHealth(i);  //TODO
	}

	public Boolean startNecrosis() 
	{
		neutralizeViruses();
		//var list_tubes:Vector.<Microtubule> = c_skeleton.getTubes();
		isCellDying = true;
		List<object> popArray = c_membrane.getPopPoints();
		int i = 0;
		List<Point> pp = popArray[0] as List<Point>;
		if (popArray[0] != null)
		{
			foreach(Point p in pp) 
			{
				if (popArray[1] != null)
				{
					List<float> pl = popArray[1] as List<float>;
					makeSplashBurst(pl[i], p.x, p.y, 1);
					//makePopVesicle(popArray[1][i], popArray[0].x, popArray[0].y, 1);
					i++;
				}
			}
		}

		c_membrane.gameObject.SetActive(false); //hide the membrane;
		_doNecrosisRoutine = StartCoroutine(doNecrosis());
		//loseResources([r_atp, r_na, r_aa, r_fa, r_g]);
		zeroResources();
		return true;
	}
		
	public void nucleusCallForHelp()
	{
		foreach(DNARepairEnzyme d in list_dnarepair) 
		{
			if (d.goingNucleus == false)
			{
				d.tryGoNucleus();
			}
		}
	}

	private IEnumerator doNecrosis() 
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (necrosisCount < NECROSIS_TIME + 1)
			{               //guaranteed to kill everything!
				float t = NECROSIS_TIME / 4;
				//loseResources([r_max_atp / t, r_max_na / t, r_max_aa / t, r_max_fa / t, r_max_g / t]);
				foreach(CellObject s in list_running) 
				{   //kill all my stuff
					if (s.dying == false && !(s is Virus))
					{ //don't kill anything that'd already dead, or a virus
						float health = s.getMaxHealth();
						if (health <= 100)
						{
							s.takeDamage(health / (NECROSIS_TIME / 4)); //wipe out low health fast
						}
						if (health <= 300)
						{
							s.takeDamage(health / (NECROSIS_TIME / 2)); //medium health semi fast
						}
						else
						{
							s.takeDamage(health / NECROSIS_TIME); //everything else slower
						}
					}
				}
			}

			necrosisCount++;
			if (necrosisCount >= NECROSIS_TIME * 0.75)
			{
				necrosisCount = 0;
				StopCoroutine(_doNecrosisRoutine);
				endNecrosis();
			}
		}
	}

	private void endNecrosis()
	{
		
		zeroResources();
		
		//p_engine.onNecrosis("lysis");   //TODO
	}

	public int getEngineSelectCode() 
	{
		return 0;//return p_engine.getSelectCode(); //TODO
	}

	public void engineUpdateSelected()
	{
		//p_engine.updateSelected();   //TODO
	}

	public void showToxinSpot(float amt, float x, float y) 
	{
		//p_engine.showToxinSpot(amt, x, y);   //TODO
	}

	public void showShieldBlock(float x, float y)
	{
		SfxManager.Play(SFX.SFXBlock);
		//p_engine.showShieldBlock(x, y);  //TODO
	}

	public void showShieldSpot(float amt, float x, float y)
	{
		//p_engine.showShieldSpot(amt, x, y);   //TODO
	}

	public void showHealSpot(float amt, float x, float y)
	{
		//p_engine.showHealSpot(amt, x, y);  //TODO
	}

	public bool canAfford(float atp, float na, float aa, float fa, float g) 
	{
		return p_engine.canAfford((int)atp, (int)na, (int)aa, (int)fa, (int)g);
	}

	public void cancelRecycle(CellObject c) 
	{
		if (c.isInVesicle == true)
		{
			c.myVesicle.cancelRecycle();
		}
	}

	public bool startRecycle(Selectable s, bool many = false) 
	{
		//trace("Cell.startRecycle( " + s + "="+s.text_id +")");
		switch(s.num_id) {
			case Selectable.RIBOSOME:
			case Selectable.LYSOSOME:
			case Selectable.MITOCHONDRION:
			case Selectable.PROTEIN_GLOB:
			case Selectable.SLICER_ENZYME:
			case Selectable.CHLOROPLAST:
			case Selectable.PEROXISOME:
			case Selectable.DNAREPAIR:
			float[] a = Costs.getRecycleCostByString(s.getTextID().ToUpper());//getRecycleCostByName(s);
			if(spendATP(a[0]) != 0)
			{
				return s.tryRecycle(many);
		}else
		{
			p_engine.showImmediateAlert(Messages.A_NO_AFFORD_RECYCLE);
			return false;
		}
		break;

		default:
			return lysosomeEatSomething(s);
		break;
		}
			//return success;
	}
		
	public void abortProduct(int i) 
	{
		//p_engine.abortProduct(i);  //TODO
	}

	public void makeStarburst(float xx, float yy) 
	{
		StarBurst s = new StarBurst();
		s.transform.position = new Vector3(xx, yy, 0);
		s.p_cell = this;
		s.transform.SetParent(this.transform, false);
		s.GotoAndPlay("burst");
		//trace("Cell.makeStarburst(" + xx + "," + yy + ")");
		SfxManager.Play(SFX.SFXHurt);
	}

	public void removeStarburst(StarBurst s)
	{
		s.transform.SetParent(null);
	}

	/**
	 * When you finish a lysosome, it looks to see if there is acid in the cytosol. If so, it sucks it out
	 * @param	l
	 */

	public void onFinishLysosome(Lysosome l)
	{
		if (ph_balance < 7.5f)
		{

			float new_ph = PH.removeFromPH(Lysosome.PH_BALANCE, l.getCircleVolumeX(),
											ph_balance, c_membrane.getCircleVolume());
			if (new_ph > 7.5f)
			{
				new_ph = 7.5f;
			}
			setPH(new_ph);
		}
	}

	public void onPopLysosome(Lysosome l)
	{
		onRecycle(l, true, true);
		onRecycle(l, true, true);
		float new_ph = PH.mergePH(Lysosome.PH_BALANCE, l.getCircleVolumeX(),
									   ph_balance, c_membrane.getCircleVolume());
		setPH(new_ph);
		//mergePH(Lysosome.ph_balance,l.getCircleVolume());
		//addAcid(l.acid_amount,l.getCircleVolume());
	}

	private void setPH(float ph)
	{
		ph_balance = ph;
		//p_interface.setPH(ph);  //TODO
		c_membrane.updatePH(ph_balance);
		onPHChange();
	}

	private void onUnitChange()
	{
		float lm = list_mito.Count;
		float lc = list_chlor.Count;
		float ls = list_slicer.Count;
		float lp = list_perox.Count;
		float ll = list_lyso.Count;

		FreeRadical.updateChances(lm, lc, ls, lp, ll);
	}

	private void onPHChange()
	{
		foreach(CellObject c in list_running) 
		{
			if (!c.isInVesicle)
			{
				c.setPHDamage(ph_balance);
			}
		}
	}

	public void onMembraneUpdate(bool hardUpdate = false)
	{
		float size = c_membrane.getRadius() * 3;
		float gridSize = size * 2; //twice as big as the radius = box
		//p_world.updateMaskSize(size);  //TODO
		updateObjectGrid(gridSize, gridSize, hardUpdate);
		updateObjectGridLoc();
	}

	public void updateObjectGridLoc()
	{
		c_objectGrid.transform.localPosition = new Vector3(cent_x - c_objectGrid.getSpanW() / 2, cent_y - c_objectGrid.getSpanH() / 2);
		

		//p_canvas.updateCanvasGridLoc(c_objectGrid.x, c_objectGrid.y);  //TODO
	}

	private void updateObjectGrid(float w, float h, bool hardUpdate = false)
	{
		c_objectGrid.wipeGrid();
		c_objectGrid.makeGrid(GRID_W, GRID_H, w, h);
		c_objectGrid.displayGrid();

		//update everything that needs to know the differences
		CellGameObject.setGrid(c_objectGrid);
		c_membrane.setObjectGrid(c_objectGrid);
		//p_canvas.updateCanvasGrid(GRID_W, GRID_H, w, h, hardUpdate);  //TODO
	}

	public void showGrid()
	{
		c_objectGrid.displayGrid();
		c_objectGrid.transform.SetParent(this.transform, false);
		//p_canvas.showGrid(); //TODO
	}

	public void hideGrid()
	{
		if (c_objectGrid.transform.parent == this.transform)//(getChildByName(c_objectGrid.name))
			c_objectGrid.transform.SetParent(null);
		//p_canvas.hideGrid();  //TODO
	}


	public void onRecycle(Selectable s, bool reimburse = true, bool announce = false)
	{
		//trace("Cell.onRecycle() " + s.text_id + " reimburse="+reimburse);
		if (reimburse && !isCellDying)
		{
			if (announce)
			{
				if (s.recycleOfMany)
				{
					//p_engine.recycleSomethingOfMany(s.num_id, new Point(s.x, s.y));  //TODO
				}
				else
				{
					//p_engine.recycleSomething(s.num_id, new Point(s.x, s.y));  //TODO
				}
			}
			else
			{
				//p_engine.recycleSomething(s.num_id);  //TODO
			}
		}
		killSomething(s);
	}

	public bool bigVesicleRecycleSomething(CellObject c) 
	{
		BigVesicle v = growBigVesicleFor(c);
		if (v) {
			c.makeDoomed();
			return true;
		}
		return false;
	}

	public bool lysosomeEatSomething(Selectable s) 
	{
			
		Lysosome l = findClosestLysosome(s.x, s.y);
		if (l) {
				return l.eatSomething(s);
			}

		//if we got here, we failed

		if (list_lyso.Count < 1)
		{ //if we have no lysosomes
			if (!(s is Virus))
			{           //don't show this alert if it's a virus we're eating
				//p_engine.showAlert(Messages.A_NO_LYSO_R); //Alert that we need lysosomes for recyclnig  //TODO
			}
		}

			return false;
	}
		
	public void killSomething(Selectable s) 
	{
		switch (s.num_id)
		{
			case Selectable.NUCLEUS:
				killNucleus(s as Nucleus);
				break;
			case Selectable._ER:
				killER(s as ER);
				break;
			case Selectable.CENTROSOME:
				killCentrosome(s as Centrosome);
				break;
			case Selectable.GOLGI:
				killGolgi(s as Golgi);
				break;
			case Selectable.LYSOSOME:
				killLysosome(s as Lysosome);
				break;
			case Selectable.PEROXISOME:
				killPeroxisome(s as Peroxisome);
				break;
			case Selectable.RIBOSOME:
				killRibosome(s as Ribosome);
				break;
			case Selectable.VESICLE:
				killBlankVesicle(s as BlankVesicle);
				break;
			case Selectable.CHLOROPLAST:
				killChloroplast(s as Chloroplast);
				break;
			case Selectable.MITOCHONDRION:
				killMitochondrion(s as Mitochondrion);
				break;
			case Selectable.BIGVESICLE:
				killBigVesicle(s as BigVesicle);
				break;
			case Selectable.PROTEIN_GLOB:
				killProteinGlob(s as ProteinGlob);
				break;
			case Selectable.SLICER_ENZYME:
				//trace("Cell.killSomething() slicerEnzyme(" + s + ")");
				killSlicerEnzyme(s as SlicerEnzyme);
				break;
			case Selectable.DNAREPAIR:
				killDNARepair(s as DNARepairEnzyme);
				break;
			case Selectable.FREE_RADICAL:
				killFreeRadical(s as FreeRadical);
				break;
			case Selectable.VIRUS:
			case Selectable.VIRUS_INFESTER:
			case Selectable.VIRUS_INJECTOR:
			case Selectable.VIRUS_INVADER:
				killVirus(s as Virus);
				break;
			default:
				Debug.LogError("Cell.killSomething() : I don't recognize code # " + s.num_id);
				break;
		}
	}

	public void killRunning(CellGameObject g)
	{
		int i = 0;
		foreach(CellGameObject gg in list_running) {
			if (g == gg)
			{
				list_running.RemoveAt(i);
			}
			i++;
		}
		if (g is HardPoint)
		{

		}
		else
		{
			g.transform.SetParent(null);
		}
		g.destruct();
		g = null;
	}

	public void killSelectable(Selectable s)
	{
		int i = 0;
		foreach(Selectable ss in list_selectable) 
		{
			if (s == ss)
			{
				list_selectable.RemoveAt(i);
			}
			i++;
		}
		dirty_selectList = true;
	}

	public void killPeroxisome(Peroxisome p)
	{
		int i = 0;
		foreach(Peroxisome pp in list_perox) 
		{
			if (pp == p)
			{
				list_perox.RemoveAt(i);
			}
			i++;
		}
		if (p.orderOnDeath)
		{

			//p_engine.replacePerox();//TODO  // (BasicUnit.PEROXISOME, 1);
		}
		//p_engine.oneLessPeroxisome();  //TODO
		onKill(p.text_id, list_perox.Count);
		killSelectable(p as Selectable);
		killRunning(p as CellGameObject);
	}

	public void killVirus(Virus v)
	{
		int i = 0;
		foreach(Virus vv in list_virus) {
			if (vv == v)
			{
				list_virus.RemoveAt(i);
			}
			i++;
		}

		killRunning(v as CellGameObject);

		
		//p_engine.onKillVirus(v.wave_id);  //TODO
	}

	public void onAbsorbRadical(Peroxisome p)
	{
		p.transform.SetSiblingIndex(this.transform.childCount - 1);  //so it doesn't get buried under organelles
		//p_engine.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "absorb_radical", "null", 1);  //TODO
	}

	public void killFreeRadical(FreeRadical f)
	{
		int i = 0;
		foreach(FreeRadical ff in list_radical) 
		{
			if (ff == f)
			{
				list_radical.RemoveAt(i);
			}
			i++;
		}
		onKill(f.text_id, list_radical.Count);
		killSelectable(f as Selectable);
		killRunning(f as CellGameObject);
	}

	public void killDNARepair(DNARepairEnzyme d)
	{
		int i = 0;
		foreach(DNARepairEnzyme dd in list_dnarepair) {
			if (dd == d)
			{
				list_dnarepair.RemoveAt(i);
			}
			i++;
		}
		onKill(d.text_id, list_dnarepair.Count);
		//p_engine.oneLessDNARepair();  //TODO
		killSelectable(d as Selectable);
		killRunning(d as CellGameObject);
	}

	public void killHardPoint(HardPoint h)
	{
		int i = 0;
		//removeChild(h);
		foreach(HardPoint hh in list_hardpoint) {
			if (hh == h)
			{
				list_hardpoint.RemoveAt(i);
			}
			i++;
		}
		killSelectable(h as Selectable);
		killRunning(h as CellGameObject);
	}

	public void killSlicerEnzyme(SlicerEnzyme s)
	{
		//trace("Cell.killSlicerEnzyme() " + s );
		int i = 0;
		foreach(SlicerEnzyme ss in list_slicer) 
		{
			if (ss == s)
			{
				list_slicer.RemoveAt(i);
			}
			i++;
		}
		onKill(s.text_id, list_slicer.Count);
		//p_engine.oneLessSlicer();  //TODO
		killSelectable(s as Selectable);
		killRunning(s as CellGameObject);

	}

	public void killProteinGlob(ProteinGlob g)
	{
		int i = 0;
		foreach(ProteinGlob pg in list_junk) 
		{
			if (pg == g)
			{
				list_junk.RemoveAt(i);
			}
			i++;
		}

		killRunning(g as CellGameObject);
		killSelectable(g as Selectable);
	}

	public void unDoomCheck(CellObject c)
	{
		//p_engine.unDoomCheck(c);  //TODO
	}

	public void killBigVesicle(BigVesicle b)
	{

		int i = 0;
		foreach(BigVesicle bi in list_bigves) 
		{
			if (b == bi)
			{
				list_bigves.RemoveAt(i);
			}
			i++;
		}
		onKill(b.text_id, list_bigves.Count);
		killRunning(b as CellGameObject);
		killSelectable(b as Selectable);
	}



	public void killNucleus(Nucleus n)
	{
		killRunning(n as CellGameObject);
		killSelectable(n as Selectable);
		onKill(n.text_id, 0);
		c_nucleus = null;
		checkNucleusScrewed();
		
	}

	public void killER(ER er)
	{
		killRunning(er as CellGameObject);
		killSelectable(er as Selectable);
		onKill(er.text_id, 0);
		c_er = null;
		//trace("Cell.killER() GAME OVER!");
	}

	public void killGolgi(Golgi g)
	{
		killRunning(g as CellGameObject);
		killSelectable(g as Selectable);
		onKill(g.text_id, 0);
		c_golgi = null;
		//trace("Cell.killGolgi()!");
	}

	public void killCentrosome(Centrosome cent)
	{
		
	}

	public void checkScrewed(CellObject c)
	{
		switch (c.num_id)
		{
			case Selectable.MITOCHONDRION: checkMitoScrewed(); break;
			case Selectable.CHLOROPLAST: checkChloroScrewed(); break;
			case Selectable.NUCLEUS: checkNucleusScrewed(); break;
		}
	}

	public void checkNucleusScrewed()
	{ //ONLY called by the nucleus when health is <= 0!
		if (!isCellDying)
		{ //so it doesn't double fire on lysis
			//p_engine.showScrewedMenu("gameover", "no_nucleus");  //TODO
		}
	}

	private void checkMitoScrewed()
	{
		if (!isCellDying)
		{
			int count = 0;
			foreach(Mitochondrion mi in list_mito) 
			{
				if (mi.getDamageLevel() < 2)
				{
					count++;
				}
			}
			if (count <= 0)
			{
				//p_engine.showScrewedMenu("screwed", MenuSystem_Screwed.NO_MITO);  //TODO
			}
		}
	}

	private void checkChloroScrewed()
	{
		if (!isCellDying)
		{
			int count = 0;
			if (lvl_sunlight > 0)
			{ //if there's no sunlight, who cares?
				foreach(Chloroplast ch in list_chlor) 
				{
					if (ch.getDamageLevel() < 2)
					{
						count++;
					}
				}
				if (count <= 0)
				{
					//p_engine.showScrewedMenu("screwed", MenuSystem_Screwed.NO_CHLORO);  //TODO
				}
			}
		}
	}

	public void killMitochondrion(Mitochondrion m)
	{
		int i = 0;
		foreach(Mitochondrion mi in list_mito) {
			if (m == mi)
			{
				list_mito.RemoveAt(i);
			}
			i++;
		}

		onKill(m.text_id, list_mito.Count);
		killRunning(m as CellGameObject);
		killSelectable(m as Selectable);
		mitoCount = list_mito.Count;
		//p_engine.setMitoCount(mitoCount);  //TODO
		checkMitoScrewed();
	}

	public void killToxinParticle(ToxinParticle t)
	{
		int i = 0;
		int t_count = 0;
		foreach(CellObject c in list_junk) 
		{
			if (c is ToxinParticle)
			{
				t_count++;
			}
			if (t == c)
			{
				list_junk.RemoveAt(i);
			}
			i++;
		}
		onKill(t.text_id, t_count);
		killRunning(t as CellGameObject);
		killSelectable(t as Selectable);
	}

	public void killChloroplast(Chloroplast c)
	{
		int i = 0;
		foreach(Chloroplast ch in list_chlor) 
		{
			if (c == ch)
			{
				list_chlor.RemoveAt(i);
			}
			i++;
		}
		onKill(c.text_id, list_chlor.Count);
		killRunning(c as CellGameObject);
		killSelectable(c as Selectable);
		chloroCount = list_chlor.Count;
		//p_engine.setChloroCount(chloroCount);  //TODO
		checkChloroScrewed();
	}

	public void killRibosome(Ribosome r)
	{
		int i = 0;
		foreach(Ribosome ri in list_ribo) 
		{
			if (r == ri)
			{
				list_ribo.RemoveAt(i);
			}
			i++;
		}
		onKill(r.text_id, list_ribo.Count);
		//p_engine.oneLessRibosome();  //TODO
		killRunning(r as CellGameObject);
		killSelectable(r as Selectable);
	}

	public void killSplashBurst(SplashBurst s)
	{
		killRunning(s as CellGameObject);
	}

	public void killLysosome(Lysosome l)
	{
		int i = 0;
		foreach(Lysosome ly in list_lyso) 
		{
			if (l == ly)
			{
				list_lyso.RemoveAt(i);
			}
			i++;
		}
		onKill(l.text_id, list_lyso.Count);

		if (!l.fusing)
		{ //if the Lysosome is just going to merge with a vesicle, don't tell the engine
			//p_engine.oneLessLysosome();  //TODO
		}

		killRunning(l as CellGameObject);
		killSelectable(l as Selectable);
	}

	public void killRNA(RNA r)
	{ //This function could use some optimization
		int i = 0;
		if (r is EvilRNA)
		{
			//p_engine.recycleRNA(0);  //TODO
		}
		else
		{
			//p_engine.recycleRNA(r.getNAValue());  //TODO
		}
		foreach(RNA rna in list_rna) 
		{
			if (rna == r)
			{
				list_rna.RemoveAt(i);
			}
			i++;
		}
		i = 0;
		foreach(CellGameObject g in list_running) 
		{
			if (r == g)
			{
				list_running.RemoveAt(i);
			}
			i++;
		}
		

		if (r is EvilRNA && !(r is EvilDNA))
		{
			//p_engine.onKillEvilRNA(r.getProductCreator());  //TODO
		}
		onKill(r.text_id, list_rna.Count);

		r.transform.SetParent(null);
		r.destruct();
		r = null;
	}

	public void onNucleusInfest(Boolean b)
	{
		//p_engine.onNucleusInfest(b);  //TODO
	}

	public float checkMembraneStrength(CellObject c)
	{
		List<MembraneNode> list = c_membrane.getClosestNodes(c.x, c.y, gravRadius2);
			//trace("CHeck membrane strength");
			float worstStretch = 10;
		foreach(MembraneNode m in list)
		{
			//trace("Stretch = " + m.stretch);
			if (m.stretch < worstStretch)
			{
				worstStretch = m.stretch;
			}
		}
			//trace("worstStretch = " + worstStretch);
			return worstStretch;
	}

	public void updateOrganelleAct(String s, List< CellAction > v) 
	{
		//var l:Vector.<CellObject>;
		if (s == "nucleus") c_nucleus.setActions(v);
		else if (s == "centrosome") c_centrosome.setActions(v);
		else if (s == "er") c_er.setActions(v);
		else if (s == "golgi") c_golgi.setActions(v);
		else if (s == "mitochondrion")
		{
			foreach(Mitochondrion m in list_mito) 
			{
				m.setActions(v);
			}
		}
		else if (s == "chloroplast")
		{
			foreach(Chloroplast c in list_chlor) 
			{
				c.setActions(v);
			}
		}
		else if (s == "slicer")
		{
			foreach(SlicerEnzyme se in list_slicer) 
			{
				se.setActions(v);
			}
		}
		else if (s == "ribosome")
		{
			foreach(Ribosome r in list_ribo) 
			{
				r.setActions(v);
			}
		}
		else if (s == "peroxisome")
		{
			foreach(Peroxisome p in list_perox) 
			{
				p.setActions(v);
			}
		}
		else if (s == "lysosome")
		{
			foreach(Lysosome l in list_lyso) 
			{
				l.setActions(v);
			}
		}
	}

	public List<CellAction> getActionListFromEngine(int i)
	{
		List<CellAction> v = new List<CellAction>();//p_engine.lookupActionList(i);  //TODO
		return v;
	}
		
		/**
		 * Gets the cost to ppod to that location
		 * @param	xx yy
		 * @return cost of movement
		 */
		
	public float getPPodCost(float xx, float yy)
	{
		Point p = new Point(xx, yy);
		//p = p_world.transformPoint(p);  //TODO
		float dx = p.x - cent_x;
		float dy = p.y - cent_y;
		float d2 = (dx * dx) + (dy * dy);
		d2 /= Costs.MOVE_DISTANCE2;
		float cost = (Costs.PSEUDOPOD[0] * d2);
		return cost;
	}
		

































































































}

