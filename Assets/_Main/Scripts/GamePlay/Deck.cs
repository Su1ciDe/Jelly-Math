using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GamePlay
{
	public class Deck : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public List<Shape> ShapesInDeck { get; private set; }

		[SerializeField] private Vector2 nodeSize = new Vector2(1, 1);
		[SerializeField] private float xSpacing = 0;
		[SerializeField] private float ySpacing = 0;
		private Vector2Int size;

		[SerializeField] private GameObject deckCellPrefab;

#if UNITY_EDITOR

		public void SetupEditor(Vector2Int gridSize)
		{
			size = gridSize;
			
			var xOffset = (nodeSize.x * size.x + xSpacing * (size.x - 1)) / 2f - nodeSize.x / 2f;
			var yOffset = (nodeSize.y * size.y + ySpacing * (size.y - 1)) / 2f - nodeSize.y / 2f;
			for (int y = 0; y < size.y; y++)
			{
				for (int x = 0; x < size.x; x++)
				{
					var cell = (GameObject)PrefabUtility.InstantiatePrefab(deckCellPrefab, transform);
					// cell.Setup(x, y, size.y * y + x, nodeSize);
					cell.name = x + " - " + y;
					cell.transform.localPosition = new Vector3(x * (nodeSize.x + xSpacing) - xOffset, 0, -y * (nodeSize.y + ySpacing) + yOffset);
				}
			}
		}

#endif
	}
}