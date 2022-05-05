using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;
 
[ExecuteInEditMode]
public class GradientTest : MonoBehaviour
{
    private Shape shape;
    private Scene m_Scene;
    private VectorUtils.TessellationOptions m_Options;
    private Mesh m_Mesh;
    private Material m_Material;
    private Texture2D m_Atlas;
 
    public float thickness;
    public Color color;
 
    public float width = 1;
    public float height = 1;
 
    public Image img;
 
    void Start()
    {
        shape = new Shape()
        {
            Contours = new BezierContour[] { new BezierContour() {
                Segments = new BezierPathSegment[5] {
                    new BezierPathSegment() { P0 = new Vector2(0, 0), P1 = new Vector2(0, 0), P2 = new Vector2(height, 0) },
                    new BezierPathSegment() { P0 = new Vector2(height, 0), P1 = new Vector2(height, 0), P2 = new Vector2(height, width) },
                    new BezierPathSegment() { P0 = new Vector2(height, width), P1 = new Vector2(height, width), P2 = new Vector2(0, width) },
                    new BezierPathSegment() { P0 = new Vector2(0, width), P1 = new Vector2(0, width), P2 = new Vector2(0, 0) },
                    new BezierPathSegment() { }
                },}},
            Fill = new GradientFill()
            {
                Type = GradientFillType.Radial,
                Stops = new GradientStop[2] {
                    new GradientStop() { Color = Color.blue, StopPercentage = 0 },
                    new GradientStop() { Color = Color.red, StopPercentage = 1 },
                },
            },
        };
 
        m_Scene = new Scene()
        {
            Root = new SceneNode() { Shapes = new List<Shape> { shape } }
        };
 
        m_Options = new VectorUtils.TessellationOptions()
        {
            StepDistance = 10.0f,
            MaxCordDeviation = 1f,
            MaxTanAngleDeviation = 5f,
            SamplingStepSize = 100f
        };
 
        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_Mesh;

        m_Material = new Material(Shader.Find("Unlit/VectorGradient"));
        GetComponent<MeshRenderer>().sharedMaterial = m_Material;
    }
 
    void Update()
    {
        if (m_Scene == null)
            Start();
 
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        var texAtlas = VectorUtils.GenerateAtlasAndFillUVs(geoms, 16);

        if (m_Atlas != null)
            Texture2D.DestroyImmediate(m_Atlas);
        m_Atlas = texAtlas.Texture;

        m_Material.mainTexture = m_Atlas;
        VectorUtils.FillMesh(m_Mesh, geoms, 1.0f);
        // img.sprite = VectorUtils.BuildSprite(geoms, 100.0f, VectorUtils.Alignment.Center, Vector2.zero, 128, true);
    }
}