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
        private enum ToolType { Pen, Brush, MirrorH, MirrorV, MirrorHV, Erase, Fill, ReplaceFill, Lighten, Darken };
        private static ToolType currentSelectedTool = ToolType.Pen;
        private static Color cachedHoverColor;
        private static bool editingPalette;
        private static Vector2 paletteScroll = Vector2.zero;
        #endregion

        #region Native Methods

        #endregion

        #region Callback Methods
        public static void SetPixelHoverTexture() {
            if (currentSelectedTool == ToolType.Erase) {
                GetSkinStyle("CanvasWindowPainterPixelOdd").hover.background = Resources.Load<Texture2D>("Ed_Dither");
                GetSkinStyle("CanvasWindowPainterPixelEven").hover.background = Resources.Load<Texture2D>("Ed_Dither");
                GetSkinStyle("CanvasWindowPainterPixelFilled").hover.background = Resources.Load<Texture2D>("Ed_Dither");
                cachedHoverColor = Color.clear;
            } else {
                if (cachedHoverColor != GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor) {
                    cachedHoverColor = GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor;
                    Texture2D _tex = new Texture2D(1, 1);
                    _tex.SetPixel(0, 0, cachedHoverColor);
                    _tex.Apply();
                    GetSkinStyle("CanvasWindowPainterPixelOdd").hover.background = _tex;
                    GetSkinStyle("CanvasWindowPainterPixelEven").hover.background = _tex;
                    GetSkinStyle("CanvasWindowPainterPixelFilled").hover.background = _tex;
                }
            }
        }
        #endregion

        #region Static Methods
        public static void DrawWindow() {
            Rect _rect = EditorGUILayout.BeginVertical(GetSkinStyle("Backdrop"));
            {
                if (!string.IsNullOrEmpty(GetCanvasWindowData().CurrentCanvasGuid) && GetCanvasWindowData().CurrentCanvasGuid == GetCanvasWindowData().SelectedCanvasGuid) {
                    GetMouseHeld();
                    SetPixelHoverTexture();
                    DrawCurrentCanvasGUI();
                    if (GUI.Button(_rect, "", GUIStyle.none)) {
                        GUI.FocusControl(null);
                    }
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
            foreach (PixelPainterEditor_CanvasData canvasData in GetCanvasWindowData().Canvases) {
                if (canvasData.Guid == guid) {
                    return canvasData;
                }
            }
            return null;
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

        private static void ResetWindow() {
            lastHoveredPixelCoord = Vector2.zero;
            lastInteractedPixelCoord = Vector2.zero;
            mouseDragIndex = -1;
            isMouseDragged = false;
            currentSelectedTool = ToolType.Pen;
            editingPalette = false;
            paletteScroll = Vector2.zero;
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
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowSplashNewCanvasHolder"), GUILayout.Width(320));
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
                            GetCanvasWindowData().NewCanvasWidth = EditorGUILayout.IntField(GetCanvasWindowData().NewCanvasWidth, GetSkinStyle("CanvasWindowSplashNewCanvasField"), GUILayout.Width(64));
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(4);
                        EditorGUILayout.BeginHorizontal(GUILayout.Height(24));
                        {
                            EditorGUILayout.LabelField("Height:", GetSkinStyle("CanvasWindowSplashNewCanvasParam"), GUILayout.Width(84));
                            GetCanvasWindowData().NewCanvasHeight = EditorGUILayout.IntField(GetCanvasWindowData().NewCanvasHeight, GetSkinStyle("CanvasWindowSplashNewCanvasField"), GUILayout.Width(64));
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
                        if (!IsEvenNumber(GetCanvasWindowData().NewCanvasWidth) && !IsEvenNumber(GetCanvasWindowData().NewCanvasHeight)) {
                            EditorUtility.DisplayDialog("Create Error", "The canvas must have even proportions", "Close");
                            return;
                        }
                        if (GetCanvasWindowData().NewCanvasWidth > 32 || GetCanvasWindowData().NewCanvasHeight > 32) {
                            EditorUtility.DisplayDialog("Create Error", "Max pixels per axis cannot exceed 32 pixels at this time", "Close");
                            return;
                        }
                        PixelPainterEditor_CanvasData _data = new PixelPainterEditor_CanvasData(GetCanvasWindowData().NewCanvasName, GetCanvasWindowData().NewCanvasWidth, GetCanvasWindowData().NewCanvasHeight);
                        GetCanvasWindowData().Canvases.Add(_data);
                        GetCanvasWindowData().ResetSplashData();
                        GetCanvasWindowData().SetCanvasGuids(_data.Guid, _data.Guid);
                        GUI.FocusControl(null);
                    }
                    GUILayout.Space(24);
                    EditorGUILayout.LabelField("··   -   ··", GetSkinStyle("CanvasWindowSplashLabelBody"));
                    GUILayout.Space(24);
                    EditorGUILayout.LabelField("« or select an existing canvas »", GetSkinStyle("CanvasWindowSplashLabelBody"));
                    GUILayout.Space(6);
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowSplashCanvasesHolder"), GUILayout.Width(320), GUILayout.Height(200));
                    {
                        GUILayout.Space(6);
                        GetCanvasWindowData().ExistingCanvasesScroll = EditorGUILayout.BeginScrollView(GetCanvasWindowData().ExistingCanvasesScroll);
                        {
                            if (GetCanvasWindowData().Canvases.Count == 0) {
                                EditorGUILayout.LabelField("~ None exist ~", GetSkinStyle("CanvasWindowSplashLabelCanvasesEmpty"), GUILayout.ExpandWidth(true), GUILayout.Height(30));
                            } else {
                                for (int i = 0; i < GetCanvasWindowData().Canvases.Count; i++) {
                                    EditorGUILayout.BeginVertical();
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Space(4);
                                            if (!string.IsNullOrEmpty(GetCanvasWindowData().Canvases[i].Guid)) {
                                                if (GUILayout.Button(GetCanvasWindowData().Canvases[i].Name, GetCanvasWindowData().SelectedCanvasGuid == GetCanvasWindowData().Canvases[i].Guid ? GetSkinStyle("CanvasWindowSplashButtonCanvasesSelected") : GetSkinStyle("CanvasWindowSplashButtonCanvases"), GUILayout.ExpandWidth(true), GUILayout.Height(30))) {
                                                    GetCanvasWindowData().SelectedCanvasGuid = GetCanvasWindowData().Canvases[i].Guid;
                                                }
                                            } else {
                                                EditorGUILayout.LabelField("Guid Invalid!", GetSkinStyle("CanvasWindowSplashLabelCanvasesError"), GUILayout.Width(206), GUILayout.Height(30));
                                            }
                                            GUILayout.Space(4);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Space(4);
                                            if (i > 0) {
                                                if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveUp"), "Move Up"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(30), GUILayout.Height(30))) {
                                                    int newIndex = i - 1;
                                                    var item = GetCanvasWindowData().Canvases[i];
                                                    GetCanvasWindowData().Canvases.RemoveAt(i);
                                                    if (newIndex > i) {
                                                        newIndex--;
                                                    }
                                                    GetCanvasWindowData().Canvases.Insert(newIndex, item);
                                                }
                                                GUILayout.Space(4);
                                            }
                                            if (i < GetCanvasWindowData().Canvases.Count - 1) {
                                                if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveDown"), "Move Down"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(30), GUILayout.Height(30))) {
                                                    int newIndex = i + 1;
                                                    var item = GetCanvasWindowData().Canvases[i];
                                                    GetCanvasWindowData().Canvases.RemoveAt(i);
                                                    if (newIndex < i) {
                                                        newIndex++;
                                                    }
                                                    GetCanvasWindowData().Canvases.Insert(newIndex, item);
                                                }
                                                GUILayout.Space(4);
                                            }
                                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Duplicate"), "Duplicate"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(30), GUILayout.Height(30))) {
                                                GetCanvasWindowData().Canvases[i].DuplicateCanvas();
                                            }
                                            GUILayout.Space(4);
                                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Delete"), "Delete"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(30), GUILayout.Height(30))) {
                                                GetCanvasWindowData().Canvases[i].DeleteCanvas();
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();
                                    if (i < GetCanvasWindowData().Canvases.Count - 1) {
                                        GUILayout.Space(-4);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.LabelField("", GetSkinStyle("CanvasWindowPainterSidePanelHDivider"), GUILayout.Width(290), GUILayout.Height(2));
                                            GUILayout.FlexibleSpace();
                                        }
                                        EditorGUILayout.EndHorizontal();
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
                    if (GUILayout.Button("Open", (!string.IsNullOrEmpty(GetCanvasWindowData().SelectedCanvasGuid) ? GetSkinStyle("CanvasWindowSplashButtonSelect") : GetSkinStyle("CanvasWindowSplashButtonSelectEmpty")))) {
                        if (!string.IsNullOrEmpty(GetCanvasWindowData().SelectedCanvasGuid)) {
                            GetCanvasWindowData().ResetSplashData();
                            GetCanvasData(GetCanvasWindowData().SelectedCanvasGuid).PixelsData.RebuildCachedTextures();
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
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelTitleBox"), GUILayout.Height(18));
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Tools", GetSkinStyle("CanvasWindowPainterSidePanelTitleLabel"), GUILayout.Width(64), GUILayout.Height(18));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Pen"), "Pen"), currentSelectedTool == ToolType.Pen ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Pen;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Brush"), "Brush"), currentSelectedTool == ToolType.Brush ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Brush;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MirrorH"), "Mirror Horizontal"), currentSelectedTool == ToolType.MirrorH ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.MirrorH;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MirrorV"), "Mirror Vertial"), currentSelectedTool == ToolType.MirrorV ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.MirrorV;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MirrorHV"), "Mirror HV"), currentSelectedTool == ToolType.MirrorHV ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.MirrorHV;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Erase"), "Erase"), currentSelectedTool == ToolType.Erase ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Erase;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Fill"), "Fill"), currentSelectedTool == ToolType.Fill ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Fill;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_ReplaceFill"), "Replace Color"), currentSelectedTool == ToolType.ReplaceFill ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.ReplaceFill;
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Lighten"), "Lighten"), currentSelectedTool == ToolType.Lighten ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Lighten;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Darken"), "Darken"), currentSelectedTool == ToolType.Darken ? ("CanvasWindowPainterSidePanelToolButtonActive") : GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            currentSelectedTool = ToolType.Darken;
                        }
                        GUILayout.FlexibleSpace();
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
                        GUILayout.FlexibleSpace();
                        GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor = EditorGUILayout.ColorField(new GUIContent("", "Primary"), GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor, true, false, false, GUILayout.Width(32), GUILayout.Height(32));
                        GUILayout.Space(6);
                        GetCanvasData(GetCanvasWindowDataGuid()).SecondaryPaintColor = EditorGUILayout.ColorField(new GUIContent("", "Secondary"), GetCanvasData(GetCanvasWindowDataGuid()).SecondaryPaintColor, true, false, false, GUILayout.Width(32), GUILayout.Height(32));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Swap"), "Swap Colors"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            Color _secondaryColor = GetCanvasData(GetCanvasWindowDataGuid()).SecondaryPaintColor;
                            GetCanvasData(GetCanvasWindowDataGuid()).SecondaryPaintColor = GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor;
                            GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor = _secondaryColor;
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Coord"), "Toggle Units"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetEditorData().ShowPixelNumbers = !GetEditorData().ShowPixelNumbers;
                        }
                        GUILayout.FlexibleSpace();
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
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_ClearAll"), "Clear All"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).PixelsData.ClearAllPixels();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_FillAll"), "Fill All"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).PixelsData.FillAllPixels(GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor);
                        }
                        GUILayout.FlexibleSpace();
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
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Add"), "Zoom In"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetEditorData().ChangePixelSize(true);
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Remove"), "Zoom Out"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetEditorData().ChangePixelSize(false);
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Import"), "Import"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ImportApplyPNGFromPath();
                            ResetWindow();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Export"), "Export"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).ExportToPNG();
                            ResetWindow();
                        }
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Back"), "Back"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasWindowData().SetCanvasGuids(string.Empty, string.Empty);
                            ResetWindow();
                        }
                        GUILayout.Space(6);
                        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Delete"), "Delete"), GetSkinStyle("CanvasWindowPainterSidePanelToolButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                            GetCanvasData(GetCanvasWindowDataGuid()).DeleteCanvas();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawCenterCanvasGUI() {
            GetCanvasWindowData().CurrentCanvasScroll = EditorGUILayout.BeginScrollView(GetCanvasWindowData().CurrentCanvasScroll);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField("Canvas Name", GetSkinStyle("CanvasWindowPainterCanvasNameParam"), GUILayout.Width(200), GUILayout.Height(24));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(-4);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        if (GetCanvasData(GetCanvasWindowDataGuid()) != null) {
                            GetCanvasData(GetCanvasWindowDataGuid()).Name = EditorGUILayout.TextField(GetCanvasData(GetCanvasWindowDataGuid()).Name, GetSkinStyle("CanvasWindowPainterCanvasNameField"), GUILayout.Width(200), GUILayout.Height(18));
                        }
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(12);
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginHorizontal();
                        {
                            bool _yWasLastEven;
                            for (int x = 0; x < GetCanvasData(GetCanvasWindowDataGuid())?.CanvasWidth; x++) {
                                bool _xIsEven = IsEvenNumber(x);
                                EditorGUILayout.BeginVertical();
                                {
                                    for (int y = GetCanvasData(GetCanvasWindowDataGuid()).CanvasHeight - 1; y >= 0; y--) {
                                        _yWasLastEven = IsEvenNumber(y);
                                        PixelPainterEditor_CanvasPixelsData.PixelInfo _pixelInfo = GetPixelInfo(x, y);
                                        if (_pixelInfo != null) {
                                            GUIStyle _style = null;
                                            if (_pixelInfo.CachedTexture != null) {
                                                _style = new GUIStyle(GetSkinStyle("CanvasWindowPainterPixelFilled"));
                                                _style.normal.background = _pixelInfo.CachedTexture;
                                            }
                                            if (GUILayout.Button(GetEditorData().ShowPixelNumbers ? $"{x + 1}.{y + 1}" : "", _style != null ? _style : GetPixelStyle(_xIsEven, _yWasLastEven), GUILayout.Width(GetEditorData().CanvasPixelSize), GUILayout.Height(GetEditorData().CanvasPixelSize))) {
                                                if (Event.current.button == 0) {
                                                    CanvasInteracted(_pixelInfo, 0);
                                                    lastInteractedPixelCoord = new Vector2(x, y);
                                                } else if (Event.current.button == 1) {
                                                    CanvasInteracted(_pixelInfo, 1);
                                                    lastInteractedPixelCoord = new Vector2(x, y);
                                                }
                                            }
                                            if (IsMouseOver()) {
                                                lastHoveredPixelCoord = new Vector2(x, y);
                                                if (lastInteractedPixelCoord != new Vector2(x, y)) {
                                                    if (isMouseDragged) {
                                                        if (mouseDragIndex == 0) {
                                                            CanvasInteracted(_pixelInfo, 0);
                                                            lastInteractedPixelCoord = new Vector2(x, y);
                                                        } else if (mouseDragIndex == 1) {
                                                            CanvasInteracted(_pixelInfo, 1);
                                                            lastInteractedPixelCoord = new Vector2(x, y);
                                                        }
                                                    }
                                                }
                                            }
                                        } else {
                                            Debug.Log($"null {x}.{y}");
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
            EditorGUILayout.EndScrollView();
        }

        private static PixelPainterEditor_CanvasPixelsData.PixelInfo GetPixelInfo(int x, int y) {
            foreach (PixelPainterEditor_CanvasPixelsData.PixelInfo pixelInfo in GetCanvasData(GetCanvasWindowDataGuid()).PixelsData.Pixels) {
                if (pixelInfo.XPixelPosition == x && pixelInfo.YPixelPosition == y) {
                    return pixelInfo;
                }
            }
            return null;
        }

        private static GUIStyle GetPixelStyle(bool xIsEven, bool yWasLastEven) {
            if (yWasLastEven) {
                return xIsEven ? GetSkinStyle("CanvasWindowPainterPixelEven") : GetSkinStyle("CanvasWindowPainterPixelOdd");
            } else {
                return xIsEven ? GetSkinStyle("CanvasWindowPainterPixelOdd") : GetSkinStyle("CanvasWindowPainterPixelEven");
            }
        }

        private static void CanvasInteracted(PixelPainterEditor_CanvasPixelsData.PixelInfo pixelInfo, int mouseButton) {
            switch (currentSelectedTool) {
                case ToolType.Pen:
                    if (mouseButton == 0) {
                        pixelInfo.PaintPixel(GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor);
                    } else if (mouseButton == 1) {
                        pixelInfo.PaintPixel(GetCanvasData(GetCanvasWindowDataGuid()).SecondaryPaintColor);
                    }
                    break;
                case ToolType.Brush:

                    break;
                case ToolType.MirrorH:

                    break;
                case ToolType.MirrorV:

                    break;
                case ToolType.MirrorHV:

                    break;
                case ToolType.Erase:
                    pixelInfo.PaintPixel(Color.clear);
                    break;
                case ToolType.Fill:

                    break;
                case ToolType.ReplaceFill:

                    break;
                case ToolType.Lighten:

                    break;
                case ToolType.Darken:

                    break;
            }
        }

        private static void DrawRightCanvasGUI() {
            EditorGUILayout.BeginHorizontal(GetSkinStyle("CanvasWindowPainterSidePanel"), GUILayout.Width(2), GUILayout.ExpandHeight(true));
            {
                EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelContainer"), GUILayout.ExpandWidth(true));
                {
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelTitleBox"), GUILayout.Height(18));
                    {
                        EditorGUILayout.LabelField("Frames", GetSkinStyle("CanvasWindowPainterSidePanelTitleLabel"), GUILayout.Width(100), GUILayout.Height(18));
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(6);
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.BeginVertical();
                        {
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Fill"), "Frame #"), GetSkinStyle("CanvasWindowPainterCanvasFrame"), GUILayout.Width(90), GUILayout.Height(90))) {

                            }
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(6);
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(6);
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelContainer"));
                    {
                        EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelTitleBox"), GUILayout.Height(18));
                        {
                            EditorGUILayout.LabelField("Positioning", GetSkinStyle("CanvasWindowPainterSidePanelTitleLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(6);
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_FlipH"), "Flip Horizontal"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                                GetCanvasData(GetCanvasWindowDataGuid()).FlipCanvasHorizontally();
                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_FlipV"), "Flip Vertical"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {
                                GetCanvasData(GetCanvasWindowDataGuid()).FlipCanvasVertically();
                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_RotateL"), "Rotate Left"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_RotateR"), "Rotate Right"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Center"), "Center"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(4);
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveUp"), "Move Up"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveDown"), "Move Down"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveLeft"), "Move Left"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_MoveRight"), "Move Right"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(32), GUILayout.Height(32))) {

                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(6);
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(12);
                    EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelContainer"));
                    {
                        EditorGUILayout.BeginVertical(GetSkinStyle("CanvasWindowPainterSidePanelTitleBox"), GUILayout.Height(18));
                        {
                            EditorGUILayout.LabelField("Palette", GetSkinStyle("CanvasWindowPainterSidePanelTitleLabel"), GUILayout.ExpandWidth(true), GUILayout.Height(18));
                        }
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(6);
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(4);
                            if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Add"), "New"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(20), GUILayout.Height(20))) {
                                GetEditorData().Palettes.Add(new PixelPainterEditor_PaletteData("New Palette"));
                                GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette = GetEditorData().Palettes.Count - 1;
                            }
                            GUILayout.Space(4);
                            if (GetEditorData().Palettes.Count != 0) {
                                if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Pen"), "Edit"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(20), GUILayout.Height(20))) {
                                    editingPalette = !editingPalette;
                                }
                                GUILayout.Space(4);
                                if (GetCanvasData(GetCanvasWindowDataGuid()) != null) {
                                    if (editingPalette) {
                                        GetEditorData().Palettes[GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette].Name = EditorGUILayout.TextField(GetEditorData().Palettes[GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette].Name, GUILayout.ExpandWidth(true), GUILayout.Height(20));
                                    } else {
                                        List<string> _paletteNames = new List<string>();
                                        for (int i = 0; i < GetEditorData().Palettes.Count; i++) {
                                            _paletteNames.Add(GetEditorData().Palettes[i].Name);
                                        }
                                        GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette = EditorGUILayout.Popup(GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette, _paletteNames.ToArray(), GUILayout.ExpandWidth(true), GUILayout.Height(20));
                                    }
                                }
                                GUILayout.Space(4);
                                if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Delete"), "Delete"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(20), GUILayout.Height(20))) {
                                    GetEditorData().Palettes[GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette].DeletePalette();
                                    GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette = 0;
                                }
                            }
                            GUILayout.Space(4);
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space(4);
                        EditorGUILayout.BeginHorizontal(GUILayout.Height(290));
                        {
                            GUILayout.Space(4);
                            paletteScroll = EditorGUILayout.BeginScrollView(paletteScroll);
                            {
                                for (int i = 0; i < GetEditorData().Palettes.Count; i++) {
                                    if (GetCanvasData(GetCanvasWindowDataGuid()) != null) {
                                        if (i == GetCanvasData(GetCanvasWindowDataGuid()).SelectedPalette) {
                                            int _colorIndex = 0;
                                            int _columns = ((int)(GetEditorData().Palettes[i].Colors.Count / 4) + 1);
                                            EditorGUILayout.BeginVertical();
                                            {
                                                for (int y = 0; y < _columns; y++) {
                                                    EditorGUILayout.BeginHorizontal();
                                                    {
                                                        for (int x = 0; x < (y == 0 ? 3 : 4); x++) {
                                                            if (y == 0 && x == 0) {
                                                                if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_AddColor"), "Add New Color"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(24), GUILayout.Height(26))) {
                                                                    GetEditorData().Palettes[i].Colors.Add(GetCanvasData(GetCanvasWindowDataGuid()).PrimaryPaintColor);
                                                                }
                                                                GUILayout.Space(26);
                                                            }
                                                            if (_colorIndex != GetEditorData().Palettes[i].Colors.Count) {
                                                                EditorGUILayout.BeginHorizontal(GUILayout.Width(48), GUILayout.Height(32));
                                                                {
                                                                    GetEditorData().Palettes[i].Colors[_colorIndex] = EditorGUILayout.ColorField(new GUIContent(""), GetEditorData().Palettes[i].Colors[_colorIndex], true, false, false, GUILayout.Width(36), GUILayout.Height(26));
                                                                    GUILayout.Space(-4);
                                                                    if (GUILayout.Button(new GUIContent("x"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(12), GUILayout.Height(12))) {
                                                                        if (GetEditorData().Palettes[i].Colors.Count == 1) {
                                                                            GetEditorData().Palettes[i].Colors[_colorIndex] = Color.white;
                                                                        } else {
                                                                            GetEditorData().Palettes[i].Colors.RemoveAt(_colorIndex);
                                                                            _colorIndex--;
                                                                        }
                                                                    }
                                                                }
                                                                EditorGUILayout.EndHorizontal();
                                                                _colorIndex++;
                                                            }
                                                        }
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }
                                            }
                                            EditorGUILayout.EndVertical();
                                            //for (int c = 0; c < GetEditorData().Palettes[i].Colors.Count; c++) {
                                            //    EditorGUILayout.BeginHorizontal(GUILayout.Width(48), GUILayout.Height(32));
                                            //    {
                                            //        GetEditorData().Palettes[i].Colors[c] = EditorGUILayout.ColorField(new GUIContent(""), GetEditorData().Palettes[i].Colors[c], true, false, false, GUILayout.Width(36), GUILayout.Height(26));
                                            //        GUILayout.Space(-4);
                                            //        if (GUILayout.Button(new GUIContent("x"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(12), GUILayout.Height(12))) {
                                            //            if (GetEditorData().Palettes[i].Colors.Count == 1) {
                                            //                GetEditorData().Palettes[i].Colors[c] = Color.white;
                                            //            } else {
                                            //                GetEditorData().Palettes[i].Colors.RemoveAt(c);
                                            //            }
                                            //        }
                                            //    }
                                            //    EditorGUILayout.EndHorizontal();
                                            //    GUILayout.Space(4);
                                            //    if (c == GetEditorData().Palettes[i].Colors.Count - 1) {
                                            //        if (GUILayout.Button(new GUIContent(Resources.Load<Texture2D>("Ed_Add"), "Add"), GetSkinStyle("CanvasWindowPainterSidePanelContainerButton"), GUILayout.Width(24), GUILayout.Height(26))) {
                                            //            GetEditorData().Palettes[i].Colors.Add(new Color(1, 1, 1));
                                            //        }
                                            //    }
                                            //}
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField($"Canvas Size: [{GetCanvasData(GetCanvasWindowDataGuid())?.CanvasWidth}x{GetCanvasData(GetCanvasWindowDataGuid())?.CanvasHeight}]", GetSkinStyle("CanvasWindowPainterSidePanelCenterLabel"));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField($"Mouse Tile: {lastHoveredPixelCoord.x + 1}.{lastHoveredPixelCoord.y + 1}", GetSkinStyle("CanvasWindowPainterSidePanelCenterLabel"));
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}