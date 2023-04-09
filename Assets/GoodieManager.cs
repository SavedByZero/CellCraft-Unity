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
        //PlaceGem("g", 10, new Vector3(7,7,0));
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

    public void PlaceGoodie(Stuff goodie)
    {
        /*goodie.Type, 
            goodie.Count, 
            goodie.Spawn*/  //spawn chance?
        for(int i=0; i < goodie.Count; i++)
        {
            int min = 2, max = 5;
            switch (goodie.Type)
            {
              
                case "aa":
                    min = 50;
                    max = 100;
                    break;
                case "na":
                    min = 10;
                    max = 30;
                    break;
                case "fa":
                    max = 30;
                    min = 10;
                    break;
                case "g":
                    break;
                case "atp":
                    break;

            }
            PlaceGem(goodie.Type, Random.Range(min,max), new Vector3(Random.Range(-15,15),Random.Range(-15,15)));
        }
    }

   

    
}
