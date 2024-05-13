using System.Collections;
using Lean.Touch;
using Managers;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		[field: SerializeField, HideInInspector] public int Timer { get; set; }

		[Title("Managers")]
		[SerializeField] private DeckManager deckManager;
		public DeckManager DeckManager => deckManager;

		[SerializeField] private GridManager gridManager;
		public GridManager GridManager => gridManager;

		private int currentTime;
		public static event UnityAction<int> OnTimerTick;

		public virtual void Load()
		{
			gameObject.SetActive(true);
		}

		public virtual void Play()
		{
			LeanTouch.OnFingerDown += OnFirstTouch;
		}

		private void OnFirstTouch(LeanFinger finger)
		{
			LeanTouch.OnFingerDown -= OnFirstTouch;

			StartTimer();
		}

		private void StartTimer()
		{
			StartCoroutine(TimerCoroutine());
		}

		private IEnumerator TimerCoroutine()
		{
			var wait = new WaitForSeconds(1);

			currentTime = Timer;
			OnTimerTick?.Invoke(currentTime);

			while (currentTime > 0)
			{
				yield return wait;

				currentTime--;
				OnTimerTick?.Invoke(currentTime);
			}
		}
	}
}