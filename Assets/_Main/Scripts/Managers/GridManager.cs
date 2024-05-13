using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using LevelEditor;
using UnityEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class GridManager : Singleton<GridManager>
	{
		public Grid CurrentGridStage { get; set; }
		public int CurrentGridStageIndex { get; set; }

		[SerializeField] private Grid gridPrefab;
		[SerializeField] [HideInInspector] private List<Grid> stageGrids = new List<Grid>();

		[Space]
		[SerializeField] private Transform loadPoint;
		[SerializeField] private Transform completePoint;

		private const float MOVE_DURATION = .5F;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnLevelStarted()
		{
			LoadNewStage();
		}

		private void LoadNewStage()
		{
			CurrentGridStage = stageGrids[CurrentGridStageIndex];
			CurrentGridStage.gameObject.SetActive(true);
			CurrentGridStage.transform.position = loadPoint.position;
			CurrentGridStage.transform.DOLocalMove(Vector3.zero, MOVE_DURATION).SetEase(Ease.OutQuart);
		}

		public void CompleteStage()
		{
			var currentStage = CurrentGridStage;

			// Next Stage
			CurrentGridStageIndex++;
			if (CurrentGridStageIndex < stageGrids.Count)
			{
				currentStage.transform.DOMove(completePoint.position, MOVE_DURATION).SetEase(Ease.OutQuart).OnComplete(() => Destroy(currentStage.gameObject));
				LoadNewStage();
			}
			else
			{
				LevelManager.Instance.Win();
			}
		}

#if UNITY_EDITOR
		public void SetupGrids(Dictionary<Color, GridNodeInfo> nodeHolderInfos, int stageCount)
		{
			var nodeInfos = nodeHolderInfos.OrderBy(x => x.Value.TabIndex).Select(x => x.Value).ToArray();
			for (int i = 0; i < stageCount; i++)
			{
				var grid = (Grid)PrefabUtility.InstantiatePrefab(gridPrefab, transform);
				grid.SetupEditor(nodeInfos[i]);
				grid.gameObject.SetActive(false);

				stageGrids.Add(grid);
			}
		}
#endif
	}
}