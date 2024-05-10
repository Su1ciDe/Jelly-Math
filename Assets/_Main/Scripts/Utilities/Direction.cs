using UnityEngine;

namespace Utilities
{
	public class Direction
	{
		public static Vector2Int[] Directions;

		public static Vector2Int Up = Vector2Int.up;
		public static Vector2Int Right = Vector2Int.right;
		public static Vector2Int Down = Vector2Int.down;
		public static Vector2Int Left = Vector2Int.left;

		static Direction()
		{
			Directions = new[] { Up, Right, Down, Left };
		}
	}
}