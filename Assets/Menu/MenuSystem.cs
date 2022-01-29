using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MovieClip
{
	public const int INGAME = 0;
	public const int OPTIONS = 1;
	public const int TUTORIAL = 2;
	public const int HISTORY = 3;
	public const int ENCYCLOPEDIA = 4;
	public const int LEVELPICKER = 5;
	public const int REWARD = 6;
	public const int SCREWED = 7;
	public const int ENDLEVEL = 8;
		
	protected int myIndex = -1;
	protected Director p_director;
	//protected var p_engine:Engine;  //TODO
		
	public const int EXIT_PICK = 1;
	public const int EXIT_TITLE = 0;
	public const int EXIT_RESET = 2;

	public void fancyCursor()
	{
		//p_director.fancyCursor();  //TODO
	}

	public void normalCursor()
	{
		//p_director.normalCursor()  //TODO
	}

	public void destruct()
	{

	}

	public virtual void init()
	{
		//p_director.tempHighQuality();  //TODO
	}

	public void exit()
	{
		//p_director.normalQuality();  //TODO
		//p_director.exitMenu();  //TODO
	}

	public void setIndex(int i)
	{
		myIndex = i;
	}

	public int getIndex() {
			return myIndex;
		}

	public void setDirector(Director d) {
		p_director = d;
	}

		/*
	public void setEngine(Engine e) {               //TODO
		p_engine = e;
	}*/

	public void onFadeOut()
	{

	}
}
