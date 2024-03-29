﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * import com.pecLevel.GameLevelInfo;
	import com.pecLevel.WaveEntry;
	import fl.controls.ProgressBar;
	import flash.display.DisplayObject;
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.display.MovieClip;
	import flash.display.StageQuality;
	import flash.events.Event;
	import flash.events.KeyboardEvent;
	import flash.events.MouseEvent;
	import flash.events.ProgressEvent;
	import flash.media.Sound;
	import flash.net.URLRequest;
	import flash.system.Security;
	import flash.ui.Keyboard;
	import com.pecSound.*;
	import SWFStats.*;
 */
/**
	 * Class Director owns the entire project, and is in fact the 
	 * class associated with the Stage. Class Director sits on top 
	 * of everything, and is in charge of a master state machine 
	 * which determines which state the game itself is in 
	 * (Paused, InGame, InMenu, etc). 
	 * 
	 * This is your jumping off point for the CellCraft project. Everything begins and ends here.
	 * 
	 * First off, some naming conventions:
	 * 
	 * VALUEOFPI 	(capitals) 				-> constants
	 * c_subclip 	(c_ prefix, lowercase) 	-> children
	 * p_someclip   (p_prefix, lowercase) 	-> pointers
	 * otherStuff   (camelcase)				-> regular variables
	 * MenuSystem 	(initial cap camelcase)	-> Class name
	 * doSomething()(camelcase)				-> function name
	 * 
	 * Note especially the naming convention for children and pointers. We take a cue from Hungarian notation
	 * to demarkate the kind of thing the variable is, so it's easy to spot errors! Garbage collection in flash is
	 * a MESS because pointers and children are ambiguous. So here's the deal: anything is only ever the child of
	 * ONE displaycontainer at any given time, and that must be the class that created that instance, NO EXCEPTIONS.
	 * That class will prefix the variable pointing to it with a "c_" for child. You kill a child like this, EVERY
	 * TIME:
	 * 
	 * if(c_child){
	 * 	  c_child.destruct();
	 *    removeChild(c_child);
	 *    c_child = null;
	 * }
	 * 
	 * Every class should have a destruct() function where it cleans up all its mess. This is how we fight the entropy
	 * of Flash's mashed-potato-salad of a garbage collector
	 * 
	 * 
	 * 
	 * @author Lars A. Doucet
	 */

public class Director : MonoBehaviour
{
    // Start is called before the first frame update
   



		public static bool BAKED_MODE = true;
		public static bool SITE_LOCK = false;
		
		public static bool SITE_LOCK_SITEA = false;
		public static bool SITE_LOCK_SITEB = false;
		
		public static bool STATS_ON = true;
		public static bool KONG_ON = false;
		
		public static string VERSION_STRING = "1.0.4";
		public Title c_title;
		private Cinema c_cinema;


		private MenuSystem c_menu;
	public MenuSystem_LevelPicker LevelPicker;
		
		public Cinema_Blank CinemaBlank;
	
		/*
		[Embed(source = "../level_00.xml", mimeType = "application/octet-stream")]
	public static const Level_00_XML:Class;
		
		[Embed(source = "../level_01.xml", mimeType = "application/octet-stream")]
	public static const Level_01_XML:Class;
		
		[Embed(source = "../level_02.xml", mimeType = "application/octet-stream")]
	public static const Level_02_XML:Class;
		
		[Embed(source = "../level_03.xml", mimeType = "application/octet-stream")]
	public static const Level_03_XML:Class;
		
		[Embed(source = "../level_04.xml", mimeType = "application/octet-stream")]
	public static const Level_04_XML:Class;
		
		[Embed(source = "../level_05.xml", mimeType = "application/octet-stream")]
	public static const Level_05_XML:Class;
		
		[Embed(source = "../level_06.xml", mimeType = "application/octet-stream")]
	public static const Level_06_XML:Class;
		
		[Embed(source = "../level_07.xml", mimeType = "application/octet-stream")]
	public static const Level_07_XML:Class;
		*/
		
		public static int STAGEWIDTH = 640; //use these constants ANYWHERE IN GAME instead of stage.width,etc
		public static int STAGEHEIGHT = 480;
	
		private const KeyCode KEY_PAUSE = KeyCode.Escape;
		
		private const KeyCode KEY_UP = KeyCode.UpArrow;
		private const KeyCode KEY_UP2 = KeyCode.PageUp;
		private const KeyCode KEY_DOWN = KeyCode.DownArrow;
		private const KeyCode KEY_DOWN2 = KeyCode.PageDown;
		private const KeyCode KEY_LEFT = KeyCode.LeftArrow;
		private const KeyCode KEY_LEFT2 = KeyCode.Alpha4;
		private const KeyCode KEY_RIGHT = KeyCode.RightArrow;
	private const KeyCode KEY_RIGHT2 = KeyCode.Alpha6;
		
		public const int CHEAT_0 = 48;
		public const int CHEAT_1 = 49;
		public const int CHEAT_2 = 50;
		public const int CHEAT_3 = 51;
		public const int CHEAT_4 = 52;
		public const int CHEAT_5 = 53;
		public const int CHEAT_6 = 54;
		public const int CHEAT_7 = 55;
		public const int CHEAT_8 = 56;
		public const int CHEAT_9 = 57;
		
		private List<bool> list_moveKeys = new List<bool> { false, false, false, false }; //UP,DOWN,LEFT,RIGHT
		
		public static bool IS_MOUSE_DOWN = false; //READ ONLY PLZ
		
		private float timestep = 1; //how much time per second is simulated
										 //default value is 1 second per second
		private DState state;       //The current state of the game. INGAME, INMENU, PAUSED, CINEMA, etc

		private static int curr_level = 0;


		private GameLevelInfo d_lvlInfo;
		private LevelProgress d_lvlProgress;
		//children:
		
		//public var c_preload:Preloader;
		public bool loaderStarted = false;
		public PauseSprite c_pauseSprite;
		private bool _started;
		public FPSCounter c_fps;
		private int exit_menu_code;
		private int picked_level;
		public Cursor c_cursor;
		private static float vol_music = 0.25f;
		private static float vol_sound = 0.40f;
		private static bool mute_music = false;
		private static bool old_mute_music = false;
		private static bool mute_sound = false;
		private static bool old_mute_sound = false;



	void Start()
	{
		c_fps = new FPSCounter();
		//siteLock();//TBD
		init();
	}

	private void init()
	{
		state = new DState();


		startItUp();
		//addEventListener(Event.ENTER_FRAME, run);
		_started = true;
	}

	private void startItUp()
	{
		Debug.Log("Director.startItUp()");
		LevelProgress.initProgress();
		//makeSoundMgr();  //TODO: probably obsolete

		makePauseSprite();  
		initListeners();
		loadGameLevelInfo();
		//showCinema(Cinema.SPLASH);  //TODO

	}

	private void run()
	{
		bool halt = true;
		listenKeyDown();
		listenKeyUp();
		//stage.addEventListener(Event.DEACTIVATE, listenDeactivate); //TODO: what is this?
		listenMouseUp();
		listenMouseDown();
		listenMouseLeave();
		listenMouseWheel();
		
		switch (state.getTop())
		{

			case GameState.TITLE:

				break;
			case GameState.CINEMA:

				break;
			case GameState.INGAME:
				halt = false;

				//c_engine.dispatchEvent(new RunFrameEvent(RunFrameEvent.RUNFRAME, null));  //TODO

				break;
			case GameState.FAUXPAUSED:
				//c_engine.dispatchEvent(new RunFrameEvent(RunFrameEvent.FAUXFRAME, null));  //TODO
				break;
			case GameState.INMENU:
				break;

			case GameState.PAUSED:
				break;
			//do nothing
			case GameState.NOSTATE:
				break;
		}
	}
	//INPUT STUFF
	private void listenKeyUp()
	{
		if (checkKeyUp())
		{
			switch (state.getTop())
			{
				case GameState.TITLE:

					break;
				case GameState.CINEMA:

					break;
				case GameState.INGAME:
					//c_engine.checkKeys();  //TODO
					break;
				case GameState.INMENU:

					break;
				case GameState.PAUSED:
					break;
			}
		}
	}



