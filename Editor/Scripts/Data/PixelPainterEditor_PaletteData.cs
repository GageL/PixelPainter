using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LucasIndustries.PixelPainter.Editor {
	[Serializable]
	public class PixelPainterEditor_PaletteData {
		#region Public/Private Variables
		public string Guid;
		public string Name;
		public List<Color> Colors = new List<Color>();
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
		public PixelPainterEditor_PaletteData(string name) {
			Guid = System.Guid.NewGuid().ToString();
			Name = name;
			Colors.Add(new Color(1, 1, 1));
		}
		#endregion

		#region Private Methods

		#endregion
	}
}