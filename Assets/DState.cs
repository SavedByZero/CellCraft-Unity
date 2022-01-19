using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DState 
{
    Stack<GameState> _stateStack;
	//private var stack:Vector.<int>;
		
	
		
	public DState()
	{
		_stateStack = new Stack<GameState>();
			_stateStack.Push(GameState.NOSTATE);
	}

	public void traceStack()
	{
		GameState[] stackArray = _stateStack.ToArray();
			string s = "StateStack = ";
			for (int i = 0; i < stackArray.Length; i++) {
				s += stateToString(stackArray[i] ) + " ,";
			}
			Debug.Log(s);
	}

	public string stateToString(GameState i) 
	{
				
			return (int)i + ":" + i.ToString();
	}
	
	public void quickTest()
	{
		Debug.Log("stack was = " + _stateStack);
		_stateStack.Push(GameState.TITLE);
		_stateStack.Push(GameState.INGAME);
		_stateStack.Push(GameState.INMENU);
		_stateStack.Push(GameState.PAUSED);
		_stateStack.Pop();
		_stateStack.Pop();
		_stateStack.Pop();
		_stateStack.Push(GameState.CINEMA);
		Debug.Log("stack = " + _stateStack);
	}

	//Note: I'm not clear what the purpose of this is -- it seems to be replacing a stack of 1 with a new value? 
	//Will translate literally for now.  
	public void setState(GameState i) 
	{
		if (_stateStack.Count == 1)
		{
			if (i >= GameState.PRELOAD && i <= GameState.CINEMA)
			{
				_stateStack.Clear();
				_stateStack.Push(i);
			}
		}
	}

	public GameState getTop()
	{
		if (_stateStack.Count >= 1)
			return _stateStack.Peek();
		
		return GameState.NOSTATE;
	}

	public GameState getPrev()
	{
		if (_stateStack.Count >= 2)
		{
			GameState[] stackArray = _stateStack.ToArray();
			return stackArray[stackArray.Length-2];
		}
		else
		{
			return GameState.NOSTATE;
		}
	}
}
		
public enum GameState
{
	 NOSTATE = -1, //error or null state
	PRELOAD = 0,
	TITLE = 1, //the title screen
	INGAME = 2, //gameplay
	PAUSED = 3, //the game is paused
	FAUXPAUSED = 4, //the game is paused, but we still let certain things run. Used for in-engine cinematics, etc
	INMENU = 5, //in-game menu
	CINEMA = 6 //we're in a cutscene
}
