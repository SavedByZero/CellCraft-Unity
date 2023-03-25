using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class COAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    //action is defined by name  
    //name maps to a sprite, a description, and a behavior 
    //behavior is called by pressing 
    public Sprite[] ActionSprites;
    public string[] ActionCaptions;
    public string[] Costs;
    public GameObject Caption;
    private int _index;
    private Text _captionText;
    public delegate void SendCostString(string costString);
    public SendCostString onSendCostString;
    public delegate void DoAction(string actionName);
    public DoAction onDoAction;
    
    void Start()
    {
        _captionText = Caption.GetComponentInChildren<Text>(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DefineAction(string name)
    {
        name = name.ToLower();
        for(int i=0; i < ActionSprites.Length; i++)
        {
            if (ActionSprites[i].name == name)
            {
                _index = i;
                GetComponentInChildren<Image>().sprite = ActionSprites[i];
                onSendCostString?.Invoke(Costs[i]);
                break;
            }
        }
    }

    public void ShowCaption(bool value)
    {
        _captionText.text = ActionCaptions[_index];
        Caption.SetActive(value);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowCaption(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowCaption(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //do the thing 
        onDoAction?.Invoke(ActionSprites[_index].name);
    }
}
