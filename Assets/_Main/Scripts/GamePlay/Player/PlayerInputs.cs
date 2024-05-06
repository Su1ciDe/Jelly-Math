using DG.Tweening;
using Fiber.AudioSystem;
using Fiber.Managers;
using Fiber.Utilities;
using Interfaces;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Events;

namespace GamePlay.Player
{
	public class PlayerInputs : MonoBehaviour, IInputs
	{
		public bool CanInput { get; set; }

		public Shape SelectedShape { get; private set; }

		[SerializeField] private Vector3 offset;
		[SerializeField] private float moveDamping = 10;
		[SerializeField] private float rotationDamping = 5;
		[SerializeField] private float rotationClamp = 75;
		[Space]
		[SerializeField] private LayerMask inputLayerMask;
		[SerializeField] private LayerMask planeLayerMask;

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
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, inputLayerMask))
			{
				if (hit.rigidbody && hit.rigidbody.TryGetComponent(out Shape pack) && pack.CanMove)
				{
					HapticManager.Instance.PlayHaptic(0.5f, 0.5f);
					// AudioManager.Instance.PlayAudio(AudioName.Pickup);

					pack.transform.DOKill();
					SelectedShape = pack;

					OnDown?.Invoke(hit.point);
				}
			}
		}

		private void OnFingerUpdate(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			if (!SelectedShape) return;
			if (!SelectedShape.CanMove) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, planeLayerMask))
			{
				var position = hit.point + offset;

				var rotateTo = Quaternion.Euler(new Vector3(Mathf.Clamp(-finger.ScreenDelta.y, -rotationClamp, rotationClamp), 0, Mathf.Clamp(finger.ScreenDelta.x, -rotationClamp, rotationClamp)));
				SelectedShape.Move(position,moveDamping, rotateTo, rotationDamping);
				OnMove?.Invoke(position);
			}
		}

		private void OnFingerUp(LeanFinger finger)
		{
			if (!CanInput) return;
			if (finger.IsOverGui) return;

			if (!SelectedShape) return;
			if (!SelectedShape.CanMove) return;

			var ray = finger.GetRay(Helper.MainCamera);
			if (Physics.Raycast(ray, out var hit, 100, planeLayerMask))
			{
				// AudioManager.Instance.PlayAudio(AudioName.Place);
				var position = hit.point + offset;
				SelectedShape.OnRelease();
				OnUp?.Invoke(position);
			}

			SelectedShape = null;
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