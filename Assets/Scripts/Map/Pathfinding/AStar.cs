using Assets.Scripts.Map.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AStar : IPathfinder
{    
    protected FieldStatus[,] map;

    protected MinHeap open;
    protected HashSet<AStarNode> closed;

    public AStar()
    {
        
    }

    public AStar(char[,] _map)
    {
        this.LoadMap(_map);
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

    public Vector2Int[] GetTraversibleFields()
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for(int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if(map[x,y] == FieldStatus.Traversable)
                {
                    result.Add(new Vector2Int(x, y));
                }
            }
        }

        return result.ToArray();
    }

    public async Task<Vector2Int[]> AsyncCalculatePath(Vector2Int start, Vector2Int finish)
    {
        return await Task<Vector2Int[]>.Run(() => CalculatePath(start, finish));
    }

    public virtual Vector2Int[] CalculatePath(Vector2Int start, Vector2Int finish)
    {
        int xMax = map.GetLength(0);
        int yMax = map.GetLength(1);

        open = new MinHeap();
        closed = new HashSet<AStarNode>();

        open.Put(new AStarNode(start, finish));
        AStarNode current = null;

        while(!open.IsEmpty)
        {
            current = open.GetMin();
            closed.Add(current);

            //Debug.Log("Closed");

            if(current.H == 0)
            {
                return ProcessPath(current);
            }

            Vector2Int currentNeighbour;
            AStarNode currentNeighbourNode;

            for(int i = 0; i < 4; i++)
            {
                currentNeighbour = current.Point.GetNeighbour(i);

                if(currentNeighbour.WithinBounds(xMax, yMax) && map[currentNeighbour.x, currentNeighbour.y] == FieldStatus.Traversable)
                {
                    currentNeighbourNode =
                        new AStarNode(
                            currentNeighbour, 
                            finish, 
                            current, 
                            current.G
                            );

                    if (!closed.Contains(currentNeighbourNode))
                    {
                        open.Put(currentNeighbourNode);
                    }
                }
            }
        }

        return null;
    }

    protected Vector2Int[] ProcessPath(AStarNode finishNode)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        AStarNode current = finishNode;

        do
        {
            result.Insert(0, current.Point);
            current = current.Parent;
        }
        while (current != null);

        return result.ToArray();
    }    
}
