using UnityEngine;

public class GameSelector : MonoBehaviour
{
    [SerializeField]
    GameObject navigation;
    [SerializeField]
    GameObject fieldBlocker;

    [SerializeField]
    GameObject restartButton;

    [SerializeField]
    GameObject gameOver;

    [SerializeField]
    GameController_2048Bricks currentGame;

    static readonly int BigField = Animator.StringToHash ("Big");
    static readonly int MiddleField = Animator.StringToHash ("Middle");
    static readonly int SmallField = Animator.StringToHash ("Small");

    void Awake ()
    {
        InitCurrentGame ();
    }

    public void MinimizeCurrentGame (bool value)
    {
        if (!value) {
            MaximizeCurrentGame ();
            currentGame.bgmSfx.Play ();
            return;
        }

        Time.timeScale = 0;
        ResetTriggers ();
        currentGame.fieldAnimator.SetTrigger (SmallField);
        navigation.SetActive (false);
        // restartButton.SetActive(false);
        currentGame.bgmSfx.Pause ();
    }

    void MaximizeCurrentGame ()
    {
        if (!gameOver.activeSelf) {
            Time.timeScale = 1;
            ResetTriggers ();
            currentGame.fieldAnimator.SetTrigger (BigField);
            navigation.SetActive (true);
            // restartButton.SetActive(true);
        } else {
            ResetTriggers ();
            currentGame.fieldAnimator.SetTrigger (MiddleField);
            navigation.SetActive (true);
            fieldBlocker.SetActive (true);
        }
    }

    void ResetTriggers ()
    {
        currentGame.fieldAnimator.ResetTrigger (BigField);
        currentGame.fieldAnimator.ResetTrigger (MiddleField);
        currentGame.fieldAnimator.ResetTrigger (SmallField);
    }

    void InitCurrentGame ()
    {
        gameOver.SetActive (false);

        Time.timeScale = 1;

        restartButton.SetActive (true);
        fieldBlocker.SetActive (false);

        currentGame.GameOver += OnGameOver;
    }

    void OnGameOver ()
    {
        ResetTriggers ();
        currentGame.fieldAnimator.SetTrigger (MiddleField);
        fieldBlocker.SetActive (true);

        restartButton.SetActive (false);
        gameOver.SetActive (true);

        currentGame.bgmSfx.Pause ();
    }
}
