using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelInfo
{
	private int max_level; 					//maximum level index. Minimum is 0
	private List<int> list_lvl_cinemas; 	//index of whether there's a cinema after a level, and what it is
	private List<int> list_lvl_designation;
	private List<string> list_lvl_names;
	private List<int> list_lvl_label;
		
	public const int LVL_INTRO = 0;
	public const int LVL_REAL = 1;
	public  GameLevelInfo(int i)
	{
		//baked mode:
		setup(7);
		bakeData();
	}

	public int maxLevel
	{
		get
        {
			return max_level;
		}
		
	}

	/**
	 * Give me the num of the level you just finished, I'll show you the next movie
	 * @param	i
	 */

	public int getLvlCinema(int i) 
	{
			return list_lvl_cinemas[i];
	}

	public int getLvlDesignation(int i)
	{
		return list_lvl_designation[i];
	}

	public string getLvlName(int i)
	{
		return list_lvl_names[i];
	}

	public int getLvlLabel(int i)
	{
		return list_lvl_label[i];
	}

	private void setup(int i) 
		{
		max_level = i;
		list_lvl_cinemas = new List<int>(max_level+1);
		list_lvl_designation = new List<int>(max_level + 1);
		list_lvl_label = new List<int>(max_level + 1);
		list_lvl_names = new List<string>(max_level + 1);

		for (int j = 0; j <= max_level; j++) { //default is no cinemas
				list_lvl_cinemas[j] = -1;//Cinema.NOTHING; //TODO
		}
	}

	private void bakeData()
	{

		//Baked Mode:

		list_lvl_designation[0] = LVL_REAL;
		list_lvl_designation[1] = LVL_REAL;
		list_lvl_designation[2] = LVL_REAL;
		list_lvl_designation[3] = LVL_REAL;
		list_lvl_designation[4] = LVL_REAL;
		list_lvl_designation[5] = LVL_REAL;
		list_lvl_designation[6] = LVL_REAL;
		list_lvl_designation[7] = LVL_REAL;

		list_lvl_label[0] = 1;
		list_lvl_label[1] = 2;
		list_lvl_label[2] = 3;
		list_lvl_label[3] = 4;
		list_lvl_label[4] = 5;
		list_lvl_label[5] = 6;
		list_lvl_label[6] = 7;
		list_lvl_label[7] = 8;

		list_lvl_names[0] = "Pseudopod for the Win";
		list_lvl_names[1] = "Let's get Nuclear";
		list_lvl_names[2] = "Insane in the Membrane";
		list_lvl_names[3] = "Invasive Infection";
		list_lvl_names[4] = "Green Thumb";
		list_lvl_names[5] = "The Longest Journey";
		list_lvl_names[6] = "Heat Shock Crisis";
		list_lvl_names[7] = "Indigestion";

		/*
		list_lvl_cinemas[0] = Cinema.SCENE_LAB_INTRO;
		list_lvl_cinemas[1] = Cinema.SCENE_LAB_BOARD;
		list_lvl_cinemas[4] = Cinema.SCENE_LAUNCH;
		list_lvl_cinemas[5] = Cinema.SCENE_CRASH;
		list_lvl_cinemas[6] = Cinema.SCENE_LAND_CROC;
		list_lvl_cinemas[7] = Cinema.SCENE_FINALE;
		*/

		list_lvl_cinemas[0] = Cinema.SCENE_A;
		list_lvl_cinemas[1] = Cinema.SCENE_A;
		list_lvl_cinemas[4] = Cinema.SCENE_A;
		list_lvl_cinemas[5] = Cinema.SCENE_A;
		list_lvl_cinemas[6] = Cinema.SCENE_A;
		list_lvl_cinemas[7] = Cinema.SCENE_FINALE;
	}
}
