using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NucleusAnim : MovieClip
{

	public MovieClip clip1;

	void Start()
	{
		clip1 = null; //to avoid a bug in SelectedPanel  //TODO: do I need this?
	}

	
}

