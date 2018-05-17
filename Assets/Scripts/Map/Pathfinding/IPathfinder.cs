using Assets.Scripts.Map.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    void LoadMap(FieldStatus[,] _map);
    Vector2Int[] CalculatePath(Vector2Int start, Vector2Int finish);
    Vector2Int[] GetTraversibleFields();
}

public static class PathfinderExtension
{
    public static bool IsTraversable(FieldStatus field) { return field == FieldStatus.Traversable; }

    public static FieldStatus[,] GetMapByFieldStatus(char[,] entryMap)
    {
        int xMax = (entryMap != null)? entryMap.GetLength(0) : 0; // + 1;
        int yMax = (entryMap != null) ? entryMap.GetLength(1) : 0;

        FieldStatus[,] map = new FieldStatus[xMax, yMax];
        TranslationResult[,] translationMap = new TranslationResult[xMax, yMax];
        TranslationResult[] container;

        for (int y = 0; y < yMax /*- 1*/; y++)            
        {
            for (int x = 0; x < xMax /*- 1*/; x++)
            {                
                container = TranslationRules.TranslateCoordinate(entryMap[x, y], x, y);

                foreach (TranslationResult trans in container)
                {
                    if (trans == null) { continue; }

                    if (translationMap[trans.X, trans.Y] != null)
                    {                        
                        translationMap[trans.X, trans.Y] =
                            TranslationRules.SolveConflicts(trans, translationMap[trans.X, trans.Y]);
                    }
                    else
                    {
                        translationMap[trans.X, trans.Y] = trans;
                    }

                    map[trans.X, trans.Y] = translationMap[trans.X, trans.Y].Status;
                }
            }
        }

        return map;
    }

    public static void LoadMap(this IPathfinder pathfinder, char[,] entryMap)
    {        
        pathfinder.LoadMap(GetMapByFieldStatus(entryMap));
    }
    
    public static Vector2Int GetFreeField(this IPathfinder traversibleMapSource, Vector2Int avoid, out bool valid)
    {
        valid = false;

        Vector2Int result = Vector2Int.left;
        Vector2Int[] traversibleFields = traversibleMapSource.GetTraversibleFields();

        if(traversibleFields.Length > 1 || (traversibleFields.Length == 1 && !traversibleFields[0].Equals(avoid)) )
        {
            int index = -1;

            do
            {
                index = Random.Range(0, traversibleFields.Length);
            }
            while (traversibleFields[index].Equals(avoid));

            result = traversibleFields[index];
            valid = true;
        }

        return result;
    }

    public static Vector2Int[] ProcessPath(this PathfindingNode finishNode)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        PathfindingNode current = finishNode;

        do
        {
            result.Insert(0, current.Point);
            current = current.Parent;
        }
        while (current != null);

        return result.ToArray();
    }
}

public static class PathfinderScoreRules
{
    private static int StraightDistanceModifier = 10;

    private static int[] dx = { 0, 1, 0, -1 };
    private static int[] dy = { 1, 0, -1, 0 };

    public static int DistanceEstimate(this Vector2Int from, Vector2Int to)
    {
        return
            (Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y)) * StraightDistanceModifier;
    }

    public static bool CanMoveTo(this Vector2Int from, Vector2Int to, int xMax, int yMax)
    {
        return
            from.WithinBounds(xMax, yMax) && 
            to.WithinBounds(xMax, yMax) && 
            from.DistanceEstimate(to) == 1 * StraightDistanceModifier;
    }

    public static bool WithinBounds(this Vector2Int point, int xMax, int yMax)
    {
        return
            point.x >= 0 && point.y >= 0 &&
            point.x < xMax && point.y < yMax;
    }

    public static Vector2Int GetNeighbour(this Vector2Int point, int index)
    {
        return new Vector2Int(point.x + dx[index], point.y + dy[index]);
    }
}
