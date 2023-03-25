using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ResourceUI : MonoBehaviour
{
    private Vector3 _StartPos;
    public Sprite[] ResourceIcons;

    private void Awake()
    {
        _StartPos = this.transform.localPosition;
        
    }

    // Start is called before the first frame update
    public void ShowResourceChange(int value, bool subtract = true, IconType IType = IconType.ATP)
    {
        
        Text text = GetComponentInChildren<Text>();
        Outline outline = text.GetComponent<Outline>();
        outline.effectColor = (subtract ? Color.red : Color.green);
        text.text = subtract ? value.ToString() : "+" + value.ToString();
       // Vector3 mouse = FastMath.GetWorldPositionOnPlane(-Camera.main.transform.position.z);//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        this.gameObject.transform.position = Input.mousePosition + (Vector3.left+new Vector3((int)IType*150,0,0));
        Image img = GetComponentInChildren<Image>(true);
        img.sprite = ResourceIcons[(int)IType];
        this.gameObject.SetActive(true);
        this.gameObject.transform.DOBlendableLocalMoveBy(new Vector3(0,50,0), 0.75f).OnComplete(new TweenCallback(delegate
        {
            this.gameObject.SetActive(false);
            this.gameObject.transform.localPosition = _StartPos;
        }));
    }

}

public enum IconType
{
    ATP,
    Glucose,
    NA,
    FA,
    AA
}
