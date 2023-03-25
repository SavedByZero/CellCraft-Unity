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
        Debug.Log("membrane anchor " + this);// Camera.main.ScreenToWorldPoint(Input.mousePosition));
        onMembraneAreaClicked?.Invoke();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CanvasObject cv = collision.GetComponentInChildren<CanvasObject>();
        if (cv != null)
        {
            if (cv is GoodieGem)
            {
                (cv as GoodieGem).onTouchCell(); //Bookmark: Goodie gem collision with cell 
            }

           
        }
        Mitochondrion mito = collision.GetComponentInChildren<Mitochondrion>();
        if (mito != null && mito.isOutsideCell)
        {
            mito.GoInsideCell();
        }

        BigVesicle bv = collision.GetComponentInChildren<BigVesicle>();
        if (bv != null)
        {
            bv.FoundMembrane();
        }

    }

}
