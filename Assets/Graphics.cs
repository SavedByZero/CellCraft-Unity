using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;


public class Graphics : MonoBehaviour
{
    //private Path m_Path;
    private Scene m_Scene;
    private VectorUtils.TessellationOptions m_Options;
    public string TextureName = "TestVector";
    private Mesh m_mesh;
    public float PixelsPerUnit = 100;
    private float m_LineWidth = 1f;
    private Color m_LineColor = Color.black;
    private Color m_FillColor = Color.black;
    private IFill m_CurrentFill = new SolidFill() { Color = Color.white };
    private GradientFill _gradientFill;

    private List<BezierPathSegment> m_Segments = new List<BezierPathSegment>();
    private List<BezierContour> m_Contours = new List<BezierContour>();
    private List<BezierSegment> m_bSegments = new List<BezierSegment>();

    private Shape m_CurrentShape;
    public bool Pattern = false;



    public void SetLineColor(float r, float g, float b, float a = 1)
    {
        m_LineColor = new Color(r, g, b, a);
    }

    public void SetLineColor(Color c)
    {
        m_LineColor = c;
    }

    public float LineWidth
    {
        get
        {
            return m_LineWidth;
        }
        set
        {
            m_LineWidth = value / PixelsPerUnit;
        }
    }

    public void lineStyle(float width, Color color)
    {
        m_LineWidth = width / PixelsPerUnit;
        m_LineColor = color;
    }

    public IFill FillStyle
    {
        get
        {
            return m_CurrentFill;
        }
        set
        {
            m_CurrentFill = value;
        }
    }

    public int SortOrder = 1;

    public void SetFillColor(float r, float g, float b, float a = 1)
    {
        m_CurrentFill = new SolidFill() { Color = new Color(r, g, b, a) };
        m_FillColor = new Color(r, g, b, a);
    }

    public void SetFillColor(Color color)
    {
        m_FillColor = color;
    }

    private PathProperties NewPathProperties()
    {
        if (m_LineWidth > 0.0f)
        {
            var v = LocalPoint(m_LineWidth, 0) - LocalPoint(0, 0);
           
            /**/
            Stroke stroke;
            if (m_LineColor == Color.clear)
            {
                stroke = new Stroke()
                {
                    HalfThickness = m_LineWidth / 2,
                    TippedCornerLimit = 0,
                    FillTransform = Matrix2D.identity,
                    Fill = _gradientFill
                    
                };
            }
            else
            {
                stroke = new Stroke()
                {
                    Color = m_LineColor,
                    HalfThickness = m_LineWidth / 2,
                    TippedCornerLimit = 0,
                };
            }

            return new PathProperties()
            {
                
                Corners = PathCorner.Round,
                Head = PathEnding.Round,
                Tail = PathEnding.Round,
              
                Stroke = stroke
            };
           
        }
        else
        {
            return new PathProperties();
        }
    }

    public void BeginContour()
    {
        CloseContour();
    }

    public void CloseContour()
    {
        if (m_Segments.Count > 0)
        {
            var contour = new BezierContour()
            {
                Segments = m_Segments.ToArray()
            };

            m_Contours.Add(contour);
            m_Segments.Clear();
            
            //if (VectorUtils.PathEndsPerfectlyMatch(contour.Segments))
            {
                contour.Closed = true;
            }
        }

        
    }


    void addFill(Shape shape)
    {
        shape.Fill = new SolidFill() { Color = m_FillColor };
    }


    void addToScene(Shape shape)
    {

        m_Scene.Root.Shapes.Add(shape);//.Children.Add(node);
        m_Contours.Clear();
    }

