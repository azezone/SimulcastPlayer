using UnityEngine;
using System.Collections;

public class CMove : MonoBehaviour {

    private RectTransform rect;

    public bool IsX = false;
    public bool IsY = false;
    public bool IsZ = false;

    private float fData = 1.0f;

    public float fWaitTime = 0.0f;

    public float fMin = 0.5f, fMax = 0.9f;

	// Use this for initialization
	void Start () {
        rect = transform.GetComponent<RectTransform>();

        fWaitTime = Random.Range(15.0f, 40.0f);

        Invoke("fnReturnInit", fWaitTime);
	}
	
	// Update is called once per frame
	void Update () {

        if (IsX)
        {
            rect.position -= new Vector3(Time.deltaTime * Random.Range(fMin, fMax) * fData, 0, 0);
        }
        else if (IsY)
        {
            rect.position -= new Vector3(0, Time.deltaTime * Random.Range(fMin, fMax) * fData, 0);
        }
        else if (IsZ)
        {
            rect.position -= new Vector3(0, 0, Time.deltaTime * Random.Range(fMin, fMax) * fData);
        }
	
	}

    void fnReturnInit()
    {
        fData = -fData;

        fWaitTime = Random.Range(15.0f, 40.0f);
        Invoke("fnReturnInit", fWaitTime);
    }
}
