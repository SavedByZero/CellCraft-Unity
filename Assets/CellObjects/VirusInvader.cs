using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VirusInvader : Virus
{
	//TODO: the onAnimFinish is called from the timeline in the original game quite a bit. make sure the code calls it manually. 
	public const int RNA_COUNT = 4;
	public const int SPAWN_COUNT = 1;
		
		
	private const float RNA_DISTANCE = 5;
		
	public VirusInvader()
	{
		singleSelect = false;
		canSelect = false;
		text_title = "Invader Virus";
		text_description = "Invades your membrane and deposits RNA!";
		text_id = "virus_invader";
		num_id = Selectable.VIRUS_INVADER;
		setMaxHealth(10, true);
		rnaCount = RNA_COUNT;
		spawnCount = SPAWN_COUNT;
		speed = 6;
	}

	protected override void whatsMyMotivation()
	{
		motivation_state = MOT_INVADING_CELL;
	}

	protected override void doRibosomeThing()
	{
		if (!isDoomed && !dying)
		{
			Vector2 vec = new Vector2(RNA_DISTANCE, 0);
			Vector2 vec2 = new Vector2(x - cent_x, y - cent_y);
			vec2 = vec2.normalized;
			vec2 *= (INJECT_DISTANCE);
			SfxManager.Play(SFX.SFXVirusInfect);
			
			float theRot = (Mathf.PI * 2) / RNA_COUNT;
			for (int i = 0; i < RNA_COUNT; i++) {
				
				vec = FastMath.rotateVector(theRot, vec);
				p_cell.generateVirusRNA(this, num_id, 1, SPAWN_COUNT, x + vec.x + vec2.x, y + vec.y + vec2.y, 0, true);
			}
			playAnim("fade"); //killMe
		}
	}



	protected override void touchingCell()
	{
		//trace("TOUCHING CELL GO W
		releaseLyso();
		StopCoroutine(_tauntCellRoutine);
		position_state = POS_TOUCHING_CELL;
		//this.transform.colorTransform = new ColorTransform(0.75,0.75,0.75,1, 0,0,0,0);
	}

	protected override void insideCell()
	{
		speed = inside_speed;
		_tauntCellRoutine = StartCoroutine(tauntCell());
		position_state = POS_INSIDE_CELL;
		//this.transform.colorTransform = new ColorTransform(1,1,1,1, 0,0,0,0);
	}

	protected override void outsideCell()
	{
		StopCoroutine(_tauntCellRoutine);
		position_state = POS_OUTSIDE_CELL;
		//this.transform.colorTransform = new ColorTransform(0.5, 0.5, 0.5, 1, 0, 0, 0, 0);
	}

	protected override void onTouchCellAnim()
	{
		playAnim("invade");
	}

	protected override void doLandThing()
	{
		mnode = null;
		playAnim("invade"); //invade the membrane
	}

	protected override void onInvade(bool doDamage = true)
	{
		base.onInvade();
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

