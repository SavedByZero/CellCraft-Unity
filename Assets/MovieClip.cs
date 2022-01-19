using System.Collections;
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
    private SpriteRenderer _sr;
    private Image _image;
    private float _maxWidth;
    private float _maxHeight;
    private float _counter = 0;
    private float _frameInterval = 0.0166f;
    private int _spriteIndex = 0;
    private bool _playing;

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
                Debug.Log("old ndex " + _spriteIndex);
                _spriteIndex++;
                Debug.Log("new index " + _spriteIndex);
                if (_spriteIndex == Sprites.Length)
                {
                    if (Loop)
                        _spriteIndex = 0;
                    else
                    {
                        _playing = false;
                        return;
                    }
                }
                this.sprite = Sprites[_spriteIndex];
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
