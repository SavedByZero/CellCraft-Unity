using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ToxinParticle : CellObject
{
	private int absorbCount = 0;
	private int ABSORB_TIME = 10;
		
	public WorldCanvas p_canvas;
		
	private float scaleSpeed = .066f;
	private float theScale = 1;
	private float MAX_SCALE = 3;
	private Coroutine _checkAbsorbRoutine;
		
	public ToxinParticle()
	{
		singleSelect = true;
		canSelect = false;
		does_recycle = false;

		//canSelect = false;
		text_title = "Toxin Particle";
		text_description = "It's a toxin particle!";
		text_id = "toxin_particle";
		num_id = Selectable.TOXIN_PARTICLE;
		bestColors = new bool[] { true, false, false };// 1, 0, 0];
		//list_actions = Vector.<int>();
		setMaxHealth(250, true);
		init();

		speed = 3;
	}

	public void setCanvas(WorldCanvas c)
	{
		p_canvas = c;
	}

	protected override void autoRadius()
	{
		setRadius(25);
	}

	public void getOuttaHere()
	{

		Vector2 v = new Vector2(x - cent_x, y - cent_y);
		v.Normalize();
		x += v.x * 45; //shove to outside the membrane
		y += v.y * 45;

		//v *= (p_canvas.getBoundary() * 1.5f);  //TODO

		moveToPoint(new Point(cent_x + v.x, cent_y + v.y), FLOAT, true);

		_checkAbsorbRoutine = StartCoroutine(checkAbsorb());//addEventListener(RunFrameEvent.RUNFRAME, checkAbsorb, false, 0, true);
	}

	private IEnumerator checkAbsorb()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			theScale += scaleSpeed;
			this.transform.localScale = new Vector3(theScale, theScale, theScale);

			float fade = ((theScale - 1) / MAX_SCALE);
			this.GetComponentInChildren<SpriteRenderer>().DOFade(1-fade, 0);
			//trace("fade = " + fade);

			if (theScale >= MAX_SCALE)
			{
				p_cell.makeToxin(x, y);
				//p_cell.notifyOHandler(EngineEvent.ENGINE_TRIGGER, "toxin_escape", "null", 1);  //TODO
				cancelMove();
				StopCoroutine(_checkAbsorbRoutine);
				p_cell.killToxinParticle(this);
			}
		}
		
	}

}

