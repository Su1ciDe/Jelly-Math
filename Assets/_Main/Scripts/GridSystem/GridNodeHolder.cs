using System.Collections.Generic;
using GamePlay;
using TMPro;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeHolder : MonoBehaviour
	{
		public int Value { get; private set; }
		public int CurrentValue { get; private set; }

		[SerializeField] private TextMeshPro txtValue;

		private List<GridNode> gridNodes = new List<GridNode>();
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
			txtValue.SetText(CurrentValue.ToString());
		}
	}
}