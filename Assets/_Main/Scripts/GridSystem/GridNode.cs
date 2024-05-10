using Interfaces;
using TMPro;
using UnityEngine;

namespace GridSystem
{
	public class GridNode : MonoBehaviour, INode
	{
		[field: SerializeField, HideInInspector] public Vector2Int Coordinates { get; private set; }
		[field: SerializeField, HideInInspector] public GridNodeHolder ParentHolder { get; private set; }

		[SerializeField] private TextMeshPro txtValue;

		public void Setup(GridNodeHolder parentHolder, int value, Vector2Int coor)
		{
			ParentHolder = parentHolder;
			SetValue(value);
			SetCoordinates(coor);
		}

		public void SetValue(int value)
		{
			txtValue.SetText(value.ToString());
		}

		public void SetCoordinates(Vector2Int coor)
		{
			Coordinates = coor;
		}
	}
}