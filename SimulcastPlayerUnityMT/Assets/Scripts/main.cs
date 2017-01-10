using System.Collections;
using UnityEngine;

public class main : MonoBehaviour
{
    [SerializeField]
    Player _player;
    [SerializeField]
    StatePanel _statePanel;

    private KeyCode StatePanelCallKey = KeyCode.JoystickButton2;
    private KeyCode StatePanelHideKey = KeyCode.JoystickButton1;
    private KeyCode VideoFoceToPlayKey = KeyCode.JoystickButton0;
    private KeyCode ShowSettingPageKey = KeyCode.JoystickButton3;
    private KeyCode VideoRePlayKey = KeyCode.JoystickButton1;

    private float originTime = -1f;
    private int clickTimes = 0;
    private int clickNeed = 3;
    private KeyCode originKey = KeyCode.E;
    private float DeltaTime = 0.4f;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            StatePanelCallKey = KeyCode.JoystickButton2;
            StatePanelHideKey = KeyCode.JoystickButton1;
            VideoFoceToPlayKey = KeyCode.JoystickButton0;
            ShowSettingPageKey = KeyCode.JoystickButton3;
            VideoRePlayKey = KeyCode.JoystickButton1;
        }
        else
        {
            StatePanelCallKey = KeyCode.Space;
            StatePanelHideKey = KeyCode.A;
            VideoFoceToPlayKey = KeyCode.B;
            ShowSettingPageKey = KeyCode.C;
        }
        _statePanel.Init();
        StartCoroutine(InitePlayerAffterConfigOK());
    }

    void Update()
    {
        //call state panel
        if (Input.GetKeyDown(StatePanelCallKey))
        {
            OprateCombKeyDown(StatePanelCallKey);
        }
        //force to play video
        //if (Input.GetKeyDown(VideoFoceToPlayKey))
        //{
        //    OprateCombKeyDown(VideoFoceToPlayKey);
        //}
        //show setting page
        if (Input.GetKeyDown(ShowSettingPageKey))
        {
            OprateCombKeyDown(ShowSettingPageKey);
        }
        //hide state panel
        //if (Input.GetKeyDown(StatePanelHideKey))
        //{
        //    StatePanel.instance.SwitchPanel(false);
        //}


        //hide state panel
        if (Input.GetKeyDown(ShowSettingPageKey))
        {
            StatePanel.instance.SwitchPanel(false);
        }

        //force to play
        if (Input.GetKeyDown(VideoFoceToPlayKey))
        {
            if (Player.instance != null) Player.instance.ForcePlay();
        }
        //force to replay
        if (Input.GetKeyDown(VideoRePlayKey))
        {
            if (Player.instance != null) Player.instance.Replay();
        }

        if (Time.time - originTime > DeltaTime)
        {
            Clear();
        }
    }

    private IEnumerator InitePlayerAffterConfigOK()
    {
        while (StatePanel.instance == null || !StatePanel.instance.isEverythingReady())
        {
            yield return new WaitForSeconds(1f);
        }
        _player.InitPlayer();
    }

    private void OprateCombKeyDown(KeyCode key)
    {
        if (clickTimes == 0)
        {
            originTime = Time.time;
            originKey = key;
        }
        if ((Time.time - originTime <= DeltaTime) && originKey == key)
        {
            clickTimes++;
        }
        else
        {
            Clear();
        }
        if (clickTimes == clickNeed)
        {
            if (originKey == StatePanelCallKey)
            {
                if (StatePanel.instance != null) StatePanel.instance.SwitchPanel(true);
            }
            //else if (originKey == VideoFoceToPlayKey)
            //{
            //    if (Player.instance != null) Player.instance.ForcePlay();
            //}
            else if (originKey == ShowSettingPageKey)
            {
                PicoUnityActivity.CallObjectMethod("ShowSettingPage");
            }
            Clear();
        }
    }

    private void Clear()
    {
        originTime = -1f;
        clickTimes = 0;
        originKey = KeyCode.E;
    }
}