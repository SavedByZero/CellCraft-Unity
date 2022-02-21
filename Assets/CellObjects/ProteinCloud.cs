using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProteinCloud : CellObject
{
	private int product = Selectable.NOTHING;
	private DockPoint exit;
	private int exit_count = 0;
	private bool exit_wait = false;
	private int EXIT_MAX = 60;
	private Coroutine _waitExitRoutine;
		
	public ProteinCloud()
	{
		//blendMode = BlendMode.DARKEN;  //TODO:?
		speed = 2;
		init();
	}

	public override void destruct()
	{
		exit = null;
	}

	public void setProduct(int i)
	{
		product = i;
	}

	public void setExit(DockPoint d, int xoff, int yoff)
	{
		exit = d.copy();
		pt_dest = new Point(exit.x + xoff, exit.y + yoff - 14);//offset 14 pixels for the animation
	}

	private IEnumerator waitExit()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			exit_count++;
			if (exit_count > EXIT_MAX)
			{
				exit_count = 0;
				exit_wait = !p_cell.askForERExit(this); //try and get an exit
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
		//p_cell.freeERExitPoint(exit.index); //free the exit point;
		p_cell.growVesicle(this, product);
		p_cell.killProteinCloud(this);
	}

	public void swimER()
	{
		if (pt_dest != null)
		{
			moveToPoint(pt_dest, FLOAT, true);
		}
	}
}

