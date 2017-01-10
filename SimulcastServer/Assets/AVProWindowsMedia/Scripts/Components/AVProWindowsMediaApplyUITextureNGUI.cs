/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------
// Copyright 2014-2015 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

#if NGUI
[AddComponentMenu("AVPro Windows Media/Apply to NGUI UITexture")]
public class AVProWindowsMediaApplyUITextureNGUI : MonoBehaviour 
{
	public UITexture _uiTexture;
	public AVProWindowsMediaMovie _movie;
	public Texture2D _defaultTexture;
	private AVProWindowsMediaMovie _currentMovie;
	private static Texture2D _blackTexture;
	[SerializeField] bool _makePixelPerfect = false;
	
	void Awake()
	{
		if (_blackTexture == null)
			CreateTexture();
	}

	void Start()
	{
		if (_defaultTexture == null)
		{
			_defaultTexture = _blackTexture;
		}
		
		Update();
	}

	public void SetNextMovie(AVProWindowsMediaMovie movie)
	{
		_movie = movie;
		Update();
	}

	void Update()
	{
		if (_movie != null)
		{
			if (_movie.OutputTexture != null)
			{
				_currentMovie = _movie;

				if (_movie.MovieInstance.RequiresFlipY)
					_uiTexture.flip = UITexture.Flip.Vertically;

				if (_makePixelPerfect)
				{
					_currentMovie.OutputTexture.filterMode = FilterMode.Point;
					_uiTexture.mainTexture = _currentMovie.OutputTexture;
					_uiTexture.MakePixelPerfect();
				}
				else
				{
					_uiTexture.mainTexture = _currentMovie.OutputTexture;
				}

			}
		}
		else
		{	
			_currentMovie = null;
			_uiTexture.mainTexture = _defaultTexture;
		}
	}
	
	public void OnDisable()
	{
		//_uiTexture.mainTexture = null;
		//_currentMovie = null;
	}

	void OnDestroy()
	{
		_defaultTexture = null;
		
		if (_blackTexture != null)
		{
			Texture2D.Destroy(_blackTexture);
			_blackTexture = null;
		}

		_uiTexture.mainTexture = null;
	}

	private static void CreateTexture()
	{
		_blackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false, false);
		_blackTexture.name = "AVProWindowsMedia-BlackTexture";
		_blackTexture.filterMode = FilterMode.Point;
		_blackTexture.wrapMode = TextureWrapMode.Clamp;
		_blackTexture.SetPixel(0, 0, Color.black);
		_blackTexture.Apply(false, true);
	}
}
#endif
