using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBubble : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

	public MovieClip icon;
		
	

	public void setIcon(string s)
	{
		icon.GotoAndStop(s);
	}

	public void matchZoom(float z)
	{
		this.transform.localScale = new Vector3(1 / z, 1 / z, 1);
		
	}
}
