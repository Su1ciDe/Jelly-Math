using TMPro;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeIndicator : MonoBehaviour
	{
		[SerializeField] private TextMeshPro txtValue;

		private void Awake()
		{
			txtValue.transform.up = Vector3.up;
		}

		public void SetValue(int value)
		{
			txtValue.SetText(value.ToString());
		}
	}
}