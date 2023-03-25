using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    private Engine _engine;
    private Cell _cell;
    public ResourceUI ATPUsed;
    public GameObject ATPButton;
    public GameObject NAButton;
    public GameObject AAButton;
    public GameObject FAButton;
    public GameObject GButton;
    public MessageBarScript MessagerUI;
    public Text NameField;
    private ActionsPanel _actionPanel;
    private CostPanel _costPanel;
    private List<GameObject> _views = new List<GameObject>();
    private string[] _names = new string[] { "Centrosome", "CytoSkeleton", "Membrane", "Nucleus", "ER", "Golgi", "Chloroplast", "Mitochondrion", "Slicer", "Ribosome", "Vesicle", "PEroxisome", "Lysosome" };
    // Start is called before the first frame update
    void Awake()
    {
        _actionPanel = GetComponentInChildren<ActionsPanel>();
        _costPanel = GetComponentInChildren<CostPanel>();
        _engine = GameObject.FindObjectOfType<Engine>();
        _cell = GameObject.FindObjectOfType<Cell>();
        _engine.EngineEvent.AddListener(receiveEngineEvent);
        _engine.EngineMessageEvent.AddListener(receiveMessageForHUD);
        _engine.onATPChanged += updateATPButton;
        _engine.onNAChanged += updateNAButton;
        _engine.onAAChanged += updateAAButton;
        _engine.onFAChanged += updateFAButton;
        _engine.onGChanged += updateGButton;
        _cell.onOrganelleSelected += organelleSelected;
        _actionPanel.onPassUpCosts += passCosts;
    }

    private void passCosts(string costString)
    {
        _costPanel.ReceiveCost(costString);
    }

    private void organelleSelected(int organelle_id, CellObject co)
    {
        NameField.text = _names[organelle_id];
        GetComponentInChildren<Imager>().ReceiveImage(organelle_id);
        GetComponentInChildren<StatsPanel>().ReceiveCO(co);
        GetComponentInChildren<ActionsPanel>().ReceiveCO(co, organelle_id);
    }

    private void updateNAButton(float na, float netChange)
    {
        NAButton.GetComponentInChildren<Text>().text = na.ToString();
        receiveEngineEvent("na", netChange);
    }

    private void updateAAButton(float aa, float netChange)
    {
        AAButton.GetComponentInChildren<Text>().text = aa.ToString();
        receiveEngineEvent("aa", netChange);
    }

    private void updateFAButton(float fa, float netChange)
    {
        FAButton.GetComponentInChildren<Text>().text = fa.ToString();
        receiveEngineEvent("fa", netChange);
    }

    private void updateGButton(float g, float netChange)
    {
        GButton.GetComponentInChildren<Text>().text = g.ToString();
        receiveEngineEvent("g", netChange);
    }

    private void updateATPButton(float atp, float netChange)
    {
        ATPButton.GetComponentInChildren<Text>().text = atp.ToString();
        receiveEngineEvent("atp", netChange);
        if (atp >= 1000)
        {
            ObjectiveManager.GetInstance().onCompleteObjective?.Invoke("make_lots_atp");
        }
    }

    private void receiveMessageForHUD(string message)
    {
        MessagerUI.ShowMessage(message);
    }

    private void receiveEngineEvent(string v, float i)
    {
        if (i == 0)
            return; 
        Debug.Log("got engine event: " + v + ", " + i);
        switch (v)
        {
            case "atp":
                GameObject atp_go = fetchResourceView();             
                atp_go.GetComponentInChildren<ResourceUI>().ShowResourceChange((int)i, i < 0);
                break;
            case "na":
                GameObject na_go = fetchResourceView();
                na_go.GetComponentInChildren<ResourceUI>().ShowResourceChange((int)i, i < 0, IconType.NA);
                break;
            case "fa":
                GameObject fa_go = fetchResourceView();
                //ATPUsed.ShowResourceChange()
                fa_go.GetComponentInChildren<ResourceUI>().ShowResourceChange((int)i, i < 0, IconType.FA);
                break;
            case "aa":
                GameObject aa_go = fetchResourceView();
                aa_go.GetComponentInChildren<ResourceUI>().ShowResourceChange((int)i, i < 0, IconType.AA);
                break;
            case "glucose":
            case "g"://glucose
                GameObject g_go = fetchResourceView();
                g_go.GetComponentInChildren<ResourceUI>().ShowResourceChange((int)i, i < 0, IconType.Glucose);
                //ATPUsed.ShowResourceChange((int)i, false, IconType.Glucose);
                ObjectiveManager.GetInstance().onCompleteObjective?.Invoke("find_g");
                break;
        }
    }

    GameObject fetchResourceView()
    {
        for(int i=0; i < _views.Count; i++)
        {
            if (!_views[i].activeSelf)
            {
                _views[i].SetActive(true);
                _views[i].transform.SetParent(ATPUsed.transform.parent);
                return _views[i];
            }
        }
        GameObject view = Instantiate(ATPUsed.gameObject) as GameObject;
        view.SetActive(true);
        view.transform.SetParent(ATPUsed.transform.parent);
        _views.Add(view);
        return view;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
