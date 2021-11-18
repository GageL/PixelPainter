using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.Editor {
	public class PixelPainterEditorWindow : EditorWindow {
		#region Public/Private Variables
		public const string CompanyName = "Lucas Industries";
		public const string EditorName = "Pixel Painter";
		#endregion

		#region Runtime Variables

		public static PixelPainterEditorData CachedPixelPainterEditorData;
		#endregion

		#region Native Methods
		private void OnEnable() {
			EditorApplication.playModeStateChanged += CacheCanvasWindowPixelDataPixels;
			if (CachedPixelPainterEditorData == null) {
				CacheEditorData();
			}
		}

		private void OnDisable() {
			EditorApplication.playModeStateChanged -= CacheCanvasWindowPixelDataPixels;
		}

		private void OnGUI() {
			PixelPainterEditorWindow _window = this;
			GUI.skin = CachedPixelPainterEditorData.EditorSkin;

			CachedPixelPainterEditorData.CurrentEvent = Event.current;
			PixelPainterEditor_CanvasWindow.DrawWindow();

			this.Repaint();

			if (GUI.changed) {
				EditorUtility.SetDirty(_window);
			}
			EditorUtility.SetDirty(CachedPixelPainterEditorData);
		}
		#endregion
		
		#region Callback Methods
		private static void CacheCanvasWindowPixelDataPixels(PlayModeStateChange state) {
			if (string.IsNullOrEmpty(CachedPixelPainterEditorData.CanvasWindowData.CurrentCanvasGuid) || string.IsNullOrEmpty(CachedPixelPainterEditorData.CanvasWindowData.SelectedCanvasGuid)) {
				return;
			}
			if (CachedPixelPainterEditorData.CanvasWindowData.CurrentCanvasGuid != CachedPixelPainterEditorData.CanvasWindowData.SelectedCanvasGuid) {
				return;
			}
			if (state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode) {
				for (int i = 0; i < PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels.Count; i++) {
					if (PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels[i].Color.a != 0) {
						PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels[i].CacheTexture(PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels[i].Color, CachedPixelPainterEditorData.CanvasPixelSize, CachedPixelPainterEditorData.CanvasPixelSize);
					}
				}
			}
		}
		#endregion

		#region Static Methods
		[MenuItem(CompanyName + "/" + EditorName + "/Open")]
		public static void OpenEditor() {
			GetWindow<PixelPainterEditorWindow>(EditorName).Show();
			CacheEditorData();
		}

		public static void CacheEditorData() {
			CachedPixelPainterEditorData = PixelPainterEditorData.GetEditorData();
			if (CachedPixelPainterEditorData == null) {
				CachedPixelPainterEditorData = PixelPainterEditorData.CreateEditorData(true);
			}
		}
		#endregion

		#region Public Methods

		#endregion

		#region Private Methods

		#endregion
	}
}