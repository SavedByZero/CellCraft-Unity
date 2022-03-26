using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    public CellAction action = CellAction.NOTHING;
    public MovieClip Reticle;
    public MovieClip Icon;
    public Text ActionTextField;  //text
    public Text CostTextField;  //text_2
    public Minicost cost; //TODO
    //reticle: crosshairs, triangles, arrowHead, redArrowHead 
    // Start is called before the first frame update

    public bool isArrow = false;
		
	private float arrow_x; //the starting point of the arrow
	private float arrow_y;
	private float arrow_rotation = 0;
	public Graphics shape_arrow; 
	private static float ARROW_HEIGHT = 15;
    private const float PPOD_RANGE = Membrane.PPOD_ANGLE; //+- X degrees	
	private bool fancy = false; //are we showing a context cursor
		//private var targetter:Targetter;
		
	private Engine p_engine;
	private Cell p_cell;
		//private World p_world;  //TODO
		


    void Start()
    {
        //cost.setWhite();  //TODO
        shape_arrow = new Graphics();
        
        //setChildIndex(shape_arrow, 0);
    }

    public void setEngine(Engine e)
    {
        p_engine = e;
    }

    /*   //TODO
    public void setWorld(World w)
    {
        p_world = w;
    }*/

    public void setCell(Cell c)
    {
        p_cell = c;
    }

	public void followMouse()
	{
		Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.transform.position = new Vector3(mouse.x, mouse.y, 0);
//		x = stage.mouseX;
	//	y = stage.mouseY;
		if (isArrow)
		{
			drawArrow();
		}
	}

	public void normal(bool yes)
	{
		if (fancy)
		{
			if (yes)
			{
				this.gameObject.SetActive(false);
				UnityEngine.Cursor.visible = true;
			}
			else
			{
				this.gameObject.SetActive(true);
				UnityEngine.Cursor.visible = false;
			}
		}
	}

	/**
		 * Sets the icon to the appropriate action, and returns whether an icon is showing
		 * @param	i the icon numeric constant from Class Act.
		 * @return whether the icon is showing or not
		 */

	public bool show(CellAction i = CellAction.NOTHING)
	{
			string s = Act.getS(i);
			//trace("Cursor.show : i = " + i + " s = " + s);
			string txt = Act.getTxt(i);
			action = i;
		if (s == "") 
		{
			this.gameObject.SetActive(false);
			UnityEngine.Cursor.visible = true;
				fancy = false;
				return false;
				action = CellAction.NOTHING;
		}
		else
		{
			fancy = true;
			ActionTextField.text = txt.ToUpper();
			this.gameObject.SetActive(true);
			//text_2.text = "";

			Icon.GotoAndStop(s);
			UnityEngine.Cursor.visible = false;
			showContextAct(i);
			return true;
		}
			
		}
		
	private void showContextAct(CellAction i) 
	{
	//trace("Cursor.showContextAct : i=" + i);
		switch (i)
		{
			case CellAction.MOVE:
				showReticle("crosshairs");
				showMoveCost();
				break;
			case CellAction.BLEB:
				showReticle("triangles");
				showBlebCost();
				break;
			case CellAction.PSEUDOPOD_PREPARE:
				showReticle("triangles");
				hideCost();
				break;
			case CellAction.PSEUDOPOD:
				showReticle("arrowHead");
				startArrow();
				showPPodCost();
				break;
			default:
				showReticle("crosshairs");
				hideCost();
				break;
		}
	}

	private void showReticle(string s)
	{
		//trace("Cursor.showReticle : " + s);
		Reticle.GotoAndStop(s);
		Reticle.gameObject.SetActive(true);
	}

	public void endArrow()
	{
		isArrow = false;
		shape_arrow.End();
		arrow_rotation = 0;
		Reticle.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
	}

	private void drawArrow()
	{
		//This block constrains the arrow to stay within the lens
		Point p = new Point(this.transform.position.x, this.transform.position.y);
		//p = p_world.transformPoint(p);  //TODO
		float dx = p.x - CellGameObject.cent_x;
		float dy = p.y - CellGameObject.cent_y;
		float d2 = (dx * dx) + (dy * dy);
		float r2 = d2; // World.LENS_R2;  //TODO

		if (d2 >= r2)
		{

			Vector2 v = new Vector2(arrow_x - this.transform.position.x, arrow_y - this.transform.position.y);
			v.Normalize();
			//v.multiply((World.LENS_SIZE * p_world._scale) - ARROW_HEIGHT);  //TODO

			Point pc = new Point(CellGameObject.cent_x, CellGameObject.cent_y);
			//pc = p_world.reverseTransformPoint(pc); //TODO
			this.transform.position = new Vector3(pc.x - v.x, pc.y - v.y, 0);
		}
		//World.LENS

		shape_arrow.Begin();
		shape_arrow.lineStyle(9, Color.black);
		shape_arrow.MoveTo(arrow_x - this.transform.position.x, arrow_y - this.transform.position.y); //draw the black line
		shape_arrow.LineTo(0, 0);

		Vector2 v1 = new Vector2(arrow_x - this.transform.position.x, arrow_y - this.transform.position.y);

		float ang = FastMath.toRotation(v1) * (180 / Mathf.PI);
		ang -= 90;

		//arrow_rotation is GUARANTEED to be between 0-360 as it is processed before it is set
		//make sure we do that with ang as well:
		if (ang < 0)
		{
			ang += 360;
		}
		else if (ang > 360)
		{
			ang -= 360;
		}

		Reticle.transform.eulerAngles = new Vector3(0,0,ang);

		float angHigh = arrow_rotation + PPOD_RANGE; //PPOD_RANGE -> 360+PPOD_RANGE
		float angLow = arrow_rotation - PPOD_RANGE; //-PPOD_RANGE -> 360-PPOD_RANGE

	

		showReticle("arrowHead");
		shape_arrow.lineStyle(7, Color.white);
		shape_arrow.MoveTo(arrow_x - this.transform.position.x, arrow_y - this.transform.position.y);
		shape_arrow.LineTo(0, 0);
		shape_arrow.Stroke();
	

		//cost.setAmount(p_cell.getPPodCost(stage.mouseX, stage.mouseY));  //TODO

	}

	public void setArrowRotation(float r)
	{
		if (r < 0)
		{
			r += 360;
		}
		else if (r > 360)
		{
			r -= 360;
		}
		Reticle.transform.eulerAngles = new Vector3(0,0,r);
		arrow_rotation = r;
	}

	public void setArrowPoint(float xx, float yy)
	{
		arrow_x = xx;
		arrow_y = yy;
	}

	private void startArrow()
	{
		isArrow = true;
	}

	private void hideCost()
	{
		cost.gameObject.SetActive(false);
		CostTextField.text = "";
	}

	public void updateMoveCost(CellAction i = CellAction.NOTHING)
	{
		if (action == CellAction.MOVE)
		{
			showMoveCost(i);
		}
	}

	private void showACost(string s)
	{
		cost.gameObject.SetActive(true);
		CostTextField.text = "Cost:";
		//cost.setType(s);  //TODO
	}

	private void showPPodCost()
	{
		showACost("atp");
		
		//cost.setAmount(p_cell.getPPodCost(stage.mouseX, stage.mouseY)); //TODO
	}

	private void showBlebCost()
	{

		showACost("atp");
		float[] a = Costs.BLEB;
		//cost.setAmount(a[0]);  //TODO
		//trace("Cursor.showBlebCost " + a[0]);
	}

	private void showMoveCost(CellAction i = CellAction.NOTHING)
	{
		//text_2.visible = true;
		if (p_engine)
		{
			showACost("atp");
			if (i == CellAction.NOTHING)
			{
				//cost.setAmount(p_engine.getMoveCost(stage.mouseX, stage.mouseY));  //TODO
			}
			else
			{
				//cost.setAmount(i);  //TODO
			}
		}
	}







}
