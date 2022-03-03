using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DNARepairEnzyme : BasicUnit
{
	public bool goingNucleus = false;
	private const int HEAL_VALUE = 5;
		
	public DNARepairEnzyme()
	{
		does_recycle = true;
		speed = 2;
		num_id = Selectable.DNAREPAIR;
		text_id = "dnarepair";
		text_title = "DNA Repair Enzyme";
		text_description = "Repairs the Nucleus' DNA";

		canSelect = true;
		setMaxHealth(1, true);
		MAX_COUNT = 5; //recalculate more often for better tracking

	}

	public override void init()
	{
		base.init();
		SfxManager.Play(SFX.SFXRepairRise);
		tryGoNucleus();
		//repairDeploy();
	}

	protected override void onRecycle()
	{
		p_cell.onRecycle(this, true, false);
	}

	protected override void onArrivePoint()
	{
		base.onArrivePoint();
		if (!dying)
		{
			if (goingNucleus == false)
			{
				tryGoNucleus();
			}
		}
	}

	protected override void onArriveObj()
	{
		//throw new Error("Testing");
		if (!dying)
		{
			if (goingNucleus == false)
			{
				tryGoNucleus();
			}
			else
			{
				if (p_cell.c_nucleus)
				{
					SfxManager.Play(SFX.SFXHeal);
					p_cell.c_nucleus.healDNA(HEAL_VALUE);
					//p_cell.c_nucleus.(HEAL_VALUE);
					p_cell.onHealSomething(p_cell.c_nucleus, HEAL_VALUE);
					p_cell.showHealSpot(HEAL_VALUE, x, y);
					goingNucleus = false;
					useMe();
				}
			}
		}
		base.onArriveObj();

		if (!dying && !goingNucleus)
		{ //if I survived and it died, go back to waiting
			repairDeploy();
		}
	}

	public void tryGoNucleus()
	{
		if (p_cell.getNucleusDamage() > 0 || p_cell.getNucleusInfestation() > 0)
		{
			goingNucleus = true;
			moveToObject(p_cell.c_nucleus, CellGameObject.FLOAT, true);
		}
	}

	public void repairDeploy()
	{
		//deployCytoplasm(p_cell.c_nucleus.x,p_cell.c_nucleus.y,170,35);
	}

	private void useMe()
	{
		playAnim("recycle");
		//trace("SlicerEnzyme.useMe() p_cell=" + p_cell);
	}




}

