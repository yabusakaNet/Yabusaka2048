using System;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class MonetizeButton : 
#if UNITY_ADS
    MonoBehaviour, IUnityAdsListener
#else
    MonoBehaviour
#endif
{
    const int AdsReward = 25;

    [SerializeField]
    Button buy;
    [SerializeField]
    Button watchAds;
    [SerializeField]
    Button getCoins;
    [SerializeField]
    GameObject loading;

    PriceLabel priceLabel;

    string Item { get; set; }
    Price Price { get; set; }

    bool isWatchAdsClicked;
    bool isGetCoinsClicked;

    public event Action PurchaseComplete = delegate { };

    int PurchaseProgress
    {
        get => UserProgress.Current.GetItemPurchaseProgress(Item);
        set => UserProgress.Current.SetItemPurchaseProgress(Item, value);
    }

    public void SetPrice(string item, Price price)
    {
        Item = item;
        Price = price;

        UpdateButtons();

        if (price.value <= UserProgress.Current.GetItemPurchaseProgress(item))
            PurchaseComplete.Invoke();
    }

    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == PlacementId.RewardedVideo)
            UpdateButtons();
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError(message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

#if UNITY_ADS
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId != PlacementId.RewardedVideo)
            return;

        switch (showResult)
        {
            case ShowResult.Failed:
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Finished when isWatchAdsClicked:
                isWatchAdsClicked = false;

                PurchaseProgress++;

                if (Price.value <= PurchaseProgress)
                    PurchaseComplete.Invoke();

                UpdateButtons();
                break;
            case ShowResult.Finished when isGetCoinsClicked:
                isGetCoinsClicked = false;
                UserProgress.Current.Coins += AdsReward;
                UpdateButtons();
                break;
        }
    }
#endif

    void UpdateButtons()
    {
        buy.gameObject.SetActive(false);
        getCoins.gameObject.SetActive(false);
        watchAds.gameObject.SetActive(false);
        loading.SetActive(false);

        switch (Price.type)
        {
            case PriceType.Coins when UserProgress.Current.Coins >= Price.value:
                buy.gameObject.SetActive(true);
                break;
#if UNITY_ADS
            case PriceType.Coins when Advertisement.IsReady(PlacementId.RewardedVideo):
                getCoins.gameObject.SetActive(true);
                break;
            case PriceType.Ads when Advertisement.IsReady(PlacementId.RewardedVideo):
                watchAds.gameObject.SetActive(true);
                break;
#endif
            default:
                loading.SetActive(true);
                break;
        }
    }

    void OnBuyClick()
    {
        UserProgress.Current.Coins -= Price.value;
        PurchaseComplete.Invoke();
    }

#if UNITY_ADS

    void OnWatchAdsClick()
    {
        isWatchAdsClicked = true;
        Advertisement.Show(PlacementId.RewardedVideo);
    }

    void OnGetCoinsClick()
    {
        isGetCoinsClicked = true;
        Advertisement.Show(PlacementId.RewardedVideo);
    }
#endif
    void Awake()
    {
        buy.onClick.AddListener(OnBuyClick);
#if UNITY_ADS
        Advertisement.AddListener(this);
        watchAds.onClick.AddListener(OnWatchAdsClick);
        getCoins.onClick.AddListener(OnGetCoinsClick);
#endif
    }
    

    void OnEnable()
    {
        UpdateButtons();
    }
}
