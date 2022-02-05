using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Centrosome : CellObject
{

		public Centrosome()
		{
			showSubtleDamage = true;
			singleSelect = true;
			text_title = "Centrosome";
			text_description = "Organizes the cell's cytoskeleton";
			text_id = "centrosome";
			num_id = Selectable.CENTROSOME;
			bestColors = [0, 0, 1];
			//list_actions = Vector.<int>([]);// Act.MAKE_BASALBODY]);
			setMaxHealth(250, true);
			init();
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

