using System.Collections.Generic;
using GamePlay;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeHolder : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public GridNodeIndicator GridNodeIndicator { get; set; }
		[field: SerializeField] public int Value { get; private set; }
		public int CurrentValue { get; private set; }

		[SerializeField, HideInInspector] private List<GridNode> gridNodes = new List<GridNode>();
		public List<GridNode> GridNodes => gridNodes;

		private void Awake()
		{
			CurrentValue = Value;
			SetValue(CurrentValue);
		}

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

			for (var i = 0; i < gridNodes.Count; i++)
			{
				var currentShape = gridNodes[i].GetCell().CurrentShape;
				if (!currentShape) continue;
				if (currentShape.Equals(shape))
					gridNodes[i].GetCell().CurrentShape = null;
			}
		}

		private void SetValue(int value)
		{
			CurrentValue = value;
			GridNodeIndicator.SetValue(CurrentValue);
		}
	}
}