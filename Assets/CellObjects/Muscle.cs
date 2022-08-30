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
        yield return new WaitForSeconds(1.5f);

        //the problem here is that the nucleus localPosition is not in the right spot before a 2.5 second delay.  
        //Somehow, though, the nucleus and other parts need to move WITH the tail end of the membrane.  

        MembraneNode[] nodes = _membrane.GetComponentsInChildren<MembraneNode>();
        GameObject sbAnchor = _membrane.gameObject.GetComponent<SoftBody>().Anchor;
        
       /// sbAnchor.transform.DOMove(GameObject.FindObjectOfType<Cell>().c_nucleus.transform.localPosition, 1).SetEase(Ease.Linear).OnComplete(new TweenCallback(delegate {
            _rb.isKinematic = true;
            _rb.transform.localPosition = Vector3.zero;
         
            _rb.gameObject.SetActive(false);
       // }));
       
    }

    public void Stretch(float xDir, float yDir, Vector3 raw)
    {
       
        _membrane.GetComponentInChildren<Wiggler>(true).gameObject.SetActive(false);
        Vector3 norm = new Vector3(xDir, yDir, 0);// -Camera.main.transform.position.z).normalized;
        Debug.Log("old norm " + norm);
        Debug.Log("Raw " + raw);
      
       
        //Vector3 clamped = FastMath.SimpleClamp(norm);
        
        //Debug.Log("norm " + norm);
        norm *= _membrane.getRadius();
        Debug.Log("membrane radius " + _membrane.getRadius());
        Debug.Log("ppod muscle to: " + norm);
   
        _rb.gameObject.SetActive(true);
        Debug.Log("current velocity " + _rb.velocity);
        if (!_stretching)
        {
           // _stretching = true;  //TODO: undo 
            Vector3 originalPos = this.transform.localPosition;
            _moveVector = norm;//(norm - _rb.transform.position); //new Vector3(norm.x * 4, norm.y * 4, 0);
            //_moveVector.z = -Camera.main.transform.position.z;
            //_rb.transform.localPosition = norm;
           // ContactFilter2D cf = new ContactFilter2D();
            
            //cf.SetLayerMask( << 6);
            RaycastHit2D[] results = new RaycastHit2D[3];
            int mask = 1 << 6;
            
            // Physics.Raycast()
            RaycastHit2D[] hitInfo;
            Ray ray = new Ray(this.transform.position, raw);
            //Physics.SphereCast(ray, 0.5f, out hitInfo,mask);
            hitInfo = Physics2D.CircleCastAll(this.transform.position, 1, norm,5,mask);
         
           // RaycastHit[] hits3D = Physics.RaycastAll(ray, Mathf.Infinity, mask);
           // Debug.DrawRay(this.transform.position, norm, Color.red,5);
           
            for(int i=0; i < hitInfo.Length; i++)
            {
                if (hitInfo[i].collider != null )
                {
                    if (hitInfo[i].transform.GetComponent<MembraneNode>())
                    {
                        hitInfo[i].transform.DOBlendableLocalMoveBy(raw, 1);
                    }
                }
            }


           
            /*_rb.transform.DOBlendableLocalMoveBy((raw/4), 1f).SetEase(Ease.OutQuad).OnComplete(new TweenCallback(delegate
            {
               
                _stretching = false;
                onFinishStretching?.Invoke();
                  onMovingTowards?.Invoke(_moveVector.x, _moveVector.y);
                StartCoroutine(Settle(originalPos));

            }));*/
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
