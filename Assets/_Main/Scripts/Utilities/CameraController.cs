using Cinemachine;
using Fiber.Utilities;
using TriInspector;
using UnityEngine;

namespace Utilities
{
	public class CameraController : Singleton<CameraController>
	{
		public CinemachineVirtualCamera CurrentCamera { get; private set; }

		[Title("iPhone")]
		[SerializeField] private CinemachineVirtualCamera iphoneCam;
		[SerializeField] private float iphoneShadowDistance = 100;
		[Title("iPad")]
		[SerializeField] private CinemachineVirtualCamera ipadCam;
		[SerializeField] private float ipadShadowDistance = 120;

		private void Awake()
		{
			CurrentCamera = iphoneCam;

			AdjustByScreenRatio();
		}

		private void AdjustByScreenRatio()
		{
			var ratio = (float)Screen.width / Screen.height;
			if (ratio < 1.6f) // iPhone
			{
				iphoneCam.gameObject.SetActive(true);
				ipadCam?.gameObject.SetActive(false);
				CurrentCamera = iphoneCam;

				QualitySettings.shadowDistance = iphoneShadowDistance;
			}
			else if (ipadCam)
			{
				iphoneCam.gameObject.SetActive(false);
				ipadCam.gameObject.SetActive(true);
				CurrentCamera = ipadCam;

				QualitySettings.shadowDistance = ipadShadowDistance;
			}
		}
	}
}