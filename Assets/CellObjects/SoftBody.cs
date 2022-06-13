using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteShapeController SpriteShape;
    public const float splineOffset = 0.5f;

    private List<Transform> points;
    private Rigidbody2D _rb;
    private bool _prepped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        points = new List<Transform>();
        SpriteShape.spline.Clear();
        
    }

  

    public void AddNode(GameObject node)
    {
        node.transform.SetParent(this.transform);
        SpriteShape.spline.InsertPointAt(points.Count, node.transform.localPosition);
 
        //SpriteShape.spline.SetTangentMode(points.Count, ShapeTangentMode.Continuous);
       
        points.Add(node.transform);
        SpringJoint2D sj = node.GetComponent<SpringJoint2D>();
        sj.connectedBody = _rb;
       // sj.autoConfigureDistance = false;
    }

    IEnumerator delayDistanceTweak(DistanceJoint2D dj)
    {
        yield return new WaitForEndOfFrame();
        dj.autoConfigureDistance = false;
    }

    public void PrepareNodes()
    {
        for (int i = 0; i < points.Count; i++)
        {
            SpringJoint2D sj = points[i].GetComponent<SpringJoint2D>();
            DistanceJoint2D dj = points[i].GetComponent<DistanceJoint2D>();
            SpringJoint2D[] springs = points[i].GetComponents<SpringJoint2D>();
            StartCoroutine(delayDistanceTweak(dj));
            sj.autoConfigureDistance = false;
            // dj.autoConfigureDistance = false;
            if (i < points.Count - 1)
            {
                dj.connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
                springs[1].connectedBody = points[i + 1].GetComponent<Rigidbody2D>();
                springs[1].autoConfigureDistance = false;
            }
            else
            {
                dj.connectedBody = points[0].GetComponent<Rigidbody2D>();
                springs[1].connectedBody = points[0].GetComponent<Rigidbody2D>();
                springs[1].autoConfigureDistance = false;
            }
            if (i > 0)
            {
                springs[2].connectedBody = points[i - 1].GetComponent<Rigidbody2D>();
                springs[2].autoConfigureDistance = false;
            }
            else
            {
                springs[2].connectedBody = points[points.Count - 1].GetComponent<Rigidbody2D>();
                springs[2].autoConfigureDistance = false;
            }
            
        }
        _prepped = true;

    }
  

    public void UpdateVertices()
    {
        
        if (_prepped)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 vertex = points[i].GetComponent<Rigidbody2D>().transform.localPosition;
                Vector2 towardsCenter = (Vector2.zero - vertex).normalized;
               // Debug.Log("local pos " + _rb.position);
                float colliderRadius = 0.5f;//points[i].gameObject.GetComponent<CircleCollider2D>().radius;
                try
                {
                    SpriteShape.spline.SetPosition(i, (vertex - towardsCenter * colliderRadius));
                    
                }
                catch
                {
                    Debug.Log("too close " + i);
                    SpriteShape.spline.SetPosition(i, (vertex - towardsCenter * (colliderRadius + splineOffset)));

                }
                SpriteShape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
                Vector2 lt = SpriteShape.spline.GetLeftTangent(i);
               
              
                Vector2 newlt = -Vector2.Perpendicular(towardsCenter) * (1f/2f);// * lt.magnitude;//   1/3 works for 2.5 radius
                

                 SpriteShape.spline.SetRightTangent(i, -newlt);
                 SpriteShape.spline.SetLeftTangent(i, newlt);

                





            }
         
        }
    }
}
