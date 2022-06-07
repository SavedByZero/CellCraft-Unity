using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class Lysosome : BasicUnit
{//nowmal, grow, digest, recycle, pop, fuse, bud 

	//public var isBusy:Boolean = false;
	private int busyBit = 0;
	private Selectable eat_target;
	private bool recycleSelfOnEat = false;
	public BigVesicle fuse_target;
	public bool fusing = false;
		
	public bool amEating = false;
		
	private BigVesicle p_bigVesicle;
		
	public const float PH_BALANCE = 4.5f;
	public const float VOL_X = 500; //effective volume mult when doing ph stuff with the cytosol
	public static float VOL_V = 25; //effective volume mult when doing ph stuff with big vesicles
	public static float L_RADIUS = 10;
	public static float LYSO_VOL = Mathf.PI* L_RADIUS * L_RADIUS;
	
		
	public override void Start()
	{
		base.Start();
		text_title = "Lysosome";
		text_description = "A small vesicle filled with digestive acid. Digests stuff and regulates pH";
		text_id = "lysosome";
		bestColors = new bool[]{true, true, false };
		num_id = Selectable.LYSOSOME;
		list_actions = new List<CellAction> { CellAction.MOVE, CellAction.RECYCLE};
		setMaxHealth(3, true);
		speed = .05f;
		init();
		makeGameDataObject();
		doesCollide = true;
		snapToObject = false; //KEEPS EM IN THE MEMBRANE!
		does_recycle = true;
	}

	protected override void autoRadius()
	{
		setRadius(L_RADIUS);
	}

	public override bool doAction(CellAction i, object parms = null) 
	{
		switch(i) {
				case CellAction.POP: doPop(); break;
		}
		return base.doAction(i, parms);
	}
		
	private void doPop()
	{
		playAnim("pop");
	}

	public override void onAnimFinish(int i, bool stop)
	{
		base.onAnimFinish(i, stop);
		switch (i)
		{
			case ANIM_GROW: finishGrow(); break;
			case ANIM_DIGEST_START: eatTheTarget(); break;
			case ANIM_DIGEST: finishDigest(); break;
			case ANIM_POP: finishPop(); break;
			case ANIM_FUSE: finishFuse(); break;
			case ANIM_BUD: finishBud(); break;
			default: break;
		}
	}

	public void setBigVesicleFuser(BigVesicle b) 
	{
		p_bigVesicle = b;
	}

	private void finishPop()
	{
		p_cell.onPopLysosome(this);
		//p_cell.onRecycle(this);
	}

	public void bud()
	{
		makeBusy();
		playAnim("bud");
	}

	public void grow()
	{
		makeBusy();
		playAnim("grow");
	}

	public void finishBud()
	{
		//trace("Lysosome.finishBud() RELEASE");
		release();
		finishGrow();
		if (p_bigVesicle)
		{
			p_bigVesicle.onLysosomeBud();
			p_bigVesicle = null;
		}
		this.transform.eulerAngles = Vector3.zero;
	}

	public void deployNucleus(bool instant = false)
	{
		//trace("SlicerEnzyme.slicerDeploy(" + instant + ")");
		if (p_cell)
		{
			deployCytoplasm(p_cell.c_nucleus.x, p_cell.c_nucleus.y, 170, 35, true, instant);
		}
	}

	public void deployGolgi(bool instant = false)
	{
		Point p = p_cell.getGolgiLoc();
		deployCytoplasm(p.x, p.y, 50, 10, true, instant);
	}

	public void finishGrow()
	{
		release();
		deployGolgi();
	}

	protected override void doMoveToGobj()
	{
		base.doMoveToGobj();
		if (eat_target)
		{
			float dx = x - eat_target.x;
			float dy = y - eat_target.y;
			float dist2 = (dx * dx) + (dy * dy);
			if (dist2 <= getRadius2() * 2)
			{
				arriveObject();
			}
		}
		else
		{

		}

	}

	public float getCircleVolumeV() 
	{
			return getCircleVolume() * VOL_V;
	}

	public float getCircleVolumeX()
	{
		return getCircleVolume() * VOL_X;
	}



	public void finishDigest()
	{
		amEating = false;
		//trace("Lysosome.finishDigest() this = " + this.name + " RELEASE");
		release();
		this.transform.eulerAngles = Vector3.zero;  //return to normal
		if (recycleSelfOnEat)
		{
			recycleSelfOnEat = false;
			p_cell.startRecycle(this);
			//super.);
		}
	}

	public void eatTheTarget()
	{
		if (eat_target)
		{
			eat_target.onDeath();
			eat_target = null;
		}
		else
		{
			//trace("Lysosome.eatTheTarget() NO TARGET RELEASE");
			release();
		}
	}

	public override void cancelMove()
	{
		if (eat_target)
		{
			//trace("LYsosome.cancelMove() RELEASE");
			release();
		}
		base.cancelMove();
	}

	public void dontFuseWithBigVesicle()
	{
		//trace("Lysosome.dontFuseWithBigVesicle()");
		if (!fusing)
		{

			release();
			fuse_target = null; //do this before cancel move to avoid bugzes!
			cancelMove();
			deployGolgi();
		}


		fuse_target = null;
	}

	public bool fuseWithBigVesicle(BigVesicle b)
	{
		if (b != null)
		{
			Vector2 v = new Vector2(x - b.x, y - b.y);
			float rotation = FastMath.toRotation(v) * (180 / Mathf.PI);
			rotation -= 90;
			this.transform.eulerAngles = new Vector3(0, 0, rotation);
			moveToObject(b, EDGE, true);
			fuse_target = b;
			makeBusy();
			return true;
		}
		return false;
	}

	public bool eatSomething(Selectable s) 
	{
		if (s != null && !isBusy) 
		{
				//trace("Lysosome(" + this.name + ").eatSomething(" + s.name + ")");
				
			if (eat_target != null) 
			{
				if (eat_target != s) 
				{
						
						eat_target.releaseFromLysosome(this);
				}
			}

		s.targetForEating(this);
		Vector2 v = new Vector2(x - s.x, y - s.y);
		float rotation = FastMath.toRotation(v) * 180 / Mathf.PI; //turn to face it so the engulf animation looks right
		rotation -= 90;                            //offset to make it work
			this.transform.eulerAngles = new Vector3(0, 0, rotation);
		moveToObject(s, FLOAT, true);
		if (eat_target == null)
		{
			eat_target = s;
		}
		else
		{
			//trace("Lysosome.eatSomething(" + s + ")! TArget Already exitsts : " + eat_target);
		}
		p_cell.onTopOf(eat_target as CellObject, this as CellObject, true);
		makeBusy();

		return true;
					}
					return false;
	}
		
	public override bool tryRecycle(bool oneOfMany = false)
	{
		if (!isBusy)
		{
			return base.tryRecycle(oneOfMany);
		}
		else
		{
			recycleSelfOnEat = true;
			return false;
		}
	}

	public void releaseByVirus(Virus v)
	{
		if (eat_target == v)
		{
			//trace("Lysosome("+this.name+".releaseByVirus("+v.name+") RELEASE");
			release();
		}
	}

	public void release()
	{
		if (eat_target)
		{
			eat_target = null;
		}
		isBusy = false;
		if (isMoving)
		{
			cancelMove();
		}
		deployNucleus();

	}

    public override void playAnim(string label)
    {
		base.playAnim(label);
       
    }

    private void makeBusy()
	{
		isBusy = true;
	}

	protected override void onArriveObj()
	{
		base.onArriveObj();
		bool proceed = false;
		if (eat_target)
		{ //hack to avoid bugs

			eat_target.x = x;
			eat_target.y = y;
			if (!eat_target.isDoomed && !eat_target.dying)
			{
				amEating = true;
				playAnim("digest");
				if (eat_target is Virus)
				{
					(eat_target as Virus).dismissAllLysosExcept(this);
				}
				eat_target.startGetEaten();
				//eat_target.getEaten();
			}
		}
		else if (fuse_target)
		{
			playAnim("fuse");
			fusing = true;
		}
		else
		{
			release();
		}
	}

	protected void finishFuse()
	{
		if (fuse_target)
		{
			fuse_target.getLysosomeFuse(this);
			p_cell.onRecycle(this, false);
		}
		else
		{
			release();
			deployGolgi();
			//don't do that
			//bud();
		}


		//p_cell.onRecycle(this);
	}

	protected override IEnumerator takePHDamage()
	{
		yield return new WaitForEndOfFrame();
		//donothing, lysosomes are immune to acid!
	}

	public override void updateLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		updateGridLoc(xx, yy);
	}







}

