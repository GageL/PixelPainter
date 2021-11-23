using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace LucasIndustries.PixelPainter.Editor {
    [Serializable]
    public class PixelPainterEditor_CanvasData {
        #region Public/Private Variables
        public string Guid;
        public string Name;
        public int CanvasWidth;
        public int CanvasHeight;
        public PixelPainterEditor_CanvasPixelsData PixelsData;
        public Color PrimaryPaintColor = Color.blue;
        public Color SecondaryPaintColor = Color.red;
        public int SelectedPalette;
        #endregion

        #region Runtime Variables

        #endregion

        #region Native Methods

        #endregion

        #region Callback Methods

        #endregion

        #region Static Methods

        #endregion

        #region Public Methods
        public PixelPainterEditor_CanvasData(string name, int canvasWidth, int canvasHeight) {
            Guid = System.Guid.NewGuid().ToString();
            Name = name;
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
            PixelsData = new PixelPainterEditor_CanvasPixelsData(canvasWidth, canvasHeight);
        }

        [Button()]
        public void DuplicateCanvas() {
            if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to duplicate the canvas: {Name}?", "Yes", "No")) {
                PixelPainterEditor_CanvasData _copy = new PixelPainterEditor_CanvasData(Name + "_Copy", CanvasWidth, CanvasHeight);
                for (int pixel = 0; pixel < PixelsData.Pixels.Count; pixel++) {
                    _copy.PixelsData.Pixels[pixel].Color = PixelsData.Pixels[pixel].Color;
                }
                PixelPainterEditorWindow.CachedPixelPainterEditorData.CanvasWindowData.Canvases.Add(_copy);
            }
        }

        [Button()]
        public void DeleteCanvas() {
            if (EditorUtility.DisplayDialog("Confirm", $"Are you sure you want to delete the canvas: {Name}?", "Yes", "Cancel")) {
                PixelPainterEditorWindow.CachedPixelPainterEditorData.CanvasWindowData.SetCanvasGuids(string.Empty, string.Empty);
                PixelPainterEditorWindow.CachedPixelPainterEditorData.CanvasWindowData.Canvases.Remove(this);
            }
        }

        [Button()]
        public void ExportToPNG() {
            if (EditorUtility.DisplayDialog("Confirm", $"Do you want to export the canvas: {Name}?", "Yes", "Cancel")) {
                string _path = EditorUtility.SaveFilePanelInProject("Export Path", Name, "png", "");
                if (!string.IsNullOrEmpty(_path)) {
                    List<Color> _colors = new List<Color>();
                    for (int i = 0; i < PixelsData.Pixels.Count; i++) {
                        _colors.Add(PixelsData.Pixels[i].Color);
                    }
                    if (_colors.Count > 0) {
                        // Create a texture the size of the canvas, RGB24 format
                        int width = CanvasWidth;
                        int height = CanvasHeight;
                        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false) {
                            wrapMode = TextureWrapMode.Clamp,
                            filterMode = FilterMode.Point
                        };

                        // Read canvas contents into the texture
                        tex.SetPixels(_colors.ToArray());
                        tex.Apply();

                        // Need to flip right side up due to current pixel storage method
                        //tex = FlipTextureVertically(tex);

                        // Encode texture into PNG
                        byte[] bytes = tex.EncodeToPNG();
                        UnityEngine.Object.DestroyImmediate(tex);

                        File.WriteAllBytes(_path, bytes);

                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        public void ImportFromPNG() {
            string _path = EditorUtility.OpenFilePanel("Select File", Application.dataPath, "png");
            //Debug.Log(_path);
            if (!string.IsNullOrEmpty(_path)) {
                Texture2D _tex = null;
                byte[] fileData;
                if (File.Exists(_path)) {
                    fileData = File.ReadAllBytes(_path);
                    _tex = new Texture2D(1, 1);
                    _tex.LoadImage(fileData);
                    if (_tex.width <= CanvasWidth && _tex.height <= CanvasHeight) {
                        PixelsData.ClearAllPixels();
                        for (int y = 0; y < _tex.height; y++) {
                            for (int x = 0; x < _tex.width; x++) {
                                PixelsData.Pixels.FirstOrDefault(p => p.XPixelPosition == x && p.YPixelPosition == y).Color = _tex.GetPixel(x, y);
                            }
                        }
                        PixelsData.RebuildCachedTextures();
                    } else {
                        EditorUtility.DisplayDialog("Error", "Cannot load a texture that is larger than the current canvas", "Close");
                    }
                }
            }
        }

        public void FlipCanvasVertically() {
            List<PixelPainterEditor_CanvasPixelsData.PixelInfo> _tmpPixels = new List<PixelPainterEditor_CanvasPixelsData.PixelInfo>();
            foreach (var pixel in PixelsData.Pixels) {
                _tmpPixels.Add(new PixelPainterEditor_CanvasPixelsData.PixelInfo() { XPixelPosition = pixel.XPixelPosition, YPixelPosition = ((CanvasHeight - 1) - pixel.YPixelPosition), Color = pixel.Color });
            }
            PixelsData.HardClearAllPixels();
            PixelsData.Pixels = _tmpPixels;
            PixelsData.RebuildCachedTextures();
        }

        public void FlipCanvasHorizontally() {
            List<PixelPainterEditor_CanvasPixelsData.PixelInfo> _tmpPixels = new List<PixelPainterEditor_CanvasPixelsData.PixelInfo>();
            foreach (var pixel in PixelsData.Pixels) {
                _tmpPixels.Add(new PixelPainterEditor_CanvasPixelsData.PixelInfo() { XPixelPosition = ((CanvasWidth - 1) - pixel.XPixelPosition), YPixelPosition = pixel.YPixelPosition, Color = pixel.Color });
            }
            PixelsData.HardClearAllPixels();
            PixelsData.Pixels = _tmpPixels;
            PixelsData.RebuildCachedTextures();
        }
        #endregion

        #region Private Methods
        private Texture2D FlipTextureVertically(Texture2D original) {
            var originalPixels = original.GetPixels();

            var newPixels = new Color[originalPixels.Length];

            var width = original.width;
            var rows = original.height;

            for (var x = 0; x < width; x++) {
                for (var y = 0; y < rows; y++) {
                    newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
                }
            }

            original.SetPixels(newPixels);
            original.Apply();
            return original;
        }

        private Texture2D FlipTextureHorizontally(Texture2D original) {
            var originalPixels = original.GetPixels();

            var newPixels = new Color[originalPixels.Length];

            var width = original.width;
            var rows = original.height;

            for (var x = 0; x < width; x++) {
                for (var y = 0; y < rows; y++) {
                    newPixels[x + y * width] = originalPixels[(width - x - 1) + y * width];
                }
            }

            original.SetPixels(newPixels);
            original.Apply();
            return original;
        }
        #endregion
    }
}