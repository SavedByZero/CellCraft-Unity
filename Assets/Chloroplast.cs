﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


 public class Chloroplast : ProducerObject
{
	public override void Start()
	{
		base.Start();
		showSubtleDamage = true;
		text_title = "Chloroplast";
		text_description = "Produces Glucose from co2, water, and sunlight";
		text_id = "chloroplast";
		num_id = CHLOROPLAST;
		setMaxHealth(100, true);
		singleSelect = true;
		//list_actions = Vector.<int>([Act.MOVE, Act.DIVIDE, Act.RECYCLE,Act.TOGGLE]);
		does_divide = true;

		produce_time = 45;
		_inputs = new float[]{0, 0, 0, 0, 0};
		_outputs = new float[] { 0, 0, 0, 0, 1 };

		spawn_radical = 1;        //up to 5 radicals
		chance_radical = 0.25f;    //much more likely to make a radical than a mitochondrion
								  //chance_invincible = 0.05; //1 in 20 chance of producing an invincible radical
		chance_invincible = 0;
		init();
	}

    public override void playAnim(string label)
    {
		Debug.Log("playing: " + label);
        base.playAnim(label);
    }

    public void setSunlight(float s)
	{
		setEfficiency(s);
		if (efficiency > 0)
		{
			is_producing = true;
		}
		else
		{
			is_producing = false;
		}
	}

	protected override void finishDivide()
	{
		//if (Director.STATS_ON) { Log.LevelCounterMetric("cell_chloroplast_divide", Director.level); }
		//if (Director.STATS_ON) { Log.LevelAverageMetric("cell_chloroplast_divide", Director.level, 1); }

		this.transform.GetComponentInParent<Cell>().placeChloroplast(this.x + 75, this.y);
		base.finishDivide();
	}
}

