/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

// Support for Editor.RequiresConstantRepaint()
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2012-2015 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

[CustomEditor(typeof(AVProWindowsMediaManager))]
public class AVProWindowsMediaManagerEditor : Editor
{
	private AVProWindowsMediaManager _manager;
	private AVProWindowsMediaMovie[] _movies;

	private void UpdateMovies()
	{
		_movies = (AVProWindowsMediaMovie[])FindObjectsOfType(typeof(AVProWindowsMediaMovie));
	}

#if AVPROWINDOWSMEDIA_UNITYFEATURE_EDITORAUTOREFRESH
	public override bool RequiresConstantRepaint ()
	{
		return (_movies != null);
	}
#endif

	public override void OnInspectorGUI()
	{
		_manager = (this.target) as AVProWindowsMediaManager;

		if (!Application.isPlaying)
		{
			this.DrawDefaultInspector();
		}

		/*if (!Application.isPlaying) 
		{
			_manager._useExternalTextures = GUILayout.Toggle(_manager._useExternalTextures, "Use External Textures (beta)");
		}*/

		if (GUILayout.Button ("Update"))
		{
			UpdateMovies();
		}

		if (_movies != null && _movies.Length > 0)
		{
			for (int i = 0; i < _movies.Length; i++)
			{
				GUILayout.BeginHorizontal();

				GUI.color = Color.white;
				if (!_movies[i].enabled || !_movies[i].gameObject.activeInHierarchy)
					GUI.color = Color.grey;

				AVProWindowsMedia media = _movies[i].MovieInstance;
				if (media != null)
				{
					GUI.color = Color.yellow;
					if (media.IsPlaying)
						GUI.color = Color.green;
				}

				if (GUILayout.Button("S"))
				{
					Selection.activeObject = _movies[i];
				}
				GUILayout.Label(i.ToString("D2") + " " + _movies[i].name, GUILayout.MinWidth(128f));
				//GUILayout.FlexibleSpace();
				if (media != null)
				{
					GUILayout.Label(media.Width + "x" + media.Height);
					GUILayout.FlexibleSpace();
					GUILayout.Label(string.Format("{0:00.0}", media.DisplayFPS) + " FPS");
					//GUILayout.FlexibleSpace();
				}
				else
				{
					GUILayout.FlexibleSpace();
				}



				GUILayout.EndHorizontal();

				if (media != null)
				{
					GUILayout.HorizontalSlider(media.PositionSeconds, 0f, media.DurationSeconds, GUILayout.MinWidth(128f), GUILayout.ExpandWidth(true));
				}
			}
		}
		else
		{
			if (Event.current.type.Equals(EventType.Repaint))
			{
				UpdateMovies();
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(_manager);
		}		
	}
}
