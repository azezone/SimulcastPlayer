/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

#if UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_5
	#define UNITY_FEATURE_UGUI
#endif

using UnityEngine;
using UnityEditor;
#if UNITY_FEATURE_UGUI
using UnityEngine.UI;
using UnityEditor.UI;

/// <summary>
/// Editor class used to edit UI Images.
/// </summary>
[CustomEditor(typeof(AVProWindowsMediaUGUIComponent), true)]
[CanEditMultipleObjects]
public class AVProWindowsMediaUGUIComponentEditor : GraphicEditor
{
    SerializedProperty m_Movie;
    SerializedProperty m_UVRect;
	SerializedProperty m_DefaultTexture;
    GUIContent m_UVRectContent;

	public override bool RequiresConstantRepaint()
	{
		AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
		return (rawImage != null && rawImage.HasValidTexture());
	}

    protected override void OnEnable()
    {
        base.OnEnable();

        // Note we have precedence for calling rectangle for just rect, even in the Inspector.
        // For example in the Camera component's Viewport Rect.
        // Hence sticking with Rect here to be consistent with corresponding property in the API.
        m_UVRectContent = new GUIContent("UV Rect");

        m_Movie = serializedObject.FindProperty("m_movie");
        m_UVRect = serializedObject.FindProperty("m_UVRect");
		m_DefaultTexture = serializedObject.FindProperty("_defaultTexture");

        SetShowNativeSize(true);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_Movie);
		EditorGUILayout.PropertyField(m_DefaultTexture);
		AppearanceControlsGUI();
        EditorGUILayout.PropertyField(m_UVRect, m_UVRectContent);
        SetShowNativeSize(false);
        NativeSizeButtonGUI();

        serializedObject.ApplyModifiedProperties();

		AVProWindowsMediaUGUIComponent component = target as AVProWindowsMediaUGUIComponent;

		if (component.m_movie != null) 
		{
			if (component.m_movie.MovieInstance == null)
			{
				if (GUILayout.Button("Load"))
				{
					component.m_movie.LoadMovie(true);
				}
			}
			else
			{
				EditorUtility.SetDirty(component.m_movie);
				if (GUILayout.Button("Unload"))
				{
					component.m_movie.UnloadMovie();
				}
			}
		}
    }

    void SetShowNativeSize(bool instant)
    {
        base.SetShowNativeSize(m_Movie.objectReferenceValue != null, instant);
    }

    /// <summary>
    /// Allow the texture to be previewed.
    /// </summary>

    public override bool HasPreviewGUI()
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
        return rawImage != null;
    }

    /// <summary>
    /// Draw the Image preview.
    /// </summary>

	public override void OnPreviewGUI(Rect drawArea, GUIStyle background)
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;
        Texture tex = rawImage.mainTexture;

        if (tex == null)
            return;

		// Create the texture rectangle that is centered inside rect.
		Rect outerRect = drawArea;

		Matrix4x4 m = GUI.matrix;
		// Flip the image vertically
		if (rawImage.HasValidTexture())
		{
			if (rawImage.m_movie.MovieInstance.RequiresFlipY)
			{
				GUIUtility.ScaleAroundPivot(new Vector2(1f, -1f), new Vector2(0, outerRect.y + (outerRect.height / 2)));
			}
		}

		EditorGUI.DrawTextureTransparent(outerRect, tex, ScaleMode.ScaleToFit);//, outer.width / outer.height);
        //SpriteDrawUtility.DrawSprite(tex, rect, outer, rawImage.uvRect, rawImage.canvasRenderer.GetColor());

		GUI.matrix = m;
    }

    /// <summary>
    /// Info String drawn at the bottom of the Preview
    /// </summary>

    public override string GetInfoString()
    {
        AVProWindowsMediaUGUIComponent rawImage = target as AVProWindowsMediaUGUIComponent;

		string text = string.Empty;
		if (rawImage.HasValidTexture())
		{
			text += string.Format("Video Size: {0}x{1}\n",
			                        Mathf.RoundToInt(Mathf.Abs(rawImage.mainTexture.width)),
			                        Mathf.RoundToInt(Mathf.Abs(rawImage.mainTexture.height)));
		}

        // Image size Text
		text += string.Format("Display Size: {0}x{1}",
                Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.width)),
                Mathf.RoundToInt(Mathf.Abs(rawImage.rectTransform.rect.height)));

        return text;
    }
}
#endif
