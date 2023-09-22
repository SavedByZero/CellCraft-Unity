using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnzymeRNA : RNA
{
    
	public override void InitRNA(int i, int count = 1, string pc_id = "")
	{
        FrameInterval = .05f;
        speed = 1f;
        base.InitRNA(i, count);
	}

    public override void playAnim(string label)
    {
       
        base.playAnim(label);
    }
}

