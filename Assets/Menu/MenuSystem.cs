using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MovieClip
{
	public static int INGAME = 0;
	public static int OPTIONS = 1;
	public static int TUTORIAL = 2;
	public static int HISTORY = 3;
	public static int ENCYCLOPEDIA = 4;
	public static int LEVELPICKER = 5;
	public static int REWARD = 6;
	public static int SCREWED = 7;
	public static int ENDLEVEL = 8;
		
	protected int myIndex = -1;
	protected Director p_director;
	//protected var p_engine:Engine;  //TODO
		
	public static int EXIT_PICK = 1;
	public static int EXIT_TITLE = 0;
	public static int EXIT_RESET = 2;

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

	public void init()
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
