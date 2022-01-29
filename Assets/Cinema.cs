using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cinema : MovieClip
{
    protected Director p_director;
		
	public const int NOTHING = -1;
	public const int SPLASH = 0;
		
		//open source test scene
	public const int SCENE_A = 1;

	//These are not used in the open source version
	public const int SCENE_LAB_INTRO = 2;
	public const int SCENE_LAB_BOARD = 3;
	public const int SCENE_LAUNCH = 4;
	public const int SCENE_CRASH = 5;
	public const int SCENE_LAND_CROC = 6;
		
		
	public const int SCENE_FINALE = 7;
	public const int SCENE_CREDITS = 8;
		
		
	public const int MITOSIS = 100;
		
	//private var colorTrans:ColorTransform;  //TODO?
	private float fadeRate = 0;
	private bool fadeBlack = true;
	//private CinemaNavigator navigation;//TODO  :CinemaNavigator;
		
	private bool atHalt = false;
		
	private int myIndex;
	protected Music myMusic = Music.None;

	public Cinema()
	{
	
		/*navigation = new CinemaNavigator();              //TODO, all
		addChild(navigation);
		colorTrans = this.transform.colorTransform;*/
	}

	

	public void setDirector(Director d)
	{
		p_director = d;
	}

	public void setIndex(int i)
	{
		myIndex = i;
	}

	public void startCinema()
	{
		
		
		this.transform.position = Vector3.zero;

		startMusic();
		GotoAndPlay(0);
		//if (Director.STATS_ON) { Log.CustomMetric("cinema_start_" + getName(myIndex), "cinema"); } //TODO?
	}

	public void replay()
	{
		GotoAndPlay(1);
		startMusic();
	}


	public void readyCinema()
	{
		/*
		navigation.readyCinema();
		navigation.setIndex(myIndex);*/  //TODO
	}

	private void startMusic()
	{
		if (myMusic != Music.None)
		{
			if (myIndex != SCENE_FINALE)
			{
				
				MusicManager.Play(myMusic);
			}
			else
			{
				
				MusicManager.Stop();
			}
		}
	}

	private void stopMusic()
	{
		if (myMusic != Music.None)
		{
			MusicManager.Stop();
			
		}
	}

	public void pause()
	{
		_playing = false;
		MusicManager.Pause();
	}

	public void forFinalCinema()
	{
		/*navigation.forFinalCinema();   //TODO
		
		
		setChildIndex(navigation, numChildren - 1);

		navigation.visible = true;*/  
	}

	public void haltForFinalCinema()
	{
		atHalt = true;
		Stop();
		//navigation.haltForFinalCinema();  //TODO
	}

	public void halt()
	{
		atHalt = true;
		Stop();
		//navigation.halt();   //TODO
	}

	public void showNext()
	{
		atHalt = false;
		//navigation.showNext();   //TODO
	}

	public void unPause()
	{
		
		//Director.pauseMusic(false);  //TODO
		if (!atHalt)
		{
			Play();
		}
	}

	public void finishCinema()
	{
		Stop();
		atHalt = true;
		stopMusic();
		p_director.onFinishCinema();
		//if (Director.STATS_ON) { Log.CustomMetric("cinema_finish_" + getName(myIndex), "cinema"); }
	}

	public void skipCinema()
	{
		finishCinema();
		//if (Director.STATS_ON) { Log.CustomMetric("cinema_skip_" + getName(myIndex), "cinema"); }
	}

	public static string getName(int i) {
			switch(i) {
				case SCENE_A: return "a";
				case SCENE_CRASH: return "crash"; 
				case SCENE_CREDITS: return "credits";
				case SCENE_FINALE: return "finale";
				case SCENE_LAB_BOARD: return "lab_board";
				case SCENE_LAB_INTRO: return "lab_intro";
				case SCENE_LAND_CROC: return "land_croc";
				case SCENE_LAUNCH: return "launch";
				case SPLASH: return "splash";
				case MITOSIS: return "mitosis";
			}
return "null";
		}
		
public static int getByName(string s)
{
	int i;

	if (s == "a") i = SCENE_A;
	else if (s == "lab_intro") i = SCENE_LAB_INTRO;
	else if (s == "lab_board") i = SCENE_LAB_BOARD;
	else if (s == "launch") i = SCENE_LAUNCH;
	else if (s == "crash") i = SCENE_CRASH;
	else if (s == "land_croc") i = SCENE_LAND_CROC;
	else if (s == "finale") i = SCENE_FINALE;
	else if (s == "credits") i = SCENE_CREDITS;
	else if (s == "mitosis") i = MITOSIS;
	else if (s == "splash") i = SPLASH;
	else i = -1;

	return i;
}

	public int getIndex()
	{
		return myIndex;
	}
}
