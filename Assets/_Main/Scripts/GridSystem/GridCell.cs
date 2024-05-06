using GamePlay;
using UnityEngine;

namespace GridSystem
{
	public class GridCell : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public GridNode CurrentNode { get; set; }
		public Shape CurrentShape { get; set; }

		public bool IsShowingHighlight => highlight.gameObject.activeSelf;

		[field: SerializeField, HideInInspector] public Vector2Int Coordinates { get; private set; }
		public int Index { get; private set; }

		[SerializeField] private MeshRenderer highlight;
		[SerializeField] private MeshRenderer negativeHighlight;

		private const float HIGHLIGHT_DURATION = .4F;

		public void Setup(int x, int y, int index, Vector2 size)
		{
			Coordinates = new Vector2Int(x, y);
			Index = index;
			transform.localScale = new Vector3(size.x, 1f, size.y);
		}

		public void ShowHighlight()
		{
			// if (IsShowingHighlight) return;

			if (CurrentNode is not null && !CurrentShape)
			{
				highlight.gameObject.SetActive(true);
			}
			else
			{
				negativeHighlight.gameObject.SetActive(true);
			}
		}

		public void HideHighlight()
		{
			negativeHighlight.gameObject.SetActive(false);
			highlight.gameObject.SetActive(false);
		}
	}
}