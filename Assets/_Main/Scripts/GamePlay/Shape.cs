using System.Collections.Generic;
using DG.Tweening;
using GridSystem;
using UnityEngine;

namespace GamePlay
{
	[SelectionBase]
	public class Shape : MonoBehaviour
	{
		public bool CanMove { get; set; } = true;

		[SerializeField] private Transform model;
		[SerializeField] private ShapeDetector[] detectors;

		private List<GridNodeHolder> touchingGridNodeHolders = new List<GridNodeHolder>();

		public void Move(Vector3 movePosition, Quaternion rotateTo, float rotationDamping)
		{
			transform.position = movePosition;
			model.rotation = Quaternion.Lerp(model.rotation, rotateTo, Time.deltaTime * rotationDamping);

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
		}

		private void ResetRotation()
		{
			model.DORotateQuaternion(Quaternion.identity, .1f);
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
	}
}