    public void Rect(float x, float y, float w, float h)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeRectangleShape(m_CurrentShape, new Rect(x / PixelsPerUnit, y / PixelsPerUnit, w / PixelsPerUnit, h / PixelsPerUnit));
        //addToScene(rectShape);
    }

    public void Circle(float x, float y, float r)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeCircleShape(m_CurrentShape, new Vector2(x / PixelsPerUnit, y / PixelsPerUnit), r / PixelsPerUnit);
        addToScene(m_CurrentShape);
    }


    public void Ellipse(float x, float y, float rx, float ry)
    {
        m_CurrentShape = new Shape();
        VectorUtils.MakeEllipseShape(m_CurrentShape, new Vector2(x, y), rx, ry);
        //addToScene(m_CurrentShape);
    }


    public void MoveTo(float x, float y, bool clearShape = true)
    {
        if (clearShape)
            m_CurrentShape = new Shape();
        m_Segments.Clear();
        m_Segments.Add(new BezierPathSegment()
        {
            P0 = LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit)//(x / PixelsPerUnit, y / PixelsPerUnit)
        });
    }

    public void LineTo(float x, float y)
    {

        if (m_Segments.Count == 0)
        {
            MoveTo(x, y);
        }
        var n = m_Segments.Count;

        var a = m_Segments[n - 1].P0;

        var b = LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit);
        var line = VectorUtils.MakeLine(a, b);

        m_Segments[n - 1] = new BezierPathSegment()
        {
            P0 = line.P0,
            P1 = line.P1,
            P2 = line.P2
        };

        m_Segments.Add(new BezierPathSegment()
        {
            P0 = line.P3
        });
    }

    public void CurveTo(float cx1, float cy1, float x, float y)
    {
        /*if (m_Segments.Count == 0)
        {
            MoveTo(x, y);
        }*///
        //MoveTo(cx1, cy1);
        var n = m_Segments.Count;

       // var a = m_Segments[n - 1].P0;
        var b = LocalPoint(cx1 / PixelsPerUnit, cy1 / PixelsPerUnit);
        var d = LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit);
       

        /*m_Segments[n - 1] = new BezierPathSegment()
        {
            P0 = a
        };*/
        
        m_Segments.Add(new BezierPathSegment()
        {
            P0 = b,
            P1 = d
        });
    }

    public void BezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        //  m_bSegments.Add(
        BezierPathSegment[] segs = ( VectorUtils.BezierSegmentToPath(new BezierSegment()
        {
            P0 = p0,
            P1 = p1,
            //P2 = p2,
            P3 = p3
        }));
        for(int i=0; i < segs.Length; i++)
        {
            m_Segments.Add(segs[i]);
        }
       
    }

    public void RectangleContour()
    {
        
    }


    public void BezierCurveTo(float cx1, float cy1, float cx2, float cy2, float x, float y)
    {
        if (m_Segments.Count == 0)
        {
            MoveTo(x, y);
        }
        var n = m_Segments.Count;

        var a = m_Segments[n - 1].P0;
        var b = LocalPoint(cx1 / PixelsPerUnit, cy1 / PixelsPerUnit);
        var c = LocalPoint(cx2 / PixelsPerUnit, cy2 / PixelsPerUnit);
        var d = LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit);

        m_Segments[n - 1] = new BezierPathSegment()
        {
            P0 = a,
            P1 = b,
            P2 = c
        };

        m_Segments.Add(new BezierPathSegment()
        {
            P0 = d
        });
    }

    public void Arc(float x, float y, float r, float startAngle, float endAngle)
    {
        m_CurrentShape = new Shape();
        // m_Segments.Clear();
        var rWorld = (LocalPoint((x / PixelsPerUnit) + (r / PixelsPerUnit), (y / PixelsPerUnit)) - LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit)).magnitude;
        var segments = VectorUtils.MakeArc(LocalPoint(x / PixelsPerUnit, y / PixelsPerUnit), startAngle, endAngle - startAngle, rWorld);// rWorld/PixelsPerUnit);
        foreach (var s in segments)
        {
            m_Segments.Add(s);
        }
    }

    public void Fill()
    {
        //var drawables = m_Scene.Root.Drawables;

        CloseContour();
        if (m_Contours.Count > 0)
        {
            m_CurrentShape.Contours = m_Contours.ToArray();
            m_Contours.Clear();

        }
        else
        {
            //m_CurrentShape
            /*var d = drawables[drawables.Count - 1];
            if (d != null)
            {
                if (d is Filled)
                {
                    ((Filled)d).Fill = m_CurrentFill;
                }
            }*/
        }
        addFill(m_CurrentShape);
        addToScene(m_CurrentShape);
    }

    public void Stroke()
    {
        CloseContour();
        if (m_Contours.Count > 0)
        {
            m_CurrentShape.Contours = m_Contours.ToArray();
           
            m_CurrentShape.PathProps = NewPathProperties();
        }
        else
        {
            /* var d = drawables[drawables.Count - 1];
             if (d != null)
             {
                 if (d is Filled)
                 {
                     ((Filled)d).PathProps = NewPathProperties();
                 }
             }*/
        }


       // addStroke(m_CurrentShape);
        addToScene(m_CurrentShape);
    }

    private void Awake()
    {
        LineWidth = m_LineWidth; //to kick in the PPU
        m_Scene = new Scene()
        {
            Root = new SceneNode() { }

        };
        m_Scene.Root.Children = new List<SceneNode>();
        m_Scene.Root.Shapes = new List<Shape>();

        m_Options = new VectorUtils.TessellationOptions()
        {
            StepDistance = 1f,
            MaxCordDeviation = 1f,
            MaxTanAngleDeviation = 5f,
            SamplingStepSize = 1f
        };

      
        Debug.Log("this is " + this);
       
    }

    private void loadTexture()
    {
        m_mesh = new Mesh();

        var meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = m_mesh;

        var meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.rendererPriority = SortOrder;
        Material material = Resources.Load(
           TextureName, typeof(Material)) as Material;
       
        meshRenderer.materials = new Material[] { material };
        _gradientFill = new GradientFill()
        {
            //Mode = FillMode.NonZero,
            Type = GradientFillType.Radial,
            Stops = new GradientStop[]
                        {
                           
                            new GradientStop()
                            {
                                Color = FastMath.ConvertFromUint(0x0066FF),
                                StopPercentage = 0.9f
                            },
                            new GradientStop()
                            {
                                Color = FastMath.ConvertFromUint(0x0066FF),
                                StopPercentage = 0.94f
                            },
                             new GradientStop()
                            {
                                Color = FastMath.ConvertFromUint(0x99ccFF),
                                StopPercentage = 0.95f
                            },
                             new GradientStop()
                            {
                                Color = FastMath.ConvertFromUint(0x0066FF),
                                StopPercentage = 0.97f
                            },
                             new GradientStop()
                            {
                                Color = FastMath.ConvertFromUint(0x0066FF),
                                StopPercentage = 0.998f
                            },
                             new GradientStop()
                            {
                                Color = Color.black,
                                StopPercentage = 1f
                            }


                        },
            Addressing = AddressMode.Clamp,
            Opacity = 255,
             RadialFocus = Vector2.zero
        };
        /*if (Pattern)
        {
            var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
            var texAtlas = VectorUtils.GenerateAtlasAndFillUVs(geoms, 16);
           material.mainTexture = texAtlas.Texture;
        }*/

        //m_mesh.uv = new Vector2[] { };
    }

    public void Begin()
    {
        // m_Scene.Root.Drawables.Clear();
        if (m_mesh == null)
            loadTexture();
        m_mesh.Clear();
        m_Scene.Root.Shapes.Clear();
    }

    public void End()
    {
        //m_mesh.Clear();
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        if (Pattern)
        {
            var texAtlas = VectorUtils.GenerateAtlasAndFillUVs(geoms, 16);
            if (texAtlas != null)
            {
                GetComponent<MeshRenderer>().material.mainTexture = texAtlas.Texture;
                VectorUtils.FillMesh(m_mesh, geoms, 1.0f);
            }
        }
        else
            VectorUtils.FillMesh(m_mesh, geoms, 1.0f);




        // var textAtlas = VectorUtils.GenerateAtlasAndFillUVs(geoms, 16);
        // .material.mainTexture = textAtlas.Texture;

    }

    private Vector2 LocalPoint(float x, float y)
    {
        // var currentCamera = Camera.main;
        //var p = currentCamera.ScreenToWorldPoint(new Vector3(x, y));
        return new Vector2(x, y);//(p.x, p.y);
    }


}
