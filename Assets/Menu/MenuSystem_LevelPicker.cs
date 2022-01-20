using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSystem_LevelPicker : MenuSystem, IConfirmCaller
{
	public LevelDot lvl_0;
		public LevelDot lvl_1;
		public LevelDot lvl_2;
		public LevelDot lvl_3;
		public LevelDot lvl_4;
		public LevelDot lvl_5;
		public LevelDot lvl_6;
		public LevelDot lvl_7;
		
		public LevelDot selected_dot;
		
		public CameraButt cin_0:;
		public CameraButt cin_1;
		public CameraButt cin_2;
		public CameraButt cin_3;
		public CameraButt cin_4;
		public CameraButt cin_5;
		public CameraButt cin_6;
		public CameraButt cin_7;
		
		public var c_lines:MovieClip;
		public var c_window:PickLevelWindow;
		/*public var c_window:PickLevelWindow;
		public var c_currentlevel:Sprite;
		public var c_title:TextField;*/
		
		//public var butt_okay:SimpleButton;
		
		public var d_lvlInfo:GameLevelInfo;
		public var d_lvlProgress:LevelProgress;
		
		private var level_pick:int = 0;
		private var maxBeaten:int = -1;
		
		private const MAX_LEVEL:int = 7;
		
		public var butt_title:SimpleButton;
		public var butt_clear:SimpleButton;
		public var c_clickhere:MovieClip;
		public var exitCode:int = EXIT_TITLE;
		
		public var c_confirm:Confirmation;
		
		public static var startImmediately:Boolean = true; //HACK
    public void confirm(string s)
    {
        
    }

    public void onConfirm(string s, bool b)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
