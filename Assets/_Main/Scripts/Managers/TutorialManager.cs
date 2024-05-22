using System.Collections;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay;
using UI;
using UnityEngine;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelUnload += Unsub;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelUnload -= Unsub;
		}

		private void OnDestroy()
		{
			Unsub();
		}

		private void Unsub()
		{
			tutorialUI?.HideHand();
			Shape.OnPlace -= OnShapePlaced;
			Shape.OnPlace -= OnShapePlacedLevel3;
			Shape.OnRemoved -= OnShapeRemoved;
		}

		private void OnLevelStarted()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(3))
			{
				Level3Tutorial();
			}
		}

		#region Level1

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

		#endregion

		#region Level3

		private void Level3Tutorial()
		{
			Shape.OnPlace += OnShapePlacedLevel3;
		}

		private Shape placedShape;

		private void OnShapePlacedLevel3(Shape shape)
		{
			Shape.OnPlace -= OnShapePlacedLevel3;

			placedShape = shape;
			tutorialUI.ShowSwipe(shape.transform.position, LevelManager.Instance.CurrentLevel.DeckManager.CurrentDeck.transform.position, Helper.MainCamera);

			Shape.OnRemoved += OnShapeRemoved;
		}

		private void OnShapeRemoved(Shape shape)
		{
			if (!placedShape.Equals(shape)) return;
			Shape.OnRemoved -= OnShapeRemoved;

			tutorialUI.HideHand();
		}

		#endregion
	}
}