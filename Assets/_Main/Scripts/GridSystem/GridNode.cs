using Interfaces;
using UnityEngine;

namespace GridSystem
{
	public class GridNode : MonoBehaviour, INode
	{
		[field: SerializeField, HideInInspector] public Vector2Int Coordinates { get; private set; }
		[field: SerializeField, HideInInspector] public GridNodeHolder ParentHolder { get; private set; }
		
		public void Setup(GridNodeHolder parentHolder, Vector2Int coor)
		{
			ParentHolder = parentHolder;
			SetCoordinates(coor);
		}

		public void SetCoordinates(Vector2Int coor)
		{
			Coordinates = coor;
		}
	}
}