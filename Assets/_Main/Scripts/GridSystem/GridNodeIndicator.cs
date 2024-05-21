using DG.Tweening;
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

		private bool isShowingHighlight;
		private Vector3 startingPosition;

		private const float HIGHLIGHT_DURATION = .25F;

		private void Awake()
		{
			txtValue.transform.up = Vector3.up;
			txtValue.color = Color.white;

			startingPosition = transform.position;
		}

		private void OnDestroy()
		{
			transform.DOKill();
			txtValue.transform.DOKill();
		}

		public void SetValue(int value, bool showWrong = true)
		{
			txtValue.SetText(value.ToString());
			if (value.Equals(0))
				txtValue.color = correctColor;
			else if (showWrong)
				txtValue.color = wrongColor;
		}

		public void Highlight()
		{
			if (isShowingHighlight) return;
			isShowingHighlight = true;

			txtValue.transform.DOKill();
			txtValue.transform.DOScale(1.25f, HIGHLIGHT_DURATION);
		}

		public void HideHighlight()
		{
			isShowingHighlight = false;
			
			txtValue.transform.DOKill();
			txtValue.transform.DOScale(1f, HIGHLIGHT_DURATION);
		}
	}
}