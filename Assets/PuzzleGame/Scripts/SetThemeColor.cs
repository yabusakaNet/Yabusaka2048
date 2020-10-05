using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class SetThemeColor : MonoBehaviour
{
    enum ColorType
    {
        Text,
        Background,
        Field,
        Label,
        Sprite,
        Empty,
    }

    [SerializeField]
    ColorType colorType;
    [SerializeField]
    int index;
    [SerializeField]
    bool setAlpha = true;

    void UpdateColor(ThemePreset theme)
    {
        Graphic graphic = GetComponent<Graphic>();
        Color color;
        switch (colorType)
        {
            case ColorType.Text:
                color = theme.text;
                break;
            case ColorType.Background:
                color = theme.background;
                break;
            case ColorType.Field:
                color = theme.fieldColor;
                break;
            case ColorType.Label:
                color = theme.labelColors[Mathf.Clamp(index, 0, theme.labelColors.Length - 1)];
                break;
            case ColorType.Sprite:
                color = theme.spriteColors[Mathf.Clamp(index, 0, theme.spriteColors.Length - 1)];
                break;
            case ColorType.Empty:
                color = theme.emptyColor;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if(!setAlpha)
            color.a = graphic.color.a;
        
        graphic.color = color;
    }

    void Start()
    {
        UpdateColor(ThemeController.Instance.CurrentTheme);
        ThemeController.Instance.ThemeChanged += UpdateColor;
    }

    void OnDestroy()
    {
        ThemeController.Instance.ThemeChanged -= UpdateColor;
    }
}
