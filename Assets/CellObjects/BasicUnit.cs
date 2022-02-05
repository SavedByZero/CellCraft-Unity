using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BasicUnit : CellObject
{
	public Boolean isBusy;
	public static float C_GRAV_R2 = 1;
		
	public const int RIBOSOME = 0;
	public const int LYSOSOME = 1;
	public const int PEROXISOME = 2;
	public const int SLICER = 3;
	public const int DNAREPAIR = 4;

	public BasicUnit()
	{
		is_basicUnit = true;
		might_collide = false;
	}

	public static void updateCGravR2(float n)
	{
		C_GRAV_R2 = n;
	}

	public override void externalMoveToPoint(Point p, int i)
	{
		if (!isBusy)
		{
			moveToPoint(p, i);
		}
	}

	protected override void doMoveToGobj()
	{
		base.doMoveToGobj();
		updateCollide();
	}

	protected override void doMoveToPoint()
	{
		base.doMoveToPoint();
		updateCollide();
	}

	protected void updateCollide()
	{
		float dx = x - cent_x;
		float dy = y - cent_y;
		float d2 = (dx * dx) + (dy * dy);
		if (d2 > C_GRAV_R2 * 0.5)
		{
			might_collide = true;
			//trace("BasicUnit.mightCollide() " + name + " might collide!");
			if (doesCollide)
			{
				updateLoc();
			}
		}
		else
		{
			might_collide = false;
		}
	}


}

