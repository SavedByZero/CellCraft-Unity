using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private Engine _engine;
    public ATPUI ATPUsed;
    public GameObject ATPButton;
    public MessageBarScript MessagerUI;
    // Start is called before the first frame update
    void Start()
    {
        _engine = GameObject.FindObjectOfType<Engine>();
        _engine.EngineEvent.AddListener(receiveEngineEvent);
        _engine.EngineMessageEvent.AddListener(receiveMessageForHUD);
        _engine.onATPChanged += updateATPButton;
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
                ATPUsed.ShowATP((int)i);
                break;
            case "atp_gain": //glucose
                ATPUsed.ShowATP((int)i, false);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
