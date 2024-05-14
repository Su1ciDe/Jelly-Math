using System.Collections.Generic;
using Fiber.Utilities;
using GamePlay;
using LevelEditor;
using Managers;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace GridSystem
{
	public class Grid : Singleton<Grid>
	{
		public static Vector2Int Size = new Vector2Int(8, 8);

		[SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = .1f;
		[SerializeField] private float ySpacing = .1f;
		[SerializeField] private GridCell cellPrefab;
		[Space]
		[SerializeField] private GridNodeHolder gridNodeHolderPrefab;
		[SerializeField] private GridNode gridNodePrefab;
		[Space]
		[SerializeField] private GridNodeIndicator gridNodeIndicatorPrefab;
		[Space]
		[SerializeField] private GameObject fillerPrefab;
		[SerializeField] private GameObject fillerDotPrefab;

		[SerializeField, HideInInspector] private GridCellMatrix gridCells;
		public GridCellMatrix GridCells => gridCells;

		[SerializeField, HideInInspector] private List<GridNodeHolder> gridNodeHolders = new List<GridNodeHolder>();
		public List<GridNodeHolder> GridNodeHolders => gridNodeHolders;

		private void OnEnable()
		{
			Shape.OnPlace += OnShapePlaced;
		}

		private void OnDisable()
		{
			Shape.OnPlace -= OnShapePlaced;
		}

		private void OnShapePlaced(Shape shape)
		{
			if (CheckGrid())
			{
				GridManager.Instance.CompleteStage();
			}
		}

		public bool CheckGrid()
		{
			bool gridCompleted = false;
			for (var i = 0; i < gridNodeHolders.Count; i++)
			{
				var gridNodeHolder = gridNodeHolders[i];
				if (gridNodeHolder.CurrentValue.Equals(0))
				{
					gridCompleted = true;
				}
				else
					return false;

				for (int j = 0; j < gridNodeHolder.GridNodes.Count; j++)
				{
					if (!gridNodeHolder.GridNodes[j].GetCell().CurrentShape)
						return false;
				}
			}

			return gridCompleted;
		}

		#region Setup

#if UNITY_EDITOR
		public void SetupEditor(List<GridNodeInfo> nodeHolderInfo)
		{
			gridCells = new GridCellMatrix(Size.x, Size.y);

			var xOffset = (nodeSize.x * Size.x + xSpacing * (Size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * Size.y + ySpacing * (Size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < Size.y; y++)
			{
				for (int x = 0; x < Size.x; x++)
				{
					var cell = (GridCell)PrefabUtility.InstantiatePrefab(cellPrefab, transform);
					cell.Setup(x, y, Size.y * y + x, nodeSize);
					cell.gameObject.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
					gridCells[x, y] = cell;
				}
			}

			foreach (var gridNodeInfo in nodeHolderInfo)
			{
				var holder = (GridNodeHolder)PrefabUtility.InstantiatePrefab(gridNodeHolderPrefab, transform);
				holder.Setup(gridNodeInfo.Value);

				for (var i = 0; i < gridNodeInfo.Cells.Count; i++)
				{
					var cellInfo = gridNodeInfo.Cells[i];
					var node = (GridNode)PrefabUtility.InstantiatePrefab(gridNodePrefab, holder.transform);
					var cell = gridCells[cellInfo.Coordinates.x, cellInfo.Coordinates.y];
					node.transform.position = cell.transform.position;
					node.Setup(holder, cellInfo.Coordinates);
					cell.CurrentNode = node;

					holder.GridNodes.Add(node);
				}

				gridNodeHolders.Add(holder);
			}

			foreach (var gridNodeHolder in gridNodeHolders)
			{
				bool setupIndicator = false;
				foreach (var gridNode in gridNodeHolder.GridNodes)
				{
					// fillers
					var coor = gridNode.Coordinates;
					var coorRight = coor + Direction.Right;
					var coorDown = coor + Direction.Up;

					if (coorRight.x < gridCells.GetLength(0) && GetCell(coorRight).CurrentNode && GetCell(coorRight).CurrentNode.ParentHolder.Equals(gridNode.ParentHolder))
					{
						var filler = Instantiate(fillerPrefab, gridNodeHolder.transform);
						filler.transform.localPosition = gridNode.transform.localPosition + .55f * Vector3.right;
						filler.transform.localEulerAngles = 90 * Vector3.up;
					}

					if (coorDown.y < gridCells.GetLength(1) && GetCell(coorDown).CurrentNode && GetCell(coorDown).CurrentNode.ParentHolder.Equals(gridNode.ParentHolder))
					{
						var filler = Instantiate(fillerPrefab, gridNodeHolder.transform);
						filler.transform.localPosition = gridNode.transform.localPosition + .55f * Vector3.back;
					}

					if ((coorRight.x < gridCells.GetLength(0) && GetCell(coorRight).CurrentNode && GetCell(coorRight).CurrentNode.ParentHolder.Equals(gridNode.ParentHolder)) &&
					    (coorDown.y < gridCells.GetLength(1) && GetCell(coorDown).CurrentNode && GetCell(coorDown).CurrentNode.ParentHolder.Equals(gridNode.ParentHolder)) &&
					    (gridCells[coor.x + 1, coor.y + 1].CurrentNode && gridCells[coor.x + 1, coor.y + 1].CurrentNode.ParentHolder.Equals(gridNode.ParentHolder)))
					{
						var filler = Instantiate(fillerDotPrefab, gridNodeHolder.transform);
						filler.transform.localPosition = gridNode.transform.localPosition + new Vector3(0.55f, 0, -0.55f);
					}

					// Indicator
					if (setupIndicator) continue;
					foreach (var dir in Direction.Directions)
					{
						var newCoor = gridNode.Coordinates + dir;
						var adjacentCell = GetCell(newCoor);
						if (!adjacentCell) continue;
						if (adjacentCell.CurrentNode) continue;

						var indicator = (GridNodeIndicator)PrefabUtility.InstantiatePrefab(gridNodeIndicatorPrefab, gridNodeHolder.transform);
						var direction = (adjacentCell.transform.position - gridNode.transform.position).normalized;
						indicator.transform.position =
							gridNode.transform.position + new Vector3(0.75f * direction.x - .25f * Mathf.Abs(direction.y), 0.75f * direction.y + .25f * Mathf.Abs(direction.x));
						gridNodeHolder.GridNodeIndicator = indicator;
						indicator.SetValue(gridNodeHolder.Value);
						setupIndicator = true;
						break;
					}
				}
			}
		}
#endif

		#endregion

		public GridCell GetCell(Vector2Int coordinates)
		{
			if (coordinates.x < 0 || coordinates.y < 0 || coordinates.x >= gridCells.GetLength(0) || coordinates.y >= gridCells.GetLength(1)) return null;
			return gridCells[coordinates.x, coordinates.y];
		}

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

			public int GetLength(int dimension)
			{
				return dimension switch
				{
					0 => Arrays.Length,
					1 => Arrays[0].Cells.Length,
					_ => 0
				};
			}
		}
	}
}