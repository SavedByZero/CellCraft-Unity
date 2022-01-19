using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter 
{
    // Start is called before the first frame update
    private int _frames;
    private float _count;
    private float _fps;
    public float FPS
    {
        get
        {
            return _fps;
        }
    }
    void Start()

    {
        
    }

    // Update is called once per frame
    public void ExternalUpdate()
    {
        _frames++;
        _count += Time.deltaTime;
        if (_count >= 1)
        {
            _fps = _frames;
            _count = 0;
            _frames = 0;
        }
    }
}
