using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


public class VirusInfester : Virus
{
	public const int RNA_COUNT = 5;
	public const int SPAWN_COUNT = 2;
		
		
	private const float RNA_DISTANCE = 5;
		
	public VirusInfester()
	{
		singleSelect = false;
		canSelect = false;
		text_title = "Infester Virus";
		text_description = "Invades your membrane and infests the Nucleus!";
		text_id = "virus_infester";
		num_id = Selectable.VIRUS_INFESTER;
		setMaxHealth(10, true);
		rnaCount = RNA_COUNT;
		spawnCount = SPAWN_COUNT;
		speed = 8;
	}

	protected override void whatsMyMotivation()
	{
		motivation_state = MOT_INFESTING_CELL;
	}

	protected void doNucleusThing()
	{
		if (!isDoomed && !dying)
		{

			Vector2 vec2 = new Vector2(x - cent_x, y - cent_y);
			vec2 = vec2.normalized;
			vec2 *= (INJECT_DISTANCE);
			SfxManager.Play(SFX.SFXVirusInfect);
		
			float theRot = (Mathf.PI * 2) / RNA_COUNT;
			for (int i = 0; i < RNA_COUNT; i++) 
			{

				p_cell.generateVirusRNA(this, num_id, 1, SPAWN_COUNT, x + vec2.x, y + vec2.y, 0, true, true);
			}
			playAnim("fade"); //killMe
		}
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
			for (int i = 0; i < RNA_COUNT; i++) 
			{
				vec = FastMath.rotateVector(theRot,vec);
				p_cell.generateVirusRNA(this, num_id, 1, SPAWN_COUNT, x + vec.x + vec2.x, y + vec.y + vec2.y, 0, true);
			}
			playAnim("fade"); //killMe
		}
	}

	public override void calcMovement()
	{
		if (nuc)
		{
			pt_dest.x = (nuc.x + nuc_node.x);
			pt_dest.y = (nuc.y + nuc_node.y);
			Vector2 v = new Vector2(pt_dest.x - x, pt_dest.y - y);
			float ang = (FastMath.toRotation(v) / (Mathf.PI * 2)) * 360;//(v.toRotation() * 180) / Math.PI;

			if (condition_state == CON_MOVE_TO_NUCLEUS)
			{
				this.transform.eulerAngles = new Vector3(0,0,ang - 90);
			}
		}
		base.calcMovement();
	}

	protected override void onArrivePoint()
	{
		base.onArrivePoint();
		if (!isDoomed)
		{
			if (position_state == POS_INSIDE_CELL)
			{
				if (nuc_node != null)
				{
					doNucleusThing();
				}
			}
		}
	}

	protected override void onInvade(bool doDamage = true)
	{
		//trace("VirusInfester.onInvade()");
		mnode = null;
		cancelMove(); //just to be sure
		if (!isNeutralized && doDamage)
		{
			p_cell.c_membrane.takeDamageAt(x, y, DMG_PIERCE_MEMBRANE);
			//p_cell.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "virus_entry_wound", wave_id, 1);
		}

		StopCoroutine(_clingCellRoutine);
		

		entering = false;
		StopCoroutine(_tryEnter);

		leaving = false;

		insideCell();

		condition_state = CON_MOVE_TO_RIBOSOME;
		nuc = p_cell.c_nucleus;
		List<object> a = (p_cell.getNucleusPore());
		nuc_node = (a[0] as Point);
		if (nuc)
		{
			moveToPoint(new Point(nuc.x + nuc_node.x, nuc.y + nuc_node.y), FLOAT, true);
		}
		
	}

	protected override void touchingCell()
	{
		releaseLyso();
		StopCoroutine(_tauntCellRoutine);
		position_state = POS_TOUCHING_CELL;
	}

	protected override void insideCell()
	{
		speed = inside_speed;
		_tauntCellRoutine = StartCoroutine(tauntCell());
		position_state = POS_INSIDE_CELL;
		
	}

	protected override void outsideCell()
	{
		position_state = POS_OUTSIDE_CELL;
		
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

