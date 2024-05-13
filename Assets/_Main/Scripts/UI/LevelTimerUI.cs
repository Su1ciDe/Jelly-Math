using DG.Tweening;
using Fiber.Managers;
using Fiber.LevelSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class LevelTimerUI : MonoBehaviour
	{
		[SerializeField] private TMP_Text txtTime;
		[SerializeField] private Image imgTime;

		private const float TIMER_ANIM_DURATION = .25f;

		private void Awake()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
		}

		private void Start()
		{
			SetupTimer();
		}

		private void OnEnable()
		{
			Level.OnTimerTick += OnTimerTick;
		}

		private void OnDisable()
		{
			Level.OnTimerTick -= OnTimerTick;
		}

		private void OnDestroy()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
		}

		private void OnLevelStarted()
		{
			SetupTimer();
		}

		private void SetupTimer()
		{
			if (!LevelManager.Instance.CurrentLevel.Timer.Equals(0))
			{
				gameObject.SetActive(true);
				OnTimerTick(LevelManager.Instance.CurrentLevel.Timer);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}

		private void OnTimerTick(int time)
		{
			txtTime.SetText(time.ToString());

			if (time < 10)
			{
				imgTime.transform.DOShakeRotation(TIMER_ANIM_DURATION, 30 * Vector3.forward, 50, 1, true, ShakeRandomnessMode.Harmonic).SetLoops(2, LoopType.Restart);
				txtTime.transform.DOScale(1.5f, TIMER_ANIM_DURATION).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo);
				txtTime.DOColor(Color.red, TIMER_ANIM_DURATION).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo);

				if (HapticManager.Instance.IsPlaying)
					HapticManager.Instance.StopHaptics();
				HapticManager.Instance.PlayHaptic(HapticManager.AdvancedHapticType.Heartbeats);
			}
		}
	}
}