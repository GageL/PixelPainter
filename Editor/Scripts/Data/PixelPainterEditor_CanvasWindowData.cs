using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LucasIndustries.PixelPainter.Editor {
	[Serializable]
	public class PixelPainterEditor_CanvasWindowData {
		#region Public/Private Variables
		public string NewCanvasName;
		public int NewCanvasWidth;
		public int NewCanvasHeight;
		public List<PixelPainterEditor_CanvasData> Canvases = new List<PixelPainterEditor_CanvasData>();
		public Vector2 ExistingCanvasesScroll = Vector2.zero;
		public string SelectedCanvasGuid;
		public string CurrentCanvasGuid;
		public Vector2 CurrentCanvasScroll = Vector2.zero;
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
		public void ResetSplashData() {
			NewCanvasName = string.Empty;
			NewCanvasWidth = 0;
			NewCanvasHeight = 0;
			CurrentCanvasScroll = Vector2.zero;
		}

		public void SetCanvasGuids(string selectedGuid, string currentGuid) {
			SelectedCanvasGuid = selectedGuid;
			CurrentCanvasGuid = currentGuid;
        }
		#endregion

		#region Private Methods

		#endregion
	}
}