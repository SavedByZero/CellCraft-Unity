using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PinkVesicle : BlankVesicle
{
    public PinkVesicle()
    {
        
    }

    public override void playAnim(string label)
    {
        base.playAnim(label);
        if (label == "fade")
        {
            this.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(new TweenCallback(delegate
            {
                onAnimFinish(CellGameObject.ANIM_FADE);
            }));
        }
    }
}

