using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SlicerEnzyme : BasicUnit
{
	public EvilRNA targetRNA;

	public bool hasRNA = false;
	public bool instant_deploy = false;
	public bool play_init_sound = true;
		
	public bool killedSomething = false;
		
	private int release_count = 0;
	private int release_time = 30;
	private Point old_spot;
	private bool _rotating;

	public override void Start()
	{
		base.Start();
		does_recycle = true;
		speed = 3;
		num_id = Selectable.SLICER_ENZYME;
		text_id = "slicer";
		text_title = "Slicer Enzyme";
		text_description = "Destroys viral RNA";
		//list_actions = Vector.<int>([Act.RECYCLE]);
		singleSelect = false;
		canSelect = true;
		setMaxHealth(1, true);
		MAX_COUNT = 5; //recalculate more often for better tracking
		snapToObject = false; //KEEPS EM IN THE MEMBRANE!
	}

    public override void playAnim(string label)
    {
		_rotating = (label == "normal");
        base.playAnim(label);
    }

    private void Update()
    {
        if (_rotating)
        {
			this.transform.eulerAngles = (new Vector3(0, 0, Time.deltaTime*60));
        }
    }

    protected override void autoRadius()
	{
		setRadius(25);
	}

	public override void init()
	{
		base.init();

		if (play_init_sound)
			SfxManager.Play(SFX.SFXSlicerRise);

		slicerDeploy(instant_deploy);
	}

	public void targetEvilRNA(EvilRNA e)
	{

		targetRNA = e;
		hasRNA = true;
		targetRNA.addSlicer(this);
		moveToObject(e, CellGameObject.FLOAT, true);

	}

	public void releaseByRNA(EvilRNA r)
	{
		if (targetRNA == r)
		{
			releaseRNA();
		}
	}

	protected override void arriveObject(bool wasCancel = false)
	{
		if (go_dest && hasRNA)
		{ //AVOID BUG WHERE SLICERS RESET TO (0,0) when their RNA target disappears
			x = go_dest.x;
			y = go_dest.y;
		}
		if (!wasCancel)
		{
			onArriveObj();
		}
		StopCoroutine(_doMoveGobjRoutine);
	}

	protected override void stopWhatYouWereDoing(bool isObj)
	{
		Debug.Log("SlicerEnzyme.stopWhatYouWEreDoing()");
		if (isObj)
		{
			cancelMoveObject();
		}
		else
		{
			cancelMovePoint();
		}
		//define rest per subclass
	}

	public void releaseRNA()
	{
		targetRNA = null;
		hasRNA = false;
		/*var c:ColorTransform = this.transform.colorTransform;
		c.redOffset = 0;
		c.blueMultiplier = 1;
		c.greenMultiplier = 1;*/
		//this.transform.colorTransform = c;
		if (isMoving)
		{
			cancelMove();
			slicerDeploy();
		}
	}

	protected override void onArriveObj()
	{
		//throw new Error("Testing");
		if (!dying)
		{
			if (hasRNA)
			{
				if (targetRNA.dying == false && targetRNA.invincible == false)
				{
					if (targetRNA.onSlicerKill())
					{
						SfxManager.Play(SFX.SFXZlap);
						targetRNA.cancelMove();
						killedSomething = true;
						targetRNA.playAnim("die");
						useMe();
						releaseRNA();
					}
				}
			}
		}

		base.onArriveObj();


		if (!dying)
		{ //if I survived and it died, go back to waiting
			if (!hasRNA)
			{
				slicerDeploy();
			}
		}

	}


	public void slicerDeploy(bool instant = false)
	{
		//trace("SlicerEnzyme.slicerDeploy(" + instant + ")");
		deployCytoplasm(p_cell.c_nucleus.x, p_cell.c_nucleus.y, 170, 35, true, instant);
	}


	protected override void onRecycle()
	{
		p_cell.onRecycle(this, true, !killedSomething);
	}

	private void useMe()
	{
		playAnim("recycle");
		//trace("SlicerEnzyme.useMe() p_cell=" + p_cell);
	}








}

