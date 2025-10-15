//using AdjustSdk;
//using Data;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ITCGameStudio
{
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance;
        private float timeCheckInternetAvailable = 2f;
        public GameObject popupCheckInternet;

        public Tween tween;
        public GameObject popupNoAdsAvailable;
        public CanvasGroup NoAdsAvailable;

        public bool AdsRemovedStatus = false;
        public bool CheckInternetAvailable = false;

        private int firstAdsInter_Level = 16;
        private int firstAdsInter_LevelInterval = 1;

        private int currentInterAds_Count = 0;

        private IAdNetwork adNetwork;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
                Destroy(gameObject);
        }
        private void Start()
        {
            //CheckAvaiialbleNetwork
            Invoke("CheckAvailableNetwork", timeCheckInternetAvailable);
            FirebaseManager.Instance.GetValueRemoteAsync("FirstAdsInter_Level", (value) =>
            {
                int.TryParse(value.StringValue.ToString(), out firstAdsInter_Level);
            });
            FirebaseManager.Instance.GetValueRemoteAsync("FirstAdsInter_LevelInterval", (value) =>
            {
                int.TryParse(value.StringValue.ToString(), out firstAdsInter_LevelInterval);
                currentInterAds_Count = firstAdsInter_LevelInterval;
            });
            adNetwork = new MaxAdNetwork();
            adNetwork.Init(()=> {  });
        }

        private void CheckAvailableNetwork()
        {
            if (CheckInternetAvailable)
                popupCheckInternet.gameObject.SetActive(Application.internetReachability == NetworkReachability.NotReachable);
            Invoke("CheckAvailableNetwork", timeCheckInternetAvailable);
        }
        #region Reward

        public void ShowReward(UnityAction reward)
        {
            adNetwork.ShowRewardedAd(reward);
        }
        public bool isRewardAvailable()
        {
            //if (AdsCD.instance.Cheating) return true;
            return adNetwork.IsRewardedAdReady();
        }
        #endregion
        #region Interstitial
        public void ShowInterstitial(UnityAction callback = null)
        {
            FirebaseManager.Instance.LogEventWithOneParam($"ads_inter_request_success");
            if (IsInterstitialAvailable())
            {
                adNetwork.ShowInterstitial(callback);
            }
            else
            {
                adNetwork.ShowInterstitial(callback);
            }
        }
        public bool IsInterstitialAvailable()
        {
            //if (MainData.Instance.playerActivity.isRemoveAds == 1)
            //{
            //    return false;
            //}
            return adNetwork.IsInterstitialReady();
        }
        #endregion
        #region Banner
        public void ShowBanner()
        {
            adNetwork.ShowBanner();
        }
        public void HideBanner()
        {
            adNetwork.HideBanner();
        }
        #endregion
        public void PerformDisableAds()
        {
            Debug.Log("PerformDisableAds");
            AdsRemovedStatus = true;
            adNetwork.PerformDisableAds();
        }
    }
}
