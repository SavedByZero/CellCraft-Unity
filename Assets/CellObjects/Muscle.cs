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
    private Vector3 _moveVector;
    public bool Debugg;
    public delegate void MovingTowards(float x, float y);
    public MovingTowards onMovingTowards;
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
        yield return new WaitForSeconds(1f);

        //_membrane.GetComponentInChildren<Wiggler>(true).gameObject.SetActive(true);
      

        MembraneNode[] nodes = _membrane.GetComponentsInChildren<MembraneNode>();
        GameObject sbAnchor = _membrane.gameObject.GetComponent<SoftBody>().Anchor;
        sbAnchor.transform.DOMove(GameObject.FindObjectOfType<Cell>().c_nucleus.transform.localPosition, 1).SetEase(Ease.Linear).OnComplete(new TweenCallback(delegate {
            _rb.isKinematic = true;
            _rb.transform.localPosition = Vector3.zero;
           // _rb.isKinematic = false;
            _rb.gameObject.SetActive(false);
        }));//.DOBlendableLocalMoveBy(_moveVector, 2).SetEase(Ease.Linear);
        for(int i=0; i < nodes.Length; i++)
        {
           // Vector3 dir = _rb.transform.localPosition - nodes[i].transform.localPosition;
           // nodes[i].gameObject.transform.DOBlendableMoveBy(_moveVector, 2).SetEase(Ease.Linear);
        }
    }

    public void Stretch(float xDir, float yDir)
    {
       
        _membrane.GetComponentInChildren<Wiggler>(true).gameObject.SetActive(false);
        Vector3 norm = new Vector3(xDir, yDir, -Camera.main.transform.position.z).normalized;
        Debug.Log("old norm " + norm);
      
        norm.z = 0;
        norm = Vector3.ClampMagnitude(norm, 1.44f);
        //norm.x = Mathf.Round(norm.x);
        //norm.y = Mathf.Round(norm.y);
        Debug.Log("norm " + norm);
        norm *= _membrane.getRadius();
        Debug.Log("membrane radius " + _membrane.getRadius());
        Debug.Log("original stretch dir " + norm);
   
        _rb.gameObject.SetActive(true);
        Debug.Log("current velocity " + _rb.velocity);
        if (!_stretching)
        {
            _stretching = true;
            Vector3 originalPos = this.transform.localPosition;
            _moveVector = norm;//(norm - _rb.transform.position); //new Vector3(norm.x * 4, norm.y * 4, 0);
            //_moveVector.z = -Camera.main.transform.position.z;
            
            _rb.transform.DOBlendableLocalMoveBy(norm, 1.5f).SetEase(Ease.Linear).OnComplete(new TweenCallback(delegate
            {
               
                _stretching = false;
                onFinishStretching?.Invoke();
                  onMovingTowards?.Invoke(_moveVector.x, _moveVector.y);
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
