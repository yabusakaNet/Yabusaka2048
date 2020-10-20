using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent (typeof (Button))]
public class RestartButton : MonoBehaviour
{
    void Start ()
    {
        GetComponent<Button> ().onClick.AddListener (OnClick);
    }

    static void OnClick ()
    {
        UserProgress.Current.GetGameState<GameState> (UserProgress.Current.CurrentGameId).SetField (new int[0]);
        UserProgress.Current.SaveGameState (UserProgress.Current.CurrentGameId);
        UserProgress.Current.Save ();

        FadeManager.Instance.LoadScene (SceneManager.GetActiveScene ().name);
    }
}