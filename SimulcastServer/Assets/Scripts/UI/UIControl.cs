using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIControl : MonoBehaviour
{
    /// <summary> movieitem 根节点 </summary>
    [SerializeField] UIGrid mItemRoot;
    /// <summary> movieitem原型 </summary>
    [SerializeField] GameObject mItemOrigin;
    /// <summary> 播放器 </summary>
    [SerializeField] AVProWindowsMediaMovie mPlayer;
    /// <summary> 播放器 </summary>
    [SerializeField] GameObject mScreen;

    /// <summary>菜单界面</summary>
    private GameObject UI_Menu;
    /// <summary>播放视频界面</summary>
    private GameObject UI_VideoView;
    /// <summary>视频播放和暂停按钮</summary>
    private UIButton palyBtn;
    /// <summary>在线数量</summary>
    private UILabel onlineCount;

    private Server server;

    /// <summary>视频状态</summary>
    private bool playStatus = false;

    public static UIControl instance;

    /// <summary> 当前视屏 </summary>
    private MovieData mCurrentMovie;


    void Awake()
    {
        instance = this;
        server = Camera.main.GetComponent<Server>();
        UI_Menu = this.transform.Find("Panel_Menu").gameObject;
        UI_VideoView = this.transform.Find("Panel_VideoView").gameObject;
        onlineCount = UI_Menu.transform.Find("OnlineCount").GetComponent<UILabel>();
        ShowMenu(true);
        palyBtn = this.transform.Find("Panel_VideoView/Btn_PlayAndStop").GetComponent<UIButton>();


        CreateItemList();
        InitClickEvent();
    }
    /// <summary>初始化按钮点击事件</summary>
    void InitClickEvent()
    {
        foreach (var item in transform.GetComponentsInChildren<UIButton>(true))
        {
            if (item.name.Contains("Btn_"))
            {
                UIEventListener.Get(item.gameObject).onClick += OnClickBtn;
            }
        }
    }


    void CreateItemList()
    {
        ResLoader.Instance.LoadConfig();

        for (int i = 0; i < ResLoader.Instance.DataList.Count; i++)
        {
            GameObject go = Instantiate(mItemOrigin);
            Transform tran = go.transform;
            tran.parent = mItemRoot.transform;
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one;
            go.SetActive(true);

            MovieItem item = go.GetComponent<MovieItem>();
            item.SetData(ResLoader.Instance.DataList[i]);
            item.click = ItemClick;
        }
    }

    void ItemClick(MovieData data)
    {
        ChangePlayBtn(true);
        server.SendData("play", data.VideoName);
        ShowMenu(false);
        Play(data);
        playStatus = true;
    }

    void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            RefreshOnlineCount(server.GetOnlineCount);
        }
    }


    /// <summary>刷新在线数量</summary>
    public void RefreshOnlineCount(int count)
    {
        onlineCount.text = "在线数量：" + count;
    }

    void OnClickBtn(GameObject target)
    {
        switch (target.name)
        {
            case "Btn_Exit":
                Application.Quit();
                break;
            case "Btn_PlayAndStop":
                if (playStatus)
                    Pause();
                else
                    Play(null);
                server.SendData(playStatus ? "pause" : "play", mCurrentMovie.VideoName);
                playStatus = !playStatus;
                ChangePlayBtn(playStatus);
                break;
            case "Btn_Back":
                ShowMenu(true);
                Stop();
                server.SendData("stop", mCurrentMovie.VideoName);
                break;
            default:
                break;
        }
    }

    void Play(MovieData data)
    {
        if (data != null)
        {
            mCurrentMovie = data;
            mPlayer.UnloadMovie();
            mPlayer._folder = "Data";
            mPlayer._filename = mCurrentMovie.VideoPath;
            mPlayer.LoadMovie(true);
            mPlayer._loop = true;
        }
        else
        {
            mPlayer.Play();
        }
    }

    void Pause()
    {
        mPlayer.Pause();
    }
    void Stop()
    {
        mPlayer.Pause();
    }


    /// <summary>显示界面状态</summary>
    void ShowMenu(bool isShow)
    {
        UI_Menu.SetActive(isShow);
        UI_VideoView.SetActive(!isShow);
        mScreen.SetActive(!isShow);
    }

    /// <summary>改变播放按钮的显示状态</summary>
    void ChangePlayBtn(bool isPlay)
    {
        if (isPlay)
        {
            palyBtn.normalSprite = "JS_pause";
            //palyBtn.hoverSprite = "暂停按钮2";
            //palyBtn.pressedSprite = "暂停按钮3";
        }
        else
        {
            palyBtn.normalSprite = "JS_play";
            //palyBtn.hoverSprite = "播放按钮2";
            //palyBtn.pressedSprite = "播放按钮3";
        }
    }
}
