using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Checkers
{
	public class AdsController : Singleton<AdsController>
	{
		[SerializeField]
		private GameController _gameManagerComponent;

		[Space(5f)]
		[SerializeField]
		private string _androidInterId;
		[SerializeField]
		private string _androidRewardId;
		[SerializeField]
		private string _androidBannerId;
		[SerializeField]
		private string _iosInterId;
		[SerializeField]
		private string _iosRewardId;
		[SerializeField]
		private string _iosBannerId;
		
		private readonly string _testBannerId = "ca-app-pub-3940256099942544/6300978111";
		private readonly string _testIntersitialId = "ca-app-pub-3940256099942544/1033173712";
		private readonly string _testRewardId = "ca-app-pub-3940256099942544/5224354917";

		[Space(5f)]
		[SerializeField]
		private int _firstCallIntersitialTime;
		[SerializeField]
		private int _repeatIntersitialTime;

		private AdRequest _requestAdmob;
		private BannerView _bannerView;
		private InterstitialAd _interstitialAndroid;

		private RewardBasedVideoAd _rewardBasedVideoAndroid;
		private RewardBasedVideoAd _rewardDeactivateAdsVideoAndroid;

		[Space]
		[Tooltip("How many times player need to lose for watching ads.")]
		public int LosesCount;

		[Space]
		public bool IsTestADS;
		public bool IsBanner;
		public bool IsIntersitial;
		public bool IsReward;

		private AdSize _currentBannerSize = AdSize.Banner;
		private int _adsCounter;

		public readonly string NoAdsKey = "NoAds";

		[Space]
		public UnityEvent NoAdsRewardClosedEvent;
		public UnityEvent NoAdsRewardNotLoadedEvent;
		public UnityEvent NoAdsRewardSuccessWatchEvent;
		public UnityEvent BannerHideEvent;

		private void Start()
		{
			InitializeADS();
		}

		/// <summary>
		/// Initialize admob requests variable.
		/// </summary>
		private void AdMobRequest()
		{
			_requestAdmob = (IsTestADS) ? new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).Build() : new AdRequest.Builder().Build();
		}

		#region Requests ADS
		private void RequestBanner()
		{
			if (IsHasKeyNoAds())
				return;
			_bannerView = new BannerView((IsTestADS) ? _testBannerId : GetPlatformBannerId(), _currentBannerSize, AdPosition.Bottom);

			if (_bannerView != null)
			{
				AdMobRequest();
				_bannerView.LoadAd(_requestAdmob);
			}
		}

		/// <summary>
		/// Intersitial video request.
		/// </summary>
		public void RequestInterstitial()
		{
			if (IsHasKeyNoAds())
				return;

			_interstitialAndroid = new InterstitialAd((IsTestADS) ? _testIntersitialId : GetPlatformIntersitialId());
			AdMobRequest();
			_interstitialAndroid.LoadAd(_requestAdmob);
		}

		/// <summary>
		/// Reward video request.
		/// </summary>
		private void RequestRewardBasedVideo()
		{
			if (IsHasKeyNoAds())
				return;

			_rewardBasedVideoAndroid = RewardBasedVideoAd.Instance;
			AdMobRequest();
			_rewardBasedVideoAndroid.LoadAd(_requestAdmob, (IsTestADS) ? _testRewardId : GetPlatformRewardId());
		}

		/// <summary>
		/// Get admob banner id for platform
		/// </summary>
		public string GetPlatformBannerId()
		{
			return Application.platform == RuntimePlatform.Android ? _androidBannerId : _iosBannerId;
		}

		/// <summary>
		/// Get admob intersitial id for platform
		/// </summary>
		public string GetPlatformIntersitialId()
		{
			return Application.platform == RuntimePlatform.Android ? _androidInterId : _iosInterId;
		}

		/// <summary>
		/// Get admob reward id for platform
		/// </summary>
		public string GetPlatformRewardId()
		{
			return Application.platform == RuntimePlatform.Android ? _androidRewardId : _iosRewardId;
		}

		/// <summary>
		/// Reward no ads video request.
		/// </summary>
		private void RequestRewardNoAdsVideo()
		{
			if (IsHasKeyNoAds())
				return;

			_rewardDeactivateAdsVideoAndroid = RewardBasedVideoAd.Instance;
			AdMobRequest();
			_rewardDeactivateAdsVideoAndroid.LoadAd(_requestAdmob, (IsTestADS) ? _testRewardId : GetPlatformRewardId());
		}
		#endregion

		#region Show/Hide ADS
		/// <summary>
		/// Show banner ads <see cref="_bannerView"/> if ads available for watch.
		/// </summary>
		public void ShowBanner()
		{
			if (IsHasKeyNoAds())
				return;
			if (_bannerView != null)
			{
				_bannerView.Show();
			}
		}

		/// <summary>
		/// Show intersitial ads <see cref="_interstitial"/> if ads available for watch.
		/// </summary>
		public void ShowInterstitial()
		{
			if (IsHasKeyNoAds())
				return;

			if (_interstitialAndroid.IsLoaded())
			{
				_interstitialAndroid.Show();

				RequestInterstitial();
			}
		}

		/// <summary>
		/// Show reward video <see cref="_rewardBasedVideoAndroid"/> if ads available for watch.
		/// </summary>
		[ContextMenu("+")]
		public void ShowRewardBasedVideo()
		{
			if (IsHasKeyNoAds())
				return;

			_adsCounter--;

			if (_adsCounter > 0)
			{
				return;
			}

			_adsCounter = LosesCount;

			if (_rewardBasedVideoAndroid.IsLoaded())
			{
				_rewardBasedVideoAndroid.Show();

				RequestRewardBasedVideo();
			}
		}

		/// <summary>
		/// This method hide Smart banner from bottom of screen.
		/// </summary>
		public void HideBanner()
		{
			if (_bannerView != null)
				_bannerView.Hide();

			BannerHideEvent?.Invoke();
		}
		/// <summary>
		/// Show reward video. If user watch it the ads will disappear for current game session.
		/// </summary>
		public void ShowRewardNoAdsVideo()
		{
			if (_rewardDeactivateAdsVideoAndroid.IsLoaded())
			{
				_rewardDeactivateAdsVideoAndroid.OnAdRewarded += HandleRewardBasedVideoRewarded;
				_rewardDeactivateAdsVideoAndroid.OnAdClosed += HandleClosedBasedVideoRewarded;
				_rewardDeactivateAdsVideoAndroid.Show();
			}
			else
			{
				NoAdsRewardNotLoadedEvent?.Invoke();
				AlertPopUpController.ShowAlertPopUp("Rewarded Video has not been loaded yet!");
			}
		}
		#endregion

		#region EventsHandlers
		/// <summary>
		/// On close reward video event.
		/// </summary>
		private void HandleClosedBasedVideoRewarded(object sender, EventArgs eventArgs)
		{
			_rewardDeactivateAdsVideoAndroid.OnAdClosed -= HandleClosedBasedVideoRewarded;

			RequestRewardNoAdsVideo();

			AlertPopUpController.ShowAlertPopUp("You have been closed ads too early!");

			_rewardDeactivateAdsVideoAndroid.OnAdClosed += HandleClosedBasedVideoRewarded;
		}

		/// <summary>
		/// On full watch reward video event.
		/// </summary>
		public void HandleRewardBasedVideoRewarded(object sender, Reward args)
		{
			PlayerPrefs.SetInt(NoAdsKey, 1);

			HideBanner();

			NoAdsRewardSuccessWatchEvent?.Invoke();

			_rewardDeactivateAdsVideoAndroid.OnAdRewarded -= HandleRewardBasedVideoRewarded;
			_rewardDeactivateAdsVideoAndroid.OnAdClosed -= HandleClosedBasedVideoRewarded;
		}
		#endregion

		/// <summary>
		/// Initialize all active advertisment.
		/// </summary>
		public void InitializeADS()
		{
			if (IsHasKeyNoAds())
				PlayerPrefs.DeleteKey(NoAdsKey);

			if (IsBanner)
			{
				RequestBanner();
				ShowBanner();
			}

			if (IsReward)
			{
				RequestRewardBasedVideo();
				RequestRewardNoAdsVideo();
			}

			if (IsIntersitial)
			{
				RequestInterstitial();
				// First call after _firstCallIntersitialTime seconds. Repeating intersitial video every _repeatIntersitialTime seconds.
				InvokeRepeating("ShowInterstitial", _firstCallIntersitialTime, _repeatIntersitialTime);
			}
			_adsCounter = LosesCount;
		}

		/// <summary>
		/// Check for exist in player prefs key <see cref="NoAdsKey"/>
		/// </summary>
		/// <returns></returns>
		private bool IsHasKeyNoAds()
		{
			return PlayerPrefs.HasKey(NoAdsKey);
		}

		private float GetAdmobBannerScaleBasedOnDPI()
		{
			//By default banner has no scaling.
			float scale = 1f ;

			//All information about scaling has provided on Google Admob API
			//Low Density Screens, around 120 DPI, scaling factor 0.75, e.g. 320×50 becomes 240×37.
			//Medium Density Screens, around 160 DPI, no scaling, e.g. 320×50 stays at 320×50.
			//High Density Screens, around 240 DPI, scaling factor 1.5, e.g. 320×50 becomes 480×75.
			//Extra High Density Screens, around 320 DPI, scaling factor 2, e.g. 320×50 becomes 640×100.
			//Extra Extra High Density Screens, around 480 DPI, scaling factor 3, e.g. 320×50 becomes 960×150.

			if (Screen.dpi > 480)
			{
				scale = 3f;
			}
			else if (Screen.dpi > 320)
			{
				scale = 2f;
			}
			else if (Screen.dpi > 240)
			{
				scale = 1.5f;
			}
			else if (Screen.dpi > 160)
			{
				scale = 1f;
			}
			else if (Screen.dpi > 120)
			{
				scale = 0.75f;
			}

			return scale;
		}
	}
}