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

	private var volume:Number;
		
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

	private Interface p_interface;
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

	private ObjectGrid c_objectGrid;
	public static bool SHOW_GRID = false;
		
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

    public Cell()
	{
		//animateOff();
	}

	public void init()
	{
		setCentLoc(0, 0);
		makeObjectGrid();
		setChildren();
		makeLists();
		_runRoutine = StartCoroutine(run());

		//addEventListener(MouseEvent.MOUSE_DOWN, mouseDown);  //TODO: make sure this listener happens in editor 
	}

	public override void destruct()
	{
		p_director = null;
		p_engine = null;
		//p_world = null;  //TODO
		p_canvas = null;
		p_interface = null;
		
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
			g = null;
		}
	}

	public void mouseDown()
	{
	
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

	public function setResources(atp:int, na:int, aa:int, fa:int, g:int, max_atp:int, max_na:int, max_aa:int, max_fa:int, max_g:int)
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
		c_membrane.setCanvasGrid(p_canvas.c_cgrid);
		//p_engine.updateMaxResources();  //TODO
	}

	public void buyToxin()
	{
		float[] cost = Costs.getMAKE_TOXIN(1);
		spend(cost);
	//	p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_toxin", 1);   //TODO

		generateToxin(cost[1]);
	}

	public void buyDefensin()
	{
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_buy_defensin", Director.level, 1); }
		float[] cost = Costs.getMAKE_DEFENSIN(1);
		spend(cost);
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_defensin", 1);  //TODO
		generateDefensin(cost[1]);
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
			if (spendATP(atpCost, p)) {
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
					if(spendATP(atpCost, p))
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
			float ex = c_er.x + c_er.exit23.x;
			float ey = c_er.y + c_er.exit23.y;
			float[] cost = Costs.getMAKE_MEMBRANE(1);
			spend(cost);// , new Point(ex, ey), 1, 0, false, true);
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_buy_membrane", Director.level, 1); }
		//p_engine.notifyOHandler(EngineEvent.EXECUTE_ACTION, "null", "make_membrane", 1);  //TODO
			generateMembrane(cost[1]);
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
		float[] cost = Costs.SELL_MEMBRANE.concat();
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
	
				p_engine.updateMaxResources();
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
		swapChildren(v, t); //make v under t
	}

	private BigVesicle growBigVesicleFor(CellObject c)
	{
			BigVesicle v = makeBigVesicle();
	v.x = c.x;
			v.y = c.y;
			v.setPH(ph_balance);
			v.startDigestGrow(c);
			swapChildren(v, c); //make v under s
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
		//var c:Vector.<Number> = circlePointsOffset(230,n,0,0);
		int i = 0;
		for (i = 0; i < n; i++)
		{
			SlicerEnzyme s = instantSlicer(0, 0);
			s.clip.GotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartLysosomes(int n)
	{
		List<float> c = circlePointsOffset(230, n, 0, 0);
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
		List<float> c = circlePointsOffset(230, n, 0, 0);
		int i = 0;
		for (i = 0; i < n; i++)
		{
			//p_engine.startAndFinishPeroxisome();  //TODO
			Peroxisome p = instantPeroxisome(0, 0, false);
			p.deployGolgi(true);
			p.isBusy = false; //HACK HACK HACK
			p.is_active = true;
			p.clip.clip.gotoAndPlay(i); //offset their animations so it looks nice; HACK
		}
	}

	private void makeStartRibosomes(int n)
	{
		List<float> c = circlePointsOffset(230, n, 0, 0);
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
		List<float> c = circlePointsOffset(300, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			Mitochondrion m = makeMitochondrion();
			m.x = c[i * 2];
			m.y = c[i * 2 + 1];
		}
	}

	private void makeTestLysosomes(int n)
	{
		List<float> c = circlePointsOffset(250, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			instantLysosome(c[i * 2], c[i * 2 + 1]);
			//var l:Lysosome = growLysosome(c[i * 2],c[i * 2 + 1]);
			//l.is_active = true;
		}
	}

	private void makeTestChloroplasts(int n)
	{
		List<float> c = circlePointsOffset(280, n, 0, 0);
		for (int i = 0; i < n; i++) 
		{
			Chloroplast l =  makeChloroplast();
			l.x = c[i * 2];
			l.y = c[i * 2 + 1];
		}
	}

	private void makeTestRibosomes(int n)
	{
		List<float> c = circlePointsOffset(230, n, 0, 0);
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

	public void setInterface(Interface i)
	{
		p_interface = i;
	}

	public override float getCircleVolume()
	{
			return c_membrane.getCircleVolume();
	}

/*************************/

	private void makeObjectGrid()
	{
		//trace("Cell.makeObjectGrid()");
		c_objectGrid = new ObjectGrid();
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
			swapChildren(thing1, thing2);
		}
		else
		{
			if (t1 < t2)
			{
				swapChildren(thing1, thing2);
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
			if (p)
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
		if (p)
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
		if (p)
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
			Virus v;
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
		DNARepairEnzyme d = new DNARepairEnzyme();
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
			listPoints.Add(circlePointsOffset(CanvasObject.LENS_RADIUS * (1.1 + mult), ring_list[count], cent_x, cent_y));
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
		for (int i = 0; i < w.count; i++) 
		{

			float jiggX = (UnityEngine.Random.Range(0f,1f) - 0.5f) * 150;
			float jiggY = (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 150;
			float theX = listCirclePoints[i * 2] + jiggX;
			float theY = listCirclePoints[(i * 2) + 1] + jiggY;

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
		Virus v;
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
		












































}

