using System.Collections.Generic;
using Fiber.Utilities;
using LevelEditor;
using UnityEditor;
using UnityEngine;

namespace GridSystem
{
	public class Grid : Singleton<Grid>
	{
		[SerializeField] private Vector2Int size = new Vector2Int(10, 10);
		[SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		[SerializeField] private GridCell cellPrefab;
		[Space]
		[SerializeField] private GridNodeHolder gridNodeHolderPrefab;
		[SerializeField] private GridNode gridNodePrefab;

		[SerializeField] private GridCellMatrix gridCells;
		public GridCellMatrix GridCells => gridCells;

		[SerializeField] private List<GridNodeHolder> gridNodeHolders = new List<GridNodeHolder>();
		public List<GridNodeHolder> GridNodeHolders => gridNodeHolders;

		#region Setup

		private void Setup()
		{
			gridCells = new GridCellMatrix(size.x, size.y);

			var xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var cell = Instantiate(cellPrefab, transform);
					cell.Setup(x, y, size.y * y + x, nodeSize);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = cell;
				}
			}
		}

#if UNITY_EDITOR
		public void SetupEditor(Dictionary<Color, GridNodeInfo> nodeHolderInfos)
		{
			gridCells = new GridCellMatrix(size.x, size.y);

			var xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var cell = (GridCell)PrefabUtility.InstantiatePrefab(cellPrefab, transform);
					cell.Setup(x, y, size.y * y + x, nodeSize);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = cell;
				}
			}

			foreach (var nodeHolderInfo in nodeHolderInfos)
			{
				var holder = (GridNodeHolder)PrefabUtility.InstantiatePrefab(gridNodeHolderPrefab, transform);
				holder.Setup(nodeHolderInfo.Value.Value);

				foreach (var cellInfo in nodeHolderInfo.Value.Cells)
				{
					var node = (GridNode)PrefabUtility.InstantiatePrefab(gridNodePrefab, holder.transform);
					var cell = gridCells[cellInfo.Coordinates.x, cellInfo.Coordinates.y];
					node.transform.position = cell.transform.position;
					node.SetValue(holder.Value);
					cell.CurrentNode = node;

					holder.GridNodes.Add(node);
				}

				gridNodeHolders.Add(holder);
			}
		}
#endif

		#endregion

		[System.Serializable]
		public class GridCellArray
		{
			public GridCell[] Cells;
			public GridCell this[int index]
			{
				get => Cells[index];
				set => Cells[index] = value;
			}

			public GridCellArray(int index0)
			{
				Cells = new GridCell[index0];
			}
		}

		[System.Serializable]
		public class GridCellMatrix
		{
			public GridCellArray[] Arrays;
			public GridCell this[int x, int y]
			{
				get => Arrays[x][y];
				set => Arrays[x][y] = value;
			}

			public GridCellMatrix(int index0, int index1)
			{
				Arrays = new GridCellArray[index0];
				for (int i = 0; i < index0; i++)
					Arrays[i] = new GridCellArray(index1);
			}
		}
	}
}