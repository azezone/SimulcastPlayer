using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StateItem : MonoBehaviour {
    [SerializeField]
    ImageSwitch _imageSwitch;
    [SerializeField]
    Text _erroText;
   
    public bool state = false;
    public void SetData(bool isok,string text)
    {
        state = isok;
        _erroText.text = text;
        _imageSwitch.SwitchImage(isok);
    }
}
