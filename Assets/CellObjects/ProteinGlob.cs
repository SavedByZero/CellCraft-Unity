using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class ProteinGlob : CellObject
{
	public ProteinGlob()
	{
		singleSelect = true;
		//canSelect = false;
		text_title = "Protein Glob";
		text_description = "It's a glob of protein. Recycle it to get lots of Amino Acids!";
		text_id = "protein_glob";
		num_id = Selectable.PROTEIN_GLOB;
		bestColors = new bool[] { true, false, false };
		list_actions = new List<CellAction> { CellAction.MOVE, CellAction.RECYCLE};// Act.MAKE_BASALBODY]);
		setMaxHealth(250, true);
		init();
	}

	protected override void autoRadius()
	{
		setRadius(40);
	}

	protected override void onRecycle()
	{
		p_cell.onRecycle(this, true, true);
	}

	public override bool tryRecycle(bool oneOfMany = false) 
	{
			//cancelMove();
			isRecycling = true;
			return p_cell.bigVesicleRecycleSomething(this);
	}
}

