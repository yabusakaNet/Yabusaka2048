using System;
using UnityEngine;

[Serializable]
public class GameState1010 : GameState
{
    [SerializeField]
    int[] figures = new int[0];
    [SerializeField]
    float[] figureRotations = new float[0];

    public void SetFigures(int[] value)
    {
        figures = (int[]) value.Clone();
    }

    public int[] GetFigures()
    {
        return (int[]) figures.Clone();
    }

    public void SetFigureRotations(float[] value)
    {
        figureRotations = (float[]) value.Clone();
    }

    public float[] GetFigureRotations()
    {
        return (float[]) figureRotations.Clone();
    }
}