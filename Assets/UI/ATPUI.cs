using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ATPUI : MonoBehaviour
{
    private Vector3 _StartPos;


    private void Awake()
    {
        _StartPos = this.transform.localPosition;
        
    }

    // Start is called before the first frame update
    public void ShowATP(int value, bool subtract = true)
    {
        Text text = GetComponentInChildren<Text>();
        text.text = subtract ? "-" + value.ToString() : value.ToString();
       // Vector3 mouse = FastMath.GetWorldPositionOnPlane(-Camera.main.transform.position.z);//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        this.gameObject.transform.position = Input.mousePosition; 
        this.gameObject.SetActive(true);
        this.gameObject.transform.DOBlendableLocalMoveBy(new Vector3(0,50,0), 0.75f).OnComplete(new TweenCallback(delegate
        {
            this.gameObject.SetActive(false);
            this.gameObject.transform.localPosition = _StartPos;
        }));
    }

}
