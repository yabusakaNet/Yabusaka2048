using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameController : MonoBehaviour
{
    public event Action GameOver = delegate { };

    public Vector2Int bricksCount;
    public RectTransform fieldTransform;
    public NumberedBrick brickPrefab;

    [Range(0f, 1f)]
    public float coinProbability;

    public Animator fieldAnimator;

    protected NumberedBrick[,] field;

    protected virtual IEnumerable<Vector2Int> GetAdjacentCoords(Vector2Int coords)
    {
        List<Vector2Int> adjacent = new List<Vector2Int>();

        Vector2Int up = new Vector2Int(coords.x, coords.y + 1);
        if (up.y < field.GetLength(1))
            adjacent.Add(up);

        Vector2Int down = new Vector2Int(coords.x, coords.y - 1);
        if (down.y >= 0)
            adjacent.Add(down);

        Vector2Int left = new Vector2Int(coords.x - 1, coords.y);
        if (left.x >= 0)
            adjacent.Add(left);

        Vector2Int right = new Vector2Int(coords.x + 1, coords.y);
        if (right.x < field.GetLength(0))
            adjacent.Add(right);

        return adjacent;
    }

    protected virtual Vector2 GetBrickPosition(Vector2 coords)
    {
        Rect rect = fieldTransform.rect;
        Vector2 brickSize = new Vector2
        {
            x = rect.width / bricksCount.x,
            y = rect.height / bricksCount.y
        };

        RectTransform brickTransform = brickPrefab.GetComponent<RectTransform>();

        Vector2 brickPosition = Vector2.Scale(coords, brickSize);
        brickPosition += Vector2.Scale(brickSize, brickTransform.pivot);

        return brickPosition;
    }

    protected void OnGameOver()
    {
        GameOver.Invoke();
    }
}
