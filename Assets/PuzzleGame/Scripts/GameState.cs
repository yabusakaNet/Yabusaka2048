using System;
using UnityEngine;

[Serializable]
public class GameState
{
    public event Action StateUpdate;

    [SerializeField]
    int score;
    [SerializeField]
    int topScore;

    [SerializeField]
    int[] field = new int[0];

    public int Score
    {
        get => score;
        set
        {
            score = value;

            if (score > topScore)
                topScore = score;

            StateUpdate?.Invoke();
        }
    }

    public int TopScore => topScore;

    public int[] GetField()
    {
        return (int[]) field.Clone();
    }

    public void SetField(int[] value)
    {
        field = (int[]) value.Clone();
    }
}
