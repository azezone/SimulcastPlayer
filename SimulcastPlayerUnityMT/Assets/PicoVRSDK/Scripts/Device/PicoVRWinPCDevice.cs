﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class PicoVRWinPCDevice : PicoVRBaseDevice
{


    public struct pvrVector3
    {
        public double x;
        public double y;
        public double z;
    };

    public struct pvrQuaternion
    {
        public double w;
        public double x;
        public double y;
        public double z;
    };
    Quaternion rot = new Quaternion();
    pvrQuaternion rot_ori = new pvrQuaternion();
    private Vector3 newpos=  new Vector3(0, 0, 0);
    public bool ResetTrackerOnLoad = true;  // if off, tracker will not reset when new scene
    private static bool PVRInit = false;
    public static int HResolution, VResolution = 0;	 // pixels
    // ResetOrientation
    public static bool ResetOrientation(int sensor)
    {
        return true;
    }

    public static float VerticalFOV()
    {
        float fov = 0.0f;
        PVR_GetFOV(ref fov);
        return fov;
    }
    public PicoVRWinPCDevice()
    {
        PicoVRManager.SDK.VRModeEnabled = false;
        PVRInit = PVR_Init();
        if (PVRInit == false)
            return;
        PicoVRManager.SDK.VRModeEnabled = isHMDpresent();
        SetQuality();
        for (int i = 0; i < eyeTextureCount - 4; i++)
        {
            if (null == eyeTextures[i])
            {
                try
                {
                    ConfigureEyeTexture(i);
                }
                catch (Exception)
                {
                    Debug.LogError("ERROR");
                    throw;
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eyeTextureIndex"></param>
    private void ConfigureEyeTexture(int eyeTextureIndex)
    {
        Vector2 renderTexSize = GetStereoScreenSize();
        
        int x = (int)renderTexSize.x / 2;
        int y = (int)renderTexSize.y;
        
        eyeTextures[eyeTextureIndex] = new RenderTexture(x, y, (int)PicoVRManager.SDK.RtBitDepth, PicoVRManager.SDK.RtFormat);
        eyeTextures[eyeTextureIndex].anisoLevel = 0;
        eyeTextures[eyeTextureIndex].antiAliasing = Mathf.Max(QualitySettings.antiAliasing, (int)PicoVRManager.SDK.RtAntiAlising);


        eyeTextures[eyeTextureIndex].Create();
        if (eyeTextures[eyeTextureIndex].IsCreated())
        {
            eyeTextureIds[eyeTextureIndex] = eyeTextures[eyeTextureIndex].GetNativeTexturePtr().ToInt32();
            Debug.Log("eyeTextureIndex : " + eyeTextureIndex.ToString());
        }

    }
    public void ReleaseEyeTextures()
    {
        for (int i = 0; i < 2; ++i)
        {
            if (eyeTextures[i])
            {
                eyeTextures[i].Release();
                eyeTextureIds[i] = 0;
            }
        }
    }
    public void SetQuality()
    {
        QualitySettings.softVegetation = true;
        QualitySettings.maxQueuedFrames = -1;
        QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        QualitySettings.vSyncCount = 0;
    }

 
    public void _Destroy()
    {
        // We may want to turn this off so that values are maintained between level / scene loads
        if (ResetTrackerOnLoad == true)
        {
            PVR_Shutdown();
            PVRInit = false;
            ReleaseEyeTextures();
        }
    }
    public override void Destroy()
    {

        try
        {
            _Destroy();
            base.Destroy();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }
    public bool isHMDpresent()
    {
        
        return PVRInit;
    }
    public override Vector2 GetStereoScreenSize()
    {
    
        PVR_GetScreenResolution(ref HResolution, ref VResolution);

        return new Vector2(HResolution, VResolution);
    }
    //upadata sensor data
    public override void UpdateState()
    {
        
        PVR_GetTrackedPose(ref rot_ori);
        rot.y = -(float)rot_ori.y;
        rot.x = -(float)rot_ori.x;
        rot.w = (float)rot_ori.w;
        rot.z = (float)rot_ori.z;
        PicoVRManager.SDK.headPose.Set(newpos, rot);

         if (Input.GetKeyDown(KeyCode.Escape))
         {
             try
             {
                 Application.Quit();
             }
             catch (Exception e)
             {
                 Debug.LogError(e.ToString());
             }
             
         }
    }

    public override void UpdateScreenData()
    {
        PicoVRManager.SDK.eyeFov = VerticalFOV();
        PicoVRManager.SDK.leftEyeView = Matrix4x4.identity;
        PicoVRManager.SDK.leftEyeView[0, 3] = -PicoVRManager.SDK.picoVRProfile.device.devLenses.separation / 2;
        PicoVRManager.SDK.rightEyeView = PicoVRManager.SDK.leftEyeView;
        PicoVRManager.SDK.rightEyeView[0, 3] *= -1;
    }
    public override bool requestHidSensor(int user)
    {
        //PVR_Shutdown();
        PVRInit = false;
        return true;
    }

#region PC plugin
    public const string dllFileName = "PicoPlugin";
    [DllImport(dllFileName)]
    private static extern bool PVR_Init();
    [DllImport(dllFileName)]
    private static extern bool PVR_Shutdown();
    [DllImport(dllFileName)]
    private static extern bool PVR_GetTrackedPose(ref pvrQuaternion pose);
    [DllImport(dllFileName)]
    private static extern bool PVR_GetTrackedPosition(ref pvrVector3 position);
    [DllImport(dllFileName)]
    private static extern bool PVR_GetScreenResolution(ref int hResolution, ref int vResolution);
    [DllImport(dllFileName)]
    private static extern bool PVR_GetFOV(ref float fov);
#endregion
}
