using UnityEngine;

namespace GridSystem
{
	public class GridCell : MonoBehaviour
	{
		public bool IsShowingHighlight => highlight.gameObject.activeSelf;

		public Vector2Int Coordinates { get; private set; }
		public int Index { get; private set; }

		[SerializeField] private MeshRenderer highlight;

		private const float HIGHLIGHT_DURATION = .4F;

		public void Setup(int x, int y, int index, Vector2 size)
		{
			Coordinates = new Vector2Int(x, y);
			Index = index;
			transform.localScale = new Vector3(size.x, 1f, size.y);
		}
	}
}