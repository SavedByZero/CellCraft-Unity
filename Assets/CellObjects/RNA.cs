using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class RNA : CellObject
{
	// infest, fast_grow   (RedRNA)
	//normal, grow, thread, dock, descend, die_thread, die, hardkill  (MRNA)
	//normal, grow, thread, thread_inplace,dock, descend, die_thread, die, hardKill  (EnzymeRNA)

	//common:  normal, grow, thread, dock, descend, die, die_thread, hardKill
	//public MovieClip Normal;
	

	protected int product;
	protected string product_creator_id; 
	public bool product_virus_vesicle = false; //if my product is a virus, is it vesicle-bound
	protected int product_count=1;
	protected Ribosome p_rib; //the ribosome I'm targetting
	protected bool atRibosome = false; //have I arrived?
		
	protected Nucleus p_nucleus;
	protected Point nuc_pore;
	protected int nuc_pore_index;
		
	protected bool rib_wait = false; //are we waiting for a ribosome?
	protected int rib_count = 0;
	protected int RIB_MAX = 60; //poll for a ribosome every second if idle
		
	public int na_value = 1;
		
	protected int dock_count = 0;
	protected const int DOCK_TIME = 300;
	public bool slicer_killed = false;
		
	public bool invincible = false;

	protected Coroutine _waitRibRoutine;
    protected Coroutine _dockWaitRoutine;

    public override void Start()
    {
       
        base.Start();
		//playAnim("normal");
		
    }

   

    public virtual void InitRNA(int i, int count = 1, string pc_id = "")
	{
        onFinished += finishedAnimating;
        canSelect = false;
		singleSelect = true;
		product = i;
		if (i == Selectable.NOTHING)
		{
			Debug.LogError("RNA received illegal product!");
		}
		product_count = count;
		//addEventListener(RunFrameEvent.RUNFRAME, run);
		speed = 2;
		init();
	}



	public override void destruct()
	{
		p_rib = null;
		if (_waitRibRoutine != null)
			StopCoroutine(_waitRibRoutine);
		base.destruct();
	}

	protected override void autoRadius()
	{
		setRadius(2);
	}

	public void setNAValue(int i)
	{
		na_value = i;
	}

	public int getNAValue() 
	{
		return na_value;
	}


	protected virtual IEnumerator waitRib()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			rib_count++;
			if (rib_count > RIB_MAX)
			{
				rib_count = 0;
				rib_wait = !p_cell.askForRibosome(this); //if it was successful, we are NOT waiting
				if (p_rib)
				{
					moveToRibosome(p_rib, FLOAT);   //if we have a ribosome, go to it!
					StopCoroutine(_waitRibRoutine);
				}
			}
		}

	}

	protected override void stopWhatYouWereDoing(bool isObj)
	{
		base.stopWhatYouWereDoing(isObj);
		if (!atRibosome)
		{
			//trace("RNA.stopWhatYouWereDoing()!");
			waitForRibosome();
		}
	}



	public void waitForRibosome()
	{

		rib_wait = true;
		rib_count = 0;
		p_rib = null;
		_waitRibRoutine = StartCoroutine(waitRib());
		//addEventListener(RunFrameEvent.RUNFRAME, waitRib, false, 0, true);
	}

	public void setNPore(Point p, Nucleus n, int i)
	{
		//trace("RNA.setNPore(" + p + "," + n + "," + i + ")");
		nuc_pore = p;
		p_nucleus = n;
		nuc_pore_index = i;
		p_rib = null;
	}

	public void setRibosome(Ribosome r)
	{
		p_rib = r;
		p_rib.setRNA(this);
		p_rib.makeBusy();
	}

	protected void moveToNucleusPore(int method)
	{
		//trace("RNA.moveToNucleusPore!");
		moveToPoint(new Point(p_nucleus.x + nuc_pore.x, p_nucleus.y + nuc_pore.y), method, true);

	}

	public override void calcMovement()
	{
		if (nuc_pore != null)
		{
			pt_dest.x = p_nucleus.x + nuc_pore.x;
			pt_dest.y = p_nucleus.y + nuc_pore.y;
		}
		base.calcMovement();
	}

	protected void moveToRibosome(Ribosome r, int method)
	{
		p_rib = r;
		moveToObject((CellGameObject)r, method, true);
	}


	protected override void onArrivePoint()
	{
		if (nuc_pore != null)
		{
			//p_nucleus.getPoreByI();
			p_nucleus.openPore(nuc_pore_index);
			playAnim("infest");
		}
	}

	protected override void onArriveObj()
	{
		if (p_rib)
		{
			dockRNA();
			//show up right over the 
			p_cell.onTopOf(this, p_rib);
		}
		//p_cell.setChildIndex(this, p_cell.getChildIndex(p_rib + 1));
	}

	private void dockRNA()
	{
		playAnim("dock");
		atRibosome = true;

		if (p_rib && !p_rib.dying)
		{
			//p_rib.setRNA(this);
			deliverCheckProduct();
		}
		else
		{
			//DESTROY
		}

		_dockWaitRoutine = StartCoroutine(dockWait());
		//addEventListener(RunFrameEvent.RUNFRAME, dockWait, false, 0, true);
	}

	private IEnumerator dockWait()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			dock_count++;
			if (dock_count > DOCK_TIME)
			{
				StopCoroutine(_dockWaitRoutine);
				dock_count = 0;
				if (p_rib)
				{
					if (p_rib.getRNA() == this)
					{
						p_rib.cancelRNA();
					}
					p_rib = null;
				}
				hardRevertAnim();
				rib_wait = true;
			}
		}
	}

	public void descend()
	{
		playAnim("descend");
	}

	public virtual void threadRNA(bool inPlace = false)
	{
		if (_dockWaitRoutine != null)
			StopCoroutine(_dockWaitRoutine);
		if (inPlace)
		{
			playAnim("thread_inplace");
			
		}
		else
		{
			playAnim("thread");
			
		}
		deliverProduct();
		//atRibosome = true;
	}

	public override void playAnim(string label)
	{
		if (label == "grow")
		{
			SfxManager.Play(SFX.SFXBlock); //TODO: the "Crack" sound needs a replacement as it never exported.
										   //Director.startSFX(SoundLibrary.SFX_CRACK);
			
		}
		
		base.playAnim(label);

	}

    private void finishedAnimating(MovieClip mc, string justPlayed)
    {
        switch (justPlayed)
        {
			case "thread":
			case "thread_inplace":
				onAnimFinish(ANIM_THREAD);
			break;
			case "grow":
				onAnimFinish(ANIM_GROW);
				break;
			case "dock":
				onAnimFinish(ANIM_DOCK);
				break;
			case "descend":
				onAnimFinish(ANIM_LAND);
				break;
			case "die":
			case "die_thread":
				onAnimFinish(ANIM_DIE);
				break;
			case "hardkill":
				onAnimFinish(ANIM_HARDKILL);
				break;
			case "infest":
				onAnimFinish(ANIM_VIRUS_INFEST);
				break;
        }
    }

    public override void onAnimFinish(int i, bool stop = true)
	{
		base.onAnimFinish(i, stop);
		//Normal.GotoAndStop(0);
		//trace("RNA.onAnimFinish() " + i);
		switch (i)
		{
			case ANIM_THREAD:
				playAnim("die_thread");
				invincible = true;
				break;
			case ANIM_GROW:
				//trace("RNA.onAnimFinish() p_rib = " + p_rib);
				if (p_rib)
				{
					moveToRibosome(p_rib, FLOAT);
				}
				break;
			case ANIM_VIRUS_INFEST:
				//invincible = true;
				killMe();
				break;
			case ANIM_DIE:
				killMe();
				break;
			case ANIM_HARDKILL:
				hardKillMe();
				break;
			default:
				break;
		}
	}

	public void mnodeMove(float xx, float yy)
	{
		x += xx;
		y += yy;
		if (isMoving)
		{
			if (pt_dest != null)
			{
				pt_dest.x += xx;
				pt_dest.y += yy;
			}
		}
	}

	public int getProduct() 
	{
			return product;
	}

	public string getProductCreator()
	{
		return product_creator_id;
	}

	public void deliverCheckProduct()
	{
		p_rib.checkProduct(product); //tell the ribosome to do something

		//dying = true;
	}

	public void deliverProduct()
	{
		if (p_rib.getProduct(product, product_count, product_creator_id, product_virus_vesicle))
		{
			product = NOTHING;

		}
		else
		{
			Debug.LogError("RNA could not deliver product " + product + " to Ribosome " + p_rib);
		}
	}

	public void hardKill()
	{
		//trace("MRNA.hardKill()!");
		hardKillMe();
		//playAnim("hardkill");
	}

	private void revertAnim()
	{
		GotoAndStop(1);
		clip.gameObject.SetActive(true);
	}

	protected void hardKillMe()
	{
		if (product != NOTHING)
		{
			p_cell.abortProduct(product);
		}
		if (p_rib)
		{
			if (!p_rib.dying)
			{
				p_rib.makeFree();
			}
			p_rib = null;
		}
		if (p_cell)
		{
			p_cell.killRNA(this);      //kill this RNA
		}
	}

	public bool onSlicerKill() 
	{
			slicer_killed = true;
			if (p_rib) {
				if(p_rib.getRNA() == this){
					p_rib.cancelRNA();
					p_rib.makeFree(); //make it so I can't deliver my product if I'm sliced
					p_rib = null;
					atRibosome = false;
				}
			}
			if (!dying)
			{             //if I haven't already been killed or haven't finished threading
				if (nuc_pore != null)
				{
					p_nucleus = null;
					nuc_pore = null;
					cancelMove();
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		
	protected override void killMe()
	{
		dying = true;
		if (p_rib && atRibosome)
		{ //check to see if I'm actually at my ribosome. If so, deliver my product
			if (!p_rib.dying)
			{
				p_rib.makeFree();
				if (this is EvilRNA || this is EnzymeRNA)
				{
					if (slicer_killed == false)
					{
						p_rib.finishRNA(true);
					}
					else
					{
						if (p_rib.getRNA() == null)
						{
							p_rib.cancelRNA();
						}
						if (p_rib.getRNA() == this)
						{
							p_rib.cancelRNA();
						}
					}
				}
				else
				{
					p_rib.finishRNA(false);
				}
				p_rib = null;
			}
			else
			{
				p_cell.abortProduct(product); //if the ribosome we're delivering to is dying, don't bother, just cancel this product
			}
		}
		else if (nuc_pore != null)
		{
			if (this is EvilRNA)
			{
				p_cell.onVirusInfest(product_creator_id, 1);
				p_nucleus.infestByRNA(this);
			}
		}
		if (p_cell)
		{
			p_cell.killRNA(this);      //kill this RNA
		}
		else
		{
			//trace("myname=" + name + " well shucks p_cell=" + p_cell);
		}
	}
		




}

