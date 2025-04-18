using Fiber.Utilities;
using UnityEngine;

namespace Fiber.Managers
{
	[DefaultExecutionOrder(-1)]
	public class GameManager : SingletonInit<GameManager>
	{
		protected override void Awake()
		{
			base.Awake();
			Input.multiTouchEnabled = false;
			Application.targetFrameRate = 60;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
		}
	}
}