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
    public bool Debugg;
    // Start is called before the first frame update
    void Start()
    {
        _membrane = GameObject.FindObjectOfType<Membrane>();
        _rb = GetComponent<Rigidbody2D>();
       
    }

    public IEnumerator Settle(Vector3 originalPos)
    {
        //stop moving

        // _rb.transform.SetParent(this.transform.parent);
        //  
        yield return new WaitForSeconds(0.01f);
        _rb.isKinematic = true;
        _rb.transform.localPosition = Vector3.zero;
        _rb.isKinematic = false;
        _rb.gameObject.SetActive(false);
        _membrane.GetComponentInChildren<Wiggler>(true).gameObject.SetActive(true);
        //Wiggler w = GetComponent<Wiggler>();
        // w.UpdateCorePos(_membrane.transform.localPosition);
        // w.Active = true;
        //this.gameObject.SetActive(false);
        //resume wiggling
    }

    public void Stretch(float xDir, float yDir)
    {
        Debug.Log("original stretch dir " + xDir + ", " + yDir);
        _membrane.GetComponentInChildren<Wiggler>(true).gameObject.SetActive(false);
        Vector2 norm = new Vector3(xDir, yDir).normalized;
        _rb.gameObject.SetActive(true);
        Debug.Log("current velocity " + _rb.velocity);
        if (!_stretching)
        {
            _stretching = true;
            Vector3 originalPos = this.transform.localPosition;

            _rb.DOMove(new Vector3(norm.x*4, norm.y*4, 0), 2f).OnComplete(new TweenCallback(delegate
            {
               
                _stretching = false;
                onFinishStretching?.Invoke();

                StartCoroutine(Settle(originalPos));

            }));
        }
    }

    private void Update()
    {
        if (Debugg)
        {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _rb.MovePosition(new Vector3(mouse.x, mouse.y, 0));
        }
    }
}
