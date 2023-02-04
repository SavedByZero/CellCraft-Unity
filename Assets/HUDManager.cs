using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private Engine _engine;
    public ResourceUI ATPUsed;
    public GameObject ATPButton;
    public GameObject NAButton;
    public GameObject AAButton;
    public GameObject FAButton;
    public GameObject GButton;
    public MessageBarScript MessagerUI;
    // Start is called before the first frame update
    void Awake()
    {
        _engine = GameObject.FindObjectOfType<Engine>();
        _engine.EngineEvent.AddListener(receiveEngineEvent);
        _engine.EngineMessageEvent.AddListener(receiveMessageForHUD);
        _engine.onATPChanged += updateATPButton;
        _engine.onNAChanged += updateNAButton;
        _engine.onAAChanged += updateAAButton;
        _engine.onFAChanged += updateFAButton;
        _engine.onGChanged += updateGButton;
    }

    private void updateNAButton(float na)
    {
        NAButton.GetComponentInChildren<Text>().text = na.ToString();
    }

    private void updateAAButton(float aa)
    {
        AAButton.GetComponentInChildren<Text>().text = aa.ToString();
    }

    private void updateFAButton(float fa)
    {
        FAButton.GetComponentInChildren<Text>().text = fa.ToString();
    }

    private void updateGButton(float g)
    {
        GButton.GetComponentInChildren<Text>().text = g.ToString();
    }

    private void updateATPButton(float atp)
    {
        ATPButton.GetComponentInChildren<Text>().text = atp.ToString();
    }

    private void receiveMessageForHUD(string message)
    {
        MessagerUI.ShowMessage(message);
    }

    private void receiveEngineEvent(string v, float i)
    {
        Debug.Log("got engine event: " + v + ", " + i);
        switch (v)
        {
            case "atp":
            case "atp_loss":
                ATPUsed.ShowResourceChange((int)i);
                break;
            case "glucose_gain": //glucose
                ATPUsed.ShowResourceChange((int)i, false, IconType.Glucose);
               ObjectiveManager.GetInstance().onCompleteObjective?.Invoke(ObjectiveManager.GameObjective.FindGlucose);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
