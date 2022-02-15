using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class WaveEntry
{
	public string id;
	public string type;
	public int count;
	public int original_count;
	public int spawned_count=0;
	public int escaped_count = 0;
	public int infest_count = 0;
	public int release_count;
	public bool active;
	public float spread;
	public float vesicle; //number between 0 & 1 signifying percent of viruses wrapped in vesicles
	public int delay; //frames to delay the virus from activating
	public bool defeated = false; //has this wave been killed by the player?
	public int sleep_seconds; //how many seconds does this thing wait before coming back?
		
	public float frac; 	 //what fraction of our original amount to we have?
	public float dormant_time = 0; //how many frames have we lied dormant
		
	public float TIME_CHANCE = 10; //how many encounter ticks to lie dormant for (1% chance)*(100*(count/original_count)) of releasing all viruses
		
	public WaveEntry()
	{

	}

	public WaveEntry copy() 
	{
			WaveEntry w = new WaveEntry();
			w.id = id;
			w.type = type;
			w.count = count;
			w.original_count = original_count;
			w.release_count = release_count;
			w.spawned_count = spawned_count;
			w.escaped_count = escaped_count;
			w.infest_count = infest_count;
			w.active = active;
			w.spread = spread;
			w.delay = delay;
			w.frac = frac;
			w.vesicle = vesicle;
			w.dormant_time = dormant_time;
			return w;
	}

	public string toString()
	{
		return ("WaveEntry{id=" + id + ",type=" + type + ",count=" + count + "}");
	}
}

