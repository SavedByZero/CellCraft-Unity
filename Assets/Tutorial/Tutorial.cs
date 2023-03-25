using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Sprite[] WindowSprites;
    private Queue<string> _slideQueue = new Queue<string>();
  

    public Image _image;
    // Start is called before the first frame update
    void Awake()
    {
       
    }

    public void showSlides(string slides)
    {
        Debug.Log("showing slides " + slides);
        if (slides == null || slides=="")
        {
            return;
        }
        _slideQueue = new Queue<string>();
        string[] slidez = slides.Split(',');
   
        for(int i=0; i <slidez.Length; i++)
        {
            _slideQueue.Enqueue(slidez[i]);
        }

        ShowWindow(_slideQueue.Dequeue());
    }

    public void ShowWindow(string name)
    {
    

        for (int i= 0; i < WindowSprites.Length; i++)
        {
            if (name == WindowSprites[i].name)
            {
                _image.sprite = WindowSprites[i];
                _image.preserveAspect = true;
                _image.SetNativeSize();
                this.gameObject.SetActive(true);
                Time.timeScale = 0;
                return;
            }
        }

        Next();
      
    }

    public void Next()
    {
        if (_slideQueue.Count > 0)
        {
            ShowWindow(_slideQueue.Dequeue());
        }
        else
            Hide();
    }


    public void Hide()
    {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);

    }

}
