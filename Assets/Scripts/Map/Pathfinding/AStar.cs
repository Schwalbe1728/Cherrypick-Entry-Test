using Assets.Scripts.Map.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AStar : PathfinderAlgorithm<AStarNode>
{        
    public AStar()
    {
        
    }

    public AStar(char[,] _map)
    {
        this.LoadMap(_map);
    }

    public override Vector2Int[] CalculatePath(Vector2Int start, Vector2Int finish)
    {
        int xMax = map.GetLength(0);
        int yMax = map.GetLength(1);

        open = new MinHeap<AStarNode>((s, f) => new AStarNode(s, f) );
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
                return current.ProcessPath();
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
}
