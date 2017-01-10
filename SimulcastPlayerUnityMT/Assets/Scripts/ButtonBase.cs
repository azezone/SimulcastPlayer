using UnityEngine;
using System.Collections;

public class ButtonBase : MonoBehaviour {
    public delegate void OnClickCallBack();
    public OnClickCallBack onClick = null;
    void Update()
    {
        if (ishover && Input.GetKeyDown(KeyCode.JoystickButton0) && onClick != null)
        {
            onClick();
        }
    }

    private bool ishover = false;
    void OnTriggerEnter(Collider collision)
    {
        //Debug.LogError("OnTriggerEnter");
        transform.localScale = 1.3f*Vector3.one;
        ishover = true;
    }
    void OnTriggerExit(Collider collision)
    {
        //Debug.LogError("OnTriggerExit");
        transform.localScale = Vector3.one;
        ishover = false;
    }
}
