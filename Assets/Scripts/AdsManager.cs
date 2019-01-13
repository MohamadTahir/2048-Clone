using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;
    private bool ShowInterstitial = false;
    public bool ShowingBanner = false;

    public static AdsManager instance;

    void Start()
    {
        #if UNITY_ANDROID
            string appId = "ca-app-pub-2295610326027167~4695415994";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-2295610326027167~1421227843";
#else
            string appId = "unexpected_platform";
#endif

        instance = this;
        MobileAds.Initialize(appId);
        RequestBanner();
    }

    public void RequestBanner()
    {

        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-2295610326027167/7940047506";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-2295610326027167/7774937106";
#else
            string adUnitId = "unexpected_platform";
#endif


        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        bannerView.OnAdFailedToLoad += HandleOnAdFailedToLoadBanner;
        bannerView.OnAdLoaded += HandleBannerLoaded;

        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void HandleBannerLoaded(object sender, EventArgs args)
    {
        ShowingBanner=true;
    }

    public void HandleOnAdFailedToLoadBanner(object sender, AdFailedToLoadEventArgs args)
    {
        ShowingBanner = false;
        InGameManager.instance.BannerRequestSent = false;
        bannerView.Destroy();
    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-2295610326027167/8866078371";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-2295610326027167/8600223172";
#else
            string adUnitId = "unexpected_platform";
#endif

        interstitial = new InterstitialAd(adUnitId);
        interstitial.OnAdClosed += HandleOnAdClosedInterstitial;
        interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoadBanner;
        interstitial.OnAdOpening += HandleOnAdOpenedInterstitial;
        interstitial.OnAdLoaded += HandleOnAdLoadedInterstitial;
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    private void HandleOnAdLoadedInterstitial(object sender, EventArgs args)
    {
        ShowInterstitial = true;
    }

    public void HandleOnAdOpenedInterstitial(object sender, EventArgs args)
    {
        InGameManager.instance.NumbOfSpawns = 0;
    }

    public void HandleOnAdClosedInterstitial(object sender, EventArgs args)
    {
        InGameManager.instance.InterstitialRequestSent = false;
        interstitial.Destroy();
    }

    public void HandleOnAdFailedToLoadInterstitial(object sender, AdFailedToLoadEventArgs args)
    {
        InGameManager.instance.NumbOfSpawns = 0;
        InGameManager.instance.InterstitialRequestSent = false;
        interstitial.Destroy();
    }

    public void ShowAdd()
    {
        if (ShowInterstitial) {
            if (interstitial.IsLoaded())
            {
                Debug.Log("shwoing Interstitial");
                interstitial.Show();
            }
        }
    }
}
