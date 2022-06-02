using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


public class Ribosome : BasicUnit
{
//
	//normal, grow, wait,dock, process, process_inplace, recycle 
	public bool instant_deploy = false;
		//private var busy:Boolean = false;
	private bool ready = false;
	private bool deploying = false;
		
	private DockPoint p_dock;
	private RNA p_rna;
	private bool er_wait = false; //are we waiting for a free docking point
	private int er_count = 0;
	private const int ER_MAX = 60; //poll every second for a new one
	private int product = Selectable.NOTHING; //what we're going to make
	private bool product_virus_vesicle = false;
	private string product_creator_id = ""; //used mostly for virus waves to keep track of child viruses
	private int product_count = 0; //how many we're going to make
	private bool movingToER = false;
	private bool processing = false;
	//private Timer busyTimer;
	private int busy_count = 0;
	private int BUSY_TIME = 60; //1 second  //Note: was 30. Assuming 60 frames/second with Unity.  
		
	private int process_count = 0;
	private int MAX_PROCESS_TIME = 150; //5 seconds (thread time ~ 110)
    private Coroutine _checkBusyTimeRoutine;
    private Coroutine _processFailSafeRoutine;
    private Coroutine _waitERroutine;

    public override void Start()
	{
		base.Start();
		canSelect = false;
		//buttonMode = false;
		//mouseEnabled = false;  //TODO:?
		singleSelect = false;
		text_title = "Ribosome";
		text_description = "A \"factory\" - builds things from an RNA \"blueprint\"";
		text_id = "ribosome";
		num_id = Selectable.RIBOSOME;
		bestColors = new bool[] { false, false, true };
		setMaxHealth(25, true);
		speed = 4;
		does_recycle = true;

		//busyTimer = new Timer(BUSY_TIME);
		_checkBusyTimeRoutine = StartCoroutine(checkBusyTime());
		init();

	}

	public override void destruct()
	{
		base.destruct();
		p_rna = null;
		product = NOTHING;
		product_count = 0;
	}

	public void makeBusy()
	{
		isBusy = true;
		busy_count = 0;
	}

