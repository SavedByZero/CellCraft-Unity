using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EvilDNA : EvilRNA
{
	public MovieClip Infest;
	public void InitEvilDNA(int i, int count = 1, string pc_id = "") 
	{
		base.InitRNA(i, count, pc_id);
		//(i, count, pc_id);
		invincible = true;
	}

	public override void playAnim(string label)
	{
		if (label == "infest")
		{

			//Infest.GotoAndPlay(0);
		}
		else if (label == "fast_grow" || label == "fastGrow")
		{
			//Grow.FrameInterval = 0.01f;
			//Grow.GotoAndPlay(0);
		}
		base.playAnim(label);
	}

	//evil DNA doesn't taunt the cell and is thus effectively immune to slicers
	protected override IEnumerator tauntCell()
	{
		yield return new WaitForEndOfFrame();
		//every second, asks the cell for something to kill it
		/*tauntCount++;
		if (tauntCount > TAUNT_TIME) {
			tauntCount = 0;
			p_cell.tauntByEvilRNA(this);
		}*/
	}
}

