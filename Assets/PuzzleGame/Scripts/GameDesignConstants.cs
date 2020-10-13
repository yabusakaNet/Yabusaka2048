using UnityEngine;

[CreateAssetMenu (menuName = "Yabusaka/Create GameDesignConstants")]
public class GameDesignConstants : ScriptableObject
{
    [SerializeField]
    public float BaseSpeed = 1f;

    [SerializeField]
    public float MaxSpeed = 3f;

    [SerializeField]
    public Color SpeedSliderFirstColor;

    [SerializeField]
    public Color SpeedSliderSecondColor;

    [SerializeField]
    public Color SpeedSliderThirdColor;

    [SerializeField]
    public Color SpeedSliderFourthColor;
}
