using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AppearanceController : MonoBehaviour
{
    public Sprite[] IntroSprites;
    public float[] TargetScales;
    public float[] StartScales;
    private float _startScale;
    private float _targetScale;
    private Material _swirlMaterial;
    private bool _showing = false;
    public GameObject[] Targets;
    public string[] TargetNames;
    private GameObject _currentTarget;
    public Sprite _currentIntroSprite;
    public delegate void FinishedShowing();
    public FinishedShowing onFinishedShowing;
    public float Speed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        _swirlMaterial = GetComponent<SpriteRenderer>().material;
       // ShowUp();
    }

    public void SetTargetByName(string name)
    {
        for(int i=0; i < TargetNames.Length; i++)
        {
            if (name == TargetNames[i])
            {
                _currentTarget = Targets[i];
                _currentIntroSprite = IntroSprites[i];
                _targetScale = TargetScales[i];
                _startScale = StartScales[i];
            }
        }
    }

    void OnEnable()
    {
        this.transform.DOMove(Vector3.zero, 0);
        //this.transform.DOScale(1, 0);
        _showing = false;
        if (_swirlMaterial == null)
            _swirlMaterial = GetComponent<SpriteRenderer>().material;

        _swirlMaterial.SetFloat("_SwirlAngle", 2f);
    }

    public void ShowUp()
    {
        SfxManager.Play(SFX.SFXBubble);
        Speed = 0.3f;
        this.transform.localScale = new Vector3(_startScale,_startScale, _startScale);
        GetComponent<SpriteRenderer>().sprite = _currentIntroSprite;
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
        this.transform.DOMove(_currentTarget.transform.position, 1);
        this.transform.DOScale(_targetScale, 1).OnComplete(new TweenCallback(delegate {
            onFinishedShowing?.Invoke();
            if (!_currentTarget.activeSelf)
                _currentTarget.SetActive(true);
            _currentTarget.GetComponentInChildren<SpriteRenderer>().DOFade(1, 0);
            if (_currentTarget.GetComponentInChildren<Centrosome>() != null)
            {
                ObjectiveManager.GetInstance().onCompleteObjective?.Invoke("start");
            }
            this.gameObject.SetActive(false);

        }));
    }
}
