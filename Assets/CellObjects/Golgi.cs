using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;


public class Golgi : CellObject
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
		
	private List<DockPoint> list_dock;
	private List<DockPoint> list_exit;
		
	private int resetCount = 0;
	private const int busyTime = 30 * 2; //2 seconds
	private Coroutine _checkBudyRoutine;

	void Start()
	{
		showSubtleDamage = true;
		singleSelect = true;
		text_title = "Golgi Body";
		text_description = "Processes vesicles";
		text_id = "golgi";
		num_id = Selectable.GOLGI;
		bestColors = new bool[] { false, false, true };
		setMaxHealth(250, true);
		Locator[] locators = GetComponentsInChildren<Locator>();
		list_dock = new List<DockPoint> ();
		list_exit = new List< DockPoint > ();
		//list_actions = Vector.<int>([Act.BUY_LYSOSOME,Act.BUY_PEROXISOME,Act.MAKE_DEFENSIN]);
		for (int i = 0; i < locators.Length; i++) 
		{ //create a list of points and remove all the locator objects
			GameObject currentLocator = locators[i].gameObject;
			string currentLocatorName = currentLocator.name;
			if (currentLocatorName.IndexOf("dock") > -1)
            {
				
				DockPoint p = new DockPoint();
				p.x = (int)(currentLocator.transform.position.x);
				p.y = (int)(currentLocator.transform.position.y);
				p.busy = false;
				p.index = i;
				p.setBusyTime(busyTime);
				list_dock.Add(p);
				
			}
			else if (currentLocatorName.IndexOf("exit") > -1)
            {
				
				DockPoint ep = new DockPoint();
				ep.x = (int)(currentLocator.transform.position.x);
				ep.y = (int)(currentLocator.transform.position.y);
				ep.busy = false;
				ep.index = i;
				ep.setBusyTime(busyTime);
				list_exit.Add(ep);
				
			}
			currentLocator.transform.SetParent(null);
			GameObject.Destroy(currentLocator);

		}
		list_exit.Reverse();
		_checkBudyRoutine = StartCoroutine(checkBusy());
		init();
		instantSetHealth(10);
	}

	public IEnumerator checkBusy()
	{
		resetCount++;
		while (true)
		{ //every twelve seconds, clear everything
			yield return new WaitForEndOfFrame();
			if (resetCount > busyTime * 6)
            {
				resetCount = 0;
				foreach (DockPoint pp in list_dock)
				{
					//freeDockingPoint(pp.index);
					pp.unBusy();

				}
				foreach (DockPoint ee in list_exit)
				{
					//freeExitPoint(ee.index);
					ee.unBusy();

				}
			}
			else
			{
				foreach (DockPoint p in list_dock)
				{
					p.busyCount();
					/*if (!p.busy) {
						freeDockingPoint(p.index);
					}*/

				}
				foreach (DockPoint e in list_exit)
				{
					e.busyCount();
					/*if(!e.busy){
						freeExitPoint(e.index);
					}*/

				}
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
}

