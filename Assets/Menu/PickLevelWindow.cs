using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickLevelWindow : MonoBehaviour
{
	public Text c_head;
	public Text c_title;
	public Button butt_okay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	
		
		

	private void onClick()
	{
		//MenuSystem_LevelPicker(parent).onClickOkay(m);  //TODO
	}

	public void setTitle(string t)
	{
		c_title.text = t;
	}
}
