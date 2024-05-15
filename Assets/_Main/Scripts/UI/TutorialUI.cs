using DG.Tweening;
using Fiber.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class TutorialUI : Singleton<TutorialUI>
	{
		[SerializeField] private Image hand;
		[Header("Text")]
		[SerializeField] private TMP_Text messageText;

		private Vector3 messagePosition;

		private const float HAND_MOVE_TIME = .7f;
		private const float HAND_TAP_TIME = .25f;

		private void Awake()
		{
			messagePosition = messageText.transform.position;
		}

		private void OnDestroy()
		{
			hand.DOKill();
			messageText.rectTransform.DOKill();
		}

		public void ShowSwipe(Vector3 from, Vector3 to)
		{
			var seq = DOTween.Sequence();
			seq.AppendCallback(() =>
			{
				hand.gameObject.SetActive(true);
				hand.rectTransform.position = from;
			});
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(.75f, HAND_TAP_TIME).SetEase(Ease.OutExpo));
			seq.Append(hand.rectTransform.DOMove(to, HAND_MOVE_TIME).SetEase(Ease.InSine));
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(1, HAND_TAP_TIME).SetEase(Ease.OutExpo));
			seq.AppendInterval(.5f);
			seq.AppendCallback(() => hand.gameObject.SetActive(false));
			seq.AppendInterval(.5f);
			seq.SetUpdate(true);
			seq.SetTarget(hand);
			seq.SetLoops(-1, LoopType.Restart);
			seq.OnKill(() =>
			{
				hand.rectTransform.localScale = Vector3.one;
				hand.gameObject.SetActive(false);
			});
		}

		public void ShowSwipe(Vector3 from, Vector3 to, Camera cam)
		{
			var _from = cam.WorldToScreenPoint(from);
			var _to = cam.WorldToScreenPoint(to);

			ShowSwipe(_from, _to);
		}

		public void ShowSwipe(Vector3 from, Vector3 to, Camera fromCamera, Camera toCamera)
		{
			var _from = fromCamera.WorldToScreenPoint(from);
			var _to = toCamera.WorldToScreenPoint(to);

			ShowSwipe(_from, _to);
		}

		public void ShowTap(Vector3 position, Camera cam = null)
		{
			var pos = position;
			if (cam) pos = cam.WorldToScreenPoint(position);

			var seq = DOTween.Sequence();
			seq.AppendCallback(() =>
			{
				hand.rectTransform.position = pos;
				hand.gameObject.SetActive(true);
			});
			seq.AppendInterval(.5f);
			seq.Append(hand.rectTransform.DOScale(.75f, HAND_TAP_TIME).SetEase(Ease.InOutExpo));
			seq.Append(hand.rectTransform.DOScale(1, HAND_TAP_TIME).SetEase(Ease.InOutExpo));
			seq.AppendInterval(.5f);
			seq.SetUpdate(true);
			seq.SetTarget(hand);
			seq.SetLoops(-1, LoopType.Restart);
			seq.OnKill(() =>
			{
				hand.rectTransform.localScale = Vector3.one;
				hand.gameObject.SetActive(false);
			});
		}

		public void HideHand()
		{
			hand.DOKill();
		}

		public void ShowText(string message, float showDuration = 0, bool isAnimated = false)
		{
			messageText.DOComplete();

			messageText.rectTransform.DOKill();
			messageText.SetText(message);
			messageText.gameObject.SetActive(true);
			if (isAnimated)
			{
				messageText.rectTransform.DOScale(1.25f, .25f).SetEase(Ease.InOutCubic).SetLoops(-1, LoopType.Yoyo).OnKill(() => messageText.rectTransform.localScale = Vector3.one)
					.SetTarget(messageText).SetUpdate(true);
			}

			if (!showDuration.Equals(0))
				DOVirtual.DelayedCall(showDuration, HideText).SetTarget(messageText).SetUpdate(true);
		}

		public void ShowText(string message, Vector3 position, Camera cam = null, float showDuration = 0, bool isAnimated = false)
		{
			if (cam) cam.WorldToScreenPoint(position);

			ShowText(message, showDuration, isAnimated);
		}

		public void HideText()
		{
			messageText.rectTransform.DOKill();
			messageText.gameObject.SetActive(false);
		}

		public bool IsShowingText()
		{
			return messageText.gameObject.activeSelf;
		}
	}
}