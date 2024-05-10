using System.Collections.Generic;
using GamePlay;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeHolder : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public GridNodeIndicator GridNodeIndicator { get; set; }
		[field: SerializeField, HideInInspector] public int Value { get; private set; }
		public int CurrentValue { get; private set; }

		[SerializeField, HideInInspector] private List<GridNode> gridNodes = new List<GridNode>();
		public List<GridNode> GridNodes => gridNodes;

		public void Setup(int value)
		{
			Value = value;
		}

		public void PlaceShape(Shape shape)
		{
			SetValue(CurrentValue - shape.Value);
		}

		public void RemoveShape(Shape shape)
		{
			SetValue(CurrentValue + shape.Value);
		}

		private void SetValue(int value)
		{
			CurrentValue = value;
			GridNodeIndicator.SetValue(CurrentValue);
		}
	}
}