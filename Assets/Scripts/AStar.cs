using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AStar
{
    private Block[,] blocks;

    private List<Vector2Int> open;
    private List<Vector2Int> closed;

    public AStar()
    {
        blocks = new Block[15, 15];
        open = new List<Vector2Int>();
        closed = new List<Vector2Int>();
        
        for (int i = 0; i < 15; i++)
        {
            for(int j = 0; j < 15; j++)
            {
                blocks[i, j] = new Block();
                blocks[i, j].isWall = false;
                blocks[i, j].h = 0;
                blocks[i, j].g = 0;
                blocks[i, j].f = 100;
                blocks[i, j].parent = new Vector2Int(i, j);
                blocks[i, j].position = new Vector2Int(85 - i * 5, 15 + j * 5);
            }
        }
        
        for (int i = 0; i < 15; i++)
        {
            blocks[i, 0].isWall = true;
            blocks[i, 14].isWall = true;
            blocks[0, i].isWall = true;
            blocks[14, i].isWall = true;
        }

        SetColWall(2, 2, 4);
        SetRowWall(2, 2, 6);
        SetColWall(6, 2, 4);
        SetRowWall(4, 4, 6);
        SetColWall(4, 4, 6);
        SetRowWall(6, 1, 4);

        SetColWall(8, 2, 4);
        SetRowWall(6, 6, 10);
        SetColWall(10, 2, 9);
        SetRowWall(2, 10, 13);

        SetColWall(12, 4, 6);

        SetRowWall(10, 1, 3);
        SetColWall(3, 8, 10);
        SetRowWall(8, 2, 8);

        SetRowWall(12, 2, 6);
        SetColWall(5, 10, 12);

        blocks[10, 7].isWall = true;
        SetColWall(8, 10, 13);

        SetColWall(10, 11, 13);

        SetColWall(12, 8, 12);
        blocks[11, 13].isWall = true;
    }

    private void SetRowWall(int row, int s, int g)
    {
        for(int i = s; i <= g; i++)
        {
            blocks[row, i].isWall = true;
        }
    }

    private void SetColWall(int col, int s, int g)
    {
        for (int i = s; i <= g; i++)
        {
            blocks[i, col].isWall = true;
        }
    }

    private void InitializeValues()
    {
        open = new List<Vector2Int>();
        closed = new List<Vector2Int>();

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                blocks[i, j].h = 0;
                blocks[i, j].g = 0;
                blocks[i, j].f = 100;
                blocks[i, j].parent = new Vector2Int(i, j);
            }
        }
    }

    public List<Path> FindPath(Vector2Int start, Vector2Int goal, char startDirection)
    {
        InitializeValues();
        start = PositionToCoordinate(start);
        goal = PositionToCoordinate(goal);

        blocks[start.x, start.y].f = 0;
        open.Add(start);

        while(open.Count > 0)
        {
            Vector2Int minNode = new Vector2Int();
            foreach(Vector2Int node in open)
            {
                int minValue = 1000;
                if (blocks[node.x, node.y].f < minValue)
                {
                    minValue = blocks[node.x, node.y].f;
                    minNode = node;
                }
            }
            
            if(minNode == goal)
            {
                return ReviewPath(start, goal, startDirection);
            }

            open.Remove(minNode);
            closed.Add(minNode);
            
            foreach (Vector2Int neighbor in FindNeighbors(minNode))
            {
                blocks[neighbor.x, neighbor.y].g = blocks[minNode.x, minNode.y].g + 1;
                blocks[neighbor.x, neighbor.y].h = Math.Abs(goal.x - neighbor.x) + Math.Abs(goal.y - neighbor.y);
                int fNew = blocks[neighbor.x, neighbor.y].g + blocks[neighbor.x, neighbor.y].h;

                if(open.Find(x => x == neighbor) != null || closed.Find(x => x == neighbor) != null)
                {
                    if(fNew > blocks[neighbor.x, neighbor.y].f)
                    {
                        continue;
                    }
                }

                blocks[neighbor.x, neighbor.y].f = fNew;

                open.Remove(neighbor);
                closed.Remove(neighbor);

                blocks[neighbor.x, neighbor.y].parent = minNode;
                open.Add(neighbor);
            }
        }

        return null;
    }

    public List<Vector2Int> FindNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        List<Vector2Int> validNeighbors = new List<Vector2Int>();

        if (node.x > 0)
        {
            neighbors.Add(new Vector2Int(node.x - 1, node.y));
        }

        if (node.x < 14)
        {
            neighbors.Add(new Vector2Int(node.x + 1, node.y));
        }

        if (node.y > 0)
        {
            neighbors.Add(new Vector2Int(node.x, node.y - 1));
        }

        if (node.y < 14)
        {
            neighbors.Add(new Vector2Int(node.x, node.y + 1));
        }

        foreach(Vector2Int neighbor in neighbors)
        {
            if (!blocks[neighbor.x, neighbor.y].isWall)
            {
                validNeighbors.Add(neighbor);
            }
        }

        return validNeighbors;
    }

    private List<Path> ReviewPath(Vector2Int start, Vector2Int goal, char startDirection)
    {
        List<Path> paths = new List<Path>();
        Vector2Int node = goal;
        int i = 0;
        while (blocks[node.x, node.y].parent != node)
        {
            Vector2Int parent = blocks[node.x, node.y].parent;
            char direction;

            if (parent.x == node.x && parent.y > node.y)
            {
                direction = 'W';
            }
            else if(parent.x == node.x && parent.y < node.y)
            {
                direction = 'E';
            }
            else if (parent.x > node.x && parent.y == node.y)
            {
                direction = 'N';
            }
            else if (parent.x < node.x && parent.y == node.y)
            {
                direction = 'S';
            }
            else
            {
                Debug.LogError("Error in finding new path");
                return null;
            }
            
            paths.Add(new Path(CoordinateToPosition(node), direction));
            node = parent;

            i++;
            if (i > 100)
            {
                Debug.LogError("Error in finding new path");
                return null;
            }
        }

        paths.Add(new Path(CoordinateToPosition(node), startDirection));
        paths.Reverse();

        return paths;
    }

    private Vector2Int PositionToCoordinate(Vector2Int position)
    {
        int x = (85 - position.y) / 5;
        int y = (position.x - 15) / 5;

        return new Vector2Int(x, y);
    }

    private Vector2Int CoordinateToPosition(Vector2Int coordinate)
    {
        int x = coordinate.y * 5 + 15;
        int y = 85 - coordinate.x * 5;

        return new Vector2Int(x, y);
    }
}

public class Block
{
    public bool isWall;
    public int h, g, f;
    public Vector2Int parent;
    public Vector2Int position;
}