using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.PixelPainter.Editor {
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
				//CachedPixelPainterEditorData.CanvasWindowData.SetCanvasGuids(string.Empty, string.Empty); // Resets the window to splash
			}
		}

		private void OnDisable() {
			EditorApplication.playModeStateChanged -= CacheCanvasWindowPixelDataPixels;
		}

		private void OnGUI() {
			PixelPainterEditorWindow _window = this;
			GUI.skin = CachedPixelPainterEditorData.EditorSkin;

			//Debug.Log(this.position.width + "." + this.position.height);
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
						PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels[i].PaintPixel(PixelPainterEditor_CanvasWindow.GetCanvasData(PixelPainterEditor_CanvasWindow.GetCanvasWindowDataGuid()).PixelsData.Pixels[i].Color);
					}
				}
			}
		}
		#endregion

		#region Static Methods
		[MenuItem(CompanyName + "/" + EditorName + "/Open")]
		public static void OpenEditor() {
			PixelPainterEditorWindow _window = GetWindow<PixelPainterEditorWindow>(true);
			_window.minSize = new Vector2(852, 592);
			_window.titleContent = new GUIContent("Lucas Industries - Pixel Painter", Resources.Load<Texture2D>("Ed_Pen"));
			_window.Show();
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