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
				list_lvl_cinemas.Add(Cinema.NOTHING);
		}
	}

	private void bakeData()
	{

		//Baked Mode:
		for(int i=0; i <= max_level; i++ )
        {
			list_lvl_designation.Add(LVL_REAL);
			list_lvl_label.Add(i + 1);
        }
		
		list_lvl_names = new List<string>{ "Pseudopod for the Win", "Let's get Nuclear","Insane in the Membrane","Invasive Infection","Green Thumb","The Longest Journey","Heat Shock Crisis","Indigestion"};
	

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
