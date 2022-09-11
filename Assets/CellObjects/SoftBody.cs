using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;

public class SoftBody : MonoBehaviour
{
    // Start is called before the first frame update
    public SpriteShapeController SpriteShape;
    public const float splineOffset = 0.5f;
    private float _establishedDistanceConstraint;

    private List<Transform> points;

    public GameObject Anchor;
    private Rigidbody2D _rb;
    private bool _prepped;

    private void Awake()
    {
        _rb = Anchor.GetComponent<Rigidbody2D>();
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
        _establishedDistanceConstraint = dj.distance;
        dj.autoConfigureDistance = false;
    }

    public void BePliable(bool value)
    {
        for (int i = 0; i < points.Count; i++)
        {
            SpringJoint2D[] joints = points[i].GetComponents<SpringJoint2D>();
            if (!value)
            {
                StartCoroutine(stiffen(joints[0],joints[1],joints[2]));
                
                //dj.distance = _establishedDistanceConstraint;
                //dj.autoConfigureDistance = false;
                
            }
            else
            {
                joints[0].frequency = 0.1f;
                joints[1].frequency = 0.1f;
                joints[2].frequency = 0.1f;
              //  joints[0].dampingRatio = 0.6f;
                //dj.autoConfigureDistance = true;
            }
        }
    }

    IEnumerator stiffen(SpringJoint2D one, SpringJoint2D two, SpringJoint2D three)
    {
        float amt = one.frequency;
        while (amt < 0.6)
        {
            yield return new WaitForSeconds(0.2f);
            amt += 0.05f;
           // one.dampingRatio -= 0.025f;
            one.frequency = amt;
            two.frequency = amt;
            three.frequency = amt;
        }

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
                Vector2 towardsCenter = ((Vector2)Anchor.transform.localPosition - vertex).normalized;
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
               
              
                Vector2 newlt = -Vector2.Perpendicular(towardsCenter) * (1f/3f);// * lt.magnitude;//   1/3 works for 2.5 radius, 1/2 works for 4 radius
                

                 SpriteShape.spline.SetRightTangent(i, -newlt);
                 SpriteShape.spline.SetLeftTangent(i, newlt);

                





            }
         
        }
    }
}
