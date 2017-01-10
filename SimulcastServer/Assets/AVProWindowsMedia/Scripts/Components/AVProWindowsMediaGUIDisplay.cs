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
// Copyright 2012-2015 RenderHeads Ltd.  All rights reserverd.
//-----------------------------------------------------------------------------

[AddComponentMenu("AVPro Windows Media/IMGUI Display")]
[ExecuteInEditMode]
public class AVProWindowsMediaGUIDisplay : MonoBehaviour
{
	public AVProWindowsMediaMovie _movie;

	public ScaleMode _scaleMode = ScaleMode.ScaleToFit;
	public Color _color = Color.white;
	public bool _alphaBlend = false;
	
	public bool _fullScreen = true;
	public int  _depth = 0;	
	public float _x = 0.0f;
	public float _y = 0.0f;
	public float _width = 1.0f;
	public float _height = 1.0f;
	
	//-------------------------------------------------------------------------
	
	public void OnGUI()
	{
		if (_movie == null)
			return;
		
		if (_movie.OutputTexture != null)
		{
			if (!_alphaBlend || _color.a > 0)
			{
				GUI.depth = _depth;
				GUI.color = _color;

				Rect rect = GetRect();

				Material conversionMaterial = _movie.MovieInstance.GetConversionMaterial();

				if (conversionMaterial == null)
				{
					if (_movie.MovieInstance.RequiresFlipY)
					{
						GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, rect.y + (rect.height / 2)));
						//Matrix4x4 offset = Matrix4x4.TRS(new Vector3(0f, -rect.height / 2, 0f), Quaternion.identity, Vector3.one);
						//GUI.matrix = Matrix4x4.TRS(new Vector3(0f, rect.height / 2, 0f), Quaternion.identity, new Vector3(1f, -1f, 1f)) * offset;
						//rect.height = -rect.height;
						//rect.width = -rect.width;
					}

					GUI.DrawTexture(rect, _movie.OutputTexture, _scaleMode, _alphaBlend);
				}
				else
				{
					if (Event.current.type.Equals(EventType.Repaint))
					{
						//Graphics.DrawTexture(rect, _movie.OutputTexture, conversionMaterial);
					}
				}
			}
		}
	}

	public Rect GetRect()
	{
		Rect rect;
		if (_fullScreen)
			rect = new Rect(0.0f, 0.0f, Screen.width, Screen.height);
		else
			rect = new Rect(_x * (Screen.width-1), _y * (Screen.height-1), _width * Screen.width, _height * Screen.height);

		return rect;
	}
}
