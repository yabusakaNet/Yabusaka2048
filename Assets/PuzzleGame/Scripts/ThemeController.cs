using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ThemeController
{
    static ThemeController instance;

    readonly ThemePreset[] themes;

    ThemePreset currentTheme;

    public event Action<ThemePreset> ThemeChanged = delegate { };
    public event Action<ThemePreset> ThemePurchased = delegate { };

    ThemeController()
    {
        themes = Resources.Load<ThemesCollection>("ThemesCollection").themes;
        CurrentTheme = Array.Find(themes, t => t.name == UserProgress.Current.CurrentThemeId) ?? themes[0];
    }

    public static ThemeController Instance => instance ?? (instance = new ThemeController());

    public ThemePreset CurrentTheme
    {
        get => currentTheme;
        set
        {
            Assert.IsTrue(Array.IndexOf(themes, value) >= 0);

            currentTheme = value;

            ThemeChanged.Invoke(currentTheme);
        }
    }

    public void SaveCurrentTheme()
    {
        UserProgress.Current.CurrentThemeId = currentTheme.name;
    }

    public ThemePreset[] GetThemes()
    {
        return (ThemePreset[]) themes.Clone();
    }

    public bool IsThemePurchased(ThemePreset theme)
    {
        return UserProgress.Current.IsItemPurchased(theme.name);
    }

    public void OnThemePurchased(ThemePreset theme)
    {
        UserProgress.Current.OnItemPurchased(theme.name);
        ThemePurchased.Invoke(theme);
    }
}
