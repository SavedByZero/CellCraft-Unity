using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    private float _max;
    private Image _fill;
    private Text _TextInterface;
    
    // Start is called before the first frame update
    void Awake()
    {
        _fill = GetComponentInChildren<Image>();
        _TextInterface = this.transform.parent.GetComponentInChildren<Text>();
    }

    public void SetMax(float max)
    {
        _max = max;
    }

    public void Set(float val)
    {
        if (_fill != null)
            _fill.fillAmount = val/_max;
        if (_TextInterface != null)
            _TextInterface.text = val.ToString();
    }
}
