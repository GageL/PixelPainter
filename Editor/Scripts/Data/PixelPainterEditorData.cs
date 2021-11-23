using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.PixelPainter.Editor {
    [CreateAssetMenu(fileName = "PixelPainterEditorData", menuName = "Lucas Industries/PixelPainterEditorData", order = 1)]
    public class PixelPainterEditorData : ScriptableObject {
        #region Public/Private Variables
        private static string SaveFolder = "PixelPainter/Editor/Data/";

        public GUISkin EditorSkin;
        public PixelPainterEditor_CanvasWindowData CanvasWindowData;
        public List<PixelPainterEditor_PaletteData> Palettes = new List<PixelPainterEditor_PaletteData>();
        public bool ShowPixelNumbers = false;
        public int CanvasPixelSize = 16;
        #endregion

        #region Runtime Variables

        #endregion

        #region Native Methods

        #endregion

        #region Callback Methods

        #endregion

        #region Static Methods
        public static PixelPainterEditorData GetEditorData(bool focus = false) {
            PixelPainterEditorData _out = null;
            string[] _paths = AssetDatabase.FindAssets($"t:{typeof(PixelPainterEditorData).Name}");
            if (_paths.Length != 0) {
                _out = (PixelPainterEditorData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_paths[0]), typeof(PixelPainterEditorData));
                if (focus) {
                    Selection.activeObject = _out;
                }
            } else {
                Debug.LogError("ATTN: No data object exists in the project");
            }
            return _out;
        }

        public static bool MultipleEditorDataExist() {
            bool _value = false;
            string[] _paths = AssetDatabase.FindAssets($"t:{typeof(PixelPainterEditorData).Name}");
            if (_paths.Length > 1) {
                for (int i = 0; i < _paths.Length; i++) {
                    Debug.LogWarning($"Found data object at path: {_paths[i]}");
                }
                _value = true;
            }
            return _value;
        }

        public static PixelPainterEditorData CreateEditorData(bool showEditor = false) {
            if (MultipleEditorDataExist()) {
                Debug.LogError("Error: Project cannot contain more than 1 data object");
                return null;
            }
            PixelPainterEditorData _out = GetEditorData(true);
            if (_out != null) {
                return _out;
            }
            _out = ScriptableObject.CreateInstance<PixelPainterEditorData>();
            string _path = Path.Combine("Assets", SaveFolder, $"{typeof(PixelPainterEditorData).Name}.asset");
            if (string.IsNullOrEmpty(_path)) {
                Debug.LogError("Error: Full save path is null");
                return null;
            }
            _path.Replace(@"\", "/");
            Debug.Log($"Asset Save Path: {_path}");
            AssetDatabase.CreateAsset(_out, _path);
            AssetDatabase.Refresh();
            if (showEditor) {
                PixelPainterEditorWindow.OpenEditor();
            }
            return _out;
        }

        public static void FocusEditorData() {
            Selection.activeObject = GetEditorData();
        }
        #endregion

        #region Public Methods
        public void ChangePixelSize(bool add) {
            CanvasPixelSize += add ? 2 : -2;
            CanvasPixelSize = Mathf.Clamp(CanvasPixelSize, 2, 32);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}