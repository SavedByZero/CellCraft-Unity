using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress
{
	private static bool[] lvl_beaten = { false, false, false, false, false, false, false, false, false, false };
	private static bool game_beaten = false;
	public static int FOREVER = 9999999;
	private static int[] lvl_times = { FOREVER, FOREVER, FOREVER, FOREVER, FOREVER, FOREVER, FOREVER, FOREVER, FOREVER, FOREVER };
	private static int[] lvl_grades = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
	private static int max_level = 1;

	//private static var mySo:SharedObject;  //PlayerPrefs in unity 

	public LevelProgress(int i)
	{
		max_level = i;
		lvl_beaten = new bool[max_level + 1];
		for (int j = 0; j <= max_level; j++) {
			lvl_beaten[j] = false;
		}
	}

	public static void initProgress()
	{
		//mySo = SharedObject.getLocal("CellCraftData");
		//PlayerPrefs
	}

	public static void clearProgress()
	{
		Debug.Log("LevelProgress.clearProgress()!");
		PlayerPrefs.SetInt("max_level_beaten", -1);
		//mySo.data.max_level_beaten = -1;
		//mySo.flush();
		getProgress();
		Debug.Log("...lvl_beaten = " + lvl_beaten);
	}

	public static void saveProgress()
	{
		Debug.Log("LevelProgress.saveProgress()!");
		PlayerPrefs.SetInt("max_level_beaten", getMaxLevelBeaten());
		PlayerPrefs.SetInt("game_beaten", getGameBeaten() ? 1 : 0);
		for (int i = 0; i < max_level; i++) {
			PlayerPrefs.SetInt("level_" + i + "_seconds", lvl_times[i]);
			PlayerPrefs.SetInt("level_" + i + "_grade", lvl_grades[i]);
		}
		//mySo.flush();
		Debug.Log("...level_beaten = " + lvl_beaten);
	}

	public static void getProgress()
	{
		Debug.Log("LevelProgress.getProgress()!");

		if (!PlayerPrefs.HasKey("game_beaten"))
		{

		}
		else
		{
			game_beaten = (PlayerPrefs.GetInt("game_beaten") == 1);
		}

		if (!PlayerPrefs.HasKey("max_level_beaten"))
		{
			Debug.Log("LevelProgress.getProgress() UNDEFINED");
			PlayerPrefs.SetInt("max_level_beaten", -1);
			clearMaxLevelBeaten(-1);
		}
		else
		{
			clearMaxLevelBeaten(PlayerPrefs.GetInt("max_level_beaten"));
		}
		Debug.Log("...lvl_beaten = " + lvl_beaten);

		for (int i = 0; i < max_level; i++) 
		{
			int time = PlayerPrefs.GetInt("level_" + i + "_seconds");
			int grade = PlayerPrefs.GetInt("level_" + i + "_grade");
			if (time > 0) { lvl_times[i] = time; }
			if (grade > 0) { lvl_grades[i] = grade; }
		}
	}

	public static bool getLevelBeaten(int i) {
			return lvl_beaten[i];
		}

	public static void setLevelBeaten(int i, bool b) {
		lvl_beaten[i] = b;
		saveProgress();
	}

	public static void setLevelTime(int i,int time) {
		if (lvl_times[i] > time)
		{
			lvl_times[i] = time;
		}
	}

	public static int getLevelTime(int i)
	{
		return lvl_times[i];
	}

	public static void setLevelGrade(int i, int grade) 
	{
		if (lvl_grades[i] < grade)
		{
			lvl_grades[i] = grade;
		}
	}

	public static int getLevelGrade(int i)
	{
		return lvl_grades[i];
	}

	public static void setGameBeaten(bool b)
	{
		game_beaten = b;
	}

	public static bool getGameBeaten()
	{
		return game_beaten;
	}

	private static void clearMaxLevelBeaten(int i) {
		Debug.Log("LevelProgress.clearMaxLevelBeaten(" + i + ")");
		int j;
		for (j = 0; j <= max_level; j++)
		{
			lvl_beaten[j] = false;
		}
		if (i > max_level)
		{
			i = max_level;
		}
		if (i >= 0)
		{
			for (j = 0; j <= i; j++)
			{
				lvl_beaten[j] = true;
			}
		}

	}

	public static int getMaxLevelBeaten()
	{
		int i = 0;
		for (int j = 0; j <= max_level; j++) {
			if (lvl_beaten[j])
			{
				i = j;
			}
			else
			{
				break;
			}
		}
		return i;
	}

	public void maxLevelBeaten(int i)
	{
		if (i > max_level)
		{
			i = max_level;
		}
		for (int j = 0; j <= i; j++) {
			lvl_beaten[j] = true;
		}
		saveProgress();
	}
}
