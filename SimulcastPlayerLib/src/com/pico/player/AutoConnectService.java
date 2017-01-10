package com.pico.player;

import java.util.List;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.net.wifi.ScanResult;
import android.net.wifi.WifiConfiguration;
import android.net.wifi.WifiInfo;
import android.net.wifi.WifiManager;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;

public class AutoConnectService extends Service {

	private WifiManager mWifiManager;
	private WifiInfo mCurrentWifiInfo;
	private IntentFilter mFilter;
	private Scanner mScanner;

	/*
	 * private String mSsid = "PICOTOYOTA"; private String mPswd = "22222222";
	 */

	@Override
	public IBinder onBind(Intent intent) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public void onCreate() {
		// TODO Auto-generated method stub
		super.onCreate();
		Log.i("my wifi", "AutoConnectService ------ onCreate");
		mWifiManager = (WifiManager) getSystemService(Context.WIFI_SERVICE);
		mScanner = new Scanner();
		mFilter = new IntentFilter();
		mFilter.addAction(WifiManager.WIFI_STATE_CHANGED_ACTION);
		mFilter.addAction(WifiManager.SCAN_RESULTS_AVAILABLE_ACTION);
		mFilter.addAction(WifiManager.NETWORK_IDS_CHANGED_ACTION);
		mFilter.addAction(WifiManager.SUPPLICANT_STATE_CHANGED_ACTION);
		mFilter.addAction(WifiManager.NETWORK_STATE_CHANGED_ACTION);
		mFilter.addAction(WifiManager.RSSI_CHANGED_ACTION);
		registerReceiver(mWifiReceiver, mFilter);
		if (mWifiManager != null && !mWifiManager.isWifiEnabled()) {
			mWifiManager.setWifiEnabled(true);
		}
	}

	private BroadcastReceiver mWifiReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			// TODO Auto-generated method stub
			Log.i("my wifi",
					"wifi BroadcastReceiver intent" + intent.getAction());
			if (WifiManager.WIFI_STATE_CHANGED_ACTION
					.equals(intent.getAction())) {
				updateWifiState(intent.getIntExtra(
						WifiManager.EXTRA_WIFI_STATE,
						WifiManager.WIFI_STATE_UNKNOWN));
			} else {
				updateAccessPoints();
			}
		}
	};

	private void updateAccessPoints() {
		final int wifiState = mWifiManager.getWifiState();
		switch (wifiState) {
		case WifiManager.WIFI_STATE_ENABLED:
			Log.i("my wifi", "updateAccessPoints:WIFI_STATE_ENABLED");
			mCurrentWifiInfo = mWifiManager.getConnectionInfo();
			Log.i("my wifi",
					"updateAccessPoints ssid:" + mCurrentWifiInfo.getSSID());
			constructAccessPoints();
			break;
		case WifiManager.WIFI_STATE_ENABLING:
			break;
		case WifiManager.WIFI_STATE_DISABLING:
			break;
		case WifiManager.WIFI_STATE_DISABLED:
			Log.i("my wifi", "WIFI_STATE_DISABLED");
			if (mWifiManager != null && !mWifiManager.isWifiEnabled()) {
				mWifiManager.setWifiEnabled(true);
			}
			break;
		}
	}

	private void constructAccessPoints() {
		final List<ScanResult> results = mWifiManager.getScanResults();
		if (results != null) {
			Log.i("my wifi", "constructAccessPoints" + results.size());
			for (ScanResult result : results) {
				if (result.SSID == null || result.SSID.length() == 0
						|| result.capabilities.contains("[IBSS]")) {
					continue;
				}
				if (result.SSID.equals(UnityActivity.SSID)
						&& !mCurrentWifiInfo.getSSID().equals(
								convertToQuotedString(UnityActivity.SSID))) {
					Log.i("my wifi", "----------wifi--->ssid:"
							+ UnityActivity.SSID + "  pswd:"
							+ UnityActivity.PSWD);
					connectWifi();
//					Log.i("my wifi", "--------------------------wifi state:"
//							+ isSuccess);
					break;
				}
			}
		} else {
			Log.i("my wifi", "wifi list is null!");
		}
	}

	private void connectWifi() {
		Log.i("my wifi", "--------------------------connectwifi");
		new Thread(new Runnable() {
			@Override
			public void run() {
				// TODO Auto-generated method stub
				
				WifiConfiguration conf = new WifiConfiguration();
				conf.SSID = convertToQuotedString(UnityActivity.SSID);
				conf.preSharedKey = convertToQuotedString(UnityActivity.PSWD);
				conf.hiddenSSID = true;
				conf.status = WifiConfiguration.Status.ENABLED;
				conf.allowedGroupCiphers.set(WifiConfiguration.GroupCipher.TKIP);
				conf.allowedGroupCiphers.set(WifiConfiguration.GroupCipher.CCMP);
				conf.allowedKeyManagement.set(WifiConfiguration.KeyMgmt.WPA_PSK);
				conf.allowedPairwiseCiphers.set(WifiConfiguration.PairwiseCipher.TKIP);
				conf.allowedPairwiseCiphers.set(WifiConfiguration.PairwiseCipher.CCMP);
				conf.allowedProtocols.set(WifiConfiguration.Protocol.RSN);
				int id = mWifiManager.addNetwork(conf);
				mWifiManager.enableNetwork(id, true);
			}
		}).start();
	}

	public static String convertToQuotedString(String string) {
		return "\"" + string + "\"";
	}

	private void updateWifiState(int state) {
		Log.i("my wifi", "updateWifiState---------------->" + state);
		switch (state) {
		case WifiManager.WIFI_STATE_ENABLED:
			mScanner.resume();
			return;
		}
		mScanner.pause();
	}

	private class Scanner extends Handler {
		private int mRetry = 0;

		void resume() {
			if (!hasMessages(0)) {
				Log.i("my wifi", "sendMessage---to---handler");
				sendEmptyMessage(0);
			} else {
				Log.i("my wifi", "Handler..........no.........sendMessage");
			}
		}

		void forceScan() {
			removeMessages(0);
			sendEmptyMessage(0);
		}

		void pause() {
			mRetry = 0;
			removeMessages(0);
		}

		@Override
		public void handleMessage(Message message) {
			if (mWifiManager.startScan()) {
				Log.i("my wifi", "startScan android.net.wifi.SCAN_RESULTS");
				mRetry = 0;
			} else if (++mRetry >= 3) {
				mRetry = 0;
				Log.i("my wifi", "鎵弿澶辫触浜�...................");
				return;
			}
			sendEmptyMessageDelayed(0, 20 * 1000);
		}
	}

	@Override
	public void onDestroy() {
		// TODO Auto-generated method stub
		super.onDestroy();
		Log.i("my wifi", "AutoConnectService ------ onDestroy");
		if (mWifiReceiver != null) {
			unregisterReceiver(mWifiReceiver);
		}
	}

}
