using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//The reason for this class NOT being a component is that we want to mitigate the tampering with the
//parsing process, by simply crashing the process if the file contains illegal symbols
public class TranslationRules
{
    #region Map Format Legend
    public const char FreeField = '-';
    public const char ObstacleOneByOne = 'A';

    public const char ObstacleTwoByOneStart = 'M';
    public const char ObstacleTwoByOneEnd = 'm';

    public const char ObstacleOneByTwoStart = 'L';
    public const char ObstacleOneByTwoEnd = 'l';

    public const char ObstacleTwoByTwoTopLeft = 'C';
    public const char ObstacleTwoByTwoRest = 'c';
    #endregion

    public static TranslationResult[] TranslateCoordinate(char symbol, int x, int y)
    {
        TranslationResult[] result = new TranslationResult[/*4*/ 1];
        FieldStatus symbolResult = TranslateSymbol(symbol);                

        for(int i = 0; i < result.Length; i++)
        {
            result[i] = new TranslationResult(x + (i % 2), y + i / 2, symbolResult);
        }

        return result;
    }

    public static TranslationResult SolveConflicts(TranslationResult A, TranslationResult B)
    {
        if (!A.IsSamePosition(B)) throw new InvalidOperationException("Results for different fields: No conflict possible!");

        return
            (!A.IsTraversable) ? A : B;
    }
                
    private static FieldStatus TranslateSymbol(char symbol)
    {
        switch(symbol)
        {
            case FreeField:
                return FieldStatus.Traversable;

            case ObstacleOneByOne:
                return FieldStatus.NonTraversable;

            case ObstacleOneByTwoEnd:
                return FieldStatus.NonTraversable;

            case ObstacleOneByTwoStart:
                return FieldStatus.NonTraversable;

            case ObstacleTwoByOneEnd:
                return FieldStatus.NonTraversable;

            case ObstacleTwoByOneStart:
                return FieldStatus.NonTraversable;

            case ObstacleTwoByTwoRest:
                return FieldStatus.NonTraversable;

            case ObstacleTwoByTwoTopLeft:
                return FieldStatus.NonTraversable;
        }

        throw new ArgumentException("Illegal symbol found during parsing!");
    }

}

/// <summary>
/// Represents traversability of one pathfinder map field
/// </summary>
public class TranslationResult : Tuple<int, int, FieldStatus>
{
    public int X { get { return Item1; } }
    public int Y { get { return Item2; } }
    public bool IsTraversable { get { return Item3 == FieldStatus.Traversable; } }
    public FieldStatus Status { get { return Item3; } }

    public TranslationResult(int item1, int item2, FieldStatus item3) : base(item1, item2, item3)
    {
    }

    public bool IsSamePosition(TranslationResult b)
    {
        return
            this.X == b.X &&
            this.Y == b.Y;

    }
}

public enum FieldStatus
{
    Traversable,
    NonTraversable
}
