using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BigVessicle : CellObject
{
		private float size=0;
		private float maxSize=0;
		private const float GROW_SPEED = 2;
		private List<CellObject> list_contents;
		private Image shape;
		
		private MembraneNode mnode;
		
		private int product = Selectable.NOTHING;
		
		public bool needsLysosomes = false;
		private int lysoNeeded = 0;
		private int lysoOrdered = 0;
		private int lysoFused = 0;
		private int lyso_wait_count = 0;
		private int lyso_wait_time = 60; //every 2 seconds poll again
			
		private int mem_wait_count = 0;
		private const int MEM_WAIT_TIME = 15;
		private Coroutine _animPHRoutine;
		private Coroutine _waitForMembraneRoutine;
		private Coroutine _grow;
		private Coroutine _shrink;
	private Coroutine _animateDigestRoutine;
	private Coroutine _waitForLysosomesRoutine;
		
		private int anim_phase = 0;

		public float ph_balance = 7.5f;
		private float ph_show = 7.5f;


		private bool isDigestGrow = false;
		
		
		
		private bool unRecycle = false;

	public BigVessicle(float startSize = 0)
	{
		size = startSize;
		canSelect = false;
		singleSelect = true;
		text_title = "Big Vesicle";
		text_description = "A large vesicle for holding things";
		text_id = "big_vesicle";
		num_id = Selectable.BIGVESICLE;

		list_actions = new List<CellAction>();
		list_contents = new List<CellObject>;
		
		//shape = new Shape();   //TODO: this will need to be added manually
		//addChild(shape);
		
		init();

		updateBigVesicle();
	}

	public void setProduct(int i)
	{
		product = i;
	}

	public void setPH(float ph)
	{
		ph_balance = ph;
		//updateBigVesicle();
		//addEventListener(RunFrameEvent.RUNFRAME, animPH, false, 0, true);
		_animPHRoutine = StartCoroutine(animPH());
	}

	IEnumerator animPH()
    {
		while (true)
		{
			float change = 0.09f;
			if (ph_show < ph_balance - change)
			{
				ph_show += change;
			}
			else if (ph_show > ph_balance + change)
			{
				ph_show -= change;
			}
			else
			{
				ph_show = ph_balance;
				StopCoroutine(_animPHRoutine);//removeEventListener(RunFrameEvent.RUNFRAME, animPH);
			}
			updateBigVesicle();
			yield return new WaitForEndOfFrame();
		}
	}


	public override void onCanvasWrapperUpdate()
	{
		updateBigVesicle();
	}

	public override void updateBubbleZoom(float n)
	{
		base.updateBubbleZoom(n);
		updateBigVesicle();
	}

	public void updateBigVesicle()
	{
		if (size > 0)
		{

			uint col;
			uint col2;
			uint col3;

			col = PH.getCytoColor(ph_show);
			col2 = PH.getLineColor(ph_show);
			col3 = PH.getGapColor(ph_show);

			/*   //TODO: we need to figure out a substitute for this 
			shape.graphics.clear();
			shape.graphics.beginFill(col, 1);
			shape.graphics.lineStyle(Membrane.OUTLINE_THICK / 1.5, 0x000000);
			shape.graphics.drawCircle(0, 0, size);
			shape.graphics.endFill();
			shape.graphics.lineStyle(Membrane.SPRING_THICK / 2, col2);
			shape.graphics.drawCircle(0, 0, size);
		
			shape.graphics.lineStyle(Membrane.GAP_THICK / 3, col3);
			shape.graphics.drawCircle(0, 0, size);
			*/

		}
	}

	public float getLysosNeeded() 
	{
		float lysos = PH.getLysosNeeded(ph_balance, getCircleVolume(), Lysosome.PH_BALANCE);
		return Mathf.Ceil(lysos);
	}

	public override float getCircleVolume()
	{
		return Mathf.PI * size * size;
	}

	public void startDigestGrow(CellObject c)
	{
		canSelect = false; //can't select a digestion vesicle
		float r = c.getRadius() * 1.25f;
		startGrow(r);
		isDigestGrow = true;
		putIn(c);
	}

	public void instantGrow(float s)
	{
		maxSize = s - (Membrane.OUTLINE_THICK_ / 2);
		size = maxSize;
		setRadius(s);
		updateBigVesicle();
	}

	IEnumerator waitForMembrane()
	{
		
		while (true)
		{
			yield return new WaitForSeconds(MEM_WAIT_TIME);
			//removeEventListener(RunFrameEvent.RUNFRAME, waitForMembrane);
			StopCoroutine(_waitForMembraneRoutine);
			goToMembrane();
		}
	}
	public void goToMembrane()
	{
		mnode = tryGetNode();
		if (mnode)
		{
			moveToPoint(new Point(mnode.x, mnode.y), CellGameObject.FLOAT, true);
			p_cell.c_membrane.acceptVesicle();
		}
		else
		{
			_waitForMembraneRoutine = StartCoroutine(waitForMembrane());
			//addEventListener(RunFrameEvent.RUNFRAME, waitForMembrane, false, 0, true);
		}
	}

	public MembraneNode tryGetNode()
	{
			if(p_cell.c_membrane.acceptingVesicles){
				return p_cell.c_membrane.findClosestMembraneNode(x, y);
			}else
			{
				return null;
			}
	}
		
	public void startGrow(float s) 
	{
		size = 0;
		setRadius(0);
		maxSize = s;
		_grow = StartCoroutine(grow());
		//addEventListener(RunFrameEvent.RUNFRAME, grow, false, 0, true);
	}

	private void startShrink()
	{
		unDigestContents();
		StartCoroutine(shrink());
		//addEventListener(RunFrameEvent.RUNFRAME, shrink, false, 0, true);
	}

	IEnumerator grow()
	{

		//trace("BigVesicle.grow()");
		while (true)
		{
			yield return new WaitForEndOfFrame();
			size += GROW_SPEED;
			setRadius(size + Membrane.OUTLINE_THICK_ / 4);
			if (size > maxSize)
			{

				onGrowFinish();
			}
			updateBigVesicle();
		}
	}

	IEnumerator shrink()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			size -= GROW_SPEED;
			setRadius(size + Membrane.OUTLINE_THICK_ / 4);
			updateBigVesicle();
			if (size <= 0)
			{
				size = 0;
				onShrinkFinish();
			}
		}

	}

	private void onGrowFinish()
	{
		size = maxSize;
		StopCoroutine(_grow);
		if (isDigestGrow)
		{
			lysoNeeded = getLysosNeeded();
			lysoOrdered = lysoNeeded;
			callForLysosomes();
		}
	}

	private void onShrinkFinish()
	{
		size = 0;
		StopCoroutine(_shrink);
		foreach(CellObject c in list_contents) 
			{
			if (c.dying == false)
			{
				c.outVesicle(unRecycle); //release the contents of the vesicle. If we canceled a recycle, undoom them.
			}
		}
		//shape.graphics.clear();
		p_cell.killSomething(this);
	}

	/**
	 * Dismisses incoming lysosomes, unfuses existing lysosomes, shrinks the vesicle, releases the organelle
	 */

	public void cancelRecycle()
	{
		unRecycle = true;
		if (lysoOrdered > lysoFused)
		{
			p_cell.dismissLysosomes(this);
		}

		if (size < maxSize)
		{
			size = maxSize;
			StopCoroutine(_grow);
		}
		anim_phase = 0;

		removeEventListener(RunFrameEvent.RUNFRAME, animateDigest);
		unPackLysos();

		if (lysoFused == 0)
		{
			startShrink(); //just do it right now, thanks
		}
	}

	private void callForLysosomes()
	{
		lysoNeeded = p_cell.askForLysosomes(this, lysoNeeded);
		if (lysoNeeded > 0)
		{
			addEventListener(RunFrameEvent.RUNFRAME, waitForLysosomes, false, 0, true);
		}
		else
		{
			lysoNeeded = 0;
		}
	}

	public void getLysosomeFuse(Lysosome l)
	{
		addToPH(l.getCircleVolumeV(), Lysosome.PH_BALANCE);
		//lysoOrdered--;
		lysoFused++;
		if (lysoFused >= lysoOrdered)
		{
			Debug.Log("BigVesicle.getLysosomeFuse(), We are GOOD!");
			digestContents();
		}
		if (unRecycle)
		{ //if we're unrecycling, spit it back out
			unPackLysos();
		}
	}

	public void onLysosomeBud()
	{
		lysoOrdered--;

		if (lysoOrdered <= 0 || lysoFused <= 0)
		{
			startShrink();
		}
	}

	private void unFuseLysosome(Lysosome l)
	{
		removeFromPH(l.getCircleVolumeV(), Lysosome.PH_BALANCE);
		l.setBigVesicleFuser(this);
		lysoFused--;
	}

	private void unDigestContents()
	{
		foreach(CellObject c in list_contents) {
			c.setPHDamage(7.5f, 0);
		}
	}

	private void digestContents()
	{
		foreach(CellObject c in list_contents) {
			c.setPHDamage(ph_balance, 3);
		}
		_animateDigestRoutine = StartCoroutine(animateDigest());
	}

	private void animateDigest()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			float max = 1.1f;
			float min = 0.9f;
			float change = 0.02f;
			float scaleX = this.transform.localScale.x;
			float scaleY = this.transform.localScale.y;

			switch (anim_phase)
			{
				case 0:
					this.transform.localScale = new Vector3(scaleX + change, scaleY - change, 1);
					
					if (scaleX > max)
					{
						scaleX = max;
						scaleY = min;
						anim_phase = 1;
					}
					break;
				case 1:
					scaleX -= change; scaleY += change;
					if (scaleY > max)
					{
						scaleY = max;
						scaleX = min;
						anim_phase = 0;
						if (checkContentsDigested())
						{
							anim_phase = 2;
						}

					}
					break;
				case 2:
					scaleX += change; scaleY -= change;
					if (scaleY < 1)
					{
						scaleY = 1;
						scaleX = 1;
						anim_phase = 0;
						StopCoroutine(_animateDigestRoutine);
						unPackLysos();
					}
					break;
			}
		}
	}

	private void unPackLysos()
	{
		Vector2 v = new Vector2(1, 0);
		v *= (size + Membrane.OUTLINE_THICK_ / 3);
		int length = lysoFused;
		for (int i = 0; i < length; i++) 
		{
			v = FastMath.rotateVector((Mathf.PI * 2) / length,v);
			float r = FastMath.toRotation(v) * 180 / Mathf.PI;
			r += 90;
			Lysosome l = p_cell.budLysosome(x + v.x, y + v.y, r);
			unFuseLysosome(l);
		}
		//startShrink();
	}

	private bool checkContentsDigested() 
	{
			int i = 0;
			foreach(CellObject c in list_contents)
	{
		if (c)
		{
			if (c.getHealth() <= 0 || c.dying)
			{
				list_contents[i] = null;
			}
			if (list_contents[i] != null)
			{
				return false;
			}
		}
		i++;
	}
	list_contents = null;
			return true;
		}

	private void removeFromPH(float vol, float ph) 
	{
		float newPh = PH.removeFromPH(ph, vol, ph_balance, getCircleVolume());
		if (newPh > 7.5f) newPh = 7.5f; //hack to avoid alkalinity
		setPH(newPh);
	}

	private void addToPH(float vol, float ph) 
	{
		float newPh = PH.mergePH(ph, vol, ph_balance, getCircleVolume());
		setPH(newPh);
	}

	IEnumerator waitForLysosomes() 
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			lyso_wait_count++;
			if (lyso_wait_count > lyso_wait_time)
			{
				StopCoroutine(waitForLysosomes());
				callForLysosomes();
				lyso_wait_count = 0;
			}
		}
	}

	public void putIn(CellObject c)
	{
		bool isIn = false;
		foreach(CellObject cc in list_contents) {
			if (cc == c)
			{
				isIn = true;
			}
		}
		if (!isIn)
		{
			list_contents.Add(c);
			c.inVesicle(this);
		}
	}

	protected override void doMoveToPoint()
	{
		float diffx = x;
		float diffy = y;
		base.doMoveToPoint();
		diffx -= x;
		diffy -= y;
		followContents(diffx, diffy);
	}

	protected override void doMoveToGobj()
	{
		float diffx = x;
		float diffy = y;
		base.doMoveToGobj();
		diffx -= x;
		diffy -= y;
		followContents(diffx, diffy);
	}

	private void followContents(float xx, float yy)
	{
		foreach(CellObject c in list_contents) 
		{
			c.push(-xx, -yy);
		}
	}

	protected override void onArrivePoint()
	{
		if (mnode != null)
		{
			p_cell.makeMembrane(new Point(x, y));
			p_cell.killSomething(this);
		}
		else
		{

		}

	}





}

