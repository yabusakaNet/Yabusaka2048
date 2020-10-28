using UnityEngine;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Events;

public class GoogleAds : MonoBehaviour
{
    // 関数を登録するもの：広告表示をしたときに実行する（Unityエディタから設定）
    public UnityEvent OnOpeningAd;
    // 関数を登録するもの：広告表示を閉じたときに実行する（Unityエディタから設定）
    public UnityEvent OnClosedAd;

    private InterstitialAd interstitial;

    private void Start()
    {
        RequestInterstitial();
    }

    // 広告読み込み：次の広告表示の前に実行しておく必要あり
    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-6239259349327423/1848044380";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        DestroyInterstitialAd();

        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    // OnAdLoaded イベントは、広告の読み込みが完了すると実行されます。
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
    }

    // OnAdFailedToLoad イベントは、広告の読み込みに失敗すると呼び出されます。Message パラメータは、発生した障害のタイプを示します。
    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args.Message);
    }

    // このメソッドは、広告がデバイスの画面いっぱいに表示されたときに呼び出されます。
    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdOpened event received");
        OnOpeningAd.Invoke();
    }

    // このメソッドは、ユーザーが「閉じる」アイコンまたは「戻る」ボタンをタップしてインタースティシャル広告を閉じたときに呼び出されます。音声出力やゲームループを一時停止するアプリの場合は、ここで再開すると効果的です。
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdClosed event received");
        OnClosedAd.Invoke();
        RequestInterstitial();
    }

    //OnAdOpened の後にユーザーが別のアプリ（Google Play ストアなど）を起動し、現在のアプリがバックグラウンドに移動すると、このメソッドが呼び出されます。
    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLeavingApplication event received");
    }

    public void ShowInterstitialAd()
    {
        if (this.interstitial.IsLoaded())
        {
            this.interstitial.Show();
        }
        else
        {
            Debug.Log("まだ読み込みができていない");
        }
    }

    // 一度リセット：次の広告表示の前に実行しておく必要あり
    public void DestroyInterstitialAd()
    {
        interstitial.Destroy();
    }
}