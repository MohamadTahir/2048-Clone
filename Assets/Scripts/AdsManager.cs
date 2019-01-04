using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    private string appId = "ca-app-pub-2295610326027167~4695415994";
    private string adUnitId = "ca-app-pub-2295610326027167/7940047506";
    private string adUnitIdVid = /*test "ca-app-pub-3940256099942544/1033173712"*/"ca-app-pub-2295610326027167/8866078371";
    private BannerView bannerView;
    private InterstitialAd interstitial;
  
    public static AdsManager instance;

    void Start()
    {
        instance = this;
        MobileAds.Initialize(appId);
        RequestBanner();
        RequestInterstitial();
    }

    public void RequestBanner()
    {
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    public void RequestInterstitial()
    {
        interstitial = new InterstitialAd(adUnitIdVid);
        interstitial.OnAdClosed += HandleOnAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
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
