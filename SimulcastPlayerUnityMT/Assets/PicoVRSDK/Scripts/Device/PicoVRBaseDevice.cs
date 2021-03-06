﻿#if !UNITY_EDITOR
#if UNITY_ANDROID
#define ANDROID_DEVICE
#elif UNITY_IPHONE
#define IOS_DEVICE
#endif
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

/// <summary>
/// PicoVRSDK  Platform 
/// </summary>
public abstract class PicoVRBaseDevice
{
    /// <summary>
    /// 异步
    /// </summary>
    public bool Async = false;

    /// <summary>
    /// 抗锯齿
    /// </summary>
    public enum RenderTextureAntiAliasing
    {
        X_1 = 1,
        X_2 = 2,
        X_4 = 4,
        X_8 = 8,
    }

    /// <summary>
    /// bit depth 图像位深
    /// </summary>
    public enum RenderTextureDepth
    {
        BD_0 = 0,
        BD_16 = 16,
        BD_24 = 24,
    }

    /// <summary>
    /// 虚基类PicoVRBaseDevice
    /// </summary>
    private static PicoVRBaseDevice device = null;

    public RtorScreen rtorScren;

    public bool PlantformSupport;

    public struct RtorScreen
    {
        public float Width;
        public float Height;
    }

    public const int eyeTextureCount = 6;

    public RenderTexture[] eyeTextures = new RenderTexture[eyeTextureCount];

    public int[] eyeTextureIds = new int[eyeTextureCount];

    public int currEyeTextureIdx = 0;

    public int nextEyeTextureIdx = 1;

    private bool canConnecttoActivity;


    public virtual bool CanConnecttoActivity
    {
        get;
        set;
    }
   
    protected PicoVRBaseDevice()
    {

    }

    public static PicoVRBaseDevice GetDevice()
    {
        if (device == null)
        {
#if UNITY_EDITOR
            if (PicoVRManager.SDK.IsVREditorDebug)
            {
                device = new PicoVRWinPCDevice();
            }
            else
            {
                device = new PicoVRUnityDevice();
            }
#elif ANDROID_DEVICE
      device = new PicoVRAndroidDevice();
#elif IOS_DEVICE
      device = new PicoVRIOSDevice();
#elif UNITY_STANDALONE_WIN 
      device = new PicoVRWinPCDevice();

#else
      throw new InvalidOperationException("Unsupported device.");
#endif
        }
        return device;
    }

    /// <summary>
    /// 初始化方法
    /// </summary>
    public virtual void Init()
    {
    }



    /// <summary>
    /// 设置是否开启VR模式
    /// </summary>
    /// <param name="enabled"></param>
    public virtual void SetVRModeEnabled(bool enabled)
    {
       
    }

    /// <summary>
    /// 设置是否开启畸变
    /// </summary>
    /// <param name="enabled"></param>
    public virtual  void SetDistortionCorrectionEnabled(bool enabled)
    {
    }

    /// <summary>
    /// 创建rendertexture
    /// </summary>
    /// <returns></returns>
    public abstract Vector2 GetStereoScreenSize();

    /// <summary>
    /// 设置rendertexture
    /// </summary>
    /// <param name="stereoScreen"></param>
    public virtual void SetStereoScreen(RenderTexture stereoScreen)
    {
    }

    /// <summary>
    /// 设置自动漂移校正Auto Drift Correction（？？？）
    /// </summary>
    /// <param name="enabled"></param>
    public virtual  void SetAutoDriftCorrectionEnabled(bool enabled)
    {
    }

    /// <summary>
    /// 在需要更新的地方，更新状态， 更新屏幕数据
    /// </summary>
    public abstract void UpdateState();

    public abstract void UpdateScreenData();

    public virtual  float GetSeparation()
    {
        return 0.0f;
    }

    public virtual  void stopHidService()
    {
    }

    public virtual  void startHidService()
    {
    }

   

    public virtual  void InitForEye(ref Material mat)
    {
    }

    public void ComputeEyesFromProfile()
    {
        PicoVRManager.SDK.leftEyeView = Matrix4x4.identity;
        PicoVRManager.SDK.leftEyeView[0, 3] = -PicoVRManager.SDK.picoVRProfile.device.devLenses.separation / 2;
        float[] rect = PicoVRManager.SDK.picoVRProfile.GetLeftEyeVisibleTanAngles(PicoVRManager.SDK.currentDevice.rtorScren.Width, PicoVRManager.SDK.currentDevice.rtorScren.Height);
        PicoVRManager.SDK.leftEyeProj = MakeProjection(rect[0], rect[1], rect[2], rect[3], 1, 1000);
        rect = PicoVRManager.SDK.picoVRProfile.GetLeftEyeNoLensTanAngles(PicoVRManager.SDK.currentDevice.rtorScren.Width, PicoVRManager.SDK.currentDevice.rtorScren.Height);
        PicoVRManager.SDK.leftEyeUndistortedProj = MakeProjection(rect[0], rect[1], rect[2], rect[3], 1, 1000);
        PicoVRManager.SDK.leftEyeRect = PicoVRManager.SDK.picoVRProfile.GetLeftEyeVisibleScreenRect(rect, PicoVRManager.SDK.currentDevice.rtorScren.Width, PicoVRManager.SDK.currentDevice.rtorScren.Height);

        PicoVRManager.SDK.rightEyeView = PicoVRManager.SDK.leftEyeView;
        PicoVRManager.SDK.rightEyeView[0, 3] *= -1;
        PicoVRManager.SDK.rightEyeProj = PicoVRManager.SDK.leftEyeProj;
        PicoVRManager.SDK.rightEyeProj[0, 2] *= -1;
        PicoVRManager.SDK.rightEyeUndistortedProj = PicoVRManager.SDK.leftEyeUndistortedProj;
        PicoVRManager.SDK.rightEyeUndistortedProj[0, 2] *= -1;
        PicoVRManager.SDK.rightEyeRect = PicoVRManager.SDK.leftEyeRect;
        PicoVRManager.SDK.rightEyeRect.x = 1 - PicoVRManager.SDK.rightEyeRect.xMax;
    }

