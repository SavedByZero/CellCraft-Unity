using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//Y position 193 
public class MessageBarScript : MonoBehaviour
{
    private Vector3 _StartPos;
    private Coroutine _displayRoutine;
    private bool _showing;
    public Sprite[] backdrops;
    public const int NORMAL_MSG = 0;
    public const int WARNING_MSG = 3;



    private void Awake()
    {
        _StartPos = this.transform.localPosition;

    }

    public void ShowMessage(string value, int type = 0)
    {
       
        _displayRoutine = StartCoroutine(displayRoutine(value));
        GetComponentInChildren<Image>().sprite = backdrops[type];
        Text text = GetComponentInChildren<Text>();
        if (type == WARNING_MSG)
            text.color = Color.white;
        else
            text.color = Color.black;
        //this.gameObject.SetActive(true);
       
    }

    IEnumerator displayRoutine(string content)
    {
        if (_showing)
            Hide(0);
        Text text = GetComponentInChildren<Text>();
        text.text = content;
        Show();
        yield return new WaitForSeconds(3.5f);
        Hide();


    }

    public void Show(float time = 0.5f)
    {
        _showing = true;
        this.gameObject.transform.DOLocalMoveY(_StartPos.y - 25, time);
    }

    public void Hide(float time = 0.5f)
    {
        this.gameObject.transform.DOLocalMoveY(_StartPos.y, time).OnComplete(new TweenCallback(delegate {
            _showing = false;
        }));
        
    }

}
