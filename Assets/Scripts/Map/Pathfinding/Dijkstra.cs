using Assets.Scripts.Map.Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra : AStar
{    
    private new HashSet<DijkstraNode> closed;

    public Dijkstra()
    {

    }

    public Dijkstra(char[,] _map)
    {
        this.LoadMap(_map);
    }

    public override Vector2Int[] CalculatePath(Vector2Int start, Vector2Int finish)
    {
        //Debug.Log("Dijkstra");

        int xMax = map.GetLength(0);
        int yMax = map.GetLength(1);

        open = new MinHeap();
        closed = new HashSet<DijkstraNode>();

        open.Put(new DijkstraNode(start));
        DijkstraNode current = null;

        while(!open.IsEmpty)
        {
            current = open.GetMin() as DijkstraNode;
            closed.Add(current);

            //Debug.Log("Closed");

            if(current.Point.Equals(finish))
            {
                return ProcessPath(current);
            }

            Vector2Int currentNeighbour;
            DijkstraNode currentNeighbourNode;

            for (int i = 0; i < 4; i++)
            {
                currentNeighbour = current.Point.GetNeighbour(i);

                if (currentNeighbour.WithinBounds(xMax, yMax) && map[currentNeighbour.x, currentNeighbour.y] == FieldStatus.Traversable)
                {
                    currentNeighbourNode =
                        new DijkstraNode(
                            currentNeighbour,
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

        //return ProcessPath( closed. [new DijkstraNode(finish)] );

        return null;
    }
}
