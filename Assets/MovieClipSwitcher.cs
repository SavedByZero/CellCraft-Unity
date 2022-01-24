using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieClipSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    public MovieClip[] Clips;
    private MovieClip _current;
    private int _index;
    public bool Sequential;
    void Start()
    {
        
        for(int i=0; i < Clips.Length; i++)
        {
            if (!Clips[i].Loop)
                Clips[i].onFinished += finishedClip;
            Clips[i].gameObject.SetActive(false);
        }
        _current = Clips[0];
        _current.gameObject.SetActive(true);
    }

    private void finishedClip(MovieClip mc)
    {
        
    }

    public void ChangeClip(int index)
    {
        _current.gameObject.SetActive(false);
        _current = Clips[index];
        _current.gameObject.SetActive(true); 
        _current.GotoAndPlay(0);
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
