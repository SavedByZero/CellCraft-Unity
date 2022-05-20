using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MembraneNode : MonoBehaviour, ICellGameObject
{
    public int index;

    public Membrane p_membrane;
    public MembraneNode p_prev;
    public MembraneNode p_next;
    public Centrosome p_cent;
    private GameDataObject gdata;
    public bool state_ppod = false; //are we pseudopoding?
    public int controlSign = 1;
    public CellObject p_org;
    private static ObjectGrid p_grid;
    bool hasRNA;
    public List<RNA> list_rna = new List<RNA>();
    private static float span_w = 0;
    private static float span_h = 0;
    private static float grid_w = 0;
    private static float grid_h = 0;
    private static float NODE_RADIUS = .50f;
    public float stretch;


    public static float D2_NODEREST; //distance^2 between nodes that I will rest at
    public static float D2_NODEREST_OLD;
    public static float D2_CENTREST;
    public static float D_NODEREST;
    public static float D_CENTREST;
    public float xdist = 0; //the distance I moved last
    public float ydist = 0;
    public float grid_x = 0;
    public float grid_y = 0;
    public bool has_collided = false;

    bool ICellGameObject.dying { get { return false; } set => _ = value; }
    public float x
    {
        get
        {
            return this.transform.position.x;
        }

        set
        {
            this.transform.position = new Vector3(this.transform.position.x, value, this.transform.position.z);
        }
    }

    public float y
    {
        get
        {
            return this.transform.position.y;
        }
        set
        {
            this.transform.position = new Vector3(value, this.transform.position.y, this.transform.position.z);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //init();
    }

    public void init()
    {
        makeGameDataObject();
        placeInGrid();
    }

    public void destruct()
    {
        p_cent = null;
        p_membrane = null;
        //p_next = null;
        //p_prev = null;
        p_org = null;

    }

    public float getRadius2()
    {
        return 25;
    }

    public void addRNA(RNA r)
    {
        if (list_rna != null)
        {

        }
        else
        {
            list_rna = new List<RNA>();
            hasRNA = true;
        }
        list_rna.Add(r);
    }

    public void removeRNA(RNA r)
    {
        int length = list_rna.Count;
        for (int i = 0; i < length; i++)
        {
            if (list_rna[i] == r)
            {
                list_rna[i] = null;
                list_rna.RemoveAt(i);
                i--;
                length--;
            }
        }
        if (length <= 0)
        {
            list_rna = null;
            hasRNA = false;
        }
    }

    public static void setGrid(ObjectGrid g)
    {
        if (g != null)
        {
            /*grid_w = g.getCellW();
            grid_h = g.getCellH();
            span_w = g.getSpanW();
            span_h = g.getSpanH();*/
            p_grid = g;
        }
    }

    public void makeGameDataObject()
    {
        gdata = new GameDataObject();
        gdata.setThing(x, y, NODE_RADIUS, this, this.GetType());
    }


    public void doCellMove(float xx, float yy)
    {
        //x += xx;
        //y += yy;
        this.transform.position += new Vector3(xx, yy, 0);
    }

    public void push(float x, float y)
    {
       //this.GetComponent<Rigidbody2D>().AddForce(new Vector2(x, y)*10);
       this.GetComponent<Rigidbody2D>().MovePosition(new Vector2(this.transform.localPosition.x + x,this.transform.localPosition.y + y));
    }

    public void placeInGrid()
    {
        float xx = x - p_cent.x + span_w / 2;
        float yy = y - p_cent.y + span_h / 2;

        gdata.x = xx;
        gdata.y = yy;

        grid_x = (int)(xx / grid_w);
        grid_y = (int)(yy / grid_h);
        if (p_grid != null)
            p_grid.putIn((int)grid_x, (int)grid_y, gdata);
    }






    // Update is called once per frame
    void Update()
    {
        
    }
}
