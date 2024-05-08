using System.Collections.Generic;
using LevelEditor;
using UnityEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class GridManager : MonoBehaviour
	{
		public int CurrentGridStage { get; set; }
		
		[SerializeField] private Grid gridPrefab;
		[SerializeField] [HideInInspector] private List<Grid> stageGrids = new List<Grid>();

#if UNITY_EDITOR
		public void SetupGrids(Dictionary<Color, GridNodeInfo> nodeHolderInfos, int stageCount)
		{
			for (int i = 0; i < stageCount; i++)
			{
				var grid = (Grid)PrefabUtility.InstantiatePrefab(gridPrefab, transform);
				grid.SetupEditor(nodeHolderInfos);

				grid.gameObject.SetActive(false);

				stageGrids.Add(grid);
			}
		}

#endif
	}
}