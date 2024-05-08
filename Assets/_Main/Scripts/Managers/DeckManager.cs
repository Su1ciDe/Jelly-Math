using System.Collections.Generic;
using GamePlay;
using LevelEditor;
using UnityEditor;
using UnityEngine;

namespace Managers
{
	public class DeckManager : MonoBehaviour
	{
		public int CurrentDeckStage { get; set; }

		[SerializeField] private Deck deckPrefab;
		[SerializeField] [HideInInspector] private List<Deck> stageDecks = new List<Deck>();

#if UNITY_EDITOR

		public void SetupDecks(Dictionary<Color, GridNodeInfo> nodeHolderInfos, int stageCount)
		{
			for (int i = 0; i < stageCount; i++)
			{
				var grid = (Deck)PrefabUtility.InstantiatePrefab(deckPrefab, transform);
				// grid.SetupEditor();

				grid.gameObject.SetActive(false);

				stageDecks.Add(grid);
			}
		}
#endif
	}
}