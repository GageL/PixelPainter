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

            public void CacheTexture(Color color, int textureWidth, int textureHeight) {
                Color = color;
                if (Color.a != 0) {
                    if (CachedTexture == null) {
                        CachedTexture = new Texture2D(textureWidth, textureHeight);
                    }
                    Color[] _colors = Enumerable.Repeat(Color, textureWidth * textureHeight).ToArray();
                    CachedTexture.SetPixels(_colors);
                    CachedTexture.Apply();
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
        #endregion

        #region Private Methods

        #endregion
    }
}