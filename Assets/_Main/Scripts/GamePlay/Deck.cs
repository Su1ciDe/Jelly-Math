using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace GamePlay
{
	public class Deck : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public List<Shape> ShapesInDeck { get; private set; }

		[SerializeField] private GameObject deckCellPrefab;

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
			ShapesInDeck.Remove(shape);
			if (ShapesInDeck.Count <= 0)
			{
				DeckManager.Instance.CompleteDeck();
			}
		}
	}
}