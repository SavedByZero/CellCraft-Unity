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
    void Start()
    {
		c_fps = new FPSCounter();
		//siteLock();//TBD
		_started = true;
	}

    // Update is called once per frame
    void Update()
    {
		//if (_started)    //TBD
			//run();       //TBD
		if (c_fps != null)
			c_fps.ExternalUpdate();
    }



		public static bool BAKED_MODE = true;
		public static bool SITE_LOCK = false;
		
		public static bool SITE_LOCK_SITEA = false;
		public static bool SITE_LOCK_SITEB = false;
		
		public static bool STATS_ON = true;
		public static bool KONG_ON = false;
		
		public static string VERSION_STRING = "1.0.4";
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
	
		private int KEY_PAUSE = (int)KeyCode.Escape;
		
		private int KEY_UP = (int)KeyCode.UpArrow;
		private int KEY_UP2 = (int)KeyCode.PageUp;
		private int KEY_DOWN = (int)KeyCode.DownArrow;
		private int KEY_DOWN2 = (int)KeyCode.PageDown;
		private int KEY_LEFT = (int)KeyCode.LeftArrow;
		private int KEY_LEFT2 = 65;
		private int KEY_RIGHT = (int)KeyCode.RightArrow;
		private int KEY_RIGHT2  = 68;
		
		public static int CHEAT_0 = 48;
		public static int CHEAT_1 = 49;
		public static int CHEAT_2 = 50;
		public static int CHEAT_3 = 51;
		public static int CHEAT_4 = 52;
		public static int CHEAT_5 = 53;
		public static int CHEAT_6 = 54;
		public static int CHEAT_7 = 55;
		public static int CHEAT_8 = 56;
		public static int CHEAT_9 = 57;
		
		private List<bool> list_moveKeys = new List<bool> { false, false, false, false }; //UP,DOWN,LEFT,RIGHT
		
		public static bool IS_MOUSE_DOWN = false; //READ ONLY PLZ
		
		private float timestep = 1; //how much time per second is simulated
										 //default value is 1 second per second
		private DState state;		//The current state of the game. INGAME, INMENU, PAUSED, CINEMA, etc
		
		//children:
		
		//public var c_preload:Preloader;
		public bool loaderStarted = false;
	private bool _started;
	public FPSCounter c_fps; //BOOKMARK: you were here 
	/*
		public var c_cursor:Cursor;
		
		
		public var c_engine:Engine;
		private var c_title:Title;
		private var c_cinema:Cinema;
		private var c_pauseSprite:PauseSprite;
		private var c_menu:MenuSystem;
		
		private int exit_menu_code;
		private int picked_level;
		
		private var c_cross:Cross;
		
		private static var c_soundMgr:SoundManager;
		
		private static float vol_music = 0.25f;
		private static float vol_sound = 0.40f;
		private static bool mute_music = false;
		private static bool old_mute_music = false;
		private static bool mute_sound = false;
		private static bool old_mute_sound = false;
		
		private static int curr_level = 0;
		
		
		private var d_lvlInfo:GameLevelInfo;
		private var d_lvlProgress:LevelProgress;
		
		// Kongregate API reference
		public static var kongregate:*;





	public static int level()  {
			return curr_level;	
		}


void siteLock()
{

	
		init();
}



private static function makeSoundMgr()
{
	c_soundMgr = new SoundManager();
	setMusicVolume(vol_music);
	setSFXVolume(vol_sound);
}

public static function getMusicVolume():Number
{
	return vol_music;
}

public static function getSFXVolume():Number
{
	return vol_sound;
}

public static function getMusicMute():Boolean
{
	return mute_music;
}

public static function getSoundMute():Boolean
{
	return mute_sound;
}

public static function rememberSFXMute()
{
	mute_sound = old_mute_sound;
	c_soundMgr.setSFXMute(mute_sound);
}

public static function rememberMusicMute()
{
	mute_music = old_mute_music;
	c_soundMgr.setMusicMute(mute_music);
}

public static function setSFXMute(doMute:Boolean,remember: Boolean = false) {
	if (remember)
	{
		old_mute_sound = mute_sound;
	}
	mute_sound = doMute;
	c_soundMgr.setSFXMute(doMute);
}

public static function setMusicMute(doMute:Boolean,remember: Boolean = false) {
	if (remember)
	{
		old_mute_music = mute_music;
	}
	mute_music = doMute;
	c_soundMgr.setMusicMute(doMute);
}

public static function setSFXVolume(vol:Number) {
	vol_sound = vol;
	c_soundMgr.setSFXVolume(vol);
}

public static function setMusicVolume(vol:Number) {
	vol_music = vol;
	c_soundMgr.setMusicVolume(vol);
}

public static function pauseAllSFX(b:Boolean) {
	c_soundMgr.pauseAllSFX(b);
}

public static function pauseMusic(b:Boolean) {
	c_soundMgr.pauseMusic(b);
}

public static function setHalfVolume(b:Boolean) {
	if (b)
	{
		setSFXVolume(MenuSystem_InGame._volSound / 200);     //MenuSystem volume are on 0-100 scale, soundMgr is on 0-1
		setMusicVolume(MenuSystem_InGame._volMusic / 200); // x/200 means - convert to 0-1, then take half
	}
	else
	{
		setSFXVolume(MenuSystem_InGame._volSound / 100);  //normal volume, just convert the scale
		setMusicVolume(MenuSystem_InGame._volMusic / 100);
	}
}


public static function pauseAudio(b:Boolean) {
	c_soundMgr.pauseAllSFX(b);
	c_soundMgr.pauseMusic(b);
}

public static function stopAllSFX()
{
	c_soundMgr.stopAllSFX();
}

public static function stopSFX(extID:int):Boolean
{
	return c_soundMgr.stopSFX(extID);
}

public static function startSFX(soundID:int, loop: Boolean = false) {
	return c_soundMgr.startSFX(soundID, loop);
}

public static function startMusicAt(position:int, soundID: int= -1, loop:Boolean = false) {
	return c_soundMgr.startMusicAt(position, soundID, loop);

}

public static function onSoundFinish(id:int) {
	switch (id)
	{
		case SoundLibrary.MUS_BATTLE_INTRO:
			startMusic(SoundLibrary.MUS_BATTLE, true);
			break;
	}
}

public static function stopMusic()
{
	c_soundMgr.stopMusic();
}

public static function startMusic(soundID:int,loop: Boolean):int
{
	return c_soundMgr.startMusic(soundID, loop);
	return 0;
}


  //Creates the state stack, creates the pause sprite, and other basic bootstrap functions


private void init()
{
	state = new DState();
	//stage.quality = MenuSystem_InGame._quality;

	startItUp();
	//addEventListener(Event.ENTER_FRAME, run);
}

private void startItUp()
{
	trace("Director.startItUp()");
	LevelProgress.initProgress();
	makeSoundMgr();
	makePauseSprite();
	initListeners();
	loadGameLevelInfo();
	showCinema(Cinema.SPLASH);
}

private void loadGameLevelInfo()
{
	d_lvlInfo = new GameLevelInfo(7);
	d_lvlProgress = new LevelProgress(7);
	LevelProgress.getProgress();
}

private void makeFPSCounter()
{
	c_fps = new FPSCounter(610, 0, 0xFFFFFF, true);
	addChild(c_fps);
}

private void fpsOnTop()
{
	if (c_fps)
	{
		setChildIndex(c_fps, numChildren - 1);
	}
}

private void initListeners()
{

	stage.addEventListener(KeyboardEvent.KEY_UP, listenKeyUp);
	stage.addEventListener(KeyboardEvent.KEY_DOWN, listenKeyDown);
	stage.addEventListener(Event.DEACTIVATE, listenDeactivate);

	//stage.addEventListener(MouseEvent.CLICK, listenClick);
	//stage.addEventListener(MouseEvent.MOUSE_MOVE, listenMouseMove);
	stage.addEventListener(MouseEvent.MOUSE_UP, listenMouseUp);
	stage.addEventListener(MouseEvent.MOUSE_DOWN, listenMouseDown);
	stage.addEventListener(Event.MOUSE_LEAVE, listenMouseLeave);
	//stage.addEventListener(MouseEvent.MOUSE_OUT, listenMouseOut);

	stage.addEventListener(MouseEvent.MOUSE_WHEEL, listenMouseWheel);
}

public function tempHighQuality()
{
	stage.quality = StageQuality.HIGH;
}

public function normalQuality()
{
	stage.quality = MenuSystem_InGame._quality;
}


 // Goes to the title screen. It pushes the TITLE state onto the stateStack and then draws it.


public function showTitle()
{
	stage.quality = StageQuality.HIGH;
	newState(DState.TITLE);
	makeTitle();
}


// Actually draws the title, attaches it and sets it up and everything


private void makeTitle()
{
	c_title = new Title();
	c_title.setDirector(this);
	addChild(c_title);
	fpsOnTop();
}


// Used just to hide the title. If you want to oldState() back to the title, you'd best show it again!


private void hideTitle()
{
	c_title.destruct();
	removeChild(c_title);
	c_title = null;
}

//Title functions

public function goPlayGame()
{
	if (Director.STATS_ON) { Log.Play(); }
	//fancyCursor();
	//remove the title screen
	hideTitle();

	newState(DState.INGAME); //we are now in the game state
							 //makeEngine();			 //create the engine and show it
	showMenu(MenuSystem.LEVELPICKER);
	fpsOnTop();
}

public function returnToGame()
{
	
	makeEngine();            //create the engine and show it
	fpsOnTop();
}

public function makeEngine()
{

	if (!c_engine)
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
	makeCursor();
}

public function nextLevel()
{//cinema:String) {
	endLevel(); //ends the engine but keeps us in the engine state

	if (Director.KONG_ON) { Director.kongregate.stats.submit("level_beaten_" + curr_level, 1); }
	if (Director.STATS_ON) { Log.LevelCounterMetric("beat_level", curr_level); }
	LevelProgress.setLevelBeaten(curr_level, true); //player gets credit for beating the level

	if (d_lvlInfo.maxLevel == curr_level)
	{
		if (Director.KONG_ON) { Director.kongregate.stats.submit("game_beaten", 1); }
		LevelProgress.setGameBeaten(true);
	}


	var success:Boolean = false;

//var cinema:int 




var cinema:int;
cinema = d_lvlInfo.getLvlCinema(curr_level);

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
		
		public function pickLevel(i:int) {
	curr_level = i;
	returnToGame();
}

public function onFinishFinale()
{

	trace("Director.onFinishFinale()");
	showCinema(Cinema.SCENE_CREDITS);
}

public function onFinishCredits()
{
	//showTitle();
	//oldState();
	switch (state.getTop())
	{
		case DState.INGAME:
			oldState(); //get out of the game, hopefully to the title
			onFinishCredits(); //do this again, hopefully should get us to the title
			break;
		case DState.INMENU:
			if (c_menu is MenuSystem_LevelPicker)
			{
				if (c_title)
				{
					c_title.onTitleStart();
				}
				MenuSystem_LevelPicker(c_menu).onTitle(null); break;
			}
		case DState.TITLE:
			if (c_title)
			{
				c_title.onTitleStart();
			}
			break;
			break;
	}
}

public function onFinishLevelCinema()
{ //when an in-between level cinema finishes, go to the next level
  //curr_level++;
  //returnToGame();
  //switch(
	showMenu(MenuSystem.LEVELPICKER);
}

public function resetGame()
{
	if (Director.STATS_ON) { Log.LevelAverageMetric("reset_level", curr_level, 1); }
	if (Director.STATS_ON) { Log.LevelCounterMetric("reset_level", curr_level); }
	removeChild(c_engine);
	c_engine.destruct();
	c_engine = null;
	makeEngine();
	fpsOnTop();
}

public function endLevel()
{
	removeChild(c_engine);
	c_engine.destruct();
	c_engine = null;
}

public function quitGame()
{
	if (Director.STATS_ON) { Log.LevelAverageMetric("quit_level", curr_level, 1); }
	if (Director.STATS_ON) { Log.LevelCounterMetric("quit_level", curr_level); }
	trace("Director.quitGame()!");
	removeChild(c_engine);
	c_engine.destruct();
	c_engine = null;
	oldState();
	//showMenu(MenuSystem.LEVELPICKER);
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

public function showCinema(i:int) {
	//normalCursor();

	stage.quality = StageQuality.HIGH;
	trace("Director.showCinema(" + i + ")");
	switch (state)
	{
		case DState.INGAME: pauseGame(); break;
	}

	newState(DState.CINEMA);
	c_cinema = null;
	var s:String;

	switch (i)
	{
		case Cinema.NOTHING: c_cinema = Cinema(new Cinema_Blank()); s = "nothing"; break;
		case Cinema.SPLASH: c_cinema = Cinema(new Cinema_Splash()); s = "splash"; break;
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
	}
	c_cinema.setIndex(i);
	c_cinema.setDirector(this);
	addChild(c_cinema);
	c_cinema.startCinema();
	fpsOnTop();

}

private void pauseGame()
{

	if (c_engine)
	{
		c_engine.pauseAnimate(true);

	}
}

private void unPauseGame()
{

	if (c_engine)
	{
		c_engine.pauseAnimate(false);
	}
}

//Cinema ceases play


private void pauseCinema()
{
	if (c_cinema)
		c_cinema.pause();
}


// Cinema resumes play


private void unPauseCinema()
{
	if (c_cinema)
		c_cinema.unPause();
}


// Called from WITHIN the Cinema movieclip. Terminates play and yields control to the director


public function onFinishCinema()
{
	var i:int = c_cinema.getIndex();
oldState(); //remove the cinema state
trace("Director.onFinishCinema() i=" + i + " state=" + state.getTop());
switch (state.getTop())
{

	case DState.INGAME:
		switch (i)
		{
			case Cinema.MITOSIS:
				unPauseGame();
				c_engine.finishMitosis();
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

	case DState.NOSTATE:
		switch (i)
		{
			case Cinema.SPLASH:
				showTitle(); break; //after the splash screen, show the title screen
		}
		break;

}
c_cinema.destruct();
removeChild(c_cinema);
c_cinema = null;
onCinemaFinished(i);
		}
		
		public function onCinemaFinished(i:int) {
	switch (i)
	{
		case Cinema.SCENE_FINALE:
			onFinishFinale();
			break;
		case Cinema.SCENE_CREDITS:
			onFinishCredits();
			break;
	}
}


//Show a specific menu
 // @param	i The index of the menu, found in MenuSystem's constants
 //

public function showMenu(i:int,params:*= null) {
	//normalCursor();


	contextPauseMenu();         //do something to respond to going into a menu
	newState(DState.INMENU);    //push INMENU onto the statestack

	switch (i)
	{                   //show the right menu
		case MenuSystem.INGAME:
			c_menu = MenuSystem(new MenuSystem_InGame());
			break;
		case MenuSystem.OPTIONS:
			//show the options menu	
			break;
		case MenuSystem.TUTORIAL:
			c_menu = MenuSystem(new MenuSystem_Tutorial(TutorialEntry(params)));
			break;
		case MenuSystem.HISTORY:
			c_menu = MenuSystem(new MenuSystem_History(TutorialArchive(params)));
			break;
		case MenuSystem.ENCYCLOPEDIA:
			stage.quality = StageQuality.HIGH;
			if (Director.STATS_ON) { Log.LevelCounterMetric("used_encyclopedia", curr_level); }
			if (Director.STATS_ON) { Log.LevelAverageMetric("used_encyclopedia", curr_level, 1); }
			if (Director.STATS_ON) { Log.CustomMetric("menu_open_encyclopedia", "menu"); }
			if (params)
						c_menu = MenuSystem(new MenuSystem_Encyclopedia(String(params)));
					else
				c_menu = MenuSystem(new MenuSystem_Encyclopedia());
			break;
		case MenuSystem.LEVELPICKER:
			c_menu = MenuSystem(new MenuSystem_LevelPicker());
			MenuSystem_LevelPicker(c_menu).setData(d_lvlProgress, d_lvlInfo);
			break;
		case MenuSystem.ENDLEVEL:
			c_menu = MenuSystem(new MenuSystem_EndLevel());
			MenuSystem_EndLevel(c_menu).setData(Number(params));
			break;
		case MenuSystem.REWARD:
			c_menu = MenuSystem(new MenuSystem_Reward());
			MenuSystem_Reward(c_menu).setData(WaveEntry(params));
			break;
		case MenuSystem.SCREWED:
			if (Director.STATS_ON) { Log.LevelCounterMetric("screwed", curr_level); }
			if (Director.STATS_ON) { Log.LevelAverageMetric("screwed", curr_level, 1); }
			if (Director.STATS_ON) { Log.CustomMetric("menu_open_screwed", "menu"); }
			c_menu = MenuSystem(new MenuSystem_Screwed());
			MenuSystem_Screwed(c_menu).setData((params as Array));
			break;
	}
	c_menu.setDirector(this);
	c_menu.setEngine(c_engine);
	addChild(c_menu);
	c_menu.setIndex(i);
	c_menu.init();
	fpsOnTop();
}

public function setExitMenuCode(i:int,param: int) {
	exit_menu_code = i;
	switch (i)
	{
		case MenuSystem.EXIT_PICK:
			picked_level = param; break;
		case MenuSystem.EXIT_TITLE:
			break;
	}
}

public function exitMenu()
{
	removeChild(c_menu);
	oldState();           //exit the menu state
	contextUnPauseMenu(); //You MUST unpause before telling the engine to respond! Bugzors otherwise!
	var i:int = c_menu.getIndex();

var reward_array:Array;
if (i == MenuSystem.REWARD)
{ //HACK
	reward_array = MenuSystem_Reward(c_menu).getRewards();
}

c_menu.destruct();
c_menu = null;

switch (i)
{
	case MenuSystem.TUTORIAL: c_engine.onExitTutorial(); break;
	case MenuSystem.HISTORY: c_engine.onExitHistory(); break;
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
	case MenuSystem.INGAME:
		if (state.getTop() == DState.INGAME)
		{
			c_engine.updateMuteButton();
		}
		break;
	case MenuSystem.REWARD:
		if (state.getTop() == DState.INGAME)
		{
			c_engine.getRewards(reward_array);
		}
		break;
	case MenuSystem.ENDLEVEL:
		if (state.getTop() == DState.INGAME)
		{
			c_engine.onEndLevelMenuClose();
		}
		break;
}
			
			
			
		}
		
		public function contextPauseMenu()
{
	//trace("Director.contextPauseMenu()");
	switch (state.getTop())
	{
		case DState.CINEMA: pauseCinema(); break;
		case DState.INGAME: pauseGame(); break; //donothing

	}
}

public function contextUnPauseMenu()
{
	//trace("Director.contextUnPauseMenu()");
	switch (state.getTop())
	{
		case DState.CINEMA: unPauseCinema(); break;
		case DState.INGAME: unPauseGame(); fancyCursor(); break; //donothing

	}
}

public function startFauxPause()
{
	if (state.getTop() != DState.FAUXPAUSED)
	{
		if (state.getTop() == DState.INGAME)
		{
			newState(DState.FAUXPAUSED);
			showPauseSprite("faux");
			releaseInput();
		}
	}
}

public function endFauxPause()
{
	if (state.getTop() == DState.FAUXPAUSED)
	{
		oldState();
		hidePauseSprite();
	}
}

public function toggleFauxPause()
{
	if (state.getTop() != DState.FAUXPAUSED)
	{
		if (state.getTop() == DState.INGAME)
		{ //can ONLY fauxpause from ingame!
		  //contextFauxPause();
			newState(DState.FAUXPAUSED);
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

public function pauseSpriteUnPause()
{
	if (state.getTop() == DState.PAUSED)
	{
		if (Director.STATS_ON) { Log.CustomMetric("unpause_game", "interface"); }
		oldState();       //ALWAYS return to the correct state first!
		contextUnPause(); //then, do the correct context action depending on which state we were in
		hidePauseSprite();
	}
}

public function keyPause()
{
	if (state.getTop() != DState.PAUSED)
	{
		if (Director.STATS_ON) { Log.CustomMetric("pause_game", "interface"); }
		if (Director.STATS_ON) { Log.LevelAverageMetric("pause_game", curr_level, 1); }
		contextPause(); //ALWAYS do the correct context action depending on which state we were in first!
		newState(DState.PAUSED); //then, we pause
		showPauseSprite();
		releaseInput();
	}
}


 // Pauses the game if not in paused state, otherwise unpauses
 //

public function togglePause()
{
	if (state.getTop() != DState.PAUSED)
	{
		if (Director.STATS_ON) { Log.CustomMetric("pause_game", "interface"); }
		if (Director.STATS_ON) { Log.LevelAverageMetric("pause_game", curr_level, 1); }
		contextPause(); //ALWAYS do the correct context action depending on which state we were in first!
		newState(DState.PAUSED); //then, we pause
		showPauseSprite();
		releaseInput();
	}
	else
	{
		if (Director.STATS_ON) { Log.CustomMetric("unpause_game", "interface"); }
		oldState();       //ALWAYS return to the correct state first!
		contextUnPause(); //then, do the correct context action depending on which state we were in
		hidePauseSprite();
	}
}

private void makeCursor()
{
	c_cursor = new Cursor();
	addChild(c_cursor);
	c_cursor.show();
	c_cursor.setEngine(c_engine);
	c_engine.receiveCursor(c_cursor);
	addEventListener(MouseEvent.MOUSE_MOVE, moveCursor);
}

public function moveCursor(m:MouseEvent) {
	c_cursor.followMouse(m);
	c_cursor.updateMoveCost();
}

public function normalCursor()
{
	if (c_cursor)
		c_cursor.normal(true);
}

public function fancyCursor()
{
	if (c_cursor)
		c_cursor.normal(false);
}

public function showCursor(i:int) {
	setChildIndex(c_cursor, numChildren - 1);
	var cursorShown:Boolean = c_cursor.show(i);
	
	c_engine.setSelectMode(!cursorShown); //if the cursor is shown, selectMode is false
}


// Makes the pause sprite and attaches it to the stage (it hides automatically until shown)


private void makePauseSprite()
{
	c_pauseSprite = new PauseSprite();
	addChild(c_pauseSprite);
	c_pauseSprite.setDirector(this);
	fpsOnTop();
}


private void showPauseSprite(whichFrame:String = "normal") {
	setChildIndex(c_pauseSprite, numChildren - 1);
	c_pauseSprite.show(whichFrame);
}

private void hidePauseSprite()
{
	c_pauseSprite.hide();
}

public function contextFauxPause()
{ //will ALWAYS be for ingame state
  //pauseGame();
}

public function contextUnFauxPause()
{
	//unPauseGame();
}

public function contextPause()
{
	switch (state.getTop())
	{
		case DState.CINEMA: pauseCinema(); break;
		case DState.INGAME: pauseGame(); break; //donothing
	}
}

public function contextUnPause()
{
	//trace("state = " + state.getTop());
	switch (state.getTop())
	{
		case DState.CINEMA: unPauseCinema(); break;
		case DState.INGAME: unPauseGame(); fancyCursor(); break; //donothing
	}
}


// Runs every frame, and gives functionality to the underlying classes


private void run() {
	bool halt = true;
	switch (state.getTop())
	{
		
		case DState.TITLE:
			//title.run();
			break;
		case DState.CINEMA:
			//cinema.run();
			break;
		case DState.INGAME:
			halt = false;
			//c_engine.run();
			c_engine.dispatchEvent(new RunFrameEvent(RunFrameEvent.RUNFRAME, null));

			break;
		case DState.FAUXPAUSED:
			c_engine.dispatchEvent(new RunFrameEvent(RunFrameEvent.FAUXFRAME, null));
			break;
		case DState.INMENU: break;
		//menuSystem.run();
		case DState.PAUSED: break;
		//do nothing
		case DState.NOSTATE: break;
	}
}

private void cheat(int i) {
	switch (state.getTop())
	{
		case DState.INGAME:
			c_engine.cheat(i);
			break;
		case DState.TITLE:
			d_lvlProgress.maxLevelBeaten(i);
			break;
	}
}

//INPUT STUFF

private void checkKeyUp(e:KeyboardEvent):Boolean
{
	switch (e.keyCode)
	{

		case CHEAT_0:
		case CHEAT_1:
		case CHEAT_2:
		case CHEAT_3:
		case CHEAT_4:
		case CHEAT_5:
		case CHEAT_6:
		case CHEAT_7:
		case CHEAT_8: cheat(e.keyCode); break;
		case CHEAT_9: LevelProgress.clearProgress(); break;


		case KEY_PAUSE: //donothing
			break;
		case KEY_DOWN:
		case KEY_DOWN2:
			release_arrow(1);
			break;
		case KEY_UP:
		case KEY_UP2:
			release_arrow(0);
			break;
		case KEY_LEFT:
		case KEY_LEFT2:
			release_arrow(2);
			break;
		case KEY_RIGHT:
		case KEY_RIGHT2:
			release_arrow(3);
			break;
		default:
			return true;            //I don't know what to do with this, pass it on
			break;

	}
	return false;
}

private void checkKeyDown(e:KeyboardEvent):Boolean
{
	switch (e.keyCode)
	{
		case KEY_PAUSE:
			keyPause();
			return false;
			break;
		case KEY_DOWN:
		case KEY_DOWN2:
			press_arrow(1);
			break;
		case KEY_UP:
		case KEY_UP2:
			press_arrow(0);
			break;
		case KEY_LEFT:
		case KEY_LEFT2:
			press_arrow(2);
			break;
		case KEY_RIGHT:
		case KEY_RIGHT2:
			press_arrow(3);
			break;
		default:
			return true;            //I don't know what to do with this, pass it on
			break;
	}
	return false;
}

private void press_arrow(i:int) {
	list_moveKeys[i] = true;
}
private void release_arrow(i:int) {
	list_moveKeys[i] = false;
}
private void clearKeys()
{
	for (var i:int = 0; i < list_moveKeys.length; i++) {
	list_moveKeys[i] = false;
}
		}
		
		public function getArrow(i:int):Boolean
{
	return list_moveKeys[i];
}

private void releaseInput()
{
	c_engine.dispatchEvent(new MouseEvent(MouseEvent.MOUSE_UP));
}

private void listenDeactivate(e:Event) {

	clearKeys(); //when the player deactivates, we have no idea what's down, so we assume they were all lifted
}



private void listenMouseWheel(m:MouseEvent) {
	if (m.delta > 0)
	{
		c_engine.mouseWheel(1);
	}
	else
	{
		c_engine.mouseWheel(-1);
	}
}

private void listenMouseUp(m:MouseEvent) {
	//trace("Director.listenMouseUp()");
	IS_MOUSE_DOWN = false;

	//filthy hack:
	if (c_engine)
	{
		c_engine.mouseUp(m);
	}
}


private void listenMouseDown(m:MouseEvent) {
	//trace("Director.listenMouseDown()");
	IS_MOUSE_DOWN = true;
}

private void listenMouseLeave(e:Event) {
	//trace("Director.listenMouseLeave()");
	IS_MOUSE_DOWN = false;
}

private void listenKeyUp(e:KeyboardEvent) {
	if (checkKeyUp(e))
	{
		switch (state.getTop())
		{
			case DState.TITLE:

				break;
			case DState.CINEMA:

				break;
			case DState.INGAME:
				c_engine.checkKeys();
				break;
			case DState.INMENU:

				break;
			case DState.PAUSED:
				break;
		}
	}
}



private void listenKeyDown(e:KeyboardEvent) {
	if (checkKeyDown(e))
	{           //if there was a key that got through our filter
		switch (state.getTop())
		{
			case DState.TITLE:

				break;
			case DState.CINEMA:

				break;
			case DState.INGAME:
				break;
			case DState.INMENU:

				break;
			case DState.PAUSED:
				break;
		}
	}
}


private void listenMouseClick(e:MouseEvent) {
	switch (state.getTop())
	{
		case DState.TITLE:
			c_title.dispatchEvent(e);
			break;
		case DState.CINEMA:
			c_cinema.dispatchEvent(e);
			break;
		case DState.INGAME:
			c_engine.dispatchEvent(e);
			break;
		case DState.INMENU:
			c_menu.dispatchEvent(e);
			break;
		case DState.PAUSED:
			break;
	}
}

private void listenMouseMove(e:MouseEvent) {
	switch (state.getTop())
	{
		case DState.TITLE:

			break;
		case DState.CINEMA:

			break;
		case DState.INGAME:

			c_engine.mouseMove(e);
			//c_engine.dispatchEvent(e);
			break;
		case DState.INMENU:

			break;
		case DState.PAUSED:
			break;
	}
}


//*********STATE STACK STUFF***************


//Sets the new state, initializing the stack
// @param	s


private void setState(s:int) {
	state.setState(s);
}


// Goes to a new state, shoves the old ones back in the stack
 // @param	s the new state code to go to
 

private void newState(s:int) {
	//trace("newState=" + s);
	state.push(s);  //push the state on
					//state.traceStack();
}


// Returns to the last state, throws away the current one


private void oldState()
{
	var prev:int = state.pop();
switch (state.getTop())
{
	case DState.CINEMA: break;
	case DState.INGAME: break;
	case DState.INMENU: break;
	case DState.PAUSED: break;
	case DState.FAUXPAUSED: break;
	case DState.TITLE:
		if (prev != DState.PAUSED)
		{ //if we were paused, we haven't hidden the title
			if (!c_title)
			{ //don't do this if the title already exists
				makeTitle(); //be sure to show the title again, BUT DONT push the state, we're already there!
			}
		}
		break;

}
			//state.traceStack();
		}
	*/
}
