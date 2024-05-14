using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.UIElements;
#endif

namespace LevelEditor
{
	public class GridNodeInfo
	{
		public int Value;
		public List<CellInfo> Cells = new List<CellInfo>();
		public int TabIndex;
#if UNITY_EDITOR
		public IntegerField IntegerField;
#endif

		public GridNodeInfo(CellInfo cell, int tabIndex)
		{
			Cells.Add(cell);
			TabIndex = tabIndex;
		}
	}
}