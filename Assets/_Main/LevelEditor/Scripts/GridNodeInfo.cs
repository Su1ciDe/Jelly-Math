using System.Collections.Generic;
using UnityEditor.UIElements;

namespace LevelEditor
{
	public class GridNodeInfo
	{
		public int Value;
		public List<CellInfo> Cells = new List<CellInfo>();
		public IntegerField IntegerField;

		public GridNodeInfo(CellInfo cell)
		{
			Cells.Add(cell);
		}
	}
}