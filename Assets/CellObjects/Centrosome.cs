using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
public class Centrosome : CellObject
{
    //mc_centrosome_anim -- main healthy animation. 
    //centrolightdam01 -- same thing, but lightly damaged 
    //centroheavydam01 -- same thing, but heavily damaged 

    public override void Start()
    {
        base.Start();
 
		showSubtleDamage = true;
		singleSelect = true;
		text_title = "Centrosome";
		text_description = "Organizes the cell's cytoskeleton";
		text_id = "centrosome";
		num_id = Selectable.CENTROSOME;
		bestColors = new bool[] { false, false, true };			//list_actions = Vector.<int>([]);// Act.MAKE_BASALBODY]);
		setMaxHealth(250, true);
		init();
		instantSetHealth(250);
	}

	protected override void autoRadius()
	{
		setRadius(35);
	}



		public override void getPpodContract(float xx, float yy)
		{
			x -= xx;
			y -= yy;
			p_cell.getPpodContract(xx, yy);
		}
}

