using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

            public void PaintPixel(Color color, int textureWidth, int textureHeight) {
                Color = color;
                if (Color.a != 0) {
                    if (CachedTexture == null) {
                        CachedTexture = new Texture2D(textureWidth, textureHeight);
                    }
                    Color[] _colors = Enumerable.Repeat(Color, textureWidth * textureHeight).ToArray();
                    CachedTexture.SetPixels(_colors);
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

        public void ClearAllPixels() {
            for (int i = 0; i < Pixels.Count; i++) {
                Pixels[i].Color = new Color(1, 1, 1, 0);
                if (Pixels[i].CachedTexture != null) {
                    Pixels[i].CachedTexture = null;
                }
            }
        }

        public void FillAllPixels(Color color, int textureWidth, int textureHeight) {
            for (int i = 0; i < Pixels.Count; i++) {
                Pixels[i].PaintPixel(color, textureWidth, textureHeight);
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}