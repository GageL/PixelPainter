using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LucasIndustries.PixelPainter.Editor {
    public class PixelPainterEditor_CanvasWindow {
        #region Public/Private Variables

        #endregion

        #region Runtime Variables
        private static Vector2 lastHoveredPixelCoord;
        private static Vector2 lastInteractedPixelCoord;
        private static int mouseDragIndex = -1;
        private static bool isMouseDragged = false;
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
                    GetMouseHeld();
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

        public static PixelPainterEditorData GetEditorData() {
            return PixelPainterEditorWindow.CachedPixelPainterEditorData;
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

        private static bool IsEvenNumber(int value) {
            bool result = false;
            if (value % 2 == 0) {
                result = true;
            } else {
                result = false;
            }
            return result;
        }

        private static bool IsMouseOver() {
            return Event.current.type == EventType.Repaint &&
                   GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }

        private static void GetMouseHeld() {
            Event e = Event.current;
            if (e.type == EventType.MouseDrag) {
                isMouseDragged = true;
            } else if (e.type == EventType.MouseUp) {
                isMouseDragged = false;
            }
            if (e.button == 0) {
                mouseDragIndex = 0;
            } else if (e.button == 1) {
                mouseDragIndex = 1;
            } else {
                mouseDragIndex = -1;
            }
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
            EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanel"), GUILayout.Width(2), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Pen"), "Pen"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {

                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Brush"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {

                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Mirror Horizontal"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Mirror Vertial"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Fill"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Fill Color"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Lighten"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Darken"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Erase"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Select"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Pick Color"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Move"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("", GetSkinStyle("CanvasWindowPainterSidePanelHDivider"), GUILayout.Width(32), GUILayout.Height(2));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Color 1"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, "Color 2"), GetSkinStyle("Backdrop"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawCenterCanvasGUI() {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        bool _yWasLastEven;
                        for (int x = 0; x < GetCanvasData(GetCanvasWindowDataGuid()).CanvasWidth; x++) {
                            bool _xIsEven = IsEvenNumber(x);
                            EditorGUILayout.BeginVertical();
                            {
                                for (int y = GetCanvasData(GetCanvasWindowDataGuid()).CanvasHeight - 1; y >= 0; y--) {
                                    _yWasLastEven = IsEvenNumber(y);
                                    Event e = Event.current;
                                    GUIStyle _xStyle;
                                    if (_yWasLastEven) {
                                        _xStyle = _xIsEven ? GetSkinStyle("CanvasWindowPainterPixelEven") : GetSkinStyle("CanvasWindowPainterPixelOdd");
                                    } else {
                                        _xStyle = _xIsEven ? GetSkinStyle("CanvasWindowPainterPixelOdd") : GetSkinStyle("CanvasWindowPainterPixelEven");
                                    }
                                    PixelPainterEditor_CanvasPixelsData.PixelInfo _pixelInfo = GetCanvasData(GetCanvasWindowDataGuid()).PixelsData.Pixels.FirstOrDefault(p => p.XPixelPosition == x && p.YPixelPosition == y);
                                    if (_pixelInfo != null) {
                                        if (GUILayout.Button(_pixelInfo.CachedTexture != null ? new GUIContent(_pixelInfo.CachedTexture) : new GUIContent(""), _pixelInfo.CachedTexture != null ? GetSkinStyle("CanvasWindowPainterPixelFilled") : _xStyle, GUILayout.Width(GetEditorData().CanvasPixelSize), GUILayout.Height(GetEditorData().CanvasPixelSize))) {
                                            if (e.button == 0) {
                                                _pixelInfo.CacheTexture(Color.red, GetEditorData().CanvasPixelSize, GetEditorData().CanvasPixelSize);
                                                lastInteractedPixelCoord = new Vector2(x, y);
                                            } else if (e.button == 1) {
                                                if (_pixelInfo.CachedTexture != null) {
                                                    _pixelInfo.Color = new Color(1, 1, 1, 0);
                                                    _pixelInfo.CachedTexture = null;
                                                    GC.Collect();
                                                }
                                                lastInteractedPixelCoord = new Vector2(x, y);
                                            }
                                        }
                                        if (IsMouseOver()) {
                                            lastHoveredPixelCoord = new Vector2(x, y);
                                            if (lastInteractedPixelCoord != new Vector2(x, y)) {
                                                if (isMouseDragged) {
                                                    if (mouseDragIndex == 0) {
                                                        _pixelInfo.CacheTexture(Color.red, GetEditorData().CanvasPixelSize, GetEditorData().CanvasPixelSize);
                                                        lastInteractedPixelCoord = new Vector2(x, y);
                                                    } else if (mouseDragIndex == 1) {
                                                        if (_pixelInfo.CachedTexture != null) {
                                                            _pixelInfo.Color = new Color(1, 1, 1, 0);
                                                            _pixelInfo.CachedTexture = null;
                                                            GC.Collect();
                                                        }
                                                        lastInteractedPixelCoord = new Vector2(x, y);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
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
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginVertical();
                    {
                        if (GUILayout.Button(new GUIContent(Texture2D.whiteTexture, ""), GetSkinStyle("Backdrop"), GUILayout.Width(220), GUILayout.Height(220))) {

                        }
                        GUILayout.Space(6);
                        EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelContainer"), GUILayout.Width(220), GUILayout.Height(72));
                        {
                            EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelTitleBox"), GUILayout.ExpandWidth(true), GUILayout.Height(24));
                            {
                                EditorGUILayout.LabelField("Layers", GetSkinStyle("CanvasWindowPainterSidePanelTitleLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(24));
                            }
                            EditorGUILayout.EndVertical();
                            EditorGUILayout.BeginHorizontal();
                            {
                                if (GUILayout.Button("B", GUILayout.Width(24), GUILayout.Height(24))) {

                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"Canvas Size: {GetCanvasData(GetCanvasWindowDataGuid()).CanvasWidth}x{GetCanvasData(GetCanvasWindowDataGuid()).CanvasHeight}", GetSkinStyle("CanvasWindowPainterSidePanelCenterLabel"));
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"Mouse Coord: {lastHoveredPixelCoord.x}.{lastHoveredPixelCoord.y}", GetSkinStyle("CanvasWindowPainterSidePanelCenterLabel"));
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
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