using UnityEngine.Events;

public interface IAdNetwork
{
    void Init(UnityAction onComplete);
    void ShowInterstitial(UnityAction onComplete = null);
    void ShowRewardedAd(UnityAction onComplete = null);
    void ShowBanner();
    void HideBanner();
    void PerformDisableAds();

    bool IsInterstitialReady();
    bool IsRewardedAdReady();
}