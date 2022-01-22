using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSprite : MonoBehaviour
{
    public Image Pauser;
	private Director p_director;
    // Start is called before the first frame update
    void Start()
    {
		hide();
    }
	public void onClick()
	{
		p_director.pauseSpriteUnPause();
	}

	public void setDirector(Director d)
	{
		p_director = d;
	}

	public void fancyCursor()
	{
		//p_director.fancyCursor();  //TODO
	}

	public void normalCursor()
	{
		//p_director.normalCursor(); //TODO
	}

	public void show(string whichFrame = "normal")
	{
		this.gameObject.SetActive(true);
		//gotoAndStop(whichFrame);  //TODO? Not sure what the point of having a "faux" state of the pause image is.
		
	}

	public void hide()
	{
		this.gameObject.SetActive(false);
		
	}

	private void doNothing()
	{
		
	}
}
