using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Mediation;
using UnityEngine.Purchasing;

namespace Gismo.Ads
{
    public enum AdType
    {
        Rewarded,
        Skippable
    }
    public class AdManager : MonoBehaviour
    {
        IInterstitialAd interstitialAd;
        IRewardedAd rewardedAd;
        public static AdManager instance;

        private string gameID_Android = "4505535";
        private string gameID_IOS= "4505534";
        private string gameID;

        private readonly string rewardedID_Android = "Rewarded_Android";
        private readonly string rewardedID_IOS = "Rewarded_iOS";
        private string rewardedID;

        private readonly string skipableID_Android = "Interstitial_Android";
        private readonly string skipableID_IOS = "Interstitial_iOS";
        private string skipableID;

        [SerializeField] private string noAdsID;

        private void Awake()
        {
            if (instance)
                return;
            instance = this;
        }

        public void Initalize()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                gameID = gameID_Android;

                rewardedID = rewardedID_Android;
                skipableID = skipableID_Android;
            }
            else
            {
                gameID = gameID_IOS;
                rewardedID = rewardedID_IOS;
                skipableID = skipableID_IOS;
            }

            InitServices();
        }

        public async void InitServices()
        {
            try
            {
                InitializationOptions initializationOptions = new InitializationOptions();
                initializationOptions.SetGameId(gameID);
                await UnityServices.InitializeAsync(initializationOptions);

                InitializationComplete();
            }
            catch (Exception e)
            {
                InitializationFailed(e);
            }
        }

        public int ShowAd(AdType adType)
        {
            if (Core.Statics.noAds)
            {
                Debug.Log("No ads purchased");
                return -1;
            }

            switch (adType)
            {
                case AdType.Rewarded:
                    if (rewardedAd == null || rewardedAd.AdState != AdState.Loaded)
                    {
                        Debug.Log("Ads aren't ready");
                        return -5;
                    }
                    break;
                default:
                case AdType.Skippable:
                    if (interstitialAd == null || interstitialAd.AdState != AdState.Loaded)
                    {
                        Debug.Log("Ads aren't ready");
                        return -5;
                    }
                    break;
            }

            switch(adType)
            {
                case AdType.Rewarded:
                    rewardedAd.Show();
                    break;
                case AdType.Skippable:
                default:
                    interstitialAd.Show(); ;
                    break;
            }
            return 0;
        }
        void SetupAds()
        {
            //Create
            rewardedAd = MediationService.Instance.CreateRewardedAd(rewardedID);
            interstitialAd = MediationService.Instance.CreateInterstitialAd(skipableID);

            rewardedAd.OnShowed += OnRewardedAdShown;

            interstitialAd.OnShowed += OnInterstitialAdShown;
        }

        void InitializationComplete()
        { 
            if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(noAdsID) != null && CodelessIAPStoreListener.Instance.StoreController.products.WithID(noAdsID).hasReceipt)
            {
                Core.Statics.noAds = true;
            }

            SetupAds();

            interstitialAd.Load();
            rewardedAd.Load();
        }

        void InitializationFailed(Exception e)
        {
            Debug.Log("Initialization Failed: " + e.Message);
        }

        void OnInterstitialAdShown(object sender, EventArgs args)
        {
            //pre-load the next ad
            interstitialAd.Load();
        }

        void OnRewardedAdShown(object sender, EventArgs args)
        {
            //pre-load the next ad
            rewardedAd.Load();
        }
    }
}