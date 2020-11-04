using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NumberedBrick : Brick
{
    public enum CollectionType
    {
        Linear,
        Parallel
    }
    
    [SerializeField]
    Text label;

    [SerializeField]
    Image labelImage;

    [SerializeField]
    float moveDuration;

    [SerializeField]
    CollectionType sritesCollection;

    int number;
    int colorIndex;

    RectTransform rectTransform;

    public int Number
    {
        get => number;
        set
        {
            number = value;
            label.text = number.ToString();
        }
    }

    public int ColorIndex
    {
        get => colorIndex;
        set
        {
            colorIndex = value;
            UpdateColors(ThemeController.Instance.CurrentTheme);
        }
    }

    public void DoLocalMove(Vector2 position, Action onComplete)
    {
        StartCoroutine(LocalMove(position, onComplete));
    }

    public void DoLocalPath(List<Vector2> path, Action onComplete)
    {
        if (path.Count == 0)
        {
            onComplete?.Invoke();
            return;
        }

        Vector2 position = path[0];
        path.RemoveAt(0);
        DoLocalMove(position, () => DoLocalPath(path, onComplete));
    }

    public void DoLandingAnimation(Action onComplete)
    {
        GetComponent<Animator>().SetTrigger("Land");
        StartCoroutine(DelayedCall(onComplete, 0.15f));
    }

    public void DoMergingAnimation(Action onComplete)
    {
        GetComponent<Animator>().SetTrigger("Merge");
        StartCoroutine(DelayedCall(onComplete, 0.15f));
    }

    public void DoBlinkingAnimation()
    {
        GetComponent<Animator>().SetTrigger("Blink");
    }

    public void DoStopBlinking()
    {
        GetComponent<Animator>().SetTrigger("Default");
    }

    IEnumerator LocalMove(Vector2 position, Action onComplete)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float t = Time.deltaTime;
        while (t < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, position, t / moveDuration);
            yield return null;
            t += Time.deltaTime;
        }

        rectTransform.anchoredPosition = position;

        onComplete?.Invoke();
    }

    IEnumerator DelayedCall(Action onComplete, float delay)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
    }

    void UpdateColors(ThemePreset theme)
    {
        
        if (label)
        {
            if(labelImage)
                labelImage.gameObject.SetActive(false);
            label.gameObject.SetActive(theme.label.labelType == LabelType.Text);
            label.color = theme.labelColors[Mathf.Clamp(colorIndex, 0, theme.labelColors.Length - 1)];
        }
        
        if (labelImage && theme.label.labelType == LabelType.Sprite)
        {
            SpritesCollection collection = sritesCollection == CollectionType.Linear
                ? theme.label.linearCollection
                : theme.label.parallelCollection;

            if (collection != null)
            {
                labelImage.sprite = collection.sprites[Mathf.Clamp(colorIndex, 0, collection.sprites.Length - 1)];
                labelImage.color = theme.labelColors[Mathf.Clamp(colorIndex, 0, theme.labelColors.Length - 1)];
            }
            labelImage.gameObject.SetActive(collection != null);
        }


        if (!label && theme.spriteNoLabelColors != null && theme.spriteNoLabelColors.Length > 0)
            sprite.color = theme.spriteNoLabelColors[colorIndex % theme.spriteNoLabelColors.Length];
        else
            sprite.color = theme.spriteColors[Mathf.Clamp(colorIndex, 0, theme.spriteColors.Length - 1)];
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        UpdateColors(ThemeController.Instance.CurrentTheme);
        ThemeController.Instance.ThemeChanged += UpdateColors;
    }

    void OnDestroy()
    {
        ThemeController.Instance.ThemeChanged -= UpdateColors;
    }
}