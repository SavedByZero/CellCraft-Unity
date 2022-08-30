using System.Collections;
using System.Collections.Generic;
using UnityEngine;
////Distribute a specified quantity of each type of gem throughout the given level
public class GoodieManager : MonoBehaviour
{
    private SpriteRenderer _fieldSR;
    public GameObject GemPrefab;

    void Start()
    {
        _fieldSR = GetComponentInChildren<Terrain>().GetComponentInChildren<SpriteRenderer>();
        PlaceGem("g", 10, new Vector3(3,3,0));
    }

    //Place a gem of a certain type at a certain spot
    public void PlaceGem(string type, int amount, Vector3 spot)
    {
        //Find a reference to the maskable canvas

        //Instantiate a Gem 
        GameObject gem = Instantiate(GemPrefab) as GameObject;
        //set it to the proper type 
        gem.GetComponent<GoodieGem>().SetGemType(type, amount);
        //position it at the spot on the canvas reference. 
        gem.transform.SetParent(this.transform);
        gem.transform.localScale = Vector3.one;
        gem.transform.localPosition = spot;

    }

   

    
}
