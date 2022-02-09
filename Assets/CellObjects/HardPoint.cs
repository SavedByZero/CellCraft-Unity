using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HardPoint : CellObject
{
	private float warble_count = 0;
	private float WARBLE_AMOUNT = 1;
	private const float WARBLE_MAX = 30;
	public int warble_sign = 1;
	private float base_radius;
		
	public CellObject p_escort;
	public bool isEscort = false;
	private float escort_grow_count = 0;
	private float ESCORT_GROW_MAX = 0;
	public bool isWarble = false;
		
	public Point newPos;
	public Point posDiff;
	public float newRadius;
	public float radDiff;
	private int UPDATE_TIME = 45;
		
	private int posCount = 0;
	private int radCount = 0;

	private Coroutine _updatePosRoutine;
	private Coroutine _updateRadiusRoutine;
	private Coroutine _warbleRoutine;
	private Coroutine _escortRoutine;

	public HardPoint()
	{
		singleSelect = true;
		canSelect = false;
		text_title = "HardPoint";
		text_description = "";
		text_id = "hardpoint";
		num_id = Selectable.HARDPOINT;
		setMaxHealth(999, true);
		doesCollide = true;
		hardCollide = true;
		makeGameDataObject();
		init();
		
	}

	public void setNewPos(float xx, float yy)
	{
		newPos = new Point(xx, yy);
		posDiff = new Point((xx - x) / UPDATE_TIME, (yy - y) / UPDATE_TIME);
		_updatePosRoutine = StartCoroutine(updatePos());
	}

	public void setNewRadius(float r)
	{
		
		newRadius = r;
		radDiff = (r - base_radius) / UPDATE_TIME;
		_updateRadiusRoutine = StartCoroutine(updateRadius());
	}

	private IEnumerator updatePos()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			x += posDiff.x;
			y += posDiff.y;
			posCount++;
			if (posCount >= UPDATE_TIME)
			{
				posCount = 0;
				x = newPos.x;
				y = newPos.y;
				StopCoroutine(_updatePosRoutine);
			}
			updateLoc();
		}
	}

	private IEnumerator updateRadius()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			setBaseRadius(base_radius + radDiff);
			//rememberRadius();
			radCount++;
			if (radCount >= UPDATE_TIME)
			{
				radCount = 0;
				setBaseRadius(newRadius);
				//rememberRadius();
				StopCoroutine(_updateRadiusRoutine);
			}
		}
	}

	public void startWarble()
	{
		isWarble = true;
		_warbleRoutine = StartCoroutine(warble());
		//addEventListener(RunFrameEvent.RUNFRAME, warble, false, 0, true);
	}

	public void resetWarble()
	{
		//warble_count = 0;
		//setRadius(base_radius + warble_count);
	}

	private IEnumerator warble()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			warble_count += warble_sign * WARBLE_AMOUNT;
			if (warble_sign == 1 && warble_count > WARBLE_MAX)
			{
				warble_count = WARBLE_MAX;
				warble_sign *= -1;
			}
			else if (warble_sign == -1 && warble_count < -WARBLE_MAX)
			{
				warble_count = -WARBLE_MAX;
				warble_sign *= -1;
			}
			setRadius(base_radius + warble_count);
		}
	}

	public void setEscort(CellObject c)
	{
		isEscort = true;
		p_escort = c;
		setRadius(c.getRadius() * 2);
		rememberRadius();
		//ESCORT_GROW_MAX = base_radius;
		x = p_escort.x;
		y = p_escort.y;
		updateLoc();
		_escortRoutine = StartCoroutine(doEscort());
	}

	private IEnumerator doEscort()
	{

		while (true)
		{
			yield return new WaitForEndOfFrame();
			if (p_escort.isMoving)
			{
				x = p_escort.x;
				y = p_escort.y;
				updateLoc();
			}
			else
			{
				p_escort = null;
				StopCoroutine(_escortRoutine);
				p_cell.killHardPoint(this);
			}
		}
	}

	protected override void autoRadius()
	{
		setRadius(50);
	}

	public void setBaseRadius(float r)
	{
		base_radius = r;
	}

	public override void setRadius(float r)
	{
		base.setRadius(r);
		clip.transform.localScale = new Vector3((r * 2) / clip.MaxWidth, (r * 2) /clip.MaxHeight);
		clip.transform.position = Vector3.zero;
		
	}

	public void rememberRadius()
	{
		base_radius = getRadius();
	}

	public override void updateLoc()
	{
		float xx = x - cent_x + span_w / 2;
		float yy = y - cent_y + span_h / 2;

		updateGridLoc(xx, yy);
	}

}

