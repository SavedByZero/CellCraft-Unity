using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PoreMatrix :MonoBehaviour
{
    List<MovieClip> ready;
    List<MovieClip> busy;

    public List<MovieClip> pores;

	void Start()
	{
		pores = new List<MovieClip>(GetComponentsInChildren<MovieClip>());
		busy = new List< MovieClip > ();
		ready = new List< MovieClip > ();
		for (int i = 0; i < pores.Count; i++) 
		{
			ready.Add(pores[i]);	
		}
	}

	private string pad(int i) 
	{
			if (i< 10) {
			return "0" + i;
			}else
	return i.ToString();
	}
		
	public void animateFinish()
	{
		MovieClip pore = null;
		if (busy.Count >= 1)
		{
			pore = busy[0]; //get the first pore in the busy array
		}
		if (pore != null)
		{
			busy.RemoveAt(0); //remove it from the busy array
			ready.Add(pore);  //push it onto the ready array
		}
	}
		
	public MovieClip getPoreByI(int i)
	{
		return pores[i];//  this["pore_" + pad(i)];
	}

	public void openPore(int i) 
	{
		MovieClip p = getPoreByI(i);
		p.GotoAndPlay("open");
	}

	public MovieClip getPore(bool doOpen = false)
	{
			float n = Mathf.Floor(UnityEngine.Random.Range(0f,1f)* ready.Count);
		MovieClip pore;
			if(ready.Count != 0){
				pore = ready[(int)n]; 		//get it to return
				ready.RemoveAt((int)n); 				//remove it from the ready array
				busy.Add(pore);					//push it onto the busy array
				//trace("pore = " + pore);
				if(doOpen){
				pore.GotoAndPlay(0);
					//pore.GotoAndPlay("open"); 			//animate it opening
				}
			}else
			{
				n = Mathf.Floor(UnityEngine.Random.Range(0f, 1f) *busy.Count);
				pore = busy[(int)n];
			}
			return pore; //return the pore
			
	}
		
		public bool freePore()
		{
			if (ready.Count > 1)
			{
				return true;
			}
			return false;
		}
		

}