	private IEnumerator checkBusyTime()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			busy_count++;
			if (busy_count > BUSY_TIME)
			{
				busy_count = 0;
				Debug.Log("Ribosome.chcekBusyTime() MAKE FREE");
				makeFree();
				StopCoroutine(_checkBusyTimeRoutine);
				/*}else {
					if (p_rna == null) {
						trace("Ribosome.checkBusyTime() I AM TIRED OF WAITING!");
						finishProduct();
					}
				}*/
			}
		}
	}

	public void checkFixBusy()
	{
		if (product == NOTHING)
		{
			makeFree();
		}
	}

	/**
	 * Tell the Ribosome it is free to accept another RNA. Will only make the Ribosome not busy if it's
	 * not doing anything else
	 */

	public void makeFree()
	{
		if (product == Selectable.NOTHING)
		{ //if I'm not trying to make some product
			if (p_rna == null)
			{
				isBusy = false;
				//this.transform.colorTransform = new ColorTransform( 1, 1, 1, 1, 0, 0, 0, 0);
			}
		}
		else
		{
			//trace("Ribosome.makeFree() FAIL! I've got stuff! product="+product + " p_rna==null : " + (p_rna == null));
		}
	}

	public override bool tryRecycle(bool oneOfMany = false) 
	{
			if(!isBusy){
				return base.tryRecycle(oneOfMany);
			}
return false;
	}
		
		
	public bool isReady()
	{
		return ready;
	}

	protected override void killMe()
	{
		if (product != Selectable.NOTHING)
		{
			p_cell.abortProduct(product);
		}

		//trace("Ribosome.killMe()!" + name);
		base.killMe();
	}

	public RNA getRNA() 
	{
			return p_rna;
	}

	public void setRNA(RNA m) {
		p_rna = m;
	}

	public bool getProduct(int i, int count = 1, string pc_id = "",bool pc_vesicle = false)
	{
		if (!dying)
		{
			if (product == Selectable.NOTHING)
			{

				product = i;
				product_count = count;
				product_creator_id = pc_id; //mostly used to keep track of virus waves
				product_virus_vesicle = pc_vesicle;
				//p_rna = null; //Don't need you anymore
				return true;
			}
			else
			{
				Debug.LogError("Ribosome.getProduct()! I've already got one!");
				return false;
			}
		}
		return false;
	}

	public void checkProduct(int i)
	{
		//trace("Ribosome.checkProduct()");

		if (!dying)
		{
			bool vesicle = false;
			switch (i)
			{
				//Bugs if you don't use Selectable.LYSOSOME, etc
				case Selectable.VIRUS:
				case Selectable.VIRUS_INFESTER:
				case Selectable.VIRUS_INJECTOR:
				case Selectable.VIRUS_INVADER:
				case Selectable.DNAREPAIR:
				case Selectable.SLICER_ENZYME: vesicle = false; break;
				case Selectable.DEFENSIN:
				case Selectable.TOXIN:
				case Selectable.LYSOSOME:
				case Selectable.MEMBRANE:
				case Selectable.PEROXISOME: vesicle = true; break;



				default: Debug.Log("Ribosome does not recognize product # : " + i + ")"); break;// throw new Error("Ribosome does not recognize product #:" + i); break;
			}
			if (vesicle)
			{
				//trace("Ribosome.checkProduct(" + i + ") TRUE!");
				dockWithER();
			}
			else
			{
				produceInPlace();
			}
		}
		else
		{
			Debug.LogError("Ribosome can't check product, is dying!");
		}
	}

	public void produceInPlace()
	{
		if (!dying)
		{
			makeBusy();
			if (p_rna)
			{
				//startProcess();
				playAnim("process_inplace");
				//addEventListener(RunFrameEvent.RUNFRAME, processFailSafe, false, 0, true);
				p_rna.threadRNA(true);
			}
		}
	}

	private IEnumerator processFailSafe()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			process_count++;
			if (process_count > MAX_PROCESS_TIME)
			{
				process_count = 0;
				//removeEventListener(RunFrameEvent.RUNFRAME, processFailSafe);
				makeFree();
				if (isBusy)
				{
					//cancelRNA();
					//makeFree();
					/*if (p_rna) {
						if (p_rna.slicer_killed) {
							cancelRNA();
							makeFree();
							trace("Ribosome.processFailSafe() HARD FREE!");
						}
					}*/
					Debug.LogError("WHat the HECK");
					//trace("Ribosome.processFailSafe() ERROR!");
				}

				if (!isBusy)
				{
					
					StopCoroutine(_processFailSafeRoutine);
				}
			}
		}
	}

	public void dockWithER()
	{
		//trace("Ribosome.dockWithER()");
		if (!dying)
		{
			makeBusy();
			if (p_cell.dockRibosomeER(this))
			{ //if it succeeded
			  //that's cool
			  //trace("Ribosome.dockWithER() Success!");
				movingToER = true;
				if (er_wait)
				{ //stop waiting
					StopCoroutine(_waitERroutine);
					er_wait = false;
					er_count = 0;
				}
			}
			else
			{
				//trace("Ribosome.dockWithER() FAILURE!");
				er_wait = true; //start waiting
				er_count = 0;
				_waitERroutine = StartCoroutine(waitER());
			}
		}
	}

	public void finishRNA(bool inPlace= false)
	{
		if (inPlace)
		{
			onAnimFinish(ANIM_PROCESS_INPLACE);
		}
		else
		{
			onAnimFinish(ANIM_PROCESS);
		}
	}

	public void cancelRNA()
	{
		//trace("Ribosome.cancelRNA()!");
		p_rna = null;
		product = Selectable.NOTHING;
		product_count = 0;
		hardRevertAnim();
		makeFree();
	}

	private IEnumerator waitER()
	{
		while (true)
		{
			er_count++;
			if (er_count > ER_MAX)
			{
				er_count = 0;
				dockWithER();
			}
		}
	}

	private void makeProduct()
	{
		if (!dying)
		{
			if (p_rna)
			{
				//startProcess();
				playAnim("process");
				p_rna.threadRNA();
				//addEventListener(RunFrameEvent.RUNFRAME, processFailSafe, false, 0, true);
			}
		}
		//trace("RIBOSOME Make some " + product);
	}

	private void finishProduct()
	{
		if (!dying)
		{

			switch (product)
			{
				case Selectable.LYSOSOME: p_cell.sendERProtein(this, Selectable.LYSOSOME); break;
				case Selectable.TOXIN: p_cell.sendERProtein(this, Selectable.TOXIN); break;
				case Selectable.DEFENSIN: p_cell.sendERProtein(this, Selectable.DEFENSIN); break;
				case Selectable.MEMBRANE: p_cell.sendERProtein(this, Selectable.MEMBRANE); break;
				case Selectable.PEROXISOME: p_cell.sendERProtein(this, Selectable.PEROXISOME); break;
				case Selectable.VIRUS:
				case Selectable.VIRUS_INJECTOR:
				case Selectable.VIRUS_INVADER:
				case Selectable.VIRUS_INFESTER: p_cell.spawnRibosomeVirus(this, product, product_count, product_creator_id, product_virus_vesicle); break;
				case Selectable.SLICER_ENZYME: p_cell.spawnRibosomeSlicer(this, product_count); break;
				case Selectable.DNAREPAIR: p_cell.spawnRibosomeDNARepair(this, product_count); break;
				default: Debug.Log("Ribosome.finishProduct()" + product); break;
			}
			p_rna = null;
			product = Selectable.NOTHING; //i've got no product now
			product_virus_vesicle = false;
			//it's not free until it deploys and goes back home!
			p_dock = null;
		}
	}

	private void revertAnim()
	{
		GotoAndStop(1);
		clip.gameObject.SetActive(true);
	}

	public override void playAnim(string label)
	{
		if (label == "grow")
		{
			//if (Math.random() > 0.5) {
			SfxManager.Play(SFX.SFXPop1);
			//Director.startSFX(SoundLibrary.SFX_POP1);
			//}else{
			//	Director.startSFX(SoundLibrary.SFX_POP2);
			//}
		}
		base.playAnim(label);
	}

	public void ribosomeDeploy(bool instant = false)
	{
		/*if (home) {
			if (instant) {
				x = home.x + cent_x;
				y = home.y + cent_y;
				ready = true;
			}else {
				deploying = true;
				moveToPoint(home, FLOAT, true);
			}
		}else{*/
		p_rna = null;
		product = NOTHING; //just to be safe
		deploying = true;
		deployCytoplasm(p_cell.c_nucleus.x, p_cell.c_nucleus.y, .90f, .20f, true, instant);
		if (instant)
		{
			ready = true; //HACK HACK HACK
		}
		//}
		//when deploying is done, it is made free 
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		//trace("Ribosome.onAnimFinish() " + i + " isDying = " + dying);
		if (!dying)
		{   //same as the super.onAnimFinish, but doesn't gotoAndStop(1);
			if (stop)
			{
				clip.Play();
				anim_vital = false;
				StopCoroutine(_doAnimRoutine);
				//removeEventListener(RunFrameEvent.RUNFRAME, doAnim);
			}

			switch (i)
			{
				case ANIM_GROW: ribosomeDeploy(instant_deploy); revertAnim(); break;
				case ANIM_DOCK: makeProduct(); break;
				case ANIM_PROCESS:
					processing = false;
					finishProduct();
					revertAnim();
					ribosomeDeploy(); break; //deploy makes it free!
				case ANIM_PROCESS_INPLACE:
					processing = true;
					finishProduct();
					revertAnim();
					makeFree(); break;
			}
		}
		else
		{ //handle the dying cases
		  //trace("Ribosome.onAnimFinish() DIE! " + name);
			switch (i)
			{
				case ANIM_DIE: break;
				case ANIM_RECYCLE:
					onRecycle();
					break;
			}
		}
	}

	protected override IEnumerator onDeadTimer(float delay)
	{
		yield return new WaitForSeconds(delay);
		//trace("Ribosome.onDeadTimer()!" + name);
		onRecycle(); //complexify this when its possible to die AND recycle separately

	}

	protected override void onRecycle()
	{
		//trace("Ribosome.onAnimFinish() RECYCLE!" + name);
		revertAnim();
		p_cell.onRecycle(this, true, false);
		//removeEventListener(TimerEvent.TIMER, onDeadTimer);  //TODO: ?  Figure out a suitable revision for this 
	}

	public void doDock()
	{
		if (p_rna)
		{
			playAnim("dock");
			p_rna.descend();
		}
		else
		{
			ready = true;
		}
	}

	protected override void arrivePoint(bool wasCancel = false)
	{
		if (!wasCancel)
		{
			onArrivePoint();
		}

		if (pt_dest != null)
		{
			x = pt_dest.x;
			y = pt_dest.y;
		}

		isMoving = false;

		StopCoroutine(_doMovePointRoutine);//removeEventListener(RunFrameEvent.RUNFRAME, doMoveToPoint);
	}


	protected override void onArrivePoint()
	{
		if (!dying)
		{
			if (movingToER)
			{
				doDock();
				movingToER = false;
			}
			else
			{
				ready = true;
			}

			if (deploying)
			{
				deploying = false;
				makeFree();
			}
		}
	}

	public void setDockPoint(DockPoint d, int xoff, int yoff)
	{
		p_dock = d.copy();
		pt_dest = new Point(p_dock.x + xoff, p_dock.y + yoff - 10);//10 pixels higher to account for my "landing" animation
		moveToPoint(pt_dest, FLOAT, true);
	}

	protected override void doMoveToPoint()
	{
		base.doMoveToPoint();
		if (p_rna)
		{
			p_rna.x = x;
			p_rna.y = y;
		}
	}

	public override void startGetEaten()
	{
		base.startGetEaten();

		if (p_rna)
		{
			p_rna.hardKill();
		}
	}

	public override void doRecycle()
	{

		base.doRecycle();
		playAnim("recycle");
	}















}

