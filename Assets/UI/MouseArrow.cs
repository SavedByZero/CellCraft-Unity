using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseArrow : MonoBehaviour
{
    public LineRenderer Body;
    public LineRenderer ArrowHead;
    LineRendererArrow _arrowScript;

    private void Start()
    {
        _arrowScript = ArrowHead.GetComponent<LineRendererArrow>();
        Show(false);
    }
    public void SetPosition(float x1, float y1, float x2, float y2)
    {
        Vector3 diff = new Vector3(x2 - x1, y2 - y1, 0);
        Body.SetPositions(new Vector3[] { new Vector3(x1, y1, -Camera.main.transform.position.z), new Vector3(x2, y2, -Camera.main.transform.position.z) });
        _arrowScript.ArrowOrigin = new Vector3(x2, y2, -Camera.main.transform.position.z);
       
        Vector3 norm = diff.normalized;
        norm *= 1.2f;
        float xIncrement = norm.x;//(x2 > 0 ? 1 : -1);
        float yIncrement = norm.y;//(y2 > 0 ? 1 : -1);
        _arrowScript.ArrowTarget = new Vector3(x2 + xIncrement, y2 + yIncrement, -Camera.main.transform.position.z);
        _arrowScript.UpdateArrow();
        
    }

    public void Show(bool value)
    {
        this.gameObject.SetActive(value);
    }
}
