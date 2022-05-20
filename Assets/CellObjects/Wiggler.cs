using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wiggler : MonoBehaviour
{
    public Vector3 _corePos;
    private Rigidbody2D _rb;
    private bool _active;
    public float Radius = 1;
    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            _active = value;
            _corePos = this.transform.position;
        }
    }

    private void Awake()
    {
        //_wiggleDirection = new Vector3(-0.1f, 0.1f, 0);
        _rb = GetComponentInChildren<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
        //StartCoroutine(Wiggle());
    }

    public void ChangeOriginalPos(Vector3 newPos)
    {
        
    }

    private void Update()
    {
        if (Active)
            _rb.MovePosition(_corePos + new Vector3(Mathf.Cos(Time.time), Mathf.Sin(Time.time), 0)*Radius);//(this.transform.localPosition + (_wiggleDirection * Time.deltaTime));
    }



    
}
