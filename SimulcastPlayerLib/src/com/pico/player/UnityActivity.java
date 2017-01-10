package com.pico.player;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;
import java.lang.reflect.Array;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.Dictionary;
import java.util.Hashtable;
import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import android.R.string;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.Service;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Environment;
import android.os.PowerManager;
import android.os.PowerManager.WakeLock;
import android.os.storage.StorageManager;
import android.util.Log;
import com.unity3d.player.UnityPlayerNativeActivityPico;

public class UnityActivity extends UnityPlayerNativeActivityPico {
	public static Activity unityActivity = null;
	private String TAG = "UnityActivity";
	public static String config_path = "";
	public static String mMrl = "";
	
	public static String SSID = "AP01";
	public static String PSWD = "22222222";
	public static String IP = "192.168.1.2";
	public static String Port = "8081";

	protected void onCreate(Bundle savedInstanceState) {
		Log.i(TAG, "UnityActivity onCreate called.");
		super.onCreate(savedInstanceState);
		unityActivity = this;
	}

	private boolean isServiceStarted = false;
	public void StartAutoConnectService() {
		Log.i(TAG, "----------StartAutoConnectService---------");
		if (isServiceStarted)
			return;
		Intent intent = new Intent(UnityActivity.this, AutoConnectService.class);
		startService(intent);
		isServiceStarted = true;
	}

	/*
	 * return：0 file not exit;1 config file read success;2 config is not legal;3
	 * element lose;4 ip not legal;5 port not legal;6 other error;
	 */
	public int ReadConfig() {
		//String sdcard = getStoragePath(this, true);
		config_path = Environment.getExternalStorageDirectory().getAbsolutePath() + "/pre_resource/config/config.txt";
		mMrl = Environment.getExternalStorageDirectory().getAbsolutePath()+"/pre_resource/video/";

//		Log.i(TAG, "configpath:" + config_path);
//		Log.i(TAG, "mMrl:" + mMrl);
		if (!CheckFileExit(config_path)) {
			return 0;
		}
		try {
			File urlFile = new File(config_path);
			InputStreamReader isr = new InputStreamReader(new FileInputStream(
					urlFile), "UTF-8");
			BufferedReader br = new BufferedReader(isr);
			String mimeTypeLine = "";
			List<String> config_listList = new LinkedList<String>();
			Dictionary<String, String> configDict = new Hashtable<String, String>();
			while ((mimeTypeLine = br.readLine()) != null) {
				config_listList.add(mimeTypeLine);
			}
			if (config_listList.size() >= 1) {
				for (int i = 0; i < config_listList.size(); i++) {
					String string = config_listList.get(i);
					if (string != null) {
						string = string.replace(" ", "");
						String[] params = string.split(":");
						if (params.length == 2) {
							configDict.put(params[0], params[1]);
						}
					}
				}
				if (configDict.size() == 4) {
					if (configDict.get("ssid") == null
							|| configDict.get("pswd") == null
							|| configDict.get("serverip") == null
							|| configDict.get("port") == null) {
						return 3;
					} else {
						SSID = configDict.get("ssid");
						PSWD = configDict.get("pswd");
						IP = configDict.get("serverip");
						Port = configDict.get("port");
						//Log.i(TAG, "SSID:"+SSID+" PSWD:"+PSWD+" IP:"+IP+" Port:"+Port);
						if (!isValidIP(IP)) {
							return 4;
						}
						if (!isInteger(Port)) {
							return 5;
						}
						return 1;
					}
				} else {
					return 2;
				}
			} else {
				return 2;
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		return 6;
	}

	public boolean GetWifiConnectedState() {
		WifiManager wifi = (WifiManager) getSystemService(Context.WIFI_SERVICE);
		if (wifi == null)
			return false;
		WifiInfo info = wifi.getConnectionInfo();
		if (info == null)
			return false;
		if (wifi.getWifiState() == WifiManager.WIFI_STATE_ENABLED) {
			return info.getSSID().equals(
					AutoConnectService.convertToQuotedString(SSID));
		} else {
			return false;
		}
	}

	public boolean isVideoFileExist(String videoname) {
		//新增先判断sd卡,如果没有再判断根目录
		if(CheckFileExit(mMrl+videoname+".mp4"))
		{
		  return true;
		}
		else {
			return false;
		}
	}

	public boolean CheckFileExit(String path) {
		boolean flag = false;
		File file = new File(path);
		flag = file.exists();
		return flag;
	}
	
	public String getMediaPath(String videoname) {
		File file;
		if(CheckFileExit(mMrl+videoname+".mp4"))
		{
			file = new File(mMrl+videoname+".mp4");
			return Uri.fromFile(file).toString();
		}
		else {
			return null;
		}
	}

	private String getStoragePath(Context mContext, boolean is_removale) {
		StorageManager mStorageManager = (StorageManager) mContext
				.getSystemService(Context.STORAGE_SERVICE);
		Class<?> storageVolumeClazz = null;
		try {
			storageVolumeClazz = Class
					.forName("android.os.storage.StorageVolume");
			Method getVolumeList = mStorageManager.getClass().getMethod(
					"getVolumeList");
			Method getPath = storageVolumeClazz.getMethod("getPath");
			Method isRemovable = storageVolumeClazz.getMethod("isRemovable");
			Object result = getVolumeList.invoke(mStorageManager);
			final int length = Array.getLength(result);
			for (int i = 0; i < length; i++) {
				Object storageVolumeElement = Array.get(result, i);
				String path = (String) getPath.invoke(storageVolumeElement);
				boolean removable = (Boolean) isRemovable
						.invoke(storageVolumeElement);
				if (is_removale == removable) {
					return path;
				}
			}
		} catch (ClassNotFoundException e) {
			e.printStackTrace();
		} catch (InvocationTargetException e) {
			e.printStackTrace();
		} catch (NoSuchMethodException e) {
			e.printStackTrace();
		} catch (IllegalAccessException e) {
			e.printStackTrace();
		}
		return null;
	}
	

	public String getKeyValue(String key) {
		Log.i(TAG, "the key is :"+key);
		if (key.equals("ssid"))
			return SSID;
		else if (key.equals("pswd"))
			return PSWD;
		else if (key.equals("serverip"))
			return IP;
		else if (key.equals("port"))
			return Port;
		return "unknow";
	}

	public static boolean isValidIP(String address) {
		String ip = "^(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|[1-9])\\."
                + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
                + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
                + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)$";
        Pattern pattern = Pattern.compile(ip);
        Matcher matcher = pattern.matcher(address);
        return matcher.matches();
	}

	public static boolean isInteger(String input) {
		Matcher mer = Pattern.compile("^[+-]?[0-9]+$").matcher(input);
		return mer.find();
	}
	
	public void ShowSettingPage() {
		Log.i(TAG, "ShowSettingPage******");
		Intent in = new Intent(Intent.ACTION_MAIN);
		in.addCategory(Intent.CATEGORY_LAUNCHER);
		ComponentName componentName = new ComponentName("com.android.settings","com.android.settings.Settings");
		in.setComponent(componentName);
		in.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
		startActivity(in);
	}
	
	public void ShowFileManager() {
		Log.i(TAG, "ShowFileManager******");
		Intent in = new Intent(Intent.ACTION_MAIN);
		in.addCategory(Intent.CATEGORY_LAUNCHER);
		ComponentName componentName = new ComponentName("com.estrongs.android.pop","com.estrongs.android.pop.view.FileExplorerActivity");
		in.setComponent(componentName);
		in.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_RESET_TASK_IF_NEEDED);
		startActivity(in);
	}

	public void onDestroy() {
		super.onDestroy();
	}
}
