using Interfaces;
using TMPro;
using UnityEngine;

namespace GridSystem
{
	public class GridNode : MonoBehaviour, INode
	{
		public GridNodeHolder ParentHolder { get; private set; }

		[SerializeField] private TextMeshPro txtValue;

		public void SetValue(int value)
		{
			txtValue.SetText(value.ToString());
		}
	}
}