using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EvilRNA : RNA
{
	private int rotateDir = 1;
		
	private int TAUNT_TIME = 15;
	private int tauntCount = 15;
	private bool hasMnode = false;
	private MembraneNode mnode;
	private Point mnode_dist;
	private List<SlicerEnzyme> list_slicer;
	public bool doesRotateUp = false;
    private Coroutine _clingToNodeRoutine;
    private Coroutine _tauntCellRoutine;
    private Coroutine _rotateUpRoutine;

    public EvilRNA(int i, int count= 1, string pc_id = "") : base(i, count)
	{
		_tauntCellRoutine = StartCoroutine(tauntCell());
		//addEventListener(RunFrameEvent.RUNFRAME, tauntCell, false, 0, true);
		speed = 3;
		product_creator_id = pc_id;
	}

	public void addSlicer(SlicerEnzyme s)
	{
		if (list_slicer == null)
		{
			list_slicer = new List<SlicerEnzyme>();
		}
		list_slicer.Add(s);
	}

	public override void destruct()
	{
		mnode = null;
		mnode_dist = null;
		//targetSlicer = null;
		if (list_slicer != null)
		{
			for (int i = 0; i < list_slicer.Count; i++) {
				list_slicer.RemoveAt(list_slicer.Count-1);
			}
			list_slicer = null;
		}
		base.destruct();

	}

	public void setMnode(MembraneNode m)
	{
		//trace("EvilRNA.setMnode(" + m.index + ") me = " + name );
		if (!hasMnode)
		{
			hasMnode = true;
			mnode = m;
			mnode.addRNA(this);
			_clingToNodeRoutine = StartCoroutine(clingToNode());
			//addEventListener(RunFrameEvent.RUNFRAME, clingToNode, false, 0, true);
		}
		//mnode_dist = new Point(mnode.x-x, mnode.y-y);
	}

	public IEnumerator clingToNode()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame(); 
			x = (mnode.x + mnode.p_next.x) / 2;
			y = (mnode.y + mnode.p_next.y) / 2;
		}
	}

	public override void getPpodContract(float xx, float yy)
	{
		if (!hasMnode || (hasMnode && mnode.state_ppod == false))
		{
			x -= xx;
			y -= yy;
			if (isMoving)
			{
				if (pt_dest != null)
				{
					pt_dest.x -= xx;
					pt_dest.y -= yy;
				}
			}
		}
	}

	protected override void killMe()
	{
		if (list_slicer != null)
		{
			int length = list_slicer.Count;
			for (int i = length - 1; i >= 0; i--) {
				list_slicer[i].releaseByRNA(this);
				list_slicer[i] = null;
				list_slicer.RemoveAt(i);
			}
		}
		base.killMe();
	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		base.onAnimFinish(i, stop); //skip the MRNA onAnimFinish
		GotoAndStop("normal");
		switch (i)
		{
			case ANIM_THREAD:
				playAnim("die_thread");
				break;
			case ANIM_VIRUS_GROW:
				rotateUp();
				break;
			case ANIM_DIE:
				killMe();
				break;
			case ANIM_HARDKILL:
				hardKillMe();
				break;
			default:
				break;
		}
	}



	protected override void doMoveToGobj()
	{
		base.doMoveToGobj();
		if (hasMnode)
		{
			float x1 = x;
			float x2 = cent_x;
			float y1 = y;
			float y2 = cent_y;
			float dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
			if (dist2 <= BasicUnit.C_GRAV_R2)
			{
				mnode.removeRNA(this);
				mnode = null;
				hasMnode = false;
			}
			else
			{
				x2 = (mnode.x + mnode.p_next.x) / 2;
				y2 = (mnode.x + mnode.p_next.y) / 2;
				dist2 = (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
				if (dist2 > (50 * 50))
				{ //HACK AND MAGIC NUMBER
				  //trace("EvilRNA dist too great! loc=(" + x + "," + y + ") mloc=(" + x2 + "," + y2 + ")");
					checkOutsideMembrane();
				}
			}
		}
	}

	private void checkOutsideMembrane()
	{

		Vector2 v_cent = new Vector2(x - cent_x, y - cent_y);
		Vector2 v_node = new Vector2(x - mnode.x, y - mnode.y);
		float ang = FastMath.angleTo(v_cent,v_node);
		if (ang < (Mathf.PI / 2))
		{
			//we're outside
			x = (mnode.x + mnode.p_next.x) / 2;
			y = (mnode.y + mnode.p_next.y) / 2;
			v_cent.Normalize();
			v_cent *= (-51);
			x += v_cent.x;
			y += v_cent.y;
		}
	}


	protected virtual IEnumerator tauntCell()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			//every second, asks the cell for something to kill it
			tauntCount++;
			if (tauntCount > TAUNT_TIME)
			{
				tauntCount = 0;
				p_cell.tauntByEvilRNA(this);
			}
		}
	}

	public override void threadRNA(bool inPlace = false)
	{
		//we ignore the inPlace parameter, because evilRNA just has a different animation for "thread"
		//we have to include the parameter to remain compatible with the overriden function

		playAnim("thread");
		deliverProduct();
		//atRibosome = true;
	}

	private void rotateUp()
	{
		if (doesRotateUp)
		{
			float rotation = this.transform.eulerAngles.z; 
			rotation = Mathf.Floor(rotation);
			if (rotation < 0) 
				rotation = 360 + rotation;
			this.transform.eulerAngles = new Vector3(0, 0, rotation);
			_rotateUpRoutine = StartCoroutine(doRotateUp(rotation));
			//addEventListener(RunFrameEvent.RUNFRAME, doRotateUp, false, 0, true);
		}
		else
		{
			if (p_rib)
			{
				ribosomeTime();
			}
			else
			{
				//cytoplasmTime();
				//trace("EvilRNA.waitForRibosome() me = " + name);
				waitForRibosome();
			}
		}
	}

	private IEnumerator doRotateUp(float rotation)
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			rotation = (0.8f) * rotation;
			this.transform.eulerAngles = new Vector3(0, 0, rotation);
			if (rotation < 1)
			{
				rotation = 0;
				StopCoroutine(_rotateUpRoutine);
				ribosomeTime();
			}
		}
	}

	protected override void onArrivePoint()
	{
		base.onArrivePoint();
		ribosomeTime();
	}

	protected override IEnumerator waitRib()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			rib_count++;
			if (rib_count > RIB_MAX)
			{
				rib_count = 0;
				rib_wait = !p_cell.askForRibosome(this); //if it was successful, we are NOT waiting
				if (p_rib)
				{
					//moveToRibosome(p_rib, FLOAT);	//if we have a ribosome, go to it!
					ribosomeTime();
					StopCoroutine(_waitRibRoutine);
				}
			}
		}


	}

	private void ribosomeTime()
	{
		//trace("EvilRNA.ribosomeTime() me = " + name + "!");
		StopCoroutine(_clingToNodeRoutine);
		if (p_rib)
		{
			moveToRibosome(p_rib, FLOAT);
		}
		if (nuc_pore != null)
		{
			moveToNucleusPore(FLOAT);
		}
	}







}

