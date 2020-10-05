using UnityEngine;
using UnityEngine.Advertisements;

public class GameOverAds : MonoBehaviour
{
    
#if UNITY_ADS
    void OnEnable()
    {
        if (UserProgress.Current.IsItemPurchased("no_ads"))
            return;

        if (Advertisement.IsReady(PlacementId.Video))
            Advertisement.Show(PlacementId.Video);
    }
#endif
}
