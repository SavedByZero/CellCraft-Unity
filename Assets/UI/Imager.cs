using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Imager : MonoBehaviour
{
    public Sprite[] ImagerSprites;
    public GameObject Screen;
    public GameObject Target;
    
    private bool _firstTime = true;
    private bool _revealing;
   // private Material _tvMaterial;
    private Image _targetImage;
    public Material NoiseMaterial;
    public Material GradientMaterial;
    public float Speed = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
       // _tvMaterial = Screen.GetComponent<Image>().material;
        _targetImage = Target.GetComponent<Image>();
    }

    public void ReceiveImage(int imageIndex)
    {
        _targetImage.sprite = ImagerSprites[imageIndex];
        if (_firstTime)
        {
            reveal();
            _firstTime = false;
        }
    }

    void reveal()
    {
        Screen.GetComponent<Image>().material = NoiseMaterial;
        NoiseMaterial.SetFloat("_Noise", 1);
        StartCoroutine(revealDelay());
       
    }

    IEnumerator revealDelay()
    {
        yield return new WaitForSeconds(1);
        _revealing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_revealing)
        {
            float oldValue = NoiseMaterial.GetFloat("_Noise");
            float newValue = oldValue -= (Time.deltaTime*Speed);
            if (newValue < 0)
            {
                newValue = 0;
                _revealing = false;
                //StartCoroutine(blink());
            }
            NoiseMaterial.SetFloat("_Noise", newValue);
            _targetImage.color = new Color(1,1,1,_targetImage.color.a + ((Time.deltaTime*(Speed/2f))));


        }
    }

    IEnumerator blink()
    {
        _targetImage.DOFade(0, 0);
        yield return new WaitForEndOfFrame();
        _targetImage.DOFade(1, 3f).SetEase(Ease.InOutBounce);
      

    }
}
