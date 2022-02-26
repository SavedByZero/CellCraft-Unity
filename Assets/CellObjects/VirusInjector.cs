using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirusInjector : Virus
{
	public const int RNA_COUNT = 1;
	public const int SPAWN_COUNT = 2; 
		
	public VirusInjector()
	{
		singleSelect = false;
		canSelect = false;
		text_title = "Injector Virus";
		text_description = "Injects RNA into your membrane!";
		text_id = "virus_injector";
		num_id = Selectable.VIRUS_INJECTOR;
		setMaxHealth(10, true);
		rnaCount = RNA_COUNT;
		spawnCount = SPAWN_COUNT;
		speed = 6;
	}

	protected override void whatsMyMotivation()
	{
		motivation_state = MOT_INJECTING_CELL;
	}


	protected override void insideCell()
	{
		speed = inside_speed;

		_tauntCellRoutine = StartCoroutine(tauntCell());
		position_state = POS_INSIDE_CELL;
		//this.transform.colorTransform = new ColorTransform(1,1,1,1, 0,0,0,0);
	}

	protected override void doLandThing()
	{
		Vector2 vec = new Vector2(x - cent_x, y - cent_y);
		vec.Normalize();
		vec *= INJECT_DISTANCE;
		SfxManager.Play(SFX.SFXVirusInfect);
	
		p_cell.generateVirusRNA(this, num_id, rnaCount, spawnCount, x + vec.x, y + vec.y, 0);
		playAnim("fade"); //kill me
	}

	protected override IEnumerator tauntCell()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			tauntCount++;
			if (tauntCount > TAUNT_TIME)
			{
				tauntCount = 0;
				p_cell.tauntByVirus(this);
			}
		}
	}

}

