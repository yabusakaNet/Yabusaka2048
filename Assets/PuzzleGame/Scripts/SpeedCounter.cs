using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Text))]
public class SpeedCounter : MonoBehaviour
{
    public Slider slider;
    public Image sliderImage;

    protected GameState currentGameState;

    protected virtual int Value {
        get { return currentGameState.Score; }
    }

    void Start ()
    {
        var constants = GameDesignConstantsBehaviour.Instance.GameDesignConstants;
        slider.minValue = constants.BaseSpeed;
        slider.maxValue = constants.MaxSpeed;

        OnProgressUpdate ();
        UserProgress.Current.ProgressUpdate += OnProgressUpdate;
    }

    void OnDestroy ()
    {
        UserProgress.Current.ProgressUpdate -= OnProgressUpdate;

        if (currentGameState != null)
            currentGameState.StateUpdate -= OnStateUpdate;
    }

    void OnProgressUpdate ()
    {
        GameState gameState = UserProgress.Current.GetGameState<GameState> (UserProgress.Current.CurrentGameId);

        if (currentGameState != null)
            currentGameState.StateUpdate -= OnStateUpdate;

        currentGameState = gameState;

        if (gameState == null)
            return;

        OnStateUpdate ();
        gameState.StateUpdate += OnStateUpdate;
    }

    void OnStateUpdate ()
    {
        var constants = GameDesignConstantsBehaviour.Instance.GameDesignConstants;

        slider.value = 1f + (Value / 1000f);

        if (slider.normalizedValue < 0.14f) {
            slider.normalizedValue = 0.14f;
        }

        if (slider.normalizedValue >= 1f) {
            sliderImage.color = constants.SpeedSliderFourthColor;
        } else if (slider.normalizedValue >= 0.75f) {
            sliderImage.color = constants.SpeedSliderThirdColor;
        } else if (slider.normalizedValue >= 0.5f) {
            sliderImage.color = constants.SpeedSliderSecondColor;
        } else if (slider.normalizedValue >= 0.25f) {
            sliderImage.color = constants.SpeedSliderFirstColor;
        }
    }
}