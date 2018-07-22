using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Theme : ScriptableObject
{
    public Sprite Corner;
    public Sprite TopDownSide;
    public Sprite RightLeftSide;
    public Sprite Default;
    public Sprite X;
    public Sprite O;
    public Texture2D CursorX;
    public Texture2D CursorO;
    public Texture2D CursorDefault;
}
