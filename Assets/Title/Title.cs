using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title : MonoBehaviour
{
    //Notes: The logging and stat recording system is obviously deprecated, but I marked down todo?s to remind myself to consider 
    //a replacement of some kind when everything else is done. 

    //Microscope slides in from left,
    ////Options button from right
    /////version number upper right 
    ///CellFraftGame siote button moves from lower right to middle-ish
    ///PlayGame button, Credits Button, Encyclopedia button, all move upward and rearrange
    ///Sponsor Button appears in upper right-ish near end
    //Menubar slides up and turns into a square to give backdrop to text buttons
    //Title Content / Cellsplat Wiggle (lotta stuff)
    //|
    //|
    ///|
    ///|
    ///|
    ///Backgroundwater 

    public Director director;
    public GameObject BGWater;
    public GameObject MenuBar;
    public GameObject Microscope;
		
	public Button buttPlayGame;
	public Button buttCredits;
	public Button buttEncyclopedia;
	public Button buttCellcraft;
    public MovieClip CellSplat;
		
	//public var sponsor:SponsorLogoLive;  //TODO
	public Button c_butt_mute; //TODO
	public Text version_txt;



    
    
    // Start is called before the first frame update
    void Start()
    {
        submitStats();
        CellSplat.SetFrameInterval((float)1/24);
        CellSplat.Play();
        float time = (float)12 / 24;
        BGWater.transform.DOScale(2.5f, time);
        MenuBar.transform.DOScaleX(.3f, time);
        MenuBar.transform.DOScaleY(10f, time);
        MenuBar.transform.DOMoveY(-1.7f, time);
        Microscope.transform.DOMoveX(-1.76f, time);
        Microscope.transform.DOScale(0.76f, time);
        buttPlayGame.transform.DOMoveX(0, time/2).SetDelay(0.5f).SetEase(Ease.InOutElastic);
        buttEncyclopedia.transform.DOMoveX(0, time/2).SetDelay(0.5f).SetEase(Ease.OutBounce);
        buttCredits.transform.DOMoveX(0, time/2).SetDelay(0.5f).SetEase(Ease.OutBounce);
    }

    private void submitStats()
    {
        /*
        var max_level:int = LevelProgress.getMaxLevelBeaten();
        for (var i:int = 0; i <= max_level; i++) {
            if (LevelProgress.getLevelBeaten(i))
            {
                if (Director.KONG_ON) { Director.kongregate.stats.submit("level_beaten_" + i, 1); }
                var time:int = LevelProgress.getLevelTime(i);
                var grade:int = LevelProgress.getLevelGrade(i);
                if (time != LevelProgress.FOREVER) { if (Director.KONG_ON) { Director.kongregate.stats.submit("level_seconds_" + i, time); } }
                if (grade != -1) { if (Director.KONG_ON) { Director.kongregate.stats.submit("level_grade_" + i, grade); } }
            }
        }
        if (LevelProgress.getGameBeaten())
        {
            if (Director.KONG_ON) { Director.kongregate.stats.submit("game_beaten", 1)};
        }*/ //TODO?
    }

    public void setDirector(Director d)
    {
        director = d;
    }

    public void onStartRoutineFinish()
    { //when the animation finishes
        setupButtons();
    }

    private void setupButtons()
    {
       /* if (sponsor)
        {
            sponsor.id = "title";
        }*/ //TODO?

       

       // buttPlayGame.addEventListener(MouseEvent.CLICK, goPlayGame, false, 0, true);
       // buttCredits.addEventListener(MouseEvent.CLICK, goCredits, false, 0, true);
       // buttCellcraft.addEventListener(MouseEvent.CLICK, goCellcraft, false, 0, true);

       

        //buttEncyclopedia.addEventListener(MouseEvent.CLICK, goEncyclopedia, false, 0, true);

        /*if (MenuSystem_InGame._muteMusic && MenuSystem_InGame._muteSound)
        {
            c_butt_mute.setIsUp(false);
        }
        else
        {
            c_butt_mute.setIsUp(true);
        }*/  //TODO?
    }

    private void goOptions()
    {
        Debug.Log("Title: goOptions");
    }

    public void goPlayGame()
    {
        if (Director.STATS_ON) 
        { 
            //Log.CustomMetric("title_play_game", "title");  //TODO?
          }
        Debug.Log("Title: goPlayGame");
        //director.goPlayGame();  //TODO!
    }

    public void goCredits()
    {
        if (Director.STATS_ON) 
        { 
        //    Log.CustomMetric("title_show_credits", "title"); //TODO?
        }
        Debug.Log("Title: goCredits");
        //director.showCinema(Cinema.SCENE_CREDITS); //TODO
    }

    public void goEncyclopedia()
    {
        if (Director.STATS_ON) 
        { 
            //Log.CustomMetric("title_show_encyclopedia", "title"); //TODO?
        }
        if (Director.STATS_ON) 
        { 
            //Log.CustomMetric("encyclopedia_open_title", "encyclopedia"); //TODO?
        }
        Debug.Log("Title: goEncyclopedia");
        //director.showMenu(MenuSystem.ENCYCLOPEDIA, "root"); //TODO
    }

    public void goCellcraft()
    {
        if (Director.STATS_ON) 
        { 
        //    Log.CustomMetric("click_cellcraft_site", "title"); //TODO?
        }
       // var url:String = "http://www.cellcraftgame.com";
       // openURL(url);
    }

   


}
