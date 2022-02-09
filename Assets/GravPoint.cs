using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class GravPoint : Point
{
	public CellObject p_obj; //the thing I represent
	public float radius;    //no protection, do it right!
	public float radius2; 
		
	public GravPoint(Point p = null, CellObject c= null, float r = 50) : base(p.x, p.y)
	{
		x = p.x;
		y = p.y;
		p_obj = c;
		radius = r;
		radius2 = r * r;
	}

	public void destruct()
	{
		p_obj = null;
	}

	public GravPoint copy()
	{
		GravPoint g = new GravPoint(new Point(x, y), p_obj, radius);
		return g;
	}
}

