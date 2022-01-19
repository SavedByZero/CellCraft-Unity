using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Confirmation : MonoBehaviour
{
	private IConfirmCaller caller;
	private string command;
		
	public Button butt_yes;
	public Button butt_no;
		
	void Start()
	{
		hide();
	}

	public void confirm(IConfirmCaller call, string comm)
	{
		caller = call;
		command = comm;
		show();
		//GotoAndStop(command);  //TODO - replace
	}

	public void onYes()//(m:MouseEvent)
	{
		if (caller != null)
			caller.onConfirm(command, true);
		hide();
	}

	public void onNo()//(m:MouseEvent)
	{
		if (caller != null)
			caller.onConfirm(command, false);
		hide();
	}

	private void hide()
	{
		caller = null;
		command = "";
		this.gameObject.SetActive(false);
		//butt_yes.removeEventListener(MouseEvent.CLICK, onYes);
		//butt_no.removeEventListener(MouseEvent.CLICK, onNo);
	}

	private void show()
	{
		this.gameObject.SetActive(true);
		//butt_yes.addEventListener(MouseEvent.CLICK, onYes, false, 0, true);
		//butt_no.addEventListener(MouseEvent.CLICK, onNo, false, 0, true);
	}
	
}
