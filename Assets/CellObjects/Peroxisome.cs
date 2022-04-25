using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Peroxisome : BasicUnit
{
	private FreeRadical targetRadical;
	public bool orderOnDeath = false;
		
	public override void Start()
	{
		text_title = "Peroxisome";
		text_description = "A large vesicle filled with hydrogen peroxide. Neutralizes toxins.";
		text_id = "peroxisome";
		num_id = Selectable.PEROXISOME;
		setMaxHealth(20, true);
		list_actions = new List<CellAction> { CellAction.MOVE, CellAction.RECYCLE };
		init();
		speed = 4;

		makeGameDataObject();
		doesCollide = true;
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		base.onAnimFinish(i, stop);
		switch (i)
		{
			case ANIM_GROW: finishGrow(); break;
			
			default: break;
		}
	}

	public void bud()
	{
		playAnim("bud");
	}

	public void grow()
	{
		playAnim("grow");
	}

	public void setTargetRadical(FreeRadical r)
	{
		targetRadical = r;
		moveToObject(r, CellGameObject.FLOAT, true);
		isBusy = true;
	}

	public void finishGrow()
	{
		deployGolgi();
	}

	public void deployGolgi(bool instant = false)
	{
		Point p = p_cell.getGolgiLoc();
		deployCytoplasm(p.x, p.y, 100, 20, true, instant);
	}

	protected override void stopWhatYouWereDoing(bool isObj)
	{
		isBusy = false;
		base.stopWhatYouWereDoing(isObj);
		//define rest per subclass
	}

	public override void cancelMove()
	{
		targetRadical = null;
		isBusy = false;
		base.cancelMove();

	}

	protected override void onArriveObj()
	{
		//throw new Error("Testing");
		if (!dying)
		{
			if (targetRadical)
			{
				if (targetRadical.dying == false && targetRadical.invincible == false)
				{
					SfxManager.Play(SFX.SFXZlap);
					targetRadical.cancelMove();
					targetRadical.playAnim("recycle"); // kill the radical
					targetRadical = null;
					isBusy = false;
					p_cell.onAbsorbRadical(this);
					takeDamage(1); //take 1 damage each time you eat a radical. Eventually you'll die
				}
				else
				{
					targetRadical = null;
					isBusy = false;
				}

			}
		}
		base.onArriveObj();

		if (!dying && targetRadical == null)
		{ //if I survived and it died, go back to waiting
			isBusy = false;
		}

	}

	public override void takeDamage(float n, bool hardKill = false)
	{
		base.takeDamage(n, hardKill);
		if (health <= 0)
		{
			orderOnDeath = true;
		}
		updateLook();
	}

	private void updateLook()
	{
		/*var dark:Number = health / maxHealth;
		if (dark < 0.25) dark = 0.25;*/
		float h = health / maxHealth;
		h *= 1.5f;
		if (h > 1) h = 1;
		if (h < 0.25) h = 0.25f;

		Color col = this.GetComponentInChildren<SpriteRenderer>().color;
		this.GetComponentInChildren<SpriteRenderer>().color = new Color(col.r * h/255, col.g * h/255, col.b * h/255);
		/*var c:ColorTransform = new ColorTransform();
		c.redMultiplier = h;
		c.blueMultiplier = h;
		c.greenMultiplier = h;
		this.transform.colorTransform = c;*/
	}

	public override bool tryRecycle(bool isOfMany = false) 
	{
		cancelMove();
		isRecycling = true;
			
		return p_cell.bigVesicleRecycleSomething(this);
	}

	public override void updateLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		updateGridLoc(xx, yy);
	}


}

