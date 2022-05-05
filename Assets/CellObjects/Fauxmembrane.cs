using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using UnityEngine.UIElements;

public class Fauxmembrane : MonoBehaviour
{
    public Transform Anchor;
    private Graphics m_Cytoplasm;
    private Graphics m_rim;
    private Graphics m_innerRim;
    private Graphics m_outerRim;
    public GameObject Rim;
    public GameObject InnerRim;
    public GameObject OuterRim;
    public GameObject PlainGrafX;
    public GameObject RimGrafx;
    public float LerpVal = -90;

    public const float STARTING_RADIUS = 4;//4.00f;
    public const int STARTING_NODES = 30;//15;
 
    // Start is called before the first frame update
    //0x55CCFF = spring
    //0x44aaff = cyto
    private FauxNode[] _nodes;
    private bool _active;
    public GameObject NodePrefab;
    private void Awake()
    {
        m_Cytoplasm = gameObject.AddComponent<Graphics>();
        m_rim = Rim.AddComponent<Graphics>();
        m_rim.TextureName = "MembraneOuter";
        m_innerRim = InnerRim.AddComponent<Graphics>();
        m_outerRim = OuterRim.AddComponent<Graphics>();
        m_Cytoplasm.PixelsPerUnit = 1;
        m_rim.PixelsPerUnit = 1;
        m_innerRim.PixelsPerUnit = 1;
        m_outerRim.PixelsPerUnit = 1;
        m_Cytoplasm.SortOrder = 2;
        m_innerRim.SortOrder = 2;
        m_outerRim.SortOrder = 3;
        
    }

    private void Start()
    {
        m_rim.LineWidth = .4f;
        m_innerRim.LineWidth = .1f;
        m_outerRim.LineWidth = .6f;
        m_rim.SetLineColor(Color.clear);//(FastMath.ConvertFromUint(0x0066FF));
        m_rim.Pattern = true;
        //m_innerRim.SetLineColor(1,1,1,0);
       // m_outerRim.SetLineColor(FastMath.ConvertFromUint());
        m_Cytoplasm.SetFillColor(FastMath.ConvertFromUint(0x44aaff));
        makeNodes(STARTING_RADIUS, STARTING_NODES);
        Redraw();
        _active = true;
       
    }

    private void makeNodes(float radius, int max)
    {


        List<float> v = FastMath.circlePoints(radius, max);
        int length = v.Count;
        float rot = 0;
        for (int i = 0; i < length; i += 2)
        {
            rot = 90 + ((i / 2) * (360 / max));
            GameObject node = Instantiate(NodePrefab) as GameObject;
            FauxNode fn = node.GetComponent<FauxNode>();
            SpringJoint2D sj2d = node.GetComponent<SpringJoint2D>();
            sj2d.connectedBody = Anchor.GetComponent<Rigidbody2D>();
            
            node.transform.position = new Vector3(v[i], v[i+1]);
           // node.transform.eulerAngles = new Vector3(0, 0, rot);
            node.transform.SetParent(this.transform);
            //sj2d.distance = Mathf.Abs(Vector3.Distance(node.transform.position, Anchor.transform.position));
            sj2d.autoConfigureDistance = false;
            fn.Index = max - 1;
            //makeNode(v[i], v[i + 1], rot, i / 2, max - 1); //do max-1 so that it knows that that's the last index in the list
        }
        _nodes = GetComponentsInChildren<FauxNode>();

    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {

        /*var paint2D = mgc.painter2D ;

        paint2D.fillColor = wireColor;
        paint2D.BeginPath();
        paint2D.MoveTo(p0);
        paint2D.LineTo(p1);
        paint2D.LineTo(p2);
        paint2D.LineTo(p3);
        paint2D.ClosePath();
        paint2D.Fill();*/
    }

    public void Redraw()
    {
        m_Cytoplasm.Begin();
        m_rim.Begin();
        m_innerRim.Begin();
        m_Cytoplasm.MoveTo(_nodes[0].transform.localPosition.x, _nodes[0].transform.localPosition.y);
        m_rim.MoveTo(_nodes[0].transform.localPosition.x, _nodes[0].transform.localPosition.y);
        int i;
        Vector3 sum = new Vector3();
        float maxY = 0;
        float minY = 0;
        float maxX = 0;
        float minX = 0;
        for (i=0; i < _nodes.Length-1; i++)
        {
            //Vector3 currentNodePosition = _nodes[i].transform.localPosition;

            Vector2 p0 = _nodes[i].transform.localPosition;
            Vector2 p3 = _nodes[i + 1].transform.localPosition;
           

            Vector2 distToNext = (p3 - p0);
            Vector2 distFromNext = (p0 - p3);
            distToNext = FastMath.rotateVector(LerpVal * Mathf.PI / 180, distToNext);
            distFromNext = FastMath.rotateVector(LerpVal * Mathf.PI / 180, distFromNext);
          
            m_rim.BezierCurveTo(p0.x, p0.y, p0.x + distToNext.x, p0.y + distToNext.y, p3.x, p3.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
           // m_innerRim.BezierCurveTo(p0.x, p0.y, p0.x + distToNext.x, p0.y + distToNext.y, p3.x, p3.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
           // m_outerRim.BezierCurveTo(p0.x, p0.y, p0.x + distToNext.x, p0.y + distToNext.y, p3.x, p3.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
             
            
            
            m_Cytoplasm.LineTo(p0.x,p0.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);
           // m_Cytoplasm.LineTo(p0.x + (distToNext.x / 2), p0.y + (distToNext.y / 2));
           // m_Cytoplasm.LineTo(p3.x + (distFromNext.x / 2), p3.y + (distFromNext.y / 2));
            m_Cytoplasm.LineTo(p3.x,p3.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);
            //m_Cytoplasm.LineTo(dist.x, dist.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);

        }
        Vector2 p0l = _nodes[i].transform.localPosition;
        Vector2 p3l = _nodes[0].transform.localPosition;
        Vector2 distToNextl = (p3l - p0l) / 2;
        Vector2 distFromNextl = (p0l - p3l) / 2;

        m_rim.BezierCurveTo(p0l.x + distToNextl.x, p0l.y + distToNextl.y, p3l.x + distFromNextl.x, p3l.y + distFromNextl.y, p3l.x, p3l.y);//(p0.x, p0.y, p0.x, p0.y, p3.x, p3.y);
        //m_Cytoplasm.MoveTo(p0l.x, p0l.y);
        m_Cytoplasm.LineTo(p3l.x, p3l.y);//.BezierCurveTo(p0.x,p0.y, p1.x, p1.y, p3.x,p3.y);//BezierCurve(p0, _nodes[i].localPosition+ perp1, _nodes[i+1].localPosition+perp2, p3);

       
        //m_Graphics.Ellipse(sum.x,sum.y,rx,ry);


        //m_Graphicsf.Stroke();
        m_rim.Stroke();
            
       // m_innerRim.Stroke();
      //  m_outerRim.Stroke();
        m_Cytoplasm.Fill();
        m_rim.End();
       // m_innerRim.End();
       // m_outerRim.End();
        m_Cytoplasm.End();
      
    }

 

    // Update is called once per frame
    void Update()
    {
        if (_active)
            Redraw();
    }
}
