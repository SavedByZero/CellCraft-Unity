using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AppearanceController : MonoBehaviour
{
    public Sprite[] IntroSprites;
    private Material _swirlMaterial;
    private bool _showing = false;
    public GameObject Target;
    public delegate void FinishedShowing();
    public FinishedShowing onFinishedShowing;
    public float Speed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        _swirlMaterial = GetComponent<SpriteRenderer>().material;
       // ShowUp();
    }

    void OnEnable()
    {
        this.transform.DOMove(Vector3.zero, 0);
        this.transform.DOScale(1, 0);
        _showing = false;
        if (_swirlMaterial == null)
            _swirlMaterial = GetComponent<SpriteRenderer>().material;

        _swirlMaterial.SetFloat("_SwirlAngle", 2f);
    }

    public void ShowUp(int targetSprite = 0)
    {
        SfxManager.Play(SFX.SFXBubble);
        Speed = 0.3f;
        GetComponent<SpriteRenderer>().sprite = IntroSprites[targetSprite];
        _showing = true;
        //play sound 
    }

    // Update is called once per frame
    void Update()
    {
        /*img.material = scene.BlurMaterial;
                float radius = 0.03f;
                if (_step.Value != "")
                    radius = float.Parse(_step.Value);
                img.material.SetFloat("_Radius", radius);*/
        if (_showing)
        {
            float oldValue = _swirlMaterial.GetFloat("_SwirlAngle");
            if (oldValue < 1.7)
                Speed += 0.05f;

                float newValue = oldValue - (Time.deltaTime*Speed);
            _swirlMaterial.SetFloat("_SwirlAngle", newValue);
          
            if (newValue <= 0)
            {
                _swirlMaterial.SetFloat("_SwirlAngle", 0);
                _showing = false;
                //shrink down
                shrink();
            }
        }
    }

    void shrink()
    {
        this.transform.DOMove(Target.transform.position, 1);
        this.transform.DOScale(.15f, 1).OnComplete(new TweenCallback(delegate {
            onFinishedShowing?.Invoke();
            if (!Target.activeSelf)
                Target.SetActive(true);
            Target.GetComponentInChildren<SpriteRenderer>().DOFade(1, 0);
            this.gameObject.SetActive(false);

        }));
    }
}