	private void listenKeyDown()
	{
		if (checkKeyDown())
		{           //if there was a key that got through our filter
			switch (state.getTop())
			{
				case GameState.TITLE:

					break;
				case GameState.CINEMA:

					break;
				case GameState.INGAME:
					break;
				case GameState.INMENU:

					break;
				case GameState.PAUSED:
					break;
			}
		}
	}
	private bool checkKeyUp()
	{
		if (Input.GetKeyUp(KEY_PAUSE))
        {
			
        }
		if (Input.GetKeyUp(KEY_DOWN))
        {
			release_arrow(1);
		}
		if (Input.GetKeyUp(KEY_LEFT))
        {
			release_arrow(2);
		}
		if (Input.GetKeyUp(KEY_UP))
        {
			release_arrow(0);
		}
		if (Input.GetKeyUp(KEY_RIGHT))
        {
			release_arrow(3);
		}
		return false;
		

			/*case CHEAT_0:  //TODO
			case CHEAT_1:
			case CHEAT_2:
			case CHEAT_3:
			case CHEAT_4:
			case CHEAT_5:
			case CHEAT_6:
			case CHEAT_7:
			case CHEAT_8: cheat(e.keyCode); break;
			case CHEAT_9: LevelProgress.clearProgress(); break;*/
		if (c_cursor != null && c_cursor.gameObject.activeSelf)
        {
			c_cursor.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

	}

	private bool checkKeyDown()
	{
		if (Input.GetKeyDown(KEY_PAUSE))
		{

		}
		if (Input.GetKeyDown(KEY_DOWN))
		{
			press_arrow(1);
		}
		if (Input.GetKeyDown(KEY_LEFT))
		{
			press_arrow(2);
		}
		if (Input.GetKeyDown(KEY_UP))
		{
			press_arrow(0);
		}
		if (Input.GetKeyDown(KEY_RIGHT))
		{
			press_arrow(3);
		}
		return false;
		
	}

	private void press_arrow(int i)
	{
		list_moveKeys[i] = true;
	}
	private void release_arrow(int i)
	{
		list_moveKeys[i] = false;
	}


	private void clearKeys()
	{
		for (int i = 0; i < list_moveKeys.Count; i++)
		{
			list_moveKeys[i] = false;
		}
	}

	public bool getArrow(int i)
	{
		return list_moveKeys[i];
	}


	// Update is called once per frame
	void Update()
	{
		if (_started)
			run();
		if (c_fps != null)
			c_fps.ExternalUpdate();
	}

	private void makePauseSprite()
	{
		//c_pauseSprite = new PauseSprite();
		//addChild(c_pauseSprite);
		c_pauseSprite.setDirector(this);
		//fpsOnTop();  //TODO?
	}


	private void showPauseSprite(string whichFrame = "normal")
	{
		c_pauseSprite.gameObject.transform.SetSiblingIndex(this.transform.childCount - 1);
		//setChildIndex(c_pauseSprite, numChildren - 1);
		c_pauseSprite.show(whichFrame);
	}

	private void hidePauseSprite()
	{
		c_pauseSprite.hide();
	}

	public void pauseSpriteUnPause()
	{
		if (state.getTop() == GameState.PAUSED)
		{
			//if (Director.STATS_ON) { Log.CustomMetric("unpause_game", "interface"); }  //TODO?


			oldState();       //ALWAYS return to the correct state first!
			contextUnPause(); //then, do the correct context action depending on which state we were in
			hidePauseSprite();
		}
	}


	public void keyPause()
	{
		if (state.getTop() != GameState.PAUSED)
		{
			//if (Director.STATS_ON) { Log.CustomMetric("pause_game", "interface"); }
			//if (Director.STATS_ON) { Log.LevelAverageMetric("pause_game", curr_level, 1); }
			contextPause(); //ALWAYS do the correct context action depending on which state we were in first!
			newState(GameState.PAUSED); //then, we pause
			showPauseSprite();
			releaseInput();
		}
	}

	public void contextPause()
	{
		switch (state.getTop())
		{
			case GameState.CINEMA: pauseCinema(); break;
			case GameState.INGAME: pauseGame(); break; //donothing
		}
	}

	private void pauseGame()
	{

		/*if (c_engine)  //TODO
		{
			c_engine.pauseAnimate(true);

		}*/
	}

	private void unPauseGame()
	{

		/*if (c_engine)  //TODO
		{
			c_engine.pauseAnimate(false);
		}*/ 
	}

	//Cinema ceases play


	private void pauseCinema()
	{
		/*if (c_cinema)               //TODO
			c_cinema.pause();*/
	}


	private void oldState()
	{
		GameState prev = state.pop();
		switch (state.getTop())
		{
			case GameState.CINEMA: break;
			case GameState.INGAME: break;
			case GameState.INMENU: break;
			case GameState.PAUSED: break;
			case GameState.FAUXPAUSED: break;
			case GameState.TITLE:
				if (prev != GameState.PAUSED)
				{ //if we were paused, we haven't hidden the title
					//if (c_title) //TODO
					//{ //don't do this if the title already exists
						//makeTitle(); //TODO //be sure to show the title again, BUT DONT push the state, we're already there!
					//}
				}
				break;

		}
		//state.traceStack();
	}

	public void contextUnPause()
	{
		/*  //TODO
		switch (state.getTop())
		{
			case DState.CINEMA: unPauseCinema(); break;
			case DState.INGAME: unPauseGame(); fancyCursor(); break; //donothing
		}*/
	}

	//*********STATE STACK STUFF***************


	//Sets the new state, initializing the stack
	// @param	s


	private void setState(GameState s)
	{
		state.setState(s);
	}


	// Goes to a new state, shoves the old ones back in the stack
	// @param	s the new state code to go to


	private void newState(GameState s)
	{
		//trace("newState=" + s);
		state.push(s);  //push the state on
						//state.traceStack();
	}

	private void releaseInput()
	{
		//c_engine.dispatchEvent(new MouseEvent(MouseEvent.MOUSE_UP));  //TODO
	}


	private void initListeners()
	{

		
	}

	private void listenMouseWheel()
	{
		/*if (m.delta > 0)  //TODO
		{
			c_engine.mouseWheel(1);
		}
		else
		{
			c_engine.mouseWheel(-1);
		}*/
	}

	private void listenMouseUp()
	{
		//trace("Director.listenMouseUp()");
		IS_MOUSE_DOWN = false;

		//filthy hack:
		/*if (c_engine)
		{
			c_engine.mouseUp(m);   //TODO
		}*/
	}


	private void listenMouseDown()
	{
		//trace("Director.listenMouseDown()");
		IS_MOUSE_DOWN = true;
	}

	private void listenMouseLeave()
	{
		//trace("Director.listenMouseLeave()");
		IS_MOUSE_DOWN = false;
	}




	private void listenMouseClick()
	{
		switch (state.getTop())
		{
			case GameState.TITLE:
				//c_title.dispatchEvent(e);  //TODO
				break;
			case GameState.CINEMA:
				//c_cinema.dispatchEvent(e); //TODO
				break;
			case GameState.INGAME:
				//c_engine.dispatchEvent(e);  //TODO
				break;
			case GameState.INMENU:
				//c_menu.dispatchEvent(e);  //TODO
				break;
			case GameState.PAUSED:
				break;
		}
	}

	private void listenMouseMove()
	{
		switch (state.getTop())
		{
			case GameState.TITLE:

				break;
			case GameState.CINEMA:

				break;
			case GameState.INGAME:

				//c_engine.mouseMove(e);  //TODO
			
				break;
			case GameState.INMENU:

				break;
			case GameState.PAUSED:
				break;
		}
	}

	private void loadGameLevelInfo()
	{
		d_lvlInfo = new GameLevelInfo(7);
		d_lvlProgress = new LevelProgress(7);
		LevelProgress.getProgress();
	}


	//
	// Runs a cinema "hassle-free." All you have to do is pass it one variable - the index
	// value associated with the movie you want, which are all public static consts in class Cinema. So, pass
	// Cinema.SPLASH to show the splash screen. 
	// 
	// This will play that movie, and when it is finished, finishCinema() will sort out what to do with it.
	//
	// ASSUMPTIONS: This framework ONLY allows you to play one cinema at a time.
	// @param	i
	//

	public void showCinema(int i)
	{
		

		//stage.quality = StageQuality.HIGH;
		//trace("Director.showCinema(" + i + ")");
		switch (state.getTop())
		{
			case GameState.INGAME : pauseGame(); break;
		}

		newState(GameState.CINEMA);
		c_cinema = null;
		string s;

		/*switch (i)  //TODO: implement this when and if the cinemas come in
		{
			case Cinema.NOTHING: c_cinema = CinemaBlank; s = "nothing"; break;
			case Cinema.SPLASH: c_cinema = CinemaBlank; s = "splash"; break;
			case Cinema.SCENE_LAB_INTRO: c_cinema = Cinema(new Cinema_Scene1()); s = "lab intro"; break;
			case Cinema.SCENE_LAB_BOARD: c_cinema = Cinema(new Cinema_Scene2()); s = "lab board"; break;
			case Cinema.SCENE_LAUNCH: c_cinema = Cinema(new Cinema_Scene3()); s = "launch"; break;
			case Cinema.SCENE_CRASH: c_cinema = Cinema(new Cinema_Scene4()); s = "crash"; break;
			case Cinema.SCENE_LAND_CROC: c_cinema = Cinema(new Cinema_Scene5()); s = "land croc"; break;
			case Cinema.SCENE_FINALE: c_cinema = Cinema(new Cinema_Finale()); s = "finale"; break;
			default: //throw new Error("Cinemas 2-5 don't exist yet, dummy!"); 
				c_cinema = Cinema(new Cinema_Blank()); break;
				break;
			case Cinema.MITOSIS: c_cinema = Cinema(new Cinema_Mitosis()); s = "mitosis"; break;
			case Cinema.SCENE_CREDITS: c_cinema = Cinema(new Cinema_Credits()); s = "credits"; break;
		}*/

		c_cinema = CinemaBlank; //Temporary
		c_cinema.setIndex(i);
		c_cinema.setDirector(this);
		c_cinema.transform.SetParent(this.transform, false);
		c_cinema.startCinema();
		//fpsOnTop();

	}

	public void onFinishCinema()
	{
		int i = c_cinema.getIndex();
		oldState(); //remove the cinema state
		Debug.Log("Director.onFinishCinema() i=" + i + " state=" + state.getTop());
		switch (state.getTop())
		{

			case GameState.INGAME:
				switch (i)
				{
					case Cinema.MITOSIS:
						unPauseGame();
						//c_engine.finishMitosis();  //TODO
						break; //after mitosis, return to the game
					case Cinema.SCENE_LAB_INTRO:
					case Cinema.SCENE_LAB_BOARD:
					case Cinema.SCENE_LAUNCH:
					case Cinema.SCENE_CRASH:
					case Cinema.SCENE_LAND_CROC:
						onFinishLevelCinema();
						break;

					default:
						break;
				}
				break;

			case GameState.NOSTATE:
				switch (i)
				{
					case Cinema.SPLASH:
						showTitle(); break; //after the splash screen, show the title screen
				}
				break;

		}
		//c_cinema.destruct();  //TODO
		c_cinema.gameObject.transform.SetParent(null);
		c_cinema = null;
		onCinemaFinished(i);
	}

	public void onCinemaFinished(int i)
	{
		switch (i)
		{
			case Cinema.SCENE_FINALE:
				//onFinishFinale();  //TODO
				break;
			case Cinema.SCENE_CREDITS:
				//onFinishCredits();  //TODO
				break;
		}
	}


	// Goes to the title screen. It pushes the TITLE state onto the stateStack and then draws it.
	public void showTitle()
	{
		//stage.quality = StageQuality.HIGH;
		newState(GameState.TITLE);
		makeTitle();
	}






	// Actually draws the title, attaches it and sets it up and everything

	private void makeTitle()
	{
		//c_title = new Title();
		c_title.setDirector(this);
		c_title.transform.SetParent(this.transform, false);
		//fpsOnTop();
	}


	public void onFinishLevelCinema()
	{ //when an in-between level cinema finishes, go to the next level
	  
		//showMenu(MenuSystem.LEVELPICKER); //TODO
	}

	//Title functions

	public void goPlayGame()
	{
		//if (Director.STATS_ON) { Log.Play(); }
		
		//remove the title screen
		hideTitle();

		newState(GameState.INGAME); //we are now in the game state
								 //makeEngine();			 //create the engine and show it
		showMenu(MenuSystem.LEVELPICKER);
		//fpsOnTop();
	}


	// Used just to hide the title. If you want to oldState() back to the title, you'd best show it again!
	private void hideTitle()
	{
		c_title.transform.gameObject.SetActive(false);
	
	}

	//Show a specific menu
	// @param	i The index of the menu, found in MenuSystem's constants
	//

	public void showMenu(int i, object parms = null)
	{
		//normalCursor();

		contextPauseMenu();         //do something to respond to going into a menu
		newState(GameState.INMENU);    //push INMENU onto the statestack

		switch (i)
		{                   //show the right menu
			
			/*case MenuSystem.INGAME:
				c_menu = MenuSystem(new MenuSystem_InGame());
				break;
			case MenuSystem.OPTIONS:
				//show the options menu	
				break;
			case MenuSystem.TUTORIAL:
				c_menu = MenuSystem(new MenuSystem_Tutorial(TutorialEntry(parms)));
				break;
			case MenuSystem.HISTORY:
				c_menu = MenuSystem(new MenuSystem_History(TutorialArchive(parms)));
				break;
			case MenuSystem.ENCYCLOPEDIA:
				stage.quality = StageQuality.HIGH;
				if (Director.STATS_ON) { Log.LevelCounterMetric("used_encyclopedia", curr_level); }
				if (Director.STATS_ON) { Log.LevelAverageMetric("used_encyclopedia", curr_level, 1); }
				if (Director.STATS_ON) { Log.CustomMetric("menu_open_encyclopedia", "menu"); }
				if (parms)
						c_menu = MenuSystem(new MenuSystem_Encyclopedia(String(parms)));
					else
					c_menu = MenuSystem(new MenuSystem_Encyclopedia());
				break;*/
			case MenuSystem.LEVELPICKER:
				c_menu = LevelPicker;
				LevelPicker.setData(d_lvlProgress, d_lvlInfo);
				break;
			/*case MenuSystem.ENDLEVEL:
				c_menu = MenuSystem(new MenuSystem_EndLevel());
				MenuSystem_EndLevel(c_menu).setData(Number(parms));
				break;
			case MenuSystem.REWARD:
				c_menu = MenuSystem(new MenuSystem_Reward());
				MenuSystem_Reward(c_menu).setData(WaveEntry(parms));
				break;
			case MenuSystem.SCREWED:
				if (Director.STATS_ON) { Log.LevelCounterMetric("screwed", curr_level); }
				if (Director.STATS_ON) { Log.LevelAverageMetric("screwed", curr_level, 1); }
				if (Director.STATS_ON) { Log.CustomMetric("menu_open_screwed", "menu"); }
				c_menu = MenuSystem(new MenuSystem_Screwed());
				MenuSystem_Screwed(c_menu).setData((parms as Array));
				break;*/
		}
		c_menu.setDirector(this);
		//c_menu.setEngine(c_engine); //TODO
		c_menu.transform.SetParent(this.transform, false);
		c_menu.setIndex(i);
		c_menu.init();
		//fpsOnTop();
	}

	public void contextPauseMenu()
	{
		//trace("Director.contextPauseMenu()");
		switch (state.getTop())
		{
			case GameState.CINEMA: pauseCinema(); break;
			case GameState.INGAME: pauseGame(); break; //donothing

		}
	}



	public void returnToGame()
	{

		makeEngine();            //create the engine and show it
		//fpsOnTop();
	}

	public void makeEngine()
	{

		/*if (!c_engine)    //TODO
		{
			c_engine = new Engine();
			addChild(c_engine);
			c_engine.setDirector(this);
			c_engine.init(curr_level);
		}
		else
		{
			throw new Error("Director.makeEngine() : c_engine = " + c_engine + "!");
		}
		makeCursor();*/
	}


	public static int level()
	{
		return curr_level;
	}


	void siteLock()
	{


		init();
	}


	public static float getMusicVolume()
	{
		return vol_music;
	}

	public static float getSFXVolume()
	{
		return vol_sound;
	}

	public static bool getMusicMute()
	{
		return mute_music;
	}

	public static bool getSoundMute()
	{
		return mute_sound;
	}

	public static void rememberSFXMute()
	{
		mute_sound = old_mute_sound;
		SfxManager.SetMute(mute_sound);
	}


	public static void rememberMusicMute()
	{
		mute_music = old_mute_music;
		MusicManager.SetMute(mute_music);
	}

	public static void setSFXMute(bool doMute, bool remember = false)
	{
		if (remember)
		{
			old_mute_sound = mute_sound;
		}
		mute_sound = doMute;
		SfxManager.SetMute(doMute);
	}

	public static void setMusicMute(bool doMute, bool remember = false)
	{
		if (remember)
		{
			old_mute_music = mute_music;
		}
		mute_music = doMute;
		MusicManager.SetMute(doMute);
	}

	public static void setSFXVolume(float vol)
	{
		vol_sound = vol;
		SfxManager.SetVolume(vol);
	}

	public static void setMusicVolume(float vol)
	{
		vol_music = vol;
		SfxManager.SetVolume(vol);
	}

	public static void pauseAllSFX(bool b)
	{
		if (b)
			SfxManager.StopAll(); //.pauseAllSFX(b);
	}

	public static void pauseMusic(bool b)
	{
		if (b)
			MusicManager.Pause();
		else
			MusicManager.Resume();
	}

	public static void setHalfVolume(bool b)
	{
		if (b)
		{
			SfxManager.SetVolume(0.5f);
			MusicManager.SetVolume(0.5f);
		}
		else
		{
			SfxManager.SetVolume(1f);
			MusicManager.SetVolume(1f);
		}
	}


	public static void pauseAudio(bool b)
	{
		if (b)
        {
			SfxManager.StopAll();
			MusicManager.Stop();
        }
	}

	public static void stopAllSFX()
	{
		SfxManager.StopAll();
	}

	public static bool stopSFX(int extId)
	{
		return false;  //deprecating this
		//return c_soundMgr.stopSFX(extID);
	}

	public static void startSFX(int soundID, bool loop = false) 
	{
		//deprecating this too
	}


	public static void onSoundFinish(SFX id)
	{
		switch (id)
		{
			case SFX.MusicBattleIntro:
				MusicManager.Play(Music.MusicBattle);
				break;
		}
	}

	public static void stopMusic()
	{
		MusicManager.Stop();
	}

	public static int startMusic(Music soundID, bool loop = true)
	{
		MusicManager.Play(soundID);//.startMusic(soundID, loop);
		return 1;
	}


	private void makeFPSCounter()
	{
		//c_fps = new FPSCounter(610, 0, 0xFFFFFF, true);
		//addChild(c_fps);
	}

	private void fpsOnTop()
	{
		/*if (c_fps)
		{
			setChildIndex(c_fps, numChildren - 1);
		}*/
	}



	public void tempHighQuality()
	{
		//stage.quality = StageQuality.HIGH;
	}

	public void normalQuality()
	{
		//stage.quality = MenuSystem_InGame._quality;
	}

	public void nextLevel()
	{
		endLevel(); //ends the engine but keeps us in the engine state

		//if (Director.KONG_ON) { Director.kongregate.stats.submit("level_beaten_" + curr_level, 1); }
		//if (Director.STATS_ON) { Log.LevelCounterMetric("beat_level", curr_level); }
		LevelProgress.setLevelBeaten(curr_level, true); //player gets credit for beating the level

		if (d_lvlInfo.maxLevel == curr_level)
		{
		//	if (Director.KONG_ON) { Director.kongregate.stats.submit("game_beaten", 1); }
			LevelProgress.setGameBeaten(true);
		}


		bool success = false;

		//var cinema:int 




		int cinema = d_lvlInfo.getLvlCinema(curr_level);

		if (cinema != Cinema.NOTHING)
		{ //if there is a cinema, figure out what it is, and then show it
		  //var i:int = Cinema.getByName(cinema);
			showCinema(cinema);
			success = true;
		}

		if (!success)
		{ //if there was no cinema, just go to the next level
			onFinishLevelCinema();
		}
	}

	public void pickLevel(int i)
	{
		curr_level = i;
		returnToGame();
	}



	public void onFinishFinale()
	{

		Debug.Log("Director.onFinishFinale()");
		showCinema(Cinema.SCENE_CREDITS);
	}

	public void onFinishCredits()
	{
		//showTitle();
		//oldState();
		switch (state.getTop())
		{
			case GameState.INGAME:
				oldState(); //get out of the game, hopefully to the title
				onFinishCredits(); //do this again, hopefully should get us to the title
				break;
			case GameState.INMENU:
				if (c_menu is MenuSystem_LevelPicker)
				{
					MenuSystem_LevelPicker lp = c_menu as MenuSystem_LevelPicker;
					if (c_title)
					{
						c_title.onTitleStart();
					}
					lp.onTitle(); 
					
				}
				break;
			case GameState.TITLE:
				if (c_title)
				{
					c_title.onTitleStart();
				}
				
				break;
		}
	}

	public void resetGame()
	{
		//if (Director.STATS_ON) { Log.LevelAverageMetric("reset_level", curr_level, 1); }
		//if (Director.STATS_ON) { Log.LevelCounterMetric("reset_level", curr_level); }

		endLevel();
		
		makeEngine();
		fpsOnTop();
	}

	public void endLevel()
	{
		/*
		removeChild(c_engine);
		c_engine.destruct();    //TODO
		c_engine = null;*/
	}


	public void quitGame()
	{
		//if (Director.STATS_ON) { Log.LevelAverageMetric("quit_level", curr_level, 1); }
		//if (Director.STATS_ON) { Log.LevelCounterMetric("quit_level", curr_level); }
		Debug.Log("Director.quitGame()!");
		endLevel();
		oldState();
		//showMenu(MenuSystem.LEVELPICKER);
	}

	// Cinema resumes play

	private void unPauseCinema()
	{
		if (c_cinema)
			c_cinema.unPause();
	}


	// Called from WITHIN the Cinema movieclip. Terminates play and yields control to the director

	public void setExitMenuCode(int i, int param)
	{
		exit_menu_code = i;
		switch (i)
		{
			case MenuSystem.EXIT_PICK:
				picked_level = param; break;
			case MenuSystem.EXIT_TITLE:
				break;
		}
	}


	public void exitMenu()
	{
		c_menu.transform.SetParent(null);
		oldState();           //exit the menu state
		contextUnPauseMenu(); //You MUST unpause before telling the engine to respond! Bugzors otherwise!
		int i = c_menu.getIndex(); 
	
		/*                                    //TODO
		var reward_array:Array;
		if (i == MenuSystem.REWARD)
		{ //HACK
			reward_array = MenuSystem_Reward(c_menu).getRewards();
		}*/

		c_menu.destruct();
		c_menu = null;

		switch (i)
		{
			case MenuSystem.TUTORIAL: 
				//c_engine.onExitTutorial();  //TODO 
				break;
			case MenuSystem.HISTORY: 
				//c_engine.onExitHistory(); //TODO
				break;
			case MenuSystem.LEVELPICKER:
				//var code:int = MenuSystem_LevelPicker(c_menu).exitCode;
				switch (exit_menu_code)
				{
					case MenuSystem.EXIT_PICK:  //go to the picked level
						pickLevel(picked_level); break;
					case MenuSystem.EXIT_TITLE: //go back to the title screen
												//showTitle
						oldState(); //exit whatever state we were in before, almost certainly ENGINE_STATE
									//if the title was beneath this, it should re-show the title!
						if (c_title)
						{
							c_title.onTitleStart();
						}
						break;
					case MenuSystem.EXIT_RESET:
						showMenu(MenuSystem.LEVELPICKER); //show the menu again!
						break;
				}
				break;
			case MenuSystem.INGAME:
				if (state.getTop() == GameState.INGAME)
				{
					//c_engine.updateMuteButton();  //TODO
				}
				break;
			case MenuSystem.REWARD:
				if (state.getTop() == GameState.INGAME)
				{
					//c_engine.getRewards(reward_array);   //TODO
				}
				break;
			case MenuSystem.ENDLEVEL:
				if (state.getTop() == GameState.INGAME)
				{
					//c_engine.onEndLevelMenuClose();  //TODO
				}
				break;
		}



	}


	public void contextUnPauseMenu()
	{
		//trace("Director.contextUnPauseMenu()");
		switch (state.getTop())
		{
			case GameState.CINEMA: unPauseCinema(); break;
			case GameState.INGAME: unPauseGame(); 
				fancyCursor(); 
				break; //donothing

		}
	}

	public void startFauxPause()
	{
		if (state.getTop() != GameState.FAUXPAUSED)
		{
			if (state.getTop() == GameState.INGAME)
			{
				newState(GameState.FAUXPAUSED);
				showPauseSprite("faux");
				releaseInput();
			}
		}
	}

	public void endFauxPause()
	{
		if (state.getTop() == GameState.FAUXPAUSED)
		{
			oldState();
			hidePauseSprite();
		}
	}

	public void toggleFauxPause()
	{
		if (state.getTop() != GameState.FAUXPAUSED)
		{
			if (state.getTop() == GameState.INGAME)
			{ //can ONLY fauxpause from ingame!
			  //contextFauxPause();
				newState(GameState.FAUXPAUSED);
				showPauseSprite("faux");
				releaseInput();
			}
			else
			{
				oldState();
				//contextUnFauxPause();
				hidePauseSprite();
			}
		}
	}

	// Pauses the game if not in paused state, otherwise unpauses
	//

	public void togglePause()
	{
		if (state.getTop() != GameState.PAUSED)
		{
		//	if (Director.STATS_ON) { Log.CustomMetric("pause_game", "interface"); }
			//if (Director.STATS_ON) { Log.LevelAverageMetric("pause_game", curr_level, 1); }
			contextPause(); //ALWAYS do the correct context action depending on which state we were in first!
			newState(GameState.PAUSED); //then, we pause
			showPauseSprite();
			releaseInput();
		}
		else
		{
			//if (Director.STATS_ON) { Log.CustomMetric("unpause_game", "interface"); }
			oldState();       //ALWAYS return to the correct state first!
			contextUnPause(); //then, do the correct context action depending on which state we were in
			hidePauseSprite();
		}
	}

	private void makeCursor()
	{
		c_cursor.show();
		//c_cursor.setEngine(c_engine);  //TODO
		//c_engine.receiveCursor(c_cursor);  //TODO
	}

	public void moveCursor()
	{
		//c_cursor.followMouse(m);
		c_cursor.updateMoveCost();
	}

	public void normalCursor()
	{
		if (c_cursor)
			c_cursor.normal(true);
	}

	public void fancyCursor()
	{
		if (c_cursor)
			c_cursor.normal(false);
	}

	public void showCursor(CellAction i)
	{
		//setChildIndex(c_cursor, numChildren - 1);
		c_cursor.transform.SetSiblingIndex(c_cursor.transform.childCount - 1);
		bool cursorShown = c_cursor.show(i);

		//c_engine.setSelectMode(!cursorShown); //if the cursor is shown, selectMode is false  //TODO
	}


	// Makes the pause sprite and attaches it to the stage (it hides automatically until shown)

	public void contextFauxPause()
	{ //will ALWAYS be for ingame state
	  //pauseGame();
	}

	public void contextUnFauxPause()
	{
		//unPauseGame();
	}

	// Runs every frame, and gives functionality to the underlying classes
	private void cheat(int i)
	{
		switch (state.getTop())
		{
			case GameState.INGAME:
				//c_engine.cheat(i);
				break;
			case GameState.TITLE:
				d_lvlProgress.maxLevelBeaten(i);
				break;
		}
	}

	private void listenDeactivate()
	{

		clearKeys(); //when the player deactivates, we have no idea what's down, so we assume they were all lifted
	}


	/*                                             //deprecating this also 
	public static function startMusicAt(position:int, soundID: int= -1, loop:Boolean = false) {
		return c_soundMgr.startMusicAt(position, soundID, loop);
	

	}*/



	//BOOKMARK: you were here 
	/*
		
		
		public var c_engine:Engine;

		
		private var c_cross:Cross;
		
		private static var c_soundMgr:SoundManager;
		
		
		
		// Kongregate API reference
		public static var kongregate:*;



  //Creates the state stack, creates the pause sprite, and other basic bootstrap functions





	*/
}
