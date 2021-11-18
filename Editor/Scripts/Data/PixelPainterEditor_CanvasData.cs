using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LucasIndustries.PixelPainter.Editor {
	[Serializable]
	public class PixelPainterEditor_CanvasData {
		#region Public/Private Variables
		public string Guid;
		public string Name;
		public int CanvasWidth;
		public int CanvasHeight;
		public PixelPainterEditor_CanvasPixelsData PixelsData;
		public string PaletteGuid;
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
		#endregion

		#region Private Methods

		#endregion
	}
}