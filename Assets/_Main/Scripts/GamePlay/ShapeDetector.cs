using GridSystem;
using UnityEngine;

namespace GamePlay
{
	public class ShapeDetector : MonoBehaviour
	{
		public GridCell CurrentCell { get; private set; }

		private void OnTriggerEnter(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell cell))
			{
				if (CurrentCell)
					CurrentCell.HideHighlight();

				CurrentCell = cell;
				CurrentCell.ShowHighlight();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.attachedRigidbody && other.attachedRigidbody.TryGetComponent(out GridCell cell))
			{
				if (CurrentCell == cell)
				{
					CurrentCell.HideHighlight();
					CurrentCell = null;
				}
			}
		}
	}
}