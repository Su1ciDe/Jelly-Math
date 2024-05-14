using System.Collections;
using Fiber.Managers;
using Lean.Touch;
using Managers;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		[field: SerializeField] public int Timer { get; set; }

		[Title("Managers")]
		[SerializeField] private DeckManager deckManager;
		public DeckManager DeckManager => deckManager;

		[SerializeField] private GridManager gridManager;
		public GridManager GridManager => gridManager;

		private int currentTime;
		private readonly WaitForSeconds waitForSecond = new WaitForSeconds(1);
		private Coroutine timerCoroutine;

		public static event UnityAction<int> OnTimerTick;

		private void OnEnable()
		{
			LevelManager.OnLevelWin += OnLevelWon;
			LevelManager.OnLevelLose += OnLevelLost;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelWin -= OnLevelWon;
			LevelManager.OnLevelLose -= OnLevelLost;
		}

		private void OnLevelWon()
		{
			if (timerCoroutine is not null)
				StopCoroutine(timerCoroutine);
		}

		private void OnLevelLost()
		{
			if (timerCoroutine is not null)
				StopCoroutine(timerCoroutine);
		}

		public virtual void Load()
		{
			currentTime = Timer;

			gameObject.SetActive(true);
		}

		public virtual void Play()
		{
			LeanTouch.OnFingerDown += OnFirstTouch;
		}

		private void OnFirstTouch(LeanFinger finger)
		{
			LeanTouch.OnFingerDown -= OnFirstTouch;

			if (!Timer.Equals(0))
			{
				StartTimer();
			}
		}

		private void StartTimer()
		{
			timerCoroutine = StartCoroutine(TimerCoroutine());
		}

		private IEnumerator TimerCoroutine()
		{
			while (currentTime > 0)
			{
				yield return waitForSecond;

				currentTime--;
				OnTimerTick?.Invoke(currentTime);
			}

			if (currentTime <= 0)
			{
				LevelManager.Instance.Lose();
			}
		}
	}
}