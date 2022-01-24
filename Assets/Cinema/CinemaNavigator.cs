using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinemaNavigator : MonoBehaviour, IConfirmCaller
{
	// Start is called before the first frame update


	public Button nextButt;
	public Button backButt;
	public Button replayButt;
	public Button skipButt;
	public Button pauseButt;
	public Cinema[] Cinemas;
	private int count = 1;
	private bool atHalt = false;
	private bool doNext = false;
	public Button c_butt_mute;//ToggleButton;
		
	private bool paused = false;
	public int myCinemaIndex = 0;
		
	public Confirmation c_confirm;


	void Start()
	{

		if (backButt != null)
		{
			//replayButt.addEventListener(MouseEvent.CLICK, replayCinema, false, 0, true);
			//backButt.addEventListener(MouseEvent.CLICK, backCinema, false, 0, true);
			//nextButt.addEventListener(MouseEvent.CLICK, nextCinema, false, 0, true);
			//skipButt.addEventListener(MouseEvent.CLICK, skipCinema, false, 0, true);
			//pauseButt.addEventListener(MouseEvent.CLICK, pauseCinema, false, 0, true);
			pauseButt.gameObject.SetActive(false);
			replayButt.gameObject.SetActive(false);
			nextButt.gameObject.SetActive(false);
			backButt.gameObject.SetActive(false);
			skipButt.gameObject.SetActive(false);
		}


		if (myCinemaIndex == Cinema.SCENE_CREDITS)
		{
			pauseButt.gameObject.SetActive(true);
			replayButt.gameObject.SetActive(true);
			skipButt.gameObject.SetActive(true);
			nextButt.gameObject.SetActive(false);
			if (backButt != null)
			{
				backButt.gameObject.SetActive(false);
			}
		}
	}

	public void setIndex(int i)
	{
		myCinemaIndex = i;
	}

	public void confirm(string s)
	{
		c_confirm.confirm(this, s);
		c_confirm.gameObject.transform.SetSiblingIndex(this.transform.childCount - 1);
		//setChildIndex(c_confirm, numChildren - 1);
		doPause(true);
	}

	public void onConfirm(string s, bool b)
	{
		//trace("CinemaNavigator.onConfirm("+s + "," + b + ")");
		if (b)
		{
			if (s == "skip")
			{
				doSkip();
			}
		}
		else
		{
			doPause(false);
		}
	}


	public void showNext()
	{
		nextButt.gameObject.SetActive(true);
	}

	public void replayCinema()
	{
		//if (Director.STATS_ON) { Log.CustomMetric("cinema_replay_" + Cinema.getName(myCinemaIndex), "cinema"); }

		//TODO: this logic may not translate, as this was originally a mouse click handler, which, in AS3, ends up being separate from the calling class.
		//make sure this is finding the current cinema, whatever that is, and replaying it.
		Cinemas[myCinemaIndex].Play();
	}

	public void readyCinema()
	{
		skipButt.gameObject.SetActive(true);
	}

	public void forFinalCinema()
	{
		//trace("CinemaNavigator.forFinalCinema()!");
		skipButt.gameObject.SetActive(true);
		nextButt.gameObject.SetActive(false);
		replayButt.gameObject.SetActive(true);
		c_butt_mute.gameObject.SetActive(true);
		pauseButt.gameObject.SetActive(true);
	}

	public void haltForFinalCinema()
	{
		//trace("CinemaNavigator.haltForFinalCinema()!");
		halt();
		backButt.gameObject.SetActive(false);
		pauseButt.gameObject.SetActive(false);
	}

	public void halt()
	{
		count++;
		if (doNext)
		{
			nextButt.gameObject.SetActive(false);
			//TODO: 
			//make sure this is finding the current cinema, whatever that is, and replaying it.
			
			Cinemas[myCinemaIndex].replay();
			doNext = false;
		}
		else
		{
			atHalt = true;
			nextButt.gameObject.SetActive(true);
			backButt.gameObject.SetActive(true);
		}

	}

	private void doPause(bool b)
	{
		paused = b;
		if (b)
		{
			Cinemas[myCinemaIndex].pause();
		}
		else
		{
			Cinemas[myCinemaIndex].unPause();
		}
	}

	public void pauseCinema()
	{
		if (paused)
		{
			paused = false;
			Cinemas[myCinemaIndex].pause();
		}
		else
		{
			paused = true;
			Cinemas[myCinemaIndex].unPause();
		}
	}

	public void backCinema()
	{
		backButt.gameObject.SetActive(false);
		nextButt.gameObject.SetActive(false);

		if (atHalt)
		{ //halt advances count by 1. So if we're at a halt, we have to go back 2 counts!
			count -= 2;
		}
		else
		{         //if not at a halt, only go back 1.
			count--;
		}

		Cinemas[myCinemaIndex].GotoAndStop("a" + count);

		if (count == 0)
		{ //if we went all the way back to the beginning
			count = 1;             //the minimum value is 1
			Cinemas[myCinemaIndex].Play(); //to avoid getting stuck, there's no halt at the beginning!
		}
	}

	public void nextCinema()
	{
		nextButt.gameObject.SetActive(false);
		backButt.gameObject.SetActive(false);

		if (atHalt)
		{
			Cinemas[myCinemaIndex].Play();
		}
		else
		{
			doNext = true;
			Cinemas[myCinemaIndex].GotoAndPlay("a" + count);
		}
	}

	public void skipCinema()
	{
		//trace("CinemaNavigator.skipCinema() index = " + myCinemaIndex);
		if (myCinemaIndex != Cinema.SPLASH)
		{
			//trace("CONFIRM SKIP");
			confirm("skip");
		}
		else
		{
			//trace(" DO SKIP");
			doSkip();
		}
	}

	private void doSkip()
	{
		Cinemas[myCinemaIndex].finishCinema();
	}
}
