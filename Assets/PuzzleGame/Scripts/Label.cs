using System;

[Serializable]
public class Label
{
    public LabelType labelType;
    public SpritesCollection linearCollection;
    public SpritesCollection parallelCollection;
}

public enum LabelType
{
    Text,
    Sprite
}