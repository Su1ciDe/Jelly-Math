using System.Collections.Generic;
using Fiber.Managers;
using GamePlay;
using LevelEditor;
using UnityEditor;
using UnityEngine;

namespace Managers
{
	public class DeckManager : MonoBehaviour
	{
		public int CurrentDeckStage { get; set; }
		public Deck CurrentDeck { get; set; }

		[SerializeField] private Deck deckPrefab;
		[SerializeField] private GameObject deckCellPrefab;

		[SerializeField] [HideInInspector] private List<Deck> stageDecks = new List<Deck>();

		private const float SIZE = 1f;

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
			CurrentDeckStage = 0;
			CurrentDeck = stageDecks[CurrentDeckStage];
		}

		#region Editor

#if UNITY_EDITOR
		public void SetupDecks(List<DeckCellInfo[,]> deckCellInfos, Dictionary<string, List<Vector2Int>> shapePairs)
		{
			foreach (var deckCells in deckCellInfos)
			{
				var deck = (Deck)PrefabUtility.InstantiatePrefab(deckPrefab, transform);
				var createdIDs = new List<string>();

				var sizeX = deckCells.GetLength(0);
				var sizeY = deckCells.GetLength(1);
				var xOffset = (SIZE * sizeX) / 2f - SIZE / 2f;
				var yOffset = (SIZE * sizeY) / 2f - SIZE / 2f;
				for (int x = 0; x < sizeX; x++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						// Deck Cells
						var cell = (GameObject)PrefabUtility.InstantiatePrefab(deckCellPrefab, deck.transform);
						cell.gameObject.name = x + " - " + y;
						cell.transform.localPosition = new Vector3(x * (SIZE) - xOffset, -y * (SIZE) + yOffset);

						// Shapes
						var info = deckCells[x, y];
						if (!info.Shape) continue;
						if (createdIDs.Contains(info.Id)) continue;

						createdIDs.Add(info.Id);
						var shape = (Shape)PrefabUtility.InstantiatePrefab(info.Shape, deck.transform);
						shape.transform.localEulerAngles = (int)info.Direction * 90 * Vector3.forward;
						var midPoint = GetMidPoint(shapePairs[info.Id]);
						shape.transform.localPosition = new Vector3(midPoint.x * SIZE - xOffset, -midPoint.y * SIZE + yOffset);
						shape.Setup(info.Value);

						deck.ShapesInDeck.Add(shape);
					}
				}

				deck.gameObject.SetActive(false);
				stageDecks.Add(deck);
			}
		}

		private Vector3 GetMidPoint(List<Vector2Int> cells)
		{
			int xSmallest = int.MaxValue, ySmallest = int.MaxValue;
			int xBiggest = 0, yBiggest = 0;

			for (int i = 0; i < cells.Count; i++)
			{
				var cell = cells[i];
				if (cell.y < ySmallest)
					ySmallest = cell.y;

				if (cell.y > yBiggest)
					yBiggest = cell.y;

				if (cell.x < xSmallest)
					xSmallest = cell.x;

				if (cell.x > xBiggest)
					xBiggest = cell.x;
			}

			var origin = (new Vector3(xBiggest, yBiggest) + new Vector3(xSmallest, ySmallest)) / 2f;

			return origin;
		}
#endif

		#endregion
	}
}