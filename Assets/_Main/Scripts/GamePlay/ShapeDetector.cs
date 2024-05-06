using System.Collections.Generic;
using GridSystem;
using UnityEngine;

namespace GamePlay
{
	public class ShapeDetector : MonoBehaviour
	{
		public GridCell CurrentCell { get; private set; }

		private GridCell currentNearestGridCell;
		private readonly List<GridCell> triggeredCells = new List<GridCell>();

		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell cell))
			{
				if (!triggeredCells.Contains(cell))
				{
					triggeredCells.Add(cell);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell cell))
			{
				if (triggeredCells.Contains(cell))
				{
					triggeredCells.Remove(cell);
					cell.HideHighlight();
				}
			}
		}

		public GridCell GetNearestCell()
		{
			if (triggeredCells.Count.Equals(0)) return null;

			GridCell nearestCell = null;
			var shortestDistance = float.MaxValue;
			for (int i = 0; i < triggeredCells.Count; i++)
			{
				if ((transform.position - triggeredCells[i].transform.position).sqrMagnitude < shortestDistance)
				{
					nearestCell = triggeredCells[i];
					shortestDistance = (transform.position - nearestCell.transform.position).sqrMagnitude;
				}
			}

			CurrentCell = nearestCell;
			return nearestCell;
		}
	}
}