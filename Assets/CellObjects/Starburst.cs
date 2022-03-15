using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StarBurst : MovieClip
{
	public Cell p_cell;
		
	public StarBurst()
	{

	}

	public void finishAnim()
	{
		p_cell.removeStarburst(this);
		p_cell = null;
	}
}

