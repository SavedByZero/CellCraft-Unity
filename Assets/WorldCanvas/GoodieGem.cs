using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoodieGem : CanvasObject
{
	public Text text;
	public MovieClip icon;
	public MovieClip icon2;	
	private string type;

	private int amount;

	public GoodieGem(string t, int am)
	{
		type = t;
		amount = am;
		icon.GotoAndStop(type);
		icon2.GotoAndStop(type);
		//text.htmlText = "<b>" + am.toString() + "</b>";  //TODO: find solution for htmltext in unity. TMP maybe?
		text.text = am.ToString();
		setRadius(25);
	}

	public string getType(){
			return type;
		}

	public float getAmount()
	{
		return amount;
	}

	public override void onTouchCell()
	{
		if (!dying)
		{
			base.onTouchCell();
			/*if (p_canvas)  //TODO
			{
				p_canvas.onCollectGoodieGem(this);
			}*/
			dying = true;
		}
	}

	/**
	 * Where d2 is the square of the distance penetrated, and v is a unit vector in the direction penetrated
	 * @param	d2
	 * @param	v
	 */

	public void onTouchCell2(float d2, Vector2 v)
	{
		onTouchCell();
	}

	private float checkResource() 
	{

		float amt = 5;// p_canvas.getResource(type);  //TODO
		float max_amt = 5;// p_canvas.getMaxResource(type);  //TODO
			if(type == "g"){
				return (amt + amount - (max_amt* 2));
			}
return (amt + amount - (max_amt));
			
			//45 G, 100 max G, get 15 = (45+15) - (100) = 60 - 100 = -40
			//99 G, 100 max G, get 15 = (99+15) - (100) = 114 - 100 = 14
			//85 G, 100 max G, get 15 = (85+15) - (100) = 100 - 100 = 0
			
	}
		
		/**
		 * Where d2 is the square of the distance penetrated, and v is a unit vector in the direction 
		 * @param	d2
		 * @param	v
		 */
		
	private void pushAway(float d2, Vector2 v) 
	{
		float dist = Mathf.Sqrt(d2);
		dist *= 4; //fudge factor to avoid crap
		v *= dist;//if this is a true penetration, should reverse the direction
		x += v.x;
		y += v.y;
	}

}

