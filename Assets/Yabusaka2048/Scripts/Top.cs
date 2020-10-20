using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Top : MonoBehaviour
{

    public void OnRestart ()
    {
        UserProgress.Current.GetGameState<GameState> (UserProgress.Current.CurrentGameId).SetField (new int[0]);
        UserProgress.Current.SaveGameState (UserProgress.Current.CurrentGameId);
        UserProgress.Current.Save ();

        FadeManager.Instance.LoadScene (SceneManager.GetActiveScene ().name);
        FadeManager.Instance.LoadScene ("Game");
    }
    public void OnRetry ()
    {
        FadeManager.Instance.LoadScene ("Game");
    }
}