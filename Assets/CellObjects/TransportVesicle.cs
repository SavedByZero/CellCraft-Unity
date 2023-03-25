using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;


public class TransportVesicle : BlankVesicle
{
	private MembraneNode mnode;
	private BigVesicle myBigVesicle;

    public override void Start()
    {
        base.Start();
		Appear();
    }
	

	public override void onAnimFinish(int i, bool stop = true)
	{
		//super.super.onAnimFinish(i,stop);
		bool passItUp = true;
		switch (i)
		{
			case CellGameObject.ANIM_GROW: 
				passItUp = false; 
				moveToMembrane(); 
				break;
			case CellGameObject.ANIM_GROW_2: passItUp = false; startFade(); break;
			case CellGameObject.ANIM_FADE: onFade(); break;
			//passItUp = false;  moveToMembrane(); break;
			case CellGameObject.ANIM_ADHERE: passItUp = false; metamorphose(); break;
		}
		if (passItUp)
		{
			base.onAnimFinish(i, stop);
		}
	}

	private void onFade()
	{
		if (myBigVesicle)
		{
			if (product == MEMBRANE)
			{
				myBigVesicle.goToMembrane();
			}
		}
		p_cell.killBlankVesicle(this);
	}

	public int getProduct() 
	{
			return product;
	}

	public void setBigVesicle(BigVesicle b)
	{
		myBigVesicle = b;
	}

	private void startFade()
	{
		playAnim("fade");
		this.GetComponent<SpriteRenderer>().DOFade(0, 1).OnComplete(new TweenCallback(delegate
		{
			onAnimFinish(CellGameObject.ANIM_FADE);
		}));
		int newRadius = 30;//MAGIC NUMBER! OOPS! width / 2;
		if (product == Selectable.MEMBRANE)
		{
			p_cell.makeMembraneVesicle(this, newRadius);
		}
	}

	private void moveToMembrane()
	{
		mnode = p_cell.c_membrane.findClosestMembraneNode(x, y);
		moveToPoint(new Point(mnode.x, mnode.y), CellGameObject.FLOAT, true);
	}

	protected override void metamorphose()
	{
		if (product == Selectable.DEFENSIN)
		{
			p_cell.showShieldSpot(product_amount, x, y);
			SfxManager.Play(SFX.SFXShield);
			p_cell.c_membrane.addDefensin(product_amount);

		}
		else if (product == Selectable.MEMBRANE)
		{
			p_cell.makeMembrane();
		}
		else if (product == Selectable.TOXIN)
		{
			p_cell.fireToxinParticle(x, y);
			//p_cell.makeToxin();
		}
		p_cell.killBlankVesicle(this);
	}

	protected override void onArrivePoint()
	{

		if (mnode != null)
		{
			//playAnim("adhere");
			metamorphose();
		}
		else
		{

		}

	}



}

