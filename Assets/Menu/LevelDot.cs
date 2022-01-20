using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelDot : MonoBehaviour
{
    public Text c_text;
	public Image c_check;
	public Image c_select;
		//public var c_butt:SimpleButton;
	public MenuSystem_LevelPicker p_master;
	public int id = 0;
	public int designation = GameLevelInfo.LVL_INTRO;
	public string title = "";
    // Start is called before the first frame update
    void Start()
    {
		selectMe(false);
		checkMe(false);
		//buttonMode = true;     //TODO: figure out unity substitutes for this whole area 
		//useHandCursor = true;
		//mouseChildren = false;
		//addEventListener(MouseEvent.CLICK, onClick, false, 0, true);
		//addEventListener(MouseEvent.DOUBLE_CLICK, onDoubleClick, false, 0, true);
	}

	

	public void setup(int i, int label, int des, string ti)
	{
		id = i;
		designation = des;
		title = ti;
		string st = "";
		switch (designation)
		{
			case GameLevelInfo.LVL_INTRO: st += "Intro"; break;
			case GameLevelInfo.LVL_REAL: st += "Level"; break;
		}
		st += "\n" + label.ToString();
		c_text.text = st;
	}

	private void onDoubleClick()
	{
		p_master.onDoubleClickLevel(this);
	}

	private void onClick()
	{
		//selectMe();
		p_master.onSelectLevel(this);
	}

	public void selectMe(bool b = true)
	{
		c_select.gameObject.SetActive(b);
	}

	public void checkMe(bool b = true)
	{
		c_check.gameObject.SetActive(b);
	}

	public void setMaster(MenuSystem_LevelPicker m)
	{
		p_master = m;
	}
}
