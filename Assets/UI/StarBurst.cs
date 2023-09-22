using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StarBurst : MovieClip
{
	public Cell p_cell;
		
	public void Start()
	{
		onFinished += finish;
	}

    private void finish(MovieClip mc, string justPlayed)
    {
		finishAnim();
    }

    public void finishAnim()
	{
		this.transform.SetParent(null);
		Destroy(this.gameObject);
		//p_cell.removeStarburst(this);
		//p_cell = null;
	}
}

