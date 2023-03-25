using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Glass : MovieClip
{

    private void Start()
    {
        onFinished += endGlassAnimation;
    }
    public void Animate(Color color)
    {
        GetComponentInChildren<Image>().DOColor(color, 0.5f).OnComplete(new TweenCallback(delegate {
            Play();
        }));
        
        
    }

    void endGlassAnimation(MovieClip me, string justPlayed)
    {
        GetComponentInChildren<Image>().DOColor(Color.white, 0.5f);
    }
    
}
