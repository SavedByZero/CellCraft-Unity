using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
