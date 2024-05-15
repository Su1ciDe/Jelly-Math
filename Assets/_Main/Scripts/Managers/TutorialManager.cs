using System.Collections;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay;
using UI;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnDestroy()
		{
			Shape.OnPlace -= OnShapePlaced;
		}

		private void OnLevelStarted()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}
		}

		private IEnumerator Level1Tutorial()
		{
			yield return new WaitForSeconds(1);

			var firstShape = DeckManager.Instance.CurrentDeck.ShapesInDeck[0];
			var firstNode = GridManager.Instance.CurrentGridStage.GridNodeHolders[0].GridNodes;
			var pos = Vector3.zero;
			foreach (var gridNode in firstNode)
				pos += gridNode.transform.position;

			pos /= firstNode.Count;
			tutorialUI.ShowSwipe(firstShape.transform.position, pos, Helper.MainCamera);

			Shape.OnPlace += OnShapePlaced;
		}

		private void OnShapePlaced(Shape shape)
		{
			Shape.OnPlace -= OnShapePlaced;

			tutorialUI.HideHand();
		}
	}
}