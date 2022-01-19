using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//	import com.pecSound.SoundLibrary; //TODO

public class EncyclopediaBook : MonoBehaviour
{
	public Button c_butt;
	public MovieClip anim;
		//public var p_tutGlass:TutorialGlass;   //TODO
    // Start is called before the first frame update
    void Start()
    {
        
    }
	
		
	/*  //TODO

	public function init(p_tut:TutorialGlass)
	{
		c_butt.addEventListener(MouseEvent.CLICK, onClick, false, 0, true);
		anim.addEventListener(MouseEvent.CLICK, onClick, false, 0, true);
		p_tutGlass = p_tut;
	}

	public function destruct()
	{
		removeEventListener(MouseEvent.CLICK, onClick);
		removeEventListener(MouseEvent.CLICK, onClick);
	}

	public function newEntry()
	{
		anim.play();
		Director.startSFX(SoundLibrary.SFX_WRITING);
	}

	public function onClick(m:MouseEvent)
	{
		p_tutGlass.onBookClick();
	}*/
}
