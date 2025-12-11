using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using static MaxSdkCallbacks;
using UnityEngine.UIElements;
using System;
using System.Threading.Tasks;

public class MaxAdNetwork : IAdNetwork
{
    string bannerAdUnitId = "";
    string interstitialAdUnitId = "";
    string rewardAdUnitId = "";
    public void Init(UnityAction onComplete)
    {

#if UNITY_IOS
        bannerAdUnitId = "1f5385515dd01eb5";
        interstitialAdUnitId ="d175c07cdf3118b2";
        rewardAdUnitId="df03eaa65912282b";
#elif UNITY_ANDROID
        bannerAdUnitId = "7e1fc85bbc422f6f";
        interstitialAdUnitId = "b1c533d7a7cece9f";
        rewardAdUnitId = "dd5ac7be7e268fa1";
#else
        bannerAdUnitId = "";
        interstitialAdUnitId="";
        rewardAdUnitId="";
#endif
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfig) =>
        {
            Debug.Log("AppLovin SDK Initialized");
            InitializeBannerAds();
            InitializeInterstitialAds();
            InitializeRewardedAds();
            onComplete?.Invoke();
        };
        MaxSdk.InitializeSdk();
    }
    #region InitBanner
    public void InitializeBannerAds()
    {
        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        var adViewConfiguration = new MaxSdk.AdViewConfiguration(MaxSdk.AdViewPosition.BottomCenter);
        MaxSdk.CreateBanner(bannerAdUnitId, adViewConfiguration);

        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }
    #endregion
    #region InitInterstitial
    int retryAttemptInterstitial;
    UnityAction actionInterstitial;

    public void InitializeInterstitialAds()
    {
        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        // Load the first interstitial
        LoadInterstitial();
    }
    private async void RetryLoadInterstitial(double delay)
    {
        // Delay không chặn main thread
        await Task.Delay(TimeSpan.FromSeconds(delay));

        // Sau khi chờ xong, thực hiện hàm load
        LoadInterstitial();
    }
    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'
        // Reset retry attempt
        retryAttemptInterstitial = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)
        retryAttemptInterstitial++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptInterstitial));
        RetryLoadInterstitial(retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        actionInterstitial?.Invoke();
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
    }
    #endregion
    #region InitReward
    int retryAttemptReward;
    UnityAction actionReward;

    public void InitializeRewardedAds()
    {
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }
    private async void RetryLoadReward(double delay)
    {
        // Delay không chặn main thread
        await Task.Delay(TimeSpan.FromSeconds(delay));

        // Sau khi chờ xong, thực hiện hàm load
        LoadRewardedAd();
    }
    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(rewardAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttemptReward = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttemptReward++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttemptReward));

        RetryLoadReward(retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdk.ErrorInfo errorInfo, MaxSdk.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdk.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdk.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        actionReward?.Invoke();
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdk.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
    }
    #endregion
    public bool IsInterstitialReady()
    {
        return MaxSdk.IsInterstitialReady(interstitialAdUnitId);
    }

    public bool IsRewardedAdReady()
    {
        return MaxSdk.IsRewardedAdReady(rewardAdUnitId);
    }

    public void PerformDisableAds()
    {
        HideBanner();
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
    }
    public void ShowInterstitial(UnityAction onComplete = null)
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
            actionInterstitial = onComplete;
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }
        else
        {
            actionInterstitial = null;
            onComplete?.Invoke();   
        }
    }

    public void ShowRewardedAd(UnityAction onComplete = null)
    {
        if (MaxSdk.IsRewardedAdReady(rewardAdUnitId))
        {
            actionReward = onComplete;
            MaxSdk.ShowRewardedAd(rewardAdUnitId);
        }
    }
}
