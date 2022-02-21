using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
public class ProducerObject : CellObject
{
	protected float efficiency = 1;
		protected int produce_time = 30; //produces every X frames
		protected const int PRODUCE_TIME = 30; //produces every X frames
		protected int produce_counter = 0; 
		protected bool is_producing = true;
		
		protected float[] _inputs;  //was Array
	protected float[] _outputs;  //was Array
		
		protected bool allowAlternateBurn = false;
		protected bool alternateBurn = false;
		protected float[] _inputs2;
		protected float[] _outputs2;
		
		protected bool firstRadical = false;
		protected bool radicalsOn = true;
		protected float count_radical = 0;
		protected float spawn_radical = 0.5f; 				//number of radicals to spawn
		
		protected float chance_radical	   =	0.10f;		//chance of producing a radical 
		protected float chance_invincible = 	0.00f;  //chance of producing an invincible radical (must be <= chance_radical)
														//it's like a dice roll - roll chance_invincible or less, and we make an invincible radical
														//roll higher than that but below chance_radical, and it's a normal radical


	private Coroutine _produceRoutine;
	/**
	 * Don't change this externally!
	 */
	private bool toggle_on = true; 
		private bool brown_fat_toggle_on = false;
		
		//private var heat_amount:Number = 0;
		private float HEAT_PER_BURN = 0.25f;
		
		public const int NOTHING = -1;
		public const int ATP = 0;
		public const int NA = 1;
		public const int FA = 2;
		public const int AA = 3;
		public const int G = 4;

	public ProducerObject()
	{
		_produceRoutine = StartCoroutine(produce());
		//addEventListener(RunFrameEvent.RUNFRAME, produce, false, 0, true);
		setRadicals(Cell.RADICALS_ON);
		makeGameDataObject();
		doesCollide = true;
	}

	public override void destruct()
	{
		StopCoroutine(_produceRoutine);
		//removeEventListener(RunFrameEvent.RUNFRAME, produce);
		base.destruct();
	}

	protected void setEfficiency(float e)
	{
		efficiency = e;
		produce_time = (int)(PRODUCE_TIME / efficiency);
	}

	protected IEnumerator produce()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (is_producing && toggle_on)
			{
				if (!anim_vital && !isDoomed && !isDamaged)
				{       //if we're not in the middle of doing something, like dividing
					produce_counter++;
					if (produce_counter > produce_time)
					{
						produce_counter = 0;
						if (p_cell.spend(_inputs, new Point(x, y + (MaxHeight / 3)), 0.5))
						{
							playAnim("process");
							if (brown_fat_toggle_on)
							{
								bumpBubble();
							}

							//trace("ProducerObject.produce() this=" + this +" name=" + this.name);
							checkRadical();
						}
						else if (allowAlternateBurn)
						{
							doAlternateBurn();

						}
					}
				}
			}
		}
	}

	protected void doAlternateBurn()
	{
		if (p_cell.spend(_inputs2, new Point(x, y + (MaxHeight / 2)), 0.5))
		{
			alternateBurn = true;
			playAnim("process");
			checkRadical();
		}
	}

	public override void takeDamage(float n, bool hardKill = false)
	{
		base.takeDamage(n);
		if (health <= 0)
		{
			if (hardKill)
			{
				base.onDamageKill(); //lets ProducerObjects  be recycled
			}
		}
	}

	protected override void onDamageKill()
	{
		//do nothing: Producer Objects can't be damagekilled!
	}

	public void setRadicals(bool b)
	{
		radicalsOn = b;
	}

	protected void checkRadical()
	{
		if (radicalsOn)
		{
			if (firstRadical)
			{ //make it so that they don't produce radicals on the first produce
				float m = UnityEngine.Random.Range(0f,1f);

				count_radical += spawn_radical;

				for (int i = 0; i < Mathf.Floor(count_radical); i++) 
				{
					bool isInvincible = (m < chance_invincible);
					p_cell.makeRadical(this, isInvincible);
					count_radical -= 1;
				}
			}
			else
			{
				firstRadical = true;
			}
		}
		
	}

	protected void finishProcess()
	{
		if (!alternateBurn)
		{
			if (brown_fat_toggle_on)
			{
				p_cell.produceHeat(HEAT_PER_BURN, 1, new Point(x, y - (MaxHeight / 3)), 0.5);
			}
			else
			{
				p_cell.produce(_outputs, 1, new Point(x, y - (MaxHeight / 3)), 0.5);
			}
		}
		else
		{
			alternateBurn = false;
			p_cell.produce(_outputs2, 1, new Point(x, y - (MaxHeight / 3)), 0.5);
		}
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		switch (i)
		{
			case ANIM_PROCESS: finishProcess(); break;
		}
		base.onAnimFinish(i, stop);
	}

	public override bool doAction(CellAction i, object parms = null) 
	{
			switch(i) {
				case CellAction.DISABLE:
					setToggle(false);
					return true;
				
				case CellAction.ENABLE:
					setToggle(true);
				return true;
				
				case CellAction.DISABLE_BROWN_FAT:
					setBrownFatToggle(false);
					return true;
					
				case CellAction.ENABLE_BROWN_FAT:
					setBrownFatToggle(true);
					return true;
					
			}
return base.doAction(i, parms);
		}

	public bool getBrownFatToggle()
	{
		return brown_fat_toggle_on;
	}

	public bool getToggle()
	{
		return toggle_on;
	}

	public void setBrownFatToggle(bool y)
	{
		brown_fat_toggle_on = y;
		/*if (brown_fat_toggle_on) {
			//do stuff

		}else {
			//do other stuff
		}*/
		showCorrectBubble();
	}

	public void setToggle(bool y)
	{
		toggle_on = y;

		/*if (toggle_on){

		}
		else {
			//trace("ProducerObject.setToggle() toggle_on = " + toggle_on + " should show bubble");
		}*/
		showCorrectBubble();
	}

	private void showCorrectBubble()
	{
		if (!toggle_on)
		{
			showBubble("cancel");
		}
		else
		{
			if (brown_fat_toggle_on)
			{
				showBubble("white_fire");
			}
			else
			{
				hideBubble();
			}
		}
	}

	public override void unDoom()
	{
		base.unDoom();
		setToggle(toggle_on); //do this so we show the enabled status even after we undoom it
	}

	public void toggle()
	{
		if (toggle_on)
		{
			setToggle(false);
		}
		else
		{
			setToggle(true);
		}
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

