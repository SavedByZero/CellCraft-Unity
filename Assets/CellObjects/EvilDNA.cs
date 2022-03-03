using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EvilDNA : EvilRNA
{
	public EvilDNA(int i, int count = 1, string pc_id = "") : base(i, count, pc_id)
	{
		//(i, count, pc_id);
		invincible = true;
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

