using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ImageSwitch : MonoBehaviour {
    [SerializeField]
    Sprite _OnImage;
    [SerializeField]
    Sprite _OffImage;

    private Image image;

    public void SwitchImage(bool on)
    {
        if (image == null)
        {
            image = gameObject.GetComponent<Image>();
        }

        image.sprite = on ? _OnImage : _OffImage;
    }
}
