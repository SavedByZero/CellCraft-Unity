using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class Nucleus : CellObject
{
	public Locator loc_nucleolus;
	//public NucleusAnim nclip;
	public MovieClip splatter;
		
	public string infester = "";
	public static bool CHECK_INFEST = false;
    private Coroutine _infestTickRoutine;

	/* NucleusAnim properties */
	public PoreMatrix pores;
	public NucleolusPoreMatrix n_pores;

	public override void Start()
	{
		base.Start();
		showSubtleDamage = true;
		singleSelect = true;
		text_title = "Nucleus";
		text_description = "Central command structure, produces DNA & RNA";
		text_id = "nucleus";
		num_id = Selectable.NUCLEUS;
		bestColors = new bool[] { false, true, false };
		list_actions = new List<CellAction>() { CellAction.BUY_RIBOSOME, CellAction.BUY_SLICER, CellAction.APOPTOSIS, CellAction.NECROSIS, CellAction.MITOSIS };// ([Act.REPAIR, Act.MITOSIS, Act.APOPTOSIS]);
		setMaxHealth(200, true);
		//nclip = clip as NucleusAnim;//clip.nclip;  //TODO: what was this? "clip.nclip"? Was that a mistake or some weird hack?
		//clip = MovieClip(nclip);
		CHECK_INFEST = false;
		//p_cell.onNucleusInfest(false);
		init();
		instantSetHealth(10);
	}

	protected override void autoRadius()
	{
		setRadius(75);
	}

	public Point getNucleolusLoc() 
	{
		return new Point(x + loc_nucleolus.transform.position.x, y + loc_nucleolus.transform.position.y);
	}




	public void healDNA(uint i)
	{
		if (hasInfestation)
		{
			infestation -= i;
			if (infestation <= 0)
			{
				infestation = 0;
				hasInfestation = false;
				CHECK_INFEST = false;
				p_cell.onNucleusInfest(false);
				StopCoroutine(_infestTickRoutine);
				//removeEventListener(RunFrameEvent.RUNFRAME, infestTick, false);
			}
			showInfestation();
		}
		else
		{
			giveHealth(i);
		}
	}

	public void infestByRNA(RNA r)
	{

		infester = r.getProductCreator();
		WaveEntry e = p_cell.getWave(infester);

		if (infestation <= 0)
		{
			//trace("Nucleus.infestByRNA start INFESTTICK!");
			hasInfestation = true;
			CHECK_INFEST = true;
			p_cell.onNucleusInfest(true);
			_infestTickRoutine = StartCoroutine(infestTick());
		}

		infestation += 1;
		if (infestation > max_infestation)
		{
			infestation = max_infestation;
		}

		showInfestation();

		callForHelp();

	}

	public override void takeDamage(float n, bool hardKill = false)
	{
		callForHelp();
		base.takeDamage(n);
		if (health <= 0)
		{
			p_cell.checkNucleusScrewed();
		}
	}

	private void callForHelp()
	{
		p_cell.nucleusCallForHelp();
	}

	private IEnumerator infestTick()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			infest_count++;
			if (infest_count > INFEST_TIME)
			{
				//trace("Nucleus.infestTime()!");
				infest_count = 0;
				onInfestTick();
			}
		}
	}

	private void onInfestTick()
	{
		//trace("Nucleus.onInfestTick()!");
		float newinfest = infestation / max_infestation * INFEST_MULT;

		WaveEntry w = p_cell.getWave(infester);

		//trace("Nucleus.onInfestTick() w.count " + w.count + "original_count = " + w.original_count+ " newinfest = " + newinfest);

		if (w.count <= w.original_count)
		{
			infest_rate += (newinfest);
		}
		else
		{
			/*var diff:Number = (w.count + newinfest) - w.original_count;
			if(newinfest-diff > 0){
				infest_rate += (newinfest - diff)
			}*/
		}

		while (infest_rate > 1)
		{
			//trace("Nucleus.onInfestTick GO BABY GO!");
			infest_rate -= 1;
			p_cell.generateInfestRNA(VIRUS_INFESTER, infester);
			if (float.IsNaN(infest_rate))
			{
				infest_rate = 0;
			}
		}

	}

	public string getInfester() 
	{
			return infester;
	}

	public void showInfestation()
	{
		//splatter.gotoAndPlay();
		float percent = infestation / max_infestation;
		if (percent <= 0.009f)
		{
			//splatter.GotoAndStop("0%");
		}
		else if (percent <= 0.2)
		{
			//splatter.GotoAndStop("20%");
		}
		else if (percent <= 0.4)
		{
			splatter.GotoAndStop("40%");
		}
		else if (percent <= 0.6)
		{
			splatter.GotoAndStop("60%");
		}
		else if (percent <= 0.8)
		{
			splatter.GotoAndStop("80%");
		}
		else
		{
			splatter.GotoAndStop("100%");
		}

		//trace("Nucleus.showInfestation()! infestation = " + infestation + " maxinfestation="+ max_infestation + " percent="+percent);

		if (selected)
		{
			p_cell.updateSelected();
		}
		//var percent:Number = (infestation / infestation_max ) * 100;

	}

	
	protected override void wiggle(bool yes)
	{
		//cacheAsBitmap = !yes;
		if (clip)
		{
			if (yes)
			{
				/*if (clip.nclip)       //TODO:  what IS this???
					clip.nclip.play();*/
			}
			else
			{
				/*if (clip.nclip)
					clip.nclip.stop();*/
			}
			base.wiggle(yes); //for now...
		}
		else
			base.wiggle(yes); //for now...
	}

	protected override void showLightDamage()
	{
		base.showLightDamage();
		//nclip.gameObject.SetActive(false);
	}

	protected override void showHeavyDamage()
	{
		showLightDamage();
	}

	//NucleusAnim-----------------------------------------------------------------------

	/**
		 * 0 for regular pore, 1 for Nucleolus pore
		 * @param	i
		 * @return
		 */

	public Point getPoreLoc(int i = 0, bool doOpen = false)
	{
		MovieClip p = null;
		if (i == 0)
		{
			p = pores.getPore(doOpen);
		}
		else
		{
			p = n_pores.getPore(doOpen);
		}
		Point pt = new Point(p.transform.position.x, p.transform.position.y);
		pt.x *= this.transform.localScale.x;
		pt.y *= this.transform.localScale.y;

		pt.x += x;
		pt.y += y;
		return pt;
	}
	public List<object> getPorePoint(int i = 0)
	{
		MovieClip p;
		if (i == 0)
		{
			p = pores.getPore(false);
		}
		else
		{
			p = n_pores.getPore(false);
		}
		Point pt = new Point(p.transform.position.x, p.transform.position.y);
		pt.x *= this.transform.localScale.x;
		pt.y *= this.transform.localScale.y;
		string name = p.name;
		//Debug.Log("pore point name " + name);
		int id = int.Parse(name.Split(new char[] { '(',')' })[1]);//int.Parse(name.Substring(5, 2));
		//trace("NucleusAnim.getPorePoint id = " + id);
		return new List<object> { pt, id };
	}

	public Point getPoreByI(int i, int type = 0)
	{
		MovieClip p;
		if (type == 0)
		{
			p = pores.getPoreByI(i);
		}
		else
		{
			p = n_pores.getPoreByI(i);
		}
		Point pt = new Point(p.transform.position.x, p.transform.position.y);
		pt.x *= this.transform.localScale.x;
		pt.y *= this.transform.localScale.y;
		return pt;
		//var p:Point = nclip.getPoreByI(i, type);
		//return p;
	}

	public void openPore(int i, int type = 0)
	{
		if (type == 0)
		{
			pores.openPore(i);
		}
		else
		{
			n_pores.openPore(i);
		}
	}


	public bool freePore(int i = 0)
	{
		if (i == 0)
		{
			return pores.freePore();
		}
		else
		{
			return n_pores.freePore();
		}
	}


	/*              NClip Code */





}

