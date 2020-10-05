using UnityEngine;

[CreateAssetMenu(fileName = "ThemePreset", menuName = "Theme Preset")]
public class ThemePreset : ScriptableObject
{
    public Price price;
    public Label label;
    public Color[] labelColors;
    public Color[] spriteColors;
    public Color[] spriteNoLabelColors;
    public Color emptyColor;
    public Color fieldColor;
    public Color text;
    public Color background;
}
