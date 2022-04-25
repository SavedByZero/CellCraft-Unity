using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ER : CellObject
{
	public Locator dock0;
	public Locator dock1;
	public Locator dock2;
	public Locator dock3;
	public Locator dock4;
	public Locator dock5;
	public Locator dock6;
	public Locator dock7;
	public Locator dock8;
	public Locator dock9;
	public Locator dock10;
	public Locator dock11;
	public Locator dock12;
	public Locator dock13;
	public Locator dock14;
	public Locator dock15;
	public Locator dock16;
	public Locator dock17;
	public Locator dock18;
	public Locator dock19;
	public Locator dock20;
	public Locator dock21;
	public Locator dock22;
	public Locator dock23;
	public Locator dock24;
	public Locator dock25;
	public Locator dock26;
	public Locator dock27;
	public Locator dock28;
	public Locator dock29;
	public Locator dock30;
	public Locator dock31;
	public Locator dock32;
	public Locator dock33;
	public Locator exit0;
	public Locator exit1;
	public Locator exit2;
	public Locator exit3;
	public Locator exit4;
	public Locator exit5;
	public Locator exit6;
	public Locator exit7;
	public Locator exit8;
	public Locator exit9;
	public Locator exit10;
	public Locator exit11;
	public Locator exit12;
	public Locator exit13;
	public Locator exit14;
	public Locator exit15;
	public Locator exit16;
	public Locator exit17;
	public Locator exit18;
	public Locator exit19;
	public Locator exit20;
	public Locator exit21;
	public Locator exit22;
	public Locator exit23;
	public Locator exit24;
	public Locator exit25;
	public Locator exit26;
	public Locator exit27;
	public Locator exit28;
	public Locator exit29;
	public Locator exit30;
	public Locator exit31;
	public Locator exit32;
	public Locator exit33;
		
	private List<DockPoint> list_dock;
	private List<DockPoint> list_exit;
	private Coroutine _checkBusyRoutine;
	List<Locator> dockLocators = new List<Locator>();
	List<Locator> exitLocators = new List<Locator>();

	public override void Start()
	{
		base.Start();
		showSubtleDamage = true;
		singleSelect = true;
		text_title = "E.R.";
		text_description = "Endoplasmic Reticulum: builds vesicles and membrane";
		text_id = "er";
		num_id = Selectable._ER;
		bestColors = new bool[] { true, false, false };
		//list_actions = Vector.<int>([Act.MAKE_MEMBRANE]);
		setMaxHealth(100, true);
		list_dock = new List< DockPoint > ();
		list_exit = new List< DockPoint > ();
		Locator[] locators = GetComponentsInChildren<Locator>();
	
		for (int i=0; i < locators.Length; i++)
        {
			if (locators[i].gameObject.name.IndexOf("dock") > -1)
				dockLocators.Add(locators[i]);
			else if (locators[i].gameObject.name.IndexOf("exit") > -1)
				exitLocators.Add(locators[i]);
        }
		
		for (int i = 0; i < 33; i++) 
		{ //create a list of points and remove all the locator objects
			Locator d = dockLocators[i];  //let's hope 33 is the magic number...=p
			
			DockPoint p = new DockPoint();
			p.x = (int)(d.transform.position.x);
			p.y = (int)(d.transform.position.y);
			p.busy = false;
			p.index = i;
			list_dock.Add(p);
			//d.visible = true;
			d.gameObject.SetActive(false);
			//d = null;
			Locator e = exitLocators[i];
			DockPoint ep = new DockPoint();
			ep.x = (int)(e.transform.position.x);
			ep.y = (int)(e.transform.position.y);
			ep.busy = false;
			ep.index = i;
			list_exit.Add(ep);
			//e.visible = true;
			e.gameObject.SetActive(false);
			//e = null;
		}
		_checkBusyRoutine = StartCoroutine(checkBusy());
		init();
		instantSetHealth(10);
	}

	private void busyDockingPoint(int i)
	{
		//list_dock[i].busy = true;
		var dock = dockLocators[i];
		dock.gameObject.SetActive(false);
	}

	private void freeDockingPoint(int i)
	{
		//list_dock[i].busy = false;
		var dock = dockLocators[i];
		dock.gameObject.SetActive(true);
	}

	private void busyExitPoint(int i)
	{
		//list_exit[i].busy = true;
		var exit = exitLocators[i];
		exit.gameObject.SetActive(false);
	}

	private void freeExitPoint(int i)
	{
		//list_exit[i].busy = false;
		var exit = exitLocators[i];
		exit.gameObject.SetActive(true);
	}

	public IEnumerator checkBusy()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			foreach(DockPoint p in list_dock) {
				p.busyCount();
				//if (p.busy) busyDockingPoint(p.index);
				//else freeDockingPoint(p.index);
			}
			foreach(DockPoint e in list_exit) {
				e.busyCount();
				//if (e.busy) busyExitPoint(e.index);
				//else freeExitPoint(e.index);
			}
		}
	}

	public DockPoint findDockingPoint() 
	{
		foreach(DockPoint p in list_dock)
		{
			if (p.busy != true)
			{
				p.makeBusy();
				return p;
			}
		}
			return null;
	}

	public DockPoint findExitPoint()
	{
		foreach(DockPoint p in list_exit) {
			if (p.busy != true)
			{
				p.makeBusy();
				return p;
			}
		}
		return null;
	}

	public DockPoint findClosestDockingPoint(float xx, float yy) 
	{
		float bestDist2 = 1000000000; //One Bill-ion
		float dist2 = bestDist2;
		DockPoint bestP = null;
		int i = 0;
		foreach(DockPoint p in list_dock)
		{
			if (p.busy != true)
			{

				dist2 = FastMath.getDist2(xx, yy, p.x, p.y);
				if (dist2 < bestDist2)
				{
					bestDist2 = dist2;
					bestP = p;
				}
			}
			i++;
		}
			return bestP;
	}

	protected override void autoRadius()
	{
		setRadius(100);
	}

}

