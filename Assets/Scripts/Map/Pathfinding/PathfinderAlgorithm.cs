using Assets.Scripts.Map.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class PathfinderAlgorithm<T> : IPathfinder
    where T: PathfindingNode
{
    protected FieldStatus[,] map;

    protected MinHeap<T> open;
    protected HashSet<T> closed;

    public bool MapExists { get { return map != null && map.GetLength(0) > 0 && map.GetLength(1) > 0; } }

    public abstract Vector2Int[] CalculatePath(Vector2Int start, Vector2Int finish);

    public Vector2Int[] GetTraversibleFields()
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == FieldStatus.Traversable)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result.ToArray();
    }

    public void LoadMap(FieldStatus[,] _map)
    {
        map = _map;
        /*
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int y = 0; y < map.GetLength(1); y++)
        {
            sb.AppendLine();

            for (int x = 0; x < map.GetLength(0); x++)
            {
                sb.Append((map[x, y] == FieldStatus.Traversable ? '_' : 'x'));
            }
        }

        Debug.Log(sb.ToString());
        */
    }
}
