using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PH : InterfaceElement
{
	private float ph;
	public Text c_text;
	public MovieClip c_icon;
		
	public PH()
	{

	}

	public void setPH(float n)
	{
		ph = n;
		c_text.text = ph.ToString();//.ToFixed(1);
	}

	/**
	 * Give me your current ph & volume, and your target ph, and I'll tell you how many lysosomes you need
	 * @param	curr_ph Current ph
	 * @param	vol     Current volume
	 * @param	targ_ph Target ph
	 * @return          Lysosomes needed
	 */
	public static float getLysosNeeded(float curr_ph, float vol, float targ_ph) 
	{
			targ_ph = (curr_ph* 0.1f) + (targ_ph* 0.9f); //90% of the way
			float lyso_ph = Lysosome.PH_BALANCE;
			float lyso_vol = Lysosome.LYSO_VOL* Lysosome.VOL_V;
	float lysoneeded = (vol* (targ_ph - curr_ph)) / (lyso_vol* (lyso_ph-targ_ph));
			return lysoneeded;

			//targph = (currph_vol + lysoph_vol * x) / (vol + lyso_volume*x);
	}

	/**
		 * Take one ph-bearing sphere and merge it with another ph bearing sphere
		 * @param	_ph ph of the first sphere
		 * @param	_vol volume of the first sphere
		 * @param	ph_ ph of the second sphere
		 * @param	vol_ volume of the second sphere
		 * @return ph of the resultant sphere
		 */

	public static float mergePH(float _ph, float _vol, float ph_, float vol_) 
	{
		float _ph_vol = _ph* _vol;
		float ph_vol_ = ph_* vol_;
		float result = (_ph_vol + ph_vol_) / (_vol + vol_);
			//trace("PH.mergePH! (" + _ph + "x" + _vol + ") + (" + ph_ +"x" + vol_ +"), result="+result);
		return result;
	}

/**
 * Take a ph-bearing sphere and remove it from a second ph-bearing sphere
 * @param	ph_1 ph of the first sphere
 * @param	vol_1 vol of the first sphere
 * @param	ph_2 ph of the second sphere
 * @param	vol_2 vol of the second sphere
 * @return ph of the bigger sphere
 */

	public static float removeFromPH(float ph_1, float vol_1, float ph_2, float vol_2)
	{
		float ph_vol_1 = ph_1 * vol_1;
		float ph_vol_2 = ph_2 * vol_2;
		float result;
		if (vol_2 > vol_1)
			result = (ph_vol_2 - ph_vol_1) / (vol_2 - vol_1);
		else
			result = (ph_vol_1 - ph_vol_2) / (vol_1 - vol_2);
		//trace("PH.removeFromPH! (" + ph_1 + "x" + vol_1 + ") , (" + ph_2 + "x" + vol_2 +"), result=" + result);
		return result;
	}

	public static Color getCytoColor(float ph_)
	{
		if (ph_ < 4.5f)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0xFFAA44), FastMath.ConvertFromUint(0xFFEE11), (ph_) / 4.5f);
		}
		else if (ph_ > 4.5 && ph_ < 7.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0xFFEE11), FastMath.ConvertFromUint(0x44AAFF), (ph_ - 4.5f) / 3f);
		}
		else if (ph_ >= 7.5f)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0x44AAFF), FastMath.ConvertFromUint(0x8833CC), (ph_ - 7.5f) / 7.5f);
		}
		return Color.clear; //error
	}

	public static Color getGapColor(float ph_)
	{
		if (ph_ < 4.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0xFF9999), FastMath.ConvertFromUint(0xFFFF99), (ph_) / 4.5f); //Color.interpolateColor(0xFF9999, 0xFFFF99, (ph_) / 4.5);
		}
		else if (ph_ > 4.5 && ph_ < 7.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0xFFFF99), FastMath.ConvertFromUint(0x99CCFF), (ph_ - 4.5f) / 3f);//Color.interpolateColor(0xFFFF99, 0x99CCFF, (ph_ - 4.5) / 3);
		}
		else if (ph_ >= 7.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0x99CCFF), FastMath.ConvertFromUint(0xBB99FF), (ph_ - 7.5f) / 7.5f);//Color.interpolateColor(0x99CCFF, 0xBB99FF, (ph_ - 7.5) / 7.5);
		}
		return Color.clear; //error
	}

	public static Color getLineColor(float ph_)
	{
		if (ph_ < 4.5f)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0xFF0000), FastMath.ConvertFromUint(0x996600), (ph_) / 4.5f);//Color.interpolateColor(0xFF0000, 0x996600, (ph_) / 4.5);
		}
		else if (ph_ > 4.5 && ph_ < 7.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0x996600), FastMath.ConvertFromUint(0x0066FF), (ph_ - 4.5f) / 3f);//Color.interpolateColor(0x996600, 0x0066FF, (ph_ - 4.5) / 3);
		}
		else if (ph_ >= 7.5)
		{
			return Color.Lerp(FastMath.ConvertFromUint(0x0066FF), FastMath.ConvertFromUint(0x663399), (ph_ - 7.5f) / 7.5f);//Color.interpolateColor(0x0066FF, 0x663399, (ph_ - 7.5) / 7.5);
		}
		return Color.clear; //error
	}

	public override void blackOut()
	{
		base.blackOut();
		c_icon.gameObject.SetActive(false);
		c_text.gameObject.SetActive(false);// = false;
	}

	public override void unBlackOut()
	{
		base.unBlackOut();
		c_icon.gameObject.SetActive(true);
		c_text.gameObject.SetActive(true);// = false;
	}



}

