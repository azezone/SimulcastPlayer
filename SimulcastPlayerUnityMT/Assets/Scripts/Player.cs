using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public GameObject originScence;
    private bool m_bFinish = false;
    public MediaPlayerCtrl scrMedia;
    public static Player instance = null;
    private enum PlayerState
    {
        unStart = 0,
        Ready = 1,
        Playing = 2,
        End = 3
    }
    private PlayerState _playerState = PlayerState.unStart;
    private PlayerState playerState
    {
        get
        {
            return _playerState;
        }
        set
        {
            _playerState = value;
        }
    }
    private bool ISPlaying { get; set; }
    private bool isInited = false;
    private Texture2D mTexture = null;
    public static bool isServiceConnected = false;

    public void InitPlayer()
    {
        if (isInited)
        {
            return;
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            scrMedia.OnEnd += OnEnd;
            instance = this;
            if (gameObject.GetComponent<Client>() == null)
            {
                gameObject.AddComponent<Client>();
            }
            //LoadMeida();
            playerState = PlayerState.Ready;

            //Debug.Log("*********InitBallUV*********");
            //var uv = scrMedia.gameObject.GetComponent<MeshFilter>().mesh.uv;
            //for (int i = 0; i < uv.Length; i++)
            //{
            //    uv[i].x = 1 - uv[i].x;
            //}
            //scrMedia.gameObject.GetComponent<MeshFilter>().mesh.uv = uv;

            isInited = true;
        }
    }

    private void LoadMeida(string videoname)
    {
        Debug.LogError("********load meida********");
        string path = "";
        PicoUnityActivity.CallObjectMethod<string>(ref path, "getMediaPath", videoname);
        Debug.LogError("the media path:" + path);
        scrMedia.Load(path);
        scrMedia.Pause();
        m_bFinish = false;
    }

    void OnEnd()
    {
        m_bFinish = true;
        OpratePlayerReachEnd();
    }

    private void OpratePlayerReachEnd()
    {
        if (playerState != PlayerState.Playing)
        {
            Debug.Log("*********the player state is not Playing*********");
            return;
        }
        playerState = PlayerState.End;
        scrMedia.UnLoad();
        scrMedia.gameObject.SetActive(false);
        originScence.SetActive(true);
    }

    private string CurVideo = string.Empty;
    public void OprateNetCommand(string cmd, string value)
    {
        Debug.Log("the commmand is :" + cmd);
        if (cmd.Equals("play"))
        {
            this.play(value);
        }
        else if (cmd.Equals("pause"))
        {
            this.pause();
        }
        else if (cmd.Equals("stop"))
        {
            this.Stop();
        }
        else if (cmd == "playing")
        {
            Debug.Log("playing state then seek the time:" + value);
            float ms = 0f;
            if (float.TryParse(value, out ms))
            {
                Debug.Log("seek the time:" + ms);
                play(value);
                seek((long)(ms * 1000f));
            }
        }
        else if (cmd == "playing-pause")
        {
            Debug.Log("playing state then seek the time:" + value);
            float ms = 0f;
            if (float.TryParse(value, out ms))
            {
                Debug.Log("seek the time:" + ms);
                play(value);
                seek((long)(ms * 1000f));
                pause();
            }
        }
    }

    public void OprateNetStateChanged(NetState state)
    {
        isServiceConnected = (state == NetState.success);
    }

    #region controle function
    public void ForcePlay()
    {
        //play(CurVideo);
    }

    public void Replay()
    {
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (playerState == PlayerState.Playing || playerState == PlayerState.End)
        //    {
        //        play(CurVideo);
        //        seek(0);
        //    }
        //}
    }

    private void Stop()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (playerState == PlayerState.Playing)
            {
                originScence.SetActive(true);
                scrMedia.UnLoad();
                CurVideo = string.Empty;
                scrMedia.gameObject.SetActive(false);
                playerState = PlayerState.Ready;
            }
        }
    }

    private void play(string videoname)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (playerState != PlayerState.unStart)
            {
                if (!StatePanel.IsVideoFileExist(videoname))
                {
                    Debug.LogError("File not exist!");
                }
                else
                {
                    originScence.SetActive(false);
                    scrMedia.gameObject.SetActive(true);

                    scrMedia.m_bLoop = true;

                    if (CurVideo.Equals(videoname))
                    {
                        scrMedia.Play();
                    }
                    else
                    {
                        Vector3 origin = scrMedia.transform.localScale;
                        scrMedia.transform.localScale = Vector3.zero;
                        Ftimer.AddEvent("show", 0.3f, () =>
                        {
                            scrMedia.transform.localScale = origin;
                            scrMedia.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                        });
                        ReLoad(videoname);
                        scrMedia.Play();
                    }

                    m_bFinish = false;

                    ISPlaying = true;
                    playerState = PlayerState.Playing;
                }
            }
        }
    }

    private void pause()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (playerState == PlayerState.Playing)
            {
                scrMedia.Pause();
                ISPlaying = false;
                playerState = PlayerState.Playing;
            }
        }
    }

    private void seek(long value)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //only in ready or end state can seek the time
            if (playerState == PlayerState.Playing)
            {
                scrMedia.SeekTo((int)value);
            }
        }
    }

    private void ReLoad(string videoname)
    {
        scrMedia.UnLoad();
        string path = "";
        PicoUnityActivity.CallObjectMethod<string>(ref path, "getMediaPath", videoname);
        Debug.LogError("the media path:" + path);
        scrMedia.Load(path);
        CurVideo = videoname;
    }
    #endregion

    public static AndroidJavaObject getActivity()
    {
        AndroidJavaObject obj = null;
        if (Application.platform == RuntimePlatform.Android)
        {
            obj = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        }
        return obj;
    }

    void OnDestroy()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            scrMedia.UnLoad();
        }
    }
}