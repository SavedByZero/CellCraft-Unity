using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGameInterface : MonoBehaviour
{

	//pointers:
	public Engine p_engine;
	public Director p_director;
		
		//children:
		/*
	public Zoomer c_zoomer;
		
	public var c_quantPanel:QuantPanel;
		
	public var c_tt:Tooltip;
		
	public var c_tutorialGlass:TutorialGlass;
		//public var c_discovery:DiscoveryScore;
	public var c_membraneHealth:MembraneHealth;
	public var c_daughterCells:DaughterCells;
	public var c_ph:PH;
	public var c_sunlight:Sunlight;
	public var c_toxinLevel:ToxinLevel;
		
	public var c_resource_atp:Resource_atp;
	public var c_resource_na:Resource_na;
	public var c_resource_aa:Resource_aa;
	public var c_resource_fa:Resource_fa;
	public var c_resource_g:Resource_g;
		
	public var c_newThing:NewThingMenu;
	//public var c_centerButt:SimpleButton;
	public var c_followButt:ToggleButton;
	public var c_menuButt:SimpleButton;
	public var c_pauseButt:SimpleButton;
		
	public var c_selectedPanel:SelectedPanel;
	public var c_actionMenu:ActionMenu;
	public var c_messageBar:MessageBar;
		
	private static var exists:Boolean = false;
		
	private const ACTIONMENU_X:Number = 260;
	private const ACTIONMENU_Y:Number = 460;
		
	private var anim_on:Boolean = true;
		
	public static const QUANT_RIBO:int = 0;
	public static const QUANT_LYSO:int = 1;
	public static const QUANT_PEROX:int = 2;
	public static const QUANT_SLICER:int = 3;
		
	private const WHEEL_ZOOM_CLICKS:int = 2;
		*/
		
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	/****Respond to stuff******/

	public void changeZoom(float n)
	{
		p_engine.changeZoom(n);
	}
}
