using Managers;
using TriInspector;
using UnityEngine;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		[Title("Managers")]
		[SerializeField] private DeckManager deckManager;
		public DeckManager DeckManager => deckManager;

		[SerializeField] private GridManager gridManager;
		public GridManager GridManager => gridManager;

		public virtual void Load()
		{
			gameObject.SetActive(true);
		}

		public virtual void Play()
		{
		}
	}
}