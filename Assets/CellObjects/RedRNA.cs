using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RedRNA : EvilRNA
{

    public override void InitRNA(int i, int count, string pc_id = "")
    {
        base.InitRNA(i, count, pc_id);
        //super(i, count, pc_id);
    }

    public override void playAnim(string label)
    {
        if (label == "infest")
        {
             
            //Infest.GotoAndPlay(0);
        }
        else if (label == "fast_grow" || label == "fastGrow")
        {
            //Grow.FrameInterval = 0.01f;
           // Grow.GotoAndPlay(0);
        }
        base.playAnim(label);
    }
}

