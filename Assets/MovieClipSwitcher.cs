using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieClipSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    public MovieClip[] Clips;
    private MovieClip _current;
    private Coroutine _sequencePlayer;
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
        ChangeClip(0);
    }

    private void finishedClip(MovieClip mc)
    {
        
    }

    public void PlaySequence(int[] indices, float[] times)
    {
        if (_sequencePlayer != null)
            StopCoroutine(_sequencePlayer);
        _sequencePlayer = StartCoroutine(sequenceHelper(indices, times));
    }

    IEnumerator sequenceHelper(int[] indices, float[] times)
    {
        for(int i=0; i < indices.Length; i++)
        {
            ChangeClip(i);
            Debug.Log("changed to " + i);
            if (times[i] == 0)
            {
                yield return new WaitForSeconds((float)_current.Sprites.Length * _current.FrameInterval);
            }
            else if (times[i] == -1)
            {
                break; //loop indefinitely
            }
            else
                yield return new WaitForSeconds(times[i]);
        }
    }

    public void ChangeClip(int index)
    {
        if (_current != null)
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
