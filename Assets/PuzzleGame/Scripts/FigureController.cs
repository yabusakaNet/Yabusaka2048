using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FigureController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public float verticalOffset;
    public List<Brick> bricks = new List<Brick>();

    public event Action<FigureController> PointerUp = delegate { };

    RectTransform rectTransform;

    Vector2 cachedPosition;
    Vector3 cachedScale;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        cachedPosition = rectTransform.anchoredPosition;
        cachedScale = rectTransform.localScale;
    }

    public void ResetPosition()
    {
        rectTransform.anchoredPosition = cachedPosition;
        rectTransform.localScale = cachedScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartCoroutine(DoLocalMove(ScreenPointToAnchoredPosition(eventData.position)));
        StartCoroutine(DoLocalScale(Vector3.one));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (bricks.Count > 0)
            PointerUp.Invoke(this);

        StartCoroutine(DoLocalMove(cachedPosition));
        StartCoroutine(DoLocalScale(cachedScale));
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
    }

    Vector2 ScreenPointToAnchoredPosition(Vector2 screenPoint)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent.GetComponent<RectTransform>(),
            screenPoint,
            null,
            out Vector2 position
        );

        float yMin = float.MaxValue;

        foreach (Brick brick in bricks)
        {
            Vector2 brickPosition = transform.localRotation * brick.GetComponent<RectTransform>().anchoredPosition;
            yMin = Mathf.Min(yMin, brickPosition.y);
        }

        position.y += verticalOffset - yMin;

        return position;
    }


    IEnumerator DoLocalMove(Vector2 position)
    {
        Vector2 startPosition = rectTransform.anchoredPosition;
        float t = Time.deltaTime;
        while (t < 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, position, t / 0.1f);
            yield return null;
            t += Time.deltaTime;
        }

        rectTransform.anchoredPosition = position;
    }

    IEnumerator DoLocalScale(Vector3 scale)
    {
        Vector3 startScale = rectTransform.localScale;
        float t = Time.deltaTime;
        while (t < 0.1f)
        {
            rectTransform.localScale = Vector3.Lerp(startScale, scale, t / 0.1f);
            yield return null;
            t += Time.deltaTime;
        }

        rectTransform.localScale = scale;
    }
}
