using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    public Sprite OnSprite;
    public Sprite OffSprite;
    private bool _muted;
    private Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
    }

    public void Toggle()
    {
        _muted = !_muted;
        SfxManager.SetMute(_muted);
        MusicManager.SetMute(_muted);
        _image.sprite = (_muted ? OffSprite : OnSprite);

    }
}
