using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private BannerView bannerView;
    private InterstitialAd interstitial;
  
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
        RequestInterstitial();
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

        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void HandleOnAdFailedToLoadBanner(object sender, AdFailedToLoadEventArgs args)
    {
        bannerView.Destroy();
        RequestBanner();
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
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public void HandleOnAdOpenedInterstitial(object sender, EventArgs args)
    {
        InGameManager.instance.NumbOfSpawns = 0;
    }


    public void HandleOnAdClosedInterstitial(object sender, EventArgs args)
    {
        interstitial.Destroy();
        RequestInterstitial();
    }

    public void HandleOnAdFailedToLoadInterstitial(object sender, AdFailedToLoadEventArgs args)
    {
        interstitial.Destroy();
        RequestInterstitial();
    }

    public void ShowAdd()
    {
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
    }
}
