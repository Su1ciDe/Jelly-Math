using Fiber.Managers;
using Interfaces;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Player
{
	public class PlayerInputs : MonoBehaviour, IInputs
	{
		public bool CanInput { get; set; }

		[SerializeField] private Vector3 offset;
		[SerializeField] private float rotationDamping = 5;
		[SerializeField] private float rotationClamp = 75;
		[Space]
		[SerializeField] private LayerMask inputLayerMask;

		public event UnityAction<Vector3> OnDown;
		public event UnityAction<Vector3> OnMove;
		public event UnityAction<Vector3> OnUp;

		private void Awake()
		{
			LeanTouch.OnFingerDown += OnFingerDown;
			LeanTouch.OnFingerUpdate += OnFingerUpdate;
			LeanTouch.OnFingerUp += OnFingerUp;

			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;
		}

		private void OnDestroy()
		{
			LeanTouch.OnFingerDown -= OnFingerDown;
			LeanTouch.OnFingerUpdate -= OnFingerUpdate;
			LeanTouch.OnFingerUp -= OnFingerUp;

			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelStart -= OnLevelWon;
			LevelManager.OnLevelStart -= OnLevelLost;
		}

		private void OnFingerDown(LeanFinger finger)
		{
		}

		private void OnFingerUpdate(LeanFinger finger)
		{
		}

		private void OnFingerUp(LeanFinger finger)
		{
		}

		private void OnLevelStarted()
		{
			CanInput = true;
		}

		private void OnLevelWon()
		{
			CanInput = false;
		}

		private void OnLevelLost()
		{
			CanInput = false;
		}
	}
}