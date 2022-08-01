using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessageBarScript : MonoBehaviour
{
    private Vector3 _StartPos;
    private Coroutine _displayRoutine;
    private bool _showing;


    private void Awake()
    {
        _StartPos = this.transform.localPosition;

    }

    public void ShowMessage(string value)
    {
       
        _displayRoutine = StartCoroutine(displayRoutine(value));
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
