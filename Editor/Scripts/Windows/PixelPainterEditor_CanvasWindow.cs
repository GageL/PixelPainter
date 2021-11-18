using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LucasIndustries.Editor {
	public class PixelPainterEditor_CanvasWindow {
		#region Public/Private Variables

		#endregion

		#region Runtime Variables

		#endregion

		#region Native Methods

		#endregion

		#region Callback Methods

		#endregion

		#region Static Methods
		public static void DrawWindow() {
			Rect _rect = EditorGUILayout.BeginVertical(GetSkinStyle("Backdrop"));
			{
				if (!string.IsNullOrEmpty(GetCanvasWindowData().CurrentCanvasGuid) && GetCanvasWindowData().CurrentCanvasGuid == GetCanvasWindowData().SelectedCanvasGuid) {
					DrawCurrentCanvasGUI();
				} else {
					DrawSplashGUI();
					if (GUI.Button(_rect, "", GUIStyle.none)) {
						GetCanvasWindowData().SelectedCanvasGuid = string.Empty;
						GUI.FocusControl(null);
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		public static PixelPainterEditor_CanvasWindowData GetCanvasWindowData() {
			return PixelPainterEditorWindow.CachedPixelPainterEditorData.CanvasWindowData;
		}

		public static string GetCanvasWindowDataGuid() {
			return GetCanvasWindowData().CurrentCanvasGuid;
		}

		public static GUIStyle GetSkinStyle(string style) {
			return PixelPainterEditorWindow.CachedPixelPainterEditorData.EditorSkin.GetStyle(style);
		}

		public static PixelPainterEditor_CanvasData GetCanvasData(string guid) {
			return GetCanvasWindowData().Canvases.FirstOrDefault(x => x.Guid == guid);
		}

		private static void DrawSplashGUI() {
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
				{
					GUILayout.FlexibleSpace();
					EditorGUILayout.LabelField("Welcome to Pixel Painter", GetSkinStyle("CanvasWindowSplashLabelHeader"));
					GUILayout.Space(12);
					EditorGUILayout.LabelField("··   · «  ∞  » ·   ··", GetSkinStyle("CanvasWindowSplashLabelHeader"));
					GUILayout.Space(24);
					EditorGUILayout.LabelField("« You can create a new canvas »", GetSkinStyle("CanvasWindowSplashLabelBody"));
					GUILayout.Space(6);
					EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowSplashNewCanvasHolder"), GUILayout.Width(300));
					{
						GUILayout.Space(6);
						EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
						{
							EditorGUILayout.LabelField("Name:", GetSkinStyle("CanvasWindowSplashNewCanvasParam"), GUILayout.Width(84));
							GetCanvasWindowData().NewCanvasName = EditorGUILayout.TextField(GetCanvasWindowData().NewCanvasName, GetSkinStyle("CanvasWindowSplashNewCanvasField"), GUILayout.ExpandWidth(true));
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(4);
						EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
						{
							EditorGUILayout.LabelField("Width:", GetSkinStyle("CanvasWindowSplashNewCanvasParam"), GUILayout.Width(84));
							GetCanvasWindowData().NewCanvasWidth = EditorGUILayout.IntField(GetCanvasWindowData().NewCanvasWidth, GetSkinStyle("CanvasWindowSplashNewCanvasField"), GUILayout.ExpandWidth(true));
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(4);
						EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
						{
							EditorGUILayout.LabelField("Height:", GetSkinStyle("CanvasWindowSplashNewCanvasParam"), GUILayout.Width(84));
							GetCanvasWindowData().NewCanvasHeight = EditorGUILayout.IntField(GetCanvasWindowData().NewCanvasHeight, GetSkinStyle("CanvasWindowSplashNewCanvasField"), GUILayout.ExpandWidth(true));
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(6);
					}
					EditorGUILayout.EndVertical();
					GUILayout.Space(6);
					if (GUILayout.Button("Create", GetSkinStyle("CanvasWindowSplashButtonCreate"))) {
						if (string.IsNullOrEmpty(GetCanvasWindowData().NewCanvasName)) {
							EditorUtility.DisplayDialog("Create Error", "Each canvas needs a name", "Ok");
							return;
						}
						if (GetCanvasWindowData().NewCanvasWidth < 2) {
							EditorUtility.DisplayDialog("Create Error", "The canvas must be 2 or more pixels wide", "Close");
							return;
						}
						if (GetCanvasWindowData().NewCanvasHeight < 2) {
							EditorUtility.DisplayDialog("Create Error", "The canvas must be 2 or more pixels tall", "Close");
							return;
						}
						PixelPainterEditor_CanvasData _data = new PixelPainterEditor_CanvasData(GetCanvasWindowData().NewCanvasName, GetCanvasWindowData().NewCanvasWidth, GetCanvasWindowData().NewCanvasHeight);
						GetCanvasWindowData().Canvases.Add(_data);
						GetCanvasWindowData().ResetNewCanvasData();
						GetCanvasWindowData().SelectedCanvasGuid = _data.Guid;
						GetCanvasWindowData().CurrentCanvasGuid = _data.Guid;
						GUI.FocusControl(null);
					}
					GUILayout.Space(24);
					EditorGUILayout.LabelField("··   -   ··", GetSkinStyle("CanvasWindowSplashLabelBody"));
					GUILayout.Space(24);
					EditorGUILayout.LabelField("« or select an existing canvas »", GetSkinStyle("CanvasWindowSplashLabelBody"));
					GUILayout.Space(6);
					EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowSplashCanvasesHolder"), GUILayout.Width(300), GUILayout.Height(200));
					{
						GUILayout.Space(6);
						GetCanvasWindowData().ExistingCanvasesScroll = EditorGUILayout.BeginScrollView(GetCanvasWindowData().ExistingCanvasesScroll);
						{
							if (GetCanvasWindowData().Canvases.Count == 0) {
								EditorGUILayout.LabelField("~ None exist ~", GetSkinStyle("CanvasWindowSplashLabelCanvasesEmpty"), GUILayout.ExpandWidth(true));
							} else {
								for (int i = 0; i < GetCanvasWindowData().Canvases.Count; i++) {
									if (!string.IsNullOrEmpty(GetCanvasWindowData().Canvases[i].Guid)) {
										if (GUILayout.Button(GetCanvasWindowData().Canvases[i].Name, GetCanvasWindowData().SelectedCanvasGuid == GetCanvasWindowData().Canvases[i].Guid ? GetSkinStyle("CanvasWindowSplashButtonCanvasesSelected") : GetSkinStyle("CanvasWindowSplashButtonCanvases"), GUILayout.ExpandWidth(true), GUILayout.Height(30))) {
											GetCanvasWindowData().SelectedCanvasGuid = GetCanvasWindowData().Canvases[i].Guid;
										}
									} else {
										EditorGUILayout.LabelField("Guid Invalid!", GetSkinStyle("CanvasWindowSplashLabelCanvasesError"), GUILayout.ExpandWidth(true));
									}
									if (i < GetCanvasWindowData().Canvases.Count - 1) {
										GUILayout.Space(4);
									}
								}
							}
						}
						EditorGUILayout.EndScrollView();
						GUILayout.Space(6);
					}
					EditorGUILayout.EndVertical();
					GUILayout.Space(6);
					if (GUILayout.Button("Select", (!string.IsNullOrEmpty(GetCanvasWindowData().SelectedCanvasGuid) ? GetSkinStyle("CanvasWindowSplashButtonSelect") : GetSkinStyle("CanvasWindowSplashButtonSelectEmpty")))) {
						if (!string.IsNullOrEmpty(GetCanvasWindowData().SelectedCanvasGuid)) {
							GetCanvasWindowData().CurrentCanvasGuid = GetCanvasWindowData().SelectedCanvasGuid;
						}
					}
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndVertical();
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawCurrentCanvasGUI() {
			EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			{
				DrawLeftCanvasGUI();
				DrawCenterCanvasGUI();
				DrawRightCanvasGUI();
			}
			EditorGUILayout.EndHorizontal();
		}

		private static void DrawLeftCanvasGUI() {
			EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanel"), GUILayout.Width(80), GUILayout.ExpandHeight(true));
			{
				EditorGUILayout.LabelField("x", GUILayout.Width(40));
			}
			EditorGUILayout.EndVertical();
		}

		static int _pixelSize = 24;
		private static void DrawCenterCanvasGUI() {
			EditorGUILayout.BeginVertical();
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					EditorGUILayout.BeginVertical();
					{
						GetCanvasWindowData().CurrentCanvasScroll = EditorGUILayout.BeginScrollView(GetCanvasWindowData().CurrentCanvasScroll);
						{
							bool _yWasLastEven;
							for (int y = 1; y < GetCanvasData(GetCanvasWindowDataGuid()).CanvasHeight + 1; y++) { //Height
								_yWasLastEven = LucasIndustries.Runtime.StaticUtilities.IsEvenNumber(y);
								EditorGUILayout.BeginHorizontal();
								{
									for (int x = 1; x < GetCanvasData(GetCanvasWindowDataGuid()).CanvasWidth + 1; x++) { //Width
										bool _xIsEven = LucasIndustries.Runtime.StaticUtilities.IsEvenNumber(x);
										GUIStyle _xStyle;
										if (_yWasLastEven) {
											_xStyle = _xIsEven ? GetSkinStyle("CanvasWindowPainterPixelEven") : GetSkinStyle("CanvasWindowPainterPixelOdd");
										} else {
											_xStyle = _xIsEven ? GetSkinStyle("CanvasWindowPainterPixelOdd") : GetSkinStyle("CanvasWindowPainterPixelEven");
										}
										PixelPainterEditor_CanvasPixelsData.PixelInfo _pixelInfo = GetCanvasData(GetCanvasWindowDataGuid()).PixelsData.Pixels.FirstOrDefault(p => p.XPixelPosition == x && p.YPixelPosition == y);
										Event e = Event.current;
										if (GUILayout.Button(_pixelInfo.CachedTexture != null ? new GUIContent(_pixelInfo.CachedTexture) : new GUIContent(""), _pixelInfo.CachedTexture != null ? GetSkinStyle("CanvasWindowPainterPixelFilled") : _xStyle, GUILayout.Width(_pixelSize), GUILayout.Height(_pixelSize))) {
											if (e.button == 0) {
												_pixelInfo.CacheTexture(Color.red, _pixelSize, _pixelSize);
											}
											if (e.button == 1) {
												if (_pixelInfo.CachedTexture != null) {
													_pixelInfo.Color = new Color(1, 1, 1, 0);
													_pixelInfo.CachedTexture = null;
													GC.Collect();
												}
											}
										}
									}
								}
								EditorGUILayout.EndHorizontal();
							}
						}
						EditorGUILayout.EndScrollView();
					}
					EditorGUILayout.EndVertical();
					GUILayout.FlexibleSpace();
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			EditorGUILayout.EndVertical();
		}

		private static void DrawRightCanvasGUI() {
			EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanel"), GUILayout.Width(240), GUILayout.ExpandHeight(true));
			{
				EditorGUILayout.LabelField("x", GUILayout.Width(240));
			}
			EditorGUILayout.EndVertical();
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}