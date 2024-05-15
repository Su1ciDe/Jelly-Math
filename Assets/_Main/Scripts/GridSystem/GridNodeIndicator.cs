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
			txtValue.color = Color.white;
		}

		public void SetValue(int value, bool showWrong = true)
		{
			txtValue.SetText(value.ToString());
			if (value.Equals(0))
				txtValue.color = correctColor;
			else if (showWrong)
				txtValue.color = wrongColor;
		}
	}
}