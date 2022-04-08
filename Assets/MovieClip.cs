﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class MovieClip : MonoBehaviour
{
    public Sprite[] Sprites;
    public MovieClip SubClip;
    public bool Loop;
    public string[] FrameNames;
    public string[] NameIndexPairs;
    private SpriteRenderer _sr;
    private Image _image;
    private float _maxWidth;
    private float _maxHeight;
    private float _counter = 0;
    public float _frameInterval = 0.0166f;
    private int _spriteIndex = 0;
    protected bool _playing;
    private List<Sprite> _currentSet;
    public delegate void Finished(MovieClip mc);
    public Finished onFinished;
    private Dictionary<string,List<Sprite>> framesByName = new Dictionary<string, List<Sprite>>();
    private Dictionary<List<Sprite>, bool> ClipLoopStatus = new Dictionary<List<Sprite>, bool>();
    public float FrameInterval
    {
        get
        {
            return _frameInterval;
        }
        set
        {
            _frameInterval = value;
        }
    }

    public Sprite sprite
    {
        get
        {
            if (_sr != null)
                return _sr.sprite;
            if (_image != null)
                return _image.sprite;
            return null;
        }
        set
        {
            if (_sr != null)
                _sr.sprite = value;
            if (_image != null)
                _image.sprite = value;
        }
    }

    public void SetFrameInterval(float value)
    {
        _frameInterval = value;
    }
    

    public int CurrentSpriteIndex
    {
        get { return _spriteIndex; }
    }
    public float MaxWidth
    {
        get
        {
            return _maxWidth;
        }
    }

    public float MaxHeight
    {
        get
        {
            return _maxHeight;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        
        _image = GetComponentInChildren<Image>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        for(int i=0; i < Sprites.Length; i++)
        {
            _maxHeight = Mathf.Max(Sprites[i].bounds.size.y, _maxHeight);
            _maxWidth = Mathf.Max(Sprites[i].bounds.size.x, _maxWidth);
        }
        for(int i=0; i < NameIndexPairs.Length; i++)
        {
            string[] info = NameIndexPairs[i].Split(':');
            string[] frameBoundaries = info[1].Split('-');
            int startFrame = int.Parse(frameBoundaries[0]);
            int endFrame = int.Parse(frameBoundaries[1]);
            bool loop = (frameBoundaries.Length > 2 && frameBoundaries[2] == "loop");
           
            List<Sprite> frames = new List<Sprite>();
            for(int j=startFrame; j <= endFrame; j++ )
            {
                frames.Add(Sprites[j]);
            }
            framesByName[info[0]] = frames;
            ClipLoopStatus[frames] = loop;
        }
    }

  

    public void Play()
    {
        this.sprite = Sprites[_spriteIndex];
        Debug.Log("current index " + _spriteIndex);
        _playing = true;
    }

    public void Stop()
    {
        _playing = false;     
    }

    private void Update()
    {
        if (_playing)
        {
            _counter += Time.deltaTime;
            if (_counter > _frameInterval)
            {
                _counter = 0;
               
                _spriteIndex++;
                if (_currentSet != null)
                {
                    if (_spriteIndex == _currentSet.Count)
                    {
                        if (ClipLoopStatus[_currentSet])
                        {
                            _spriteIndex = 0;
                        }
                        else
                        {
                            _playing = false;
                            onFinished.Invoke(this);
                            return;
                        }
                    }
                }
                else
                {
                    if (_spriteIndex == Sprites.Length)
                    {
                        if (Loop)
                            _spriteIndex = 0;
                        else
                        {
                            _playing = false;
                            onFinished?.Invoke(this);
                            return;
                        }
                    }
                    this.sprite = Sprites[_spriteIndex];
                }
            }
        }
    }

    public void GotoAndPlay(int keyframe)
    {
        GotoAndStop(keyframe);
        _playing = true;
    }

    public void GotoAndPlay(string framename)
    {
        GotoAndStop(framename);
        _playing = true;
    }


    public void GotoAndStop(int keyframe)
    {
        this.sprite = Sprites[keyframe];
        _spriteIndex = keyframe;
        _playing = false;
    }

    public void GotoAndStop(string frameName)
    {
        if (framesByName.ContainsKey(frameName))
        {
            List<Sprite> frames = framesByName[frameName];
     
            this.sprite = frames[0];
            _currentSet = frames;
            _spriteIndex = 0;
            _playing = false;
            return;
        }
        
        //or....
        int index = Array.IndexOf(FrameNames, frameName);
        if (index < 0)
        {
            index = 0;
            Debug.LogWarning("no frame matched name: " + frameName);
        }
        this.sprite = Sprites[index];
        _spriteIndex = index;
        _playing = false;
    }
}
