using UnityEngine;
using System.Collections;
using System.IO;

public class MovieItem : MonoBehaviour
{
    public delegate void ItemClickCallBack(MovieData data);
    public ItemClickCallBack click = null;
    [SerializeField]
    UILabel mMovieName;
    [SerializeField]
    UITexture mCovTexture;

    public void SetData(MovieData data)
    {
        mMovieName.text = data.VideoTitle;
        StartCoroutine(LoadTexture(data.CoverPath));

        UIEventListener.Get(gameObject).onClick = (GameObject go) =>
        {
            if (click != null)
            {
                click(data);
            }
        };
    }

    private IEnumerator LoadTexture(string path)
    {
        path = Path.Combine(ResLoader.Instance.GetRootPath(), path);
        WWW www = new WWW("file:///" + path);
        yield return www;

        if (www.error == null)
        {
            byte[] data = www.bytes;

            Texture2D mtemTex = new Texture2D(554, 312, TextureFormat.RGBA32, true);
            mtemTex.LoadImage(data);
            mtemTex.filterMode = FilterMode.Bilinear;
            mtemTex.wrapMode = TextureWrapMode.Clamp;
            mtemTex.Apply();

            mCovTexture.mainTexture = mtemTex;
        }
        else
        {
            Debug.LogError("LoadTexture error:" + www.error.ToString());
        }
    }
}
