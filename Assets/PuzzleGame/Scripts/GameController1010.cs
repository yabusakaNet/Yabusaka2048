using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController1010 : BaseGameController
{
    public Brick emptyBrickPrefab;

    public FigureController[] figureControllers;

    public PlaySfx landingSfx;
    public PlaySfx mergingSfx;

    int[] figures = new int[0];
    float[] figureRotations = new float[0];

    GameState1010 gameState;

    void Start()
    {
        field = new NumberedBrick[bricksCount.x, bricksCount.y];

        for (int x = 0; x < bricksCount.x; x++)
        {
            for (int y = 0; y < bricksCount.y; y++)
            {
                SpawnEmptyBrick(new Vector2Int(x, y));
            }
        }

        gameState = UserProgress.Current.GetGameState<GameState1010>(name);
        if (gameState == null)
        {
            gameState = new GameState1010();
            UserProgress.Current.SetGameState(name, gameState);
        }

        UserProgress.Current.CurrentGameId = name;

        foreach (FigureController figureController in figureControllers)
        {
            figureController.PointerUp += FigureOnPointerUp;
        }

        if (LoadGame())
            return;

        gameState.Score = 0;

        SpawnNewFigures();
        SpawnStartingBricks();
    }

    void SpawnStartingBricks()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        for (int i = 0; i < bricksCount.x; i++)
        {
            for (int j = 0; j < bricksCount.y; j++)
            {
                positions.Add(new Vector2Int(i, j));
            }
        }

        for (int i = 1; i <= 9; i++)
        {
            int rand = Random.Range(0, positions.Count);
            Spawn(positions[rand]);
            positions.RemoveAt(rand);
        }
    }

    bool LoadGame()
    {
        int[] numbers = gameState.GetField();
        if (numbers == null || numbers.Length != bricksCount.x * bricksCount.y)
            return false;

        for (int x = 0; x < bricksCount.x; x++)
        {
            for (int y = 0; y < bricksCount.y; y++)
            {
                if (numbers[x * bricksCount.y + y] > 0)
                    Spawn(new Vector2Int(x, y));
            }
        }

        figures = gameState.GetFigures();
        figureRotations = gameState.GetFigureRotations();

        if (figures.Length != figureControllers.Length)
            return false;

        for (int i = 0; i < figureControllers.Length; i++)
        {
            if (figures[i] >= 0)
                SpawnFigure(figureControllers[i], figures[i], figureRotations[i]);
        }

        return true;
    }

    void SaveGame()
    {
        int[] numbers = new int[bricksCount.x * bricksCount.y];
        for (int x = 0; x < bricksCount.x; x++)
        {
            for (int y = 0; y < bricksCount.y; y++)
            {
                numbers[x * bricksCount.y + y] = field[x, y] != null ? 1 : 0;
            }
        }

        gameState.SetField(numbers);

        gameState.SetFigures(figures);
        gameState.SetFigureRotations(figureRotations);
        
        UserProgress.Current.SaveGameState(name);
    }

    void Spawn(Vector2Int coords)
    {
        NumberedBrick brick = Instantiate(brickPrefab, fieldTransform);

        brick.transform.SetParent(fieldTransform, false);
        brick.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        brick.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        brick.GetComponent<RectTransform>().anchoredPosition = GetBrickPosition(coords);

        brick.ColorIndex = Random.Range(0, 6);

        field[coords.x, coords.y] = brick;
    }

    void SpawnEmptyBrick(Vector2Int coords)
    {
        Brick brick = Instantiate(emptyBrickPrefab, fieldTransform);

        brick.transform.SetParent(fieldTransform, false);
        brick.GetComponent<RectTransform>().anchorMin = Vector2.zero;
        brick.GetComponent<RectTransform>().anchorMax = Vector2.zero;
        brick.GetComponent<RectTransform>().anchoredPosition = GetBrickPosition(new Vector2(coords.x, coords.y));
    }

    void SpawnNewFigures()
    {
        figures = new int[figureControllers.Length];
        figureRotations = new float[figureControllers.Length];
        for (int i = 0; i < figureControllers.Length; i++)
        {
            int figure = Random.Range(0, Figures1010.Figures.Length);
            float rotation = Random.Range(0, 4) * 90f;

            SpawnFigure(figureControllers[i], figure, rotation);

            figures[i] = figure;
            figureRotations[i] = rotation;
        }
    }

    void SpawnFigure(FigureController figureController, int figureIndex, float rotation)
    {
        figureController.transform.localRotation = Quaternion.identity;

        int colorIndex = Random.Range(0, 6);

        int[,] figure = Figures1010.Figures[figureIndex];
        for (int i = 0; i < figure.GetLength(0); i++)
        {
            for (int j = 0; j < figure.GetLength(1); j++)
            {
                if (figure[figure.GetLength(0) - i - 1, j] == 0)
                    continue;

                NumberedBrick brick = Instantiate(brickPrefab, figureController.transform);
                figureController.bricks.Add(brick);

                RectTransform brickRectTransform = brick.GetComponent<RectTransform>();

                brickRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                brickRectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                Rect rect = figureController.GetComponent<RectTransform>().rect;
                Vector2 brickSize = new Vector2
                {
                    x = rect.width / 4,
                    y = rect.height / 4
                };

                Vector2 coords = new Vector2(j - figure.GetLength(1) / 2f, i - figure.GetLength(0) / 2f);
                Vector2 brickPosition = Vector2.Scale(coords, brickSize);
                brickPosition += Vector2.Scale(brickSize, brickRectTransform.pivot);
                brick.GetComponent<RectTransform>().anchoredPosition = brickPosition;

                brick.ColorIndex = colorIndex;
            }
        }

        figureController.transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    void FigureOnPointerUp(FigureController figureController)
    {
        Vector2Int[] coords = new Vector2Int[figureController.bricks.Count];
        for (int i = 0; i < figureController.bricks.Count; i++)
        {
            Brick brick = figureController.bricks[i];

            Vector2 pivot = brick.GetComponent<RectTransform>().pivot;
            coords[i] = BrickPositionToCoords(brick.transform.position, pivot);

            if (coords[i].x < 0 || coords[i].y < 0 || coords[i].x >= bricksCount.x || coords[i].y >= bricksCount.y ||
                field[coords[i].x, coords[i].y] != null)
                return;
        }

        for (int i = 0; i < figureController.bricks.Count; i++)
        {
            Brick brick = figureController.bricks[i];

            RectTransform rectTransform = brick.GetComponent<RectTransform>();

            rectTransform.SetParent(fieldTransform, false);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchoredPosition = GetBrickPosition(coords[i]);

            field[coords[i].x, coords[i].y] = brick as NumberedBrick;

            gameState.Score++;
        }

        if (Random.Range(0f, 1f) < coinProbability)
        {
            UserProgress.Current.Coins++;

            GameObject vfx = Resources.Load<GameObject>("CoinVFX");
            vfx = Instantiate(vfx, fieldTransform.parent);

            vfx.transform.position = figureController.transform.position;

            Destroy(vfx, 1.5f);
        }

        landingSfx.Play();

        int index = Array.IndexOf(figureControllers, figureController);
        figures[index] = -1;

        figureController.bricks.Clear();
        figureController.ResetPosition();

        CheckLines();

        if (figureControllers.All(c => c.bricks.Count == 0))
            SpawnNewFigures();

        SaveGame();

        CheckGameOver();
    }

    Vector2Int BrickPositionToCoords(Vector3 position, Vector2 pivot)
    {
        Vector3[] worldCorners = new Vector3[4];
        fieldTransform.GetWorldCorners(worldCorners);

        Vector2 brickSize = new Vector2
        {
            x = (worldCorners[2].x - worldCorners[0].x) / bricksCount.x,
            y = (worldCorners[2].y - worldCorners[0].y) / bricksCount.y
        };

        Vector2 localPoint = position - worldCorners[0] - Vector3.Scale(brickSize, pivot);
        Vector2 coords = localPoint / brickSize;

        return Vector2Int.RoundToInt(coords);
    }

    void CheckLines()
    {
        List<Vector2Int> bricksToDestroy = new List<Vector2Int>();

        for (int x = 0; x < bricksCount.x; x++)
        {
            bool line = true;

            for (int y = 0; y < bricksCount.y; y++)
            {
                if (field[x, y] != null)
                    continue;

                line = false;
                break;
            }

            if (!line)
                continue;

            for (int y = 0; y < bricksCount.y; y++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                if (bricksToDestroy.Contains(coords))
                    continue;

                bricksToDestroy.Add(coords);
            }
        }

        for (int y = 0; y < bricksCount.y; y++)
        {
            bool line = true;

            for (int x = 0; x < bricksCount.x; x++)
            {
                if (field[x, y] != null)
                    continue;

                line = false;
                break;
            }

            if (!line)
                continue;

            for (int x = 0; x < bricksCount.x; x++)
            {
                Vector2Int coords = new Vector2Int(x, y);
                if (bricksToDestroy.Contains(coords))
                    continue;

                bricksToDestroy.Add(coords);
            }
        }

        if (bricksToDestroy.Count > 0)
            mergingSfx.Play();

        foreach (Vector2Int c in bricksToDestroy)
        {
            NumberedBrick brick = field[c.x, c.y];
            brick.DoMergingAnimation(() => Destroy(brick.gameObject));

            field[c.x, c.y] = null;

            gameState.Score++;
        }
    }

    void CheckGameOver()
    {
        foreach (FigureController figureController in figureControllers)
        {
            if (figureController.bricks.Count == 0)
                continue;

            for (int x = 0; x < bricksCount.x; x++)
            {
                for (int y = 0; y < bricksCount.y; y++)
                {
                    if (IsCanPlaceFigure(x, y, figureController))
                        return;
                }
            }
        }

        gameState.SetField(new int[0]);
        UserProgress.Current.SaveGameState(name);

        OnGameOver();
    }

    bool IsCanPlaceFigure(int x, int y, FigureController figureController)
    {
        Quaternion rotation = figureController.transform.localRotation;

        Vector2 minPosition = new Vector2(float.MaxValue, float.MaxValue);
        foreach (Brick brick in figureController.bricks)
        {
            Vector2 localPosition = rotation * brick.GetComponent<RectTransform>().anchoredPosition;

            minPosition.x = Mathf.Min(minPosition.x, localPosition.x);
            minPosition.y = Mathf.Min(minPosition.y, localPosition.y);
        }

        foreach (Brick brick in figureController.bricks)
        {
            RectTransform rectTransform = brick.GetComponent<RectTransform>();

            Vector2 position = rotation * rectTransform.anchoredPosition;
            position -= minPosition;

            Vector2Int coords = Vector2Int.RoundToInt(position / rectTransform.rect.size);
            coords.x += x;
            coords.y += y;

            if (coords.x < 0 || coords.y < 0 || coords.x >= bricksCount.x || coords.y >= bricksCount.y ||
                field[coords.x, coords.y] != null)
                return false;
        }

        return true;
    }
}