using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Mitochondrion : ProducerObject
{
	public Mitochondrion()
	{
		showSubtleDamage = true;
		singleSelect = true;
		text_title = "Mitochondrion";
		text_description = "Consumes Glucose and produces ATP";
		text_id = "mitochondrion";
		num_id = Selectable.MITOCHONDRION;
		setMaxHealth(100, true);
		bestColors = new bool[] { true, false, false };
		list_actions = new List<CellAction> { CellAction.MOVE, CellAction.DIVIDE, CellAction.RECYCLE, CellAction.TOGGLE } ; //add recycle later
		does_divide = true;

		produce_time = 45;
		produce_counter = (int) (UnityEngine.Random.Range(0f,1f) * produce_time);



		//ATP,NA,AA,FA,G
		_inputs = new float[] { 0, 0, 0, 0, 1 };
		_outputs = new float[] { 38, 0, 0, 0, 0 };

		_inputs2 = new float[] { 0, 0, 0, 1, 0 };
		_outputs2 = new float[] { 108, 0, 0, 0, 0 };

		allowAlternateBurn = true;
		init();
	}

	protected override void doAlternateBurn()
	{
		if (!p_cell.canAfford(250, 0, 0, 0, 0))
		{ //if we're below 250 ATP, burn that fat!
			base.doAlternateBurn();
		}
		/*if (p_cell.spend(_inputs2, new Point(x, y + (height / 2)), 0.5)) {
			alternateBurn = true;
			playAnim("process");
			checkRadical();
		}*/
	}

	protected override void finishDivide()
	{
		//if (Director.STATS_ON) { Log.LevelCounterMetric("cell_mitochondrion_divide", Director.level); }
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_mitochondrion_divide", Director.level, 1); }
		GetComponentInParent<Cell>().placeMitochondrion(this.x + 75, this.y, false, true, false);
		base.finishDivide();
	}
}

