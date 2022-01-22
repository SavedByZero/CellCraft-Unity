using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuSystem_LevelPicker : MenuSystem, IConfirmCaller
{
	/*public LevelDot lvl_0;
		public LevelDot lvl_1;
		public LevelDot lvl_2;
		public LevelDot lvl_3;
		public LevelDot lvl_4;
		public LevelDot lvl_5;
		public LevelDot lvl_6;
		public LevelDot lvl_7;*/
	LevelDot[] levelDots;
	public LevelDot selected_dot;
	/*	
		public CameraButt cin_0;
		public CameraButt cin_1;
		public CameraButt cin_2;
		public CameraButt cin_3;
		public CameraButt cin_4;
		public CameraButt cin_5;
		public CameraButt cin_6;
		public CameraButt cin_7;*/
	CameraButt[] cbuttons;
	public float[] LinePercents;
	public Image c_lines;
		public PickLevelWindow c_window;
		/*public var c_window:PickLevelWindow;
		public var c_currentlevel:Sprite;
		public var c_title:TextField;*/
		
		//public var butt_okay:SimpleButton;
		
		public GameLevelInfo d_lvlInfo;
		public LevelProgress d_lvlProgress;
		
		private int level_pick = 0;
		private int maxBeaten = -1;
		
		private int MAX_LEVEL = 7;
		
		public Button butt_title;
		public Button butt_clear;
		public Image c_clickhere;
		public int exitCode = EXIT_TITLE;
		
		public Confirmation c_confirm;
		
		public static bool startImmediately = true; //HACK
    public void confirm(string s)
    {
		c_confirm.confirm(this, s);
	}

    public void onConfirm(string s, bool b)
    {
		if (b)
		{
			if (s == "clear")
			{
				doClear();
			}
		}
	}

	public MenuSystem_LevelPicker()
	{
		//butt_title.addEventListener(MouseEvent.CLICK, onTitle, false, 0, true);
		//butt_clear.addEventListener(MouseEvent.CLICK, onClear, false, 0, true);
	}

	void Start()
    {
		
		init();
    }

	public override void init()
	{
		base.init();
	
		levelDots = GetComponentsInChildren<LevelDot>();//new LevelDot[] { lvl_0, lvl_1, lvl_2, lvl_3, lvl_4, lvl_5, lvl_6, lvl_7 };
		cbuttons = GetComponentsInChildren<CameraButt>();//new CameraButt[] { cin_0, cin_1, cin_2, cin_3, cin_4, cin_5, cin_6, cin_7 };
		c_window.gameObject.SetActive(false);
		//butt_okay.visible = false;
		//c_currentlevel.visible = false;			
		//butt_okay.addEventListener(MouseEvent.CLICK, onClickOkay, false, 0, true);

		if (maxBeaten > MAX_LEVEL)
		{
			maxBeaten = MAX_LEVEL;
		}

		updateAll();
		showLevelLineFor(maxBeaten);
		if (maxBeaten == -1)
		{
			level_pick = 0;
			if (startImmediately)
			{
				onClickOkay();
			}
			else
			{
				startImmediately = true;
			}
		}
	}

	public void onClear()
	{
		confirm("clear");
	}

	public void doClear()
	{
		LevelProgress.clearProgress();
		//p_director.setExitMenuCode(EXIT_RESET, 0);  //TODO
		startImmediately = false; //HACK 
								  //Normally, when you have 0 progress and start the levelpicker, it dumps you 
								  //straight into level 0. However, if you clear the data, you don't want that.
								  //so what this does, is ONLY if you have pressed clear do you get to see level 0 and
								  //only level 0
		exit();
	}

	public void onTitle()
	{
		//exitCode = EXIT_TITLE;
		//p_director.setExitMenuCode(EXIT_TITLE, 0); //TODO
		exit();
	}

	public void onClickCinema(int id)
	{
		//trace("MenuSystem_LevelPicker.onClickCinema id=" + id + " cinema=" + d_lvlInfo.getLvlCinema(id));
		//p_director.showCinema(d_lvlInfo.getLvlCinema(id));  //TODO
	}

	private void setupButtons()
	{
		
		for (int i = 0; i <= d_lvlInfo.maxLevel; i++) {
			cbuttons[i].id = i;
		}
	}

	public void setData(LevelProgress l, GameLevelInfo g)
	{
		d_lvlInfo = g;
		d_lvlProgress = l;
		setupButtons();
		maxBeaten = -1;
		for (int i = 0; i <= d_lvlInfo.maxLevel; i++) {
			if (LevelProgress.getLevelBeaten(i))
			{
				//trace("MenuSystem_LevelPicker.levelBeaten : " + i);
				maxBeaten = i;
			}
		}
	}

	public void updateAll()
	{
		
		for (int i = 0; i <= d_lvlInfo.maxLevel; i++) {
			LevelDot l = levelDots[i];
			l.setMaster(this);
			l.setup(i, d_lvlInfo.getLvlLabel(i), d_lvlInfo.getLvlDesignation(i), d_lvlInfo.getLvlName(i));

			if (LevelProgress.getLevelBeaten(i))
			{
				l.checkMe();
			}
			else
			{
				l.checkMe(false);
			}

			showCinemas(false);
			

		}
	}

	/**
		 * Give me a level number, unlock that level
		 * @param	i
		 */

	public void showLevelLineFor(int i)
	{
		showLevels(false);

		for (int j = 0; j <= i + 1; j++) { //show all the levels & cinemas in between
			showLevel(j, true);
			int c = j - 2;
			if (c >= 0)
			{
				if (d_lvlInfo.getLvlCinema(c) != Cinema.NOTHING)
				{
					//trace("MenuSystem_LevelPicker.showLevelLineFor() cinema SHOW " + (c) + " : " + d_lvlInfo.getLvlCinema(c));
					showCinema(c);
				}
				else
				{
					//trace("MenuSystem_LevelPicker.showLevelLineFor() cinema HIDE " + i + " : " + d_lvlInfo.getLvlCinema(c));
					showCinema(c, false);
				}
			}
			//showCinema(j, true);
		}

		if (i == -1)
		{
			lineAt(0);
		}
		else
		{
			SfxManager.Play(SFX.SFXBlipSteps);//Director.startSFX(SoundLibrary.SFX_BLIPSTEPS);
			lineAtAndPlay(i);
		}
	}

	private void tryShowCinema(int i)
	{
		if (i >= 0)
		{
			if (d_lvlInfo.getLvlCinema(i) != Cinema.NOTHING)
			{
				showCinema(i);
			}
		}
	}

	public void onEndLine()
	{
		//trace("MenuSystem_LevelPicker.onEndLine() level_pick = " + level_pick);
		if (level_pick <= MAX_LEVEL)
		{
			onSelectLevel(levelDots[level_pick]);
			tryShowCinema(level_pick - 1);

		}
		else if (level_pick >= MAX_LEVEL)
		{
			//trace("MenuSystem_LevelPicker.onEndLine() MAXED level_pick = " + level_pick);
			tryShowCinema(MAX_LEVEL);
		}
	}

	public void onSelectLevel(LevelDot l)
	{
		unselectLevels();
		l.selectMe();
		if (level_pick == l.id)
		{
			c_clickhere.gameObject.SetActive(true);
		}
		level_pick = l.id;
		selected_dot = l;
		//trace("MenuSystem_LevelPicker.onSelectLevel() " + level_pick);
		c_window.gameObject.SetActive(true);
		c_window.setTitle(l.title);
		/*c_window.x = l.x;
		c_window.y = l.y;*/
		//c_title.text = l.title;
		//butt_okay.visible = true;
		//c_currentlevel.visible = true;
	}

	public void onDoubleClickLevel(LevelDot l)
	{
		onSelectLevel(l);
		onClickOkay();
	}

	public int getLevelPick() 
	{
			return level_pick;
	}

	public void onClickOkay()
	{
		
		//p_director.setExitMenuCode(EXIT_PICK, level_pick);  //TODO
		exit();
	}

	private void unselectLevels()
	{
		for (int i = 0; i <= d_lvlInfo.maxLevel; i++) {
			levelDots[i].selectMe(false);
		}
	}

	private void showLevel(int i, bool b = true)
	{
		if (i <= MAX_LEVEL)
			levelDots[i].gameObject.SetActive(b);
	}

	private void showCinema(int i, bool b = true)
	{
		//trace("MenuSystem_LevelPicker.showCinema(" + i + ")");
		if (i <= MAX_LEVEL)
			cbuttons[i].gameObject.SetActive(b);
	}

	private void lineAt(int i)
	{
		level_pick = i;
		c_lines.DOFillAmount(LinePercents[i],0.25f);
		//c_lines.GotoAndStop("at_lvl_" + i);  //Note: onEndLine() should be called whenever the line reaches its destination 
		onEndLine();
	}

	private void lineAtAndPlay(int i)
	{
		level_pick = i + 1;
		c_lines.fillAmount = LinePercents[i];
		c_lines.DOFillAmount(LinePercents[i + 1], 1).OnComplete(new TweenCallback(delegate
		   {
			   onEndLine();
		   }));
		//c_lines.GotoAndStop("at_lvl_" + i);
		//c_lines.Play();
	}

	private void showLevels(bool b= true)
	{
		
		for (int i = 0; i < levelDots.Length; i++)
			levelDots[i].gameObject.SetActive(b);
	}

	private void showCinemas(bool b = true)
	{
		
		for (int i = 0; i < cbuttons.Length; i++)
			cbuttons[i].gameObject.SetActive(b);
	}




}
