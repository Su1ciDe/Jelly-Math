using TMPro;
using UnityEngine;

namespace GridSystem
{
	public class GridNodeIndicator : MonoBehaviour
	{
		[SerializeField] private TextMeshPro txtValue;
		[Space]
		[SerializeField] private Color correctColor;
		[SerializeField] private Color wrongColor;

		private void Awake()
		{
			txtValue.transform.up = Vector3.up;
		}

		public void SetValue(int value)
		{
			txtValue.SetText(value.ToString());
			if (value.Equals(0))
			{
				txtValue.color = correctColor;
			}
		}
	}
}