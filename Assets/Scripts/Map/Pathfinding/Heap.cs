using Assets.Scripts.Map.Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap<T> where T: PathfindingNode
{    
    private T[] heapArray;

    private int length;

    public int Count { get { return length; } private set { length = value; } }
    public bool IsEmpty { get { return Count == 0; } }
    public T PeekMin { get { return heapArray[1]; } }    

    public MinHeap(System.Func<Vector2Int, Vector2Int, T> nodeConstructor)
    {
        heapArray = new T[2];        
        heapArray[1] = nodeConstructor(new Vector2Int(-1, -1), new Vector2Int(-1, -1));
    }

    public T GetMin()
    {
        T result = PeekMin;

        RemoveFromHeap(1);      

        if(Count < heapArray.Length / 2 && heapArray.Length > 2)
        {
            System.Array.Resize<T>(ref heapArray, heapArray.Length / 2);
        }
                
        WriteHeap("==GET MIN==");

        return result;
    }

    public void Put(T node)
    {
        int existingCopyIndex;

        if(ExistsInHeap(node.Point.x, node.Point.y, out existingCopyIndex))
        {
            if (CompareNodes(node, heapArray[existingCopyIndex]) == 1)
            {
                RemoveFromHeap(existingCopyIndex);
            }
            else
            {
                return;
            }
        }

        T temp = node;

        Count++;

        if(heapArray.Length <= Count)
        {
            System.Array.Resize<T>(ref heapArray, heapArray.Length * 2);
        }        

        heapArray[Count] = temp;
        UpHeap(Count);

        WriteHeap("==PUT==");

    }

    private bool ExistsInHeap(int x, int y)
    {
        return IndexInHeap(x, y) > 0;
    }

    private bool ExistsInHeap(int x, int y, out int index)
    {
        index = IndexInHeap(x, y);

        return index > 0;
    }

    private int IndexInHeap(int x, int y)
    {
        return
            System.Array.FindIndex<T>(
                heapArray,
                elem => elem != null && elem.Point.x == x && elem.Point.y == y);
    }

    private void RemoveFromHeap(int index)
    {
        Swap(index, Count);
        Count--;
        DownHeap(index);
    }

    private void UpHeap(int index)
    {
        int previous = index / 2;

        if(index > 1 && CompareNodes(heapArray[index], heapArray[previous]) == 1)
        {
            Swap(index, previous);
            UpHeap(previous);
        }
    }

    private void DownHeap(int index)
    {
        int nextLeft = 2 * index;

        if (nextLeft <= Count)
        {
            int nextRight = (nextLeft < Count) ? nextLeft + 1 : nextLeft;

            int swapIndex =
                //(heapArray[nextLeft].F > heapArray[nextRight].F) ?
                (CompareNodes(heapArray[nextLeft], heapArray[nextRight]) == -1)?
                    nextRight : nextLeft;

            Swap(index, swapIndex);
            DownHeap(swapIndex);
        }
    }

    /// <summary>
    /// 1 == nodeA smaller
    /// -1 == nodeB smaller
    /// 
    /// If F of A and B are equal, take one that is closer to the target
    /// </summary>
    /// <param name="nodeA"></param>
    /// <param name="nodeB"></param>
    /// <returns></returns>
    private int CompareNodes(AStarNode nodeA, AStarNode nodeB)
    {
        int needRandom = -999;
        int result =
            nodeA.F > nodeB.F ? -1 :
                (nodeA.F < nodeB.F ? 1 :
                    ( nodeA.H < nodeB.H? 1 :
                        (nodeA.H > nodeB.H ? -1 : needRandom)                    
                    )
                );

        if(result == needRandom)
        {
            result =
                Random.Range(0, 1) == 0 ? -1 : 1;
        }

        return result;     
    }

    private int CompareNodes(T nodeA, T nodeB)
    {
        int needRandom = -999;
        int result =
            nodeA.F > nodeB.F ? -1 :
                (nodeA.F < nodeB.F ? 1 : needRandom);

        if (result == needRandom)
        {
            result =
                Random.Range(0, 1) == 0 ? -1 : 1;
        }

        return result;
    }

    private void Swap(int indexA, int indexB)
    {
        T A = heapArray[indexA];
        T B = heapArray[indexB];

        heapArray[indexA] = B;
        heapArray[indexB] = A;
    }

    private void WriteHeap(string message)
    {
        /*
        Debug.Log(message);

        for (int i = 0; i <= Count; i++)
        {
            if (heapArray[i] == null) continue;

            Debug.Log(i + ": " + heapArray[i].ToString());
        }
        */
    }
}