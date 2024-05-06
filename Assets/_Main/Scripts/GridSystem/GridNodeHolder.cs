using System.Collections.Generic;
using GamePlay;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeHolder : MonoBehaviour
	{
		public int Value { get; private set; }
		public int CurrentValue { get; private set; }

		private List<GridNode> gridNodes = new List<GridNode>();
		public List<GridNode> GridNodes => gridNodes;

		public void Setup(int value)
		{
			Value = value;
		}

		public void PlaceShape(Shape shape)
		{
			
		}
	}
}