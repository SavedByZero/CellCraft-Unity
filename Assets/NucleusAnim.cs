using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NucleusAnim : MovieClip
{
	public PoreMatrix pores;
	public NucleolusPoreMatrix n_pores;
	public MovieClip clip1;

	void Start()
	{
		clip1 = null; //to avoid a bug in SelectedPanel  //TODO: do I need this?
	}

	/**
		 * 0 for regular pore, 1 for Nucleolus pore
		 * @param	i
		 * @return
		 */

	public Point getPoreLoc(int i = 0, bool doOpen= false)
	{
			MovieClip p = null;
			if(i == 0){
				p = pores.getPore(doOpen);
			}
			else
			{
				p = n_pores.getPore(doOpen);
			}
			Point pt = new Point(p.transform.position.x, p.transform.position.y);
			pt.x *= this.transform.localScale.x;
			pt.y *= this.transform.localScale.y;
			return pt;
		}
	public List<object> getPorePoint(int i = 0) 
	{
			MovieClip p;
			if (i == 0) {
				p = pores.getPore(false);
			}else
			{
				p = n_pores.getPore(false);
			}
		Point pt = new Point(p.transform.position.x, p.transform.position.y);
		pt.x *= this.transform.localScale.x;
		pt.y *= this.transform.localScale.y;
		string name = p.name;
		int id = int.Parse(name.Substring(5, 2));
//trace("NucleusAnim.getPorePoint id = " + id);
			return new List<object> { pt, id };
	}
		
	public Point getPoreByI(int i, int type = 0)
	{
		MovieClip p;
		if (type == 0)
		{
			p = pores.getPoreByI(i);
		}
		else
		{
			p = n_pores.getPoreByI(i);
		}
		Point pt = new Point(p.transform.position.x, p.transform.position.y);
		pt.x *= this.transform.localScale.x;
		pt.y *= this.transform.localScale.y;
		return pt;
		//var p:Point = nclip.getPoreByI(i, type);
		//return p;
	}

	public void openPore(int i, int type = 0) 
	{
		if (type == 0)
		{
			pores.openPore(i);
		}
		else
		{
			n_pores.openPore(i);
		}
	}


	public bool freePore(int i = 0) {
		if (i == 0)
		{
			return pores.freePore();
		}
		else
		{
			return n_pores.freePore();
		}
	}

}

