using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is really more than the anchor -- more like the sensory area.
public class MembraneAnchor : MonoBehaviour
{
    public delegate void MembraneAreaClicked();
    public MembraneAreaClicked onMembraneAreaClicked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnMouseDown()
    {

        onMembraneAreaClicked?.Invoke();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanvasObject cv = collision.GetComponentInChildren<CanvasObject>();
        if (cv != null)
        {
            if (cv is GoodieGem)
            {
                (cv as GoodieGem).onTouchCell();
            }
        }

    }

}
