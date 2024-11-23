using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    public Vector2Int goal;
    public char direction;

    public Path(Vector2Int goal, char direction)
    {
        this.goal = goal;
        this.direction = direction;
    }
}
