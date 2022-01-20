using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraButt : Button
{
	public int id = 0;
		
		
	public CameraButt()
	{
		//buttonMode = true;
		//mouseChildren = false;
		//useHandCursor = true;
		//addEventListener(MouseEvent.CLICK, onClick, false, 0, true);
	}

	void OnMouseClick()
	{
		MenuSystem_LevelPicker(parent).onClickCinema(id);
	}

}
