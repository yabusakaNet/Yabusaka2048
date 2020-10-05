using UnityEngine;
using UnityEngine.UI;

public class ThemePanel : Panel
{
    [SerializeField]
    ThemePreview themePreviewPrefab;
    [SerializeField]
    Transform themePreviewParent;

    [SerializeField]
    GameObject playButton;
    [SerializeField]
    MonetizeButton monetizeButton;

    ThemePreset lastAvailableTheme;

    protected override void Show()
    {
        UpdateButtons();
        content.SetActive(true);
    }

    protected override void Hide()
    {
        ThemeController.Instance.CurrentTheme = lastAvailableTheme;
        ThemeController.Instance.SaveCurrentTheme();
        content.SetActive(false);
    }

    void UpdateButtons()
    {
        ThemePreset theme = ThemeController.Instance.CurrentTheme;

        playButton.SetActive(false);
        monetizeButton.gameObject.SetActive(false);

        if (theme.price.value == 0 || UserProgress.Current.IsItemPurchased(theme.name))
        {
            lastAvailableTheme = theme;

            playButton.SetActive(true);
        }
        else
        {
            monetizeButton.SetPrice(theme.name, theme.price);
            monetizeButton.gameObject.SetActive(true);
        }
    }

    void OnThemeClick(ThemePreview themePreview)
    {
        ThemeController.Instance.CurrentTheme = themePreview.Theme;
        UpdateButtons();
    }

    void OnPurchaseComplete()
    {
        ThemeController.Instance.OnThemePurchased(ThemeController.Instance.CurrentTheme);
        lastAvailableTheme = ThemeController.Instance.CurrentTheme;

        UpdateButtons();
    }

    protected override void Awake()
    {
        base.Awake();

        ThemePreset[] themePresets = ThemeController.Instance.GetThemes();
        foreach (ThemePreset theme in themePresets)
        {
            ThemePreview themePreview = Instantiate(themePreviewPrefab, themePreviewParent);
            themePreview.Theme = theme;
            themePreview.Click += OnThemeClick;
        }

        monetizeButton.PurchaseComplete += OnPurchaseComplete;

        UpdateButtons();
    }
}
