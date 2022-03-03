using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class BlankVesicle : CellObject
{
	protected int product = Selectable.NOTHING;
	protected float product_amount = 0;
	private DockPoint dock;
	private DockPoint exit;
	private Boolean exit_wait = false;
	private int exit_count = 0;
	private int EXIT_MAX = 60;
	private Boolean dock_wait = false;
	private int dock_count = 0;
	private int DOCK_MAX = 60;
	Coroutine _waitDockRoutine;
	Coroutine _waitExitRoutine;
		
	public BlankVesicle()
	{
		canSelect = false;
		singleSelect = true;
		text_title = "Vesicle";
		text_description = "A small vesicle on its way to becoming something else";
		text_id = "vesicle";
		bestColors = new bool[] { true, true, false };
		num_id = Selectable.VESICLE;
		list_actions = new List<CellAction> ();
		setMaxHealth(25, true);
		speed = 4;
		init();
	}

	public void setProduct(int i, float amount = 1)
	{
		product = i;
		product_amount = amount;

	}

	public override void onAnimFinish(int i, bool stop = true)
	{
		base.onAnimFinish(i, stop);
		switch (i)
		{
			case CellGameObject.ANIM_GROW: moveToGolgi(); break;
			case CellGameObject.ANIM_ADHERE: metamorphose(); break;
		}
	}

	protected virtual void metamorphose()
	{
		
		dock = null;
		moveToGolgiExit();
		this.gameObject.SetActive(false);
	}

	IEnumerator waitDock()
	{
		while (true)
		{
			yield return new WaitForSeconds(DOCK_MAX/60);
			{
				dock_count = 0;
				dock_wait = !moveToGolgi(); //try and get a dock
				if (!dock_wait)
				{ //if we're not waiting anymore!
					StopCoroutine(_waitDockRoutine);
				}
			}
		}
	}

	public void waitForDock()
	{ //wait for a dock
		if (!dock_wait)
		{ //if I'm not ALREADY waiting
			dock_wait = true;
			_waitDockRoutine = StartCoroutine(waitDock());
		}
	}

	IEnumerator waitExit()
	{
		exit_count++;
		while (true)
		{
			yield return new WaitForSeconds(EXIT_MAX / 60);
			if (exit_count > EXIT_MAX)
			{
				exit_count = 0;
				exit_wait = !p_cell.askForGolgiExit(this); //try and get an exit
				if (!exit_wait)
				{ //if we're not waiting anymore!
					StopCoroutine(_waitExitRoutine);
				}
			}
		}
	}

	public void waitForExit()
	{ //wait for an exit
		if (!exit_wait)
		{ //if I'm not ALREADY waiting
			exit_wait = true;
			_waitExitRoutine = StartCoroutine(waitExit());
		}
	}

	protected override void onArrivePoint()
	{

		if (dock != null)
		{
			playAnim("adhere");
		}
		else if (exit != null)
		{

			//p_cell.freeGolgiExitPoint(exit.index);
			p_cell.growFinalVesicle(new Point(x, y), product);
			p_cell.killBlankVesicle(this);
		}

	}

	public void setDockPoint(DockPoint d, int xoff, int yoff)
	{
		dock = d.copy();
		pt_dest = new Point(dock.x + xoff, dock.y + yoff - 14);
	}

	public void setExit(DockPoint d, int xoff, int yoff)
	{
		exit = d.copy();
		pt_dest = new Point(exit.x + xoff, exit.y + yoff - 14);
	}

	public void moveToGolgiExit()
	{
		p_cell.askForGolgiExit(this);
	}

	public void swimThroughGolgi()
	{
		moveToPoint(pt_dest, FLOAT, true);
	}

	public bool moveToGolgi() 
	{
			//trace("Move to golgi!");
			bool move = p_cell.dockGolgiVesicle(this);
			if(move){
				moveToPoint(pt_dest, FLOAT,true);
				return true;
			}else
		{
			waitForDock();
			return false;
		}
			//moveToObject(p_cell.getGolgiDock());
	}
		
		public void growER()
		{
			playAnim("grow_er");
		}

		public void grow()
		{
			playAnim("grow");
		}

}

