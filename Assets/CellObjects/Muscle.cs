using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Muscle : MonoBehaviour
{
    public delegate void FinishedStretching();
    public FinishedStretching onFinishStretching;
    private bool _stretching;
    private Rigidbody2D _rb;
    private Membrane _membrane;
    public bool Debug;
    // Start is called before the first frame update
    void Start()
    {
        _membrane = GameObject.FindObjectOfType<Membrane>();
        _rb = GetComponent<Rigidbody2D>();
       
    }

    public IEnumerator Settle()
    {
        //stop moving

        // _rb.transform.SetParent(this.transform.parent);
        //  
        yield return new WaitForSeconds(2);
        _rb.MovePosition(_membrane.transform.position);
        GetComponent<Wiggler>().Active = true;
        //this.gameObject.SetActive(false);
        //resume wiggling
    }

    public void Stretch(float xDir, float yDir)
    {
         GetComponent<Wiggler>().Active = false;
       
        if (!_stretching)
        {
            _stretching = true;
            Vector3 originalPos = this.transform.localPosition;

            _rb.DOMove(new Vector3(xDir, yDir, 0), 1f).OnComplete(new TweenCallback(delegate
            {
               
                _stretching = false;
                onFinishStretching?.Invoke();

                StartCoroutine(Settle());

            }));
        }
    }

    private void Update()
    {
        if (Debug)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _rb.MovePosition(new Vector3(mouse.x, mouse.y, 0));
        }
    }
}