using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Threading.Tasks;


class FreeRadical : CellObject
{
	private string targetStr = "";
	private CellObject targetObj;
	private CellObject makerObj;
	public bool invincible = false;
	private float myDamage = 5;
		
	public static float chance_nuc;
	public static float chance_mito;
	public static float chance_lyso;
	public static float chance_chlor;
	public static float chance_slicer;
	public static float chance_perox;
		
	private const float MITO_WEIGHT = 10;
	private const float CHLORO_WEIGHT = 10;
	private const float LYSO_WEIGHT = 2;
	private const float SLICER_WEIGHT = 0.5f;
	private const float PEROX_WEIGHT = 1;
		
	private float decelerate = 1 / 30;
	private float minSpeed = 2;

	private int tauntCounter = 0;
	private int TAUNT_TIME = 15;

	public FreeRadical()
	{
		does_recycle = true;
		speed = 2.5;
		num_id = Selectable.FREE_RADICAL;
		text_id = "freeradical";
		text_title = "Free Radical";
		text_description = "Damages DNA & Organelles!";
		list_actions = new List<CellAction> { CellAction.RECYCLE, CellAction.DISABLE };
		//singleSelect = false;
		canSelect = false;
		singleSelect = false;

	}

	protected override void doMoveToGobj()
	{
		if (speed > minSpeed)
		{
			speed -= decelerate;
		}
		tauntCounter++;
		if (tauntCounter > TAUNT_TIME)
		{
			tauntCounter = 0;
			if (!invincible)
			{
				p_cell.tauntByRadical(this);
			}
		}
		base.doMoveToGobj();
	}

	public void setMaker(CellObject c)
	{
		makerObj = c;
	}

	public override void init()
	{
		base.init();
		SfxManager.Play(SFX.SFXRadicalRise);
		radicalDeploy();
		p_cell.tauntByRadical(this);
	}

	public void setTargetStr(string t)
	{
		targetStr = t;
	}

	public void targetSomething(CellObject e, bool goThereNow = true)
	{
		targetObj = e;
		if (goThereNow)
		{
			moveToObject(e, CellGameObject.FLOAT, true);
		}
		else
		{
			Point p = new Point((x + cent_x) / 2, (y + cent_y) / 2);
			moveToPoint(p, CellGameObject.FLOAT, true);
		}
	}

	protected override void onArrivePoint()
	{
		base.onArrivePoint();
		if (targetObj)
		{
			moveToObject(targetObj, CellGameObject.FLOAT, true);
		}
		else
		{
			radicalDeploy();
		}
	}

	public override void cancelMove()
	{
		targetObj = null;
		base.cancelMove();
	}

	protected override void onArriveObj()
	{

		if (!dying)
		{
			if (targetObj)
			{
				if (targetObj.dying == false && targetObj.getHealth() > 0)
				{
		
					targetObj.takeDamage(myDamage);
					p_cell.onRadicalHit(targetObj);
					p_cell.makeStarburst(x, y);
					
					targetObj = null;
					useMe();
				}
				else
				{
					targetObj = null;
					radicalDeploy();
				}
			}
		}
		base.onArriveObj();

		if (!dying && targetObj == null)
		{ //if I survived and it died, go back to waiting
			radicalDeploy();
		}
	}

	public void radicalDeploy()
	{
		//deployCytoplasm(p_cell.c_nucleus.x,p_cell.c_nucleus.y,170,35);

		CellObject c = findRadicalTarget();
		bool goThereNow = true; //if we are targetting something, go directly there
		if (c == null)
		{           //if no target was returned, target my maker
			c = makerObj;
			goThereNow = false;     //if we are targetting the maker, go away then come back
		}
		targetSomething(c, goThereNow);
		//p_cell.tauntByRadical(this);
	}

	private CellObject findRadicalTarget() 
	{
		if (targetStr != "") 
		{
			if (targetStr == "nucleus") 
			{
				c = p_cell.c_nucleus;
			}
			else if (targetStr == "mitochondrion")
			{
				c = p_cell.getRandomMito();
			}
			else if (targetStr == "chloroplast")
			{
				c = p_cell.getRandomChloro();
			}
			else if (targetStr == "slicer")
			{
				c = p_cell.getRandomSlicer();
			}
			else if (targetStr == "peroxisome")
			{
				c = p_cell.getRandomPerox();
			}
			else if (targetStr == "lysosome")
			{
				c = p_cell.getRandomLyso();
			}
			else
			{
				c = null;
			}
		}
		else
		{
			float m = UnityEngine.Random.Range(0f,1f);
			CellObject c;
			if (m < chance_nuc)
			{
				c = p_cell.c_nucleus;
			}
			else if (m < chance_mito)
			{
				c = p_cell.getRandomMito();
			}
			else if (m < chance_chlor)
			{
				c = p_cell.getRandomChloro();
			}
			else if (m < chance_slicer)
			{
				c = p_cell.getRandomSlicer();
			}
			else if (m < chance_perox)
			{
				c = p_cell.getRandomPerox();
			}
			else if (m < chance_lyso)
			{
				c = p_cell.getRandomLyso();
			}
			else
			{
				c = null;
			}
			if (c == makerObj)
			{ //if we somehow targetted our maker, set it to null so we target ourselves correctly
				c = null;           //fixes instant damage bug
			}
		}
		return c;
		}
		
	private void useMe()
	{
		playAnim("recycle");
		//trace("SlicerEnzyme.useMe() p_cell=" + p_cell);
	}

	public static void updateChances(float lm, float lc, float ls, float lp, float ll)
	{
		lm *= MITO_WEIGHT;
		lc *= CHLORO_WEIGHT;
		ls *= SLICER_WEIGHT;
		lp *= PEROX_WEIGHT;
		ll *= LYSO_WEIGHT;
		float chance_total = lm + lc + ls + lp + ll;

		float nuc = chance_total / 3; //this makes a 25% of affecting the nucleus, no matter what
		chance_total += nuc;

		chance_total *= 1.05f; //extra % - will return null

		chance_nuc = nuc / chance_total;
		chance_mito = (nuc + lm) / chance_total;
		chance_chlor = (nuc + lm + lc) / chance_total;
		chance_slicer = (nuc + lm + lc + ls) / chance_total;
		chance_perox = (nuc + lm + lc + ls + lp) / chance_total;
		chance_lyso = (nuc + lm + lc + ls + ll) / chance_total;

		//trace("FreeRadical.updateChances() total = " + chance_total + " n,m,c,s,p,l=" + chance_nuc + "," + chance_mito + "," + chance_chlor + "," + chance_slicer + "," + chance_perox + "," + chance_lyso);
	}




}