    public static Matrix4x4 MakeProjection(float l, float t, float r, float b, float n, float f)
    {
        Matrix4x4 m = Matrix4x4.zero;
        m[0, 0] = 2 * n / (r - l);
        m[1, 1] = 2 * n / (t - b);
        m[0, 2] = (r + l) / (r - l);
        m[1, 2] = (t + b) / (t - b);
        m[2, 2] = (n + f) / (n - f);
        m[2, 3] = 2 * n * f / (n - f);
        m[3, 2] = -1;
        return m;
    }

    public virtual void Destroy()
    {
        if (device == this)
        {
            device = null;
        }
    }

    public virtual void StartHeadTrack()
    {
    }

    public virtual void ResetHeadTrack()
    {
    }

    public virtual  void CloseHMDSensor() {  }

    public virtual void OpenHMDSensor()
    {
    }

    public virtual  void IsFocus(bool state)
    {
    }


    public virtual  void StopHeadTrack()
    {
    }

    public virtual  Vector3 GetBoxSensorAcc()
    {
      return  new Vector3(0f,0f,0f);
    }

    public virtual  Vector3 GetBoxSensorGyr()
    {
        return new Vector3(0.0f, 0.0f, 0.0f);
    }

    public virtual void ChangeHeadwear(int headwear)
    {
    }

    public virtual  void playeffect(int effectID, int whichHaptic)
    {
    }

    public virtual  void playEffectSequence(string sequence, int whichHaptic)
    {
    }

    public virtual  void setAudioHapticEnabled(bool enable, int whichHaptic)
    {
    }

    public virtual void stopPlayingEffect(int whichHaptic)
    {
    }

    /*******************************************************************************/

    public virtual void enableTouchPad(bool enable)
    {
    } //触控板是否可用

    public virtual void switchTouchType(int device)
    {
    } //鼠标/触摸板切换

    public virtual int getTouchPadStatus()
    {
        return 0;
    } //获取触摸板类型

    public virtual bool setDeviceProp(int device_id, string value)
    {
        return true;
    }

    public virtual string getDeviceProp(int device_id)
    {
        return "";
    }

    public virtual bool requestHidSensor(int user)
    {
        return false;
    }

    public virtual int getHidSensorUser()
    {
        return 1;
    }

    public virtual bool setThreadRunCore(int pid, int core_id)
    {
        return true;
    }

    public virtual int getThreadRunCore(int pid)
    {
        return 1;
    }

    public virtual bool setSystemRunLevel(int device_id, int level)
    {
        return true;
    }

    public virtual int getSystemRunLevel(int device_id)
    {
        return 1;
    }

    /*****************************音量亮度*************************************/

    public virtual bool initBatteryVolClass()
    {
        return true;
    }

    public virtual bool startAudioReceiver()
    {
        return true;
    }

    public virtual bool startBatteryReceiver()
    {
        return true;
    }

    public virtual bool stopAudioReceiver()
    {
        return true;
    }

    public virtual bool stopBatteryReceiver()
    {
        return true;

    }

    public virtual int getMaxVolumeNumber()
    {
        return 1;
    }

    public virtual int getCurrentVolumeNumber()
    {
        return 1;
    }

    public virtual bool volumeUp()
    {
        return true;
    }

    public virtual bool volumeDown()
    {
        return true;
    }

    public virtual bool setVolumeNum(int volume)
    {
        return true;
    }


    public virtual bool setBrightness(int brightness)
    {
        return true;
    }

    public virtual int getCurrentBrightness()
    {
        return 1;
    }

    /****************************音量亮度*********************************************/

	/****************************BLE蓝牙*********************************************/


			public EventHandler FindBLEDeviceEvent;
			public EventHandler NotFindBLEDeviceEvent;
			public EventHandler BLEActionEvent;
			public EventHandler BLEVersionChangedEvent;
			public EventHandler BluetoothStateChangedEvent;
			public EventHandler BLEConnectedStateChangedEvent;



	public virtual bool OpenBLECentral()
	{
			return false;
	}
	public virtual bool StopBLECentral()
	{
			return false;
	}
	public virtual bool ScanBLEDevice()
	{
			return false;
	}
	public virtual bool ConnectBLEDevice(string mac)
	{
			return false;
	}

    public virtual bool setDevicePropForUser(PicoVRConfigProfile.DeviceCommand deviceid, string number)
    {
        return false;
    }
    public virtual string getDevicePropForUser(PicoVRConfigProfile.DeviceCommand deviceid)
    {
        return 1+"";
    }

    public virtual string getDeviceModelName()
    {
        return "";
    }

    public virtual bool IsBluetoothOpened()
	{
		return false;
	}

	public virtual int GetBluetoothState()
	{
		return 0;
	}

	public virtual int GetBLEConnectState(){
		return 0;
	}

	public virtual string GetBLEVersion(){
		return null;
	}

	public virtual void DevicePowerStateChanged (string state){

	}

	public virtual void DeviceConnectedStateChanged (string state){

	}

	public virtual void DeviceFindNotification (string msg){

	}

	public virtual void AcceptDeviceKeycode (string keykode){

	}
	/****************************BLE蓝牙*********************************************/
}
