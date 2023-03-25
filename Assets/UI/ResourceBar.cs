using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResourceBar : MonoBehaviour
{
    private float _max;
    private Image _fill;
    public Text TextInterface;

    
    // Start is called before the first frame update
    void Awake()
    {
        _fill = GetComponentInChildren<Image>();
        //_TextInterface = this.transform.parent.GetComponentInChildren<Text>();
    }

    public void SetMax(float max)
    {
        _max = max;
    }

    public void Set(float val)
    {
        if (_fill != null)
        {
            _fill.DOFillAmount(val / _max, 1);
            //_fill.fillAmount = val / _max;
        }
        if (TextInterface != null)
            TextInterface.text = val.ToString();
    }
}
