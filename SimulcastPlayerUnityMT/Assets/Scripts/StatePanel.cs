using UnityEngine;
using System.Collections;
using System;

public class StatePanel : MonoBehaviour
{
    [SerializeField]
    StateItem _configFile;
    [SerializeField]
    StateItem _videoFile;
    [SerializeField]
    StateItem _wifiState;
    [SerializeField]
    StateItem _serviceConnectState;

    public static StatePanel instance = null;
    public bool isConfigFileExist
    {
        get { return _configFile.state; }
    }
    public bool isVideoFileExist
    {
        get { return _videoFile.state; }
    }
    public bool isWifiConnected
    {
        get { return _wifiState.state; }
    }
    public bool isServiceConnected
    {
        get { return _serviceConnectState.state; }
    }

    private bool isPanelOpen = false;

    public void Init()
    {
        _configFile.SetData(false, "");
        _videoFile.SetData(false, "");
        _wifiState.SetData(false, "");
        _serviceConnectState.SetData(false, "");
        instance = this;
        RefreshState();
        ShowPanel(false);
    }

    public void SwitchPanel(bool on)
    {
        isPanelOpen = on;
        ShowPanel(isPanelOpen);
        if (isPanelOpen)
        {
            RefreshState();
        }
    }

    private void ShowPanel(bool isopen)
    {
        transform.localPosition = isopen ? new Vector3(0, 0, 1.83f) : new Vector3(0, 0, 10000);
    }

    public void RefreshState()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            CheckConfigFile();
            if (isConfigFileExist)
            {
                StartWifiConnectService();
            }
            //CheckVideoFile();
            CheckWifiState();
            CheckServiceConnected();
        }
    }

    private void CheckConfigFile()
    {
        int flag = -1;
        PicoUnityActivity.CallObjectMethod<int>(ref flag, "ReadConfig");
        switch (flag)
        {
            case 0:
                _configFile.SetData(false, Constant.FileNotExist);
                break;
            case 1:
                _configFile.SetData(true, Constant.Success);
                break;
            case 2:
                _configFile.SetData(false, Constant.ConfigNotLigal);
                break;
            case 3:
                _configFile.SetData(false, Constant.ElementsLose);
                break;
            case 4:
                _configFile.SetData(false, Constant.IPNotLigal);
                break;
            case 5:
                _configFile.SetData(false, Constant.PortNotLigal);
                break;
            case 6:
                _configFile.SetData(false, Constant.OtheError);
                break;
            default:
                _configFile.SetData(false, Constant.Unknow);
                break;
        }

    }

    private void CheckVideoFile()
    {
        bool flag = false;
        PicoUnityActivity.CallObjectMethod<bool>(ref flag, "isVideoFileExist");
        _videoFile.SetData(flag, flag ? Constant.Success : Constant.FileNotExist);
    }

    public static bool IsVideoFileExist(string videoname)
    {
        bool flag = false;
        PicoUnityActivity.CallObjectMethod<bool>(ref flag, "isVideoFileExist", videoname);
        return flag;
    }

    private void StartWifiConnectService()
    {
        //TODO
        PicoUnityActivity.CallObjectMethod("StartAutoConnectService");
    }

    private void CheckWifiState()
    {
        bool flag = false;
        PicoUnityActivity.CallObjectMethod<bool>(ref flag, "GetWifiConnectedState");
        string str = "";
        PicoUnityActivity.CallObjectMethod<string>(ref str, "getKeyValue", Constant.SSID);
        _wifiState.SetData(flag, flag ? str : Constant.Connecting);
    }

    private void CheckServiceConnected()
    {
        string str = "";
        PicoUnityActivity.CallObjectMethod<string>(ref str, "getKeyValue", Constant.SERVERIP);
        _serviceConnectState.SetData(Player.isServiceConnected, Player.isServiceConnected ? str : Constant.Connecting);
    }

    public bool isEverythingReady()
    {
        //TODO
        //return (isConfigFileExist && isVideoFileExist);
        //return isVideoFileExist;
        return isConfigFileExist;
    }
}
