using UnityEngine;

/// <summary>
/// システム上でかならずどこかに一つ作られている Singleton っぽい MonoBehavior を取得するためのインターフェイス
/// </summary>
public class MonoBehaviourSingleton<T> : MonoBehaviour
    where T : MonoBehaviourSingleton<T>
{
    #region unity message handlers

    void Awake ()
    {
        if (Instance != this) {
            Debug.LogError ("Internal error, _Instance is already set. MAYBE MULTIPLE " + typeof (T) + " exist");
        }
        Instance = (T)this;
        UserAwake ();
    }

    void OnDestroy ()
    {
        UserOnDestroy ();
        Instance = null;
    }

    #endregion

    protected virtual void UserAwake ()
    {
    }

    protected virtual void UserOnDestroy ()
    {
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static T Instance {
        get {
            if (_Instance == null) {
                _Instance = GameObject.FindObjectOfType<T> ();
            }
            return _Instance;
        }
        private set {
            _Instance = value;
        }
    }

    private static T _Instance;
}