using System.Collections.Generic;
using GridSystem;
using LevelEditor;
using UnityEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		public int CurrentStage { get; set; }
		[SerializeField] private Grid gridPrefab;

		[SerializeField] private List<Grid> stageGrids = new List<Grid>();

		public virtual void Load()
		{
			gameObject.SetActive(true);
		}

		public virtual void Play()
		{
		}

#if UNITY_EDITOR

		public void SetupLevel(Dictionary<Color, GridNodeInfo> nodeHolderInfos, int stageCount)
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