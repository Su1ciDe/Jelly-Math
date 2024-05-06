using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace GamePlay
{
	[SelectionBase]
	public class Shape : MonoBehaviour
	{
		public int Value { get; private set; }
		public bool CanMove { get; set; } = true;

		[SerializeField] private Transform model;
		[SerializeField] private ShapeDetector[] detectors;

		private readonly List<GridNodeHolder> touchingGridNodeHolders = new List<GridNodeHolder>();

		public static event UnityAction<Shape> OnPlace;

		public void Move(Vector3 movePosition, float moveDamping, Quaternion rotateTo, float rotationDamping)
		{
			transform.position = Vector3.Lerp(transform.position, movePosition, Time.deltaTime * moveDamping);
			// model.rotation = Quaternion.Lerp(model.rotation, rotateTo, Time.deltaTime * rotationDamping);

			for (int i = 0; i < detectors.Length; i++)
			{
				detectors[i].CurrentCell?.HideHighlight();
				detectors[i].GetNearestCell();
				detectors[i].CurrentCell?.ShowHighlight();
			}

			// var nearestNode = GetNearestNode();
			// if (currentNearestGridCell)
			// {
			// 	if (!currentNearestGridCell.Equals(nearestNode))
			// 	{
			// 		currentNearestGridCell.HideHighlight();
			// 	}
			// }
			//
			// currentNearestGridCell = nearestNode;
			// if (currentNearestGridCell && !currentNearestGridCell.IsShowingHighlight)
			// {
			// 	currentNearestGridCell.ShowHighlight();
			// 	HapticManager.Instance.PlayHaptic(0.3f, 0);
			// }
		}

		public void OnRelease()
		{
			if (touchingGridNodeHolders.Count > 0)
			{
				// TODO: clear values
				for (var i = 0; i < touchingGridNodeHolders.Count; i++)
				{
				}

				touchingGridNodeHolders.Clear();
			}

			var canBePlaced = CanBePlaced();
			if (canBePlaced)
			{
				Place();
			}
			else
			{
				ResetPosition();
			}

			ResetRotation();
		}

		private void Place()
		{
			for (var i = 0; i < detectors.Length; i++)
			{
				if (!touchingGridNodeHolders.Contains(detectors[i].CurrentCell.CurrentNode.ParentHolder))
				{
					touchingGridNodeHolders.Add(detectors[i].CurrentCell.CurrentNode.ParentHolder);
				}
			}

			for (var i = 0; i < touchingGridNodeHolders.Count; i++)
			{
				touchingGridNodeHolders[i].PlaceShape(this);
			}

			var pos = GetMiddlePointOfDetectedCells();
			transform.DOMove(pos, .35f).SetEase(Ease.OutBack);

			OnPlace?.Invoke(this);
		}

		private void ResetPosition()
		{
		}

		private void ResetRotation()
		{
			// model.DORotateQuaternion(Quaternion.identity, .1f);
		}

		public bool CanBePlaced()
		{
			for (int i = 0; i < detectors.Length; i++)
			{
				var detector = detectors[i];
				if (!detector.CurrentCell) return false;
				if (detector.CurrentCell.CurrentNode is null || detector.CurrentCell.CurrentShape) return false;
			}

			return true;
		}

		private Vector3 GetMiddlePointOfDetectedCells()
		{
			var biggestAndLower = GetBiggestAndLower();
			var origin = (Grid.Instance.GridCells[biggestAndLower.xBiggest, biggestAndLower.yBiggest].transform.position +
			              Grid.Instance.GridCells[biggestAndLower.xSmallest, biggestAndLower.ySmallest].transform.position) / 2f;

			return origin;
		}

		private (int xBiggest, int yBiggest, int xSmallest, int ySmallest) GetBiggestAndLower()
		{
			int xSmallest = int.MaxValue, ySmallest = int.MaxValue;
			int xBiggest = 0, yBiggest = 0;

			for (int i = 0; i < detectors.Length; i++)
			{
				var cell = detectors[i].CurrentCell;
				if (cell.Coordinates.y < ySmallest)
					ySmallest = cell.Coordinates.y;

				if (cell.Coordinates.y > yBiggest)
					yBiggest = cell.Coordinates.y;

				if (cell.Coordinates.x < xSmallest)
					xSmallest = cell.Coordinates.x;

				if (cell.Coordinates.x > xBiggest)
					xBiggest = cell.Coordinates.x;
			}

			return (xBiggest, yBiggest, xSmallest, ySmallest);
		}
	}
}