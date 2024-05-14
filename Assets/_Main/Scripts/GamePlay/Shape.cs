using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Grid = GridSystem.Grid;

namespace GamePlay
{
	[SelectionBase]
	public class Shape : MonoBehaviour
	{
		public bool IsInGrid { get; set; }
		public bool IsInDeck { get; set; } = true;

		[field: SerializeField] public int Value { get; private set; }
		public bool CanMove { get; set; } = true;

		[SerializeField] private Transform model;
		public ShapeDetector[] Detectors => detectors;
		[SerializeField] private ShapeDetector[] detectors;
		[Space]
		[SerializeField] private TextMeshPro txtValue;

		[SerializeField, HideInInspector] private Vector3 startingPosition;

		private readonly List<GridNodeHolder> touchingGridNodeHolders = new List<GridNodeHolder>();

		private const float MOVE_DURATION = .35f;

		public static event UnityAction<Shape> OnPlace;

		private void Awake()
		{
			SetValue(Value);
			txtValue.transform.up = Vector3.up;
		}

		public void Setup(int value)
		{
			SetValue(value);
			startingPosition = transform.position;
		}

		public void Move(Vector3 movePosition, float moveDamping)
		{
			transform.position = Vector3.Lerp(transform.position, movePosition, Time.deltaTime * moveDamping);

			for (int i = 0; i < detectors.Length; i++)
			{
				detectors[i].CurrentCell?.HideHighlight();
				detectors[i].GetNearestCell();
				detectors[i].CurrentCell?.ShowHighlight();
			}
		}

		public void OnPickUp()
		{
			SetActiveDetectors(true);
			if (touchingGridNodeHolders.Count > 0)
			{
				for (var i = 0; i < touchingGridNodeHolders.Count; i++)
					touchingGridNodeHolders[i].RemoveShape(this);

				touchingGridNodeHolders.Clear();
			}
		}

		public void OnRelease()
		{
			if (!IsInGrid)
			{
				
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

			for (var i = 0; i < detectors.Length; i++)
			{
				if (detectors[i].CurrentCell)
				{
					detectors[i].CurrentCell.HideHighlight();
					detectors[i].CurrentCell = null;
				}
			}
		}

		private void Place()
		{
			for (var i = 0; i < detectors.Length; i++)
			{
				var holder = detectors[i].CurrentCell.CurrentNode.ParentHolder;
				if (!holder) continue;
				if (!touchingGridNodeHolders.Contains(holder))
				{
					touchingGridNodeHolders.Add(holder);
					holder.PlaceShape(this);
				}

				detectors[i].CurrentCell.CurrentShape = this;
			}

			CanMove = false;
			var pos = GetMiddlePointOfDetectedCells();
			transform.DOMove(pos, MOVE_DURATION).SetEase(Ease.OutBack).OnComplete(() =>
			{
				CanMove = true;
				OnPlace?.Invoke(this);
			});

			transform.SetParent(GridManager.Instance.CurrentGridStage.transform);

			IsInGrid = true;
			IsInDeck = false;
		}

		public void ResetPosition()
		{
			CanMove = false;
			transform.DOMove(startingPosition, MOVE_DURATION).SetEase(Ease.OutExpo).OnComplete(() => CanMove = true);
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

		public void SetValue(int value)
		{
			Value = value;
			txtValue.SetText(Value.ToString());
		}

		public void SetActiveDetectors(bool active)
		{
			for (int i = 0; i < detectors.Length; i++)
			{
				detectors[i].SetActiveDetector(active);
			}
		}
	}
}