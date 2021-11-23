using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.PixelPainter.Editor {
    [Serializable]
    public class PixelPainterEditor_CanvasPixelsData {
        #region Public/Private Variables
        [Serializable]
        public class PixelInfo {
            public int XPixelPosition;
            public int YPixelPosition;
            public Color Color;
            public Texture2D CachedTexture;

            public void PaintPixel(Color color) {
                Color = color;
                if (Color.a != 0) {
                    if (CachedTexture == null) {
                        CachedTexture = new Texture2D(1, 1);
                    }
                    CachedTexture.SetPixel(0, 0, Color);
                    CachedTexture.Apply();
                } else {
                    Color = new Color(1, 1, 1, 0);
                    if (CachedTexture != null) {
                        CachedTexture = null;
                    }
                }
            }
        }
        public List<PixelInfo> Pixels = new List<PixelInfo>();
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
        public PixelPainterEditor_CanvasPixelsData(int canvasWidth, int canvasHeight) {
            Pixels.Clear();
            for (int y = 0; y < canvasHeight; y++) {
                for (int x = 0; x < canvasWidth; x++) {
                    Pixels.Add(new PixelInfo() {
                        XPixelPosition = x,
                        YPixelPosition = y,
                        Color = new Color(1, 1, 1, 0)
                    });
                }
            }
        }

        public void SetAndApplyCachedTexturePixels(PixelInfo pixelInfo) {
            if (pixelInfo.CachedTexture != null) {
                Color[] _colors = Enumerable.Repeat(pixelInfo.Color, pixelInfo.CachedTexture.width * pixelInfo.CachedTexture.height).ToArray();
                pixelInfo.CachedTexture.SetPixels(_colors);
                pixelInfo.CachedTexture.Apply();
            }
        }

        public void RebuildCachedTextures() {
            for (int i = 0; i < Pixels.Count; i++) {
                if (Pixels[i].CachedTexture == null) {
                    Pixels[i].PaintPixel(Pixels[i].Color);
                }
            }
        }

        public void ClearAllPixels() {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to clear all of the painted tiles?", "Yes", "No")) {
                for (int i = 0; i < Pixels.Count; i++) {
                    Pixels[i].Color = new Color(1, 1, 1, 0);
                    if (Pixels[i].CachedTexture != null) {
                        Pixels[i].CachedTexture = null;
                    }
                }
            }
        }

        public void HardClearAllPixels() {
            for (int i = 0; i < Pixels.Count; i++) {
                Pixels[i].Color = new Color(1, 1, 1, 0);
                if (Pixels[i].CachedTexture != null) {
                    Pixels[i].CachedTexture = null;
                }
            }
        }

        public void FillAllPixels(Color color) {
            if (EditorUtility.DisplayDialog("Confirm", "Are you sure you want to fill all tiles with the primary color?", "Yes", "No")) {
                for (int i = 0; i < Pixels.Count; i++) {
                    Pixels[i].PaintPixel(color);
                }
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}