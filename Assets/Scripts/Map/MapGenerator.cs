using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    public const int MinSize = 10;
    public const int MinObstacles = 0;
    public const float MaxObstaclesPercent = 0.3f;

    private int configMapSize;
    private int configObstacleCount;

    public MapGenerator(int size = MinSize, int obstacles = MinObstacles)
    {
        if(!SetNParameter(size))
        {
            SetNParameter(MinSize);
        }

        if(!SetMParameter(obstacles))
        {
            SetMParameter(MinObstacles);
        }
    }

    public bool SetNParameter(int n)
    {
        bool result = n >= MinSize;

        if (result)
        {
            configMapSize = n;
            SetMParameter(configObstacleCount);
        }

        return result;
    }

    public bool SetMParameter(int m)
    {
        bool result = m >= MinObstacles;

        if (result)
        {
            int max = Mathf.RoundToInt(configMapSize * configMapSize * MaxObstaclesPercent);
            configObstacleCount = m <= max ? m : max;
        }

        return result;
    }

    public char[,] GenerateMap()
    {
        char[,] result = new char[configMapSize, configMapSize];
        result.FillWithValue(TranslationRules.FreeField);

        int obstaclesLeft = configObstacleCount;

        Vector2Int tempCoord = Vector2Int.zero;
        List<ObstacleType> possibleFits = new List<ObstacleType>();
        char[,] probe = new char[2, 2];

        while(obstaclesLeft > 0)
        {
            possibleFits.Clear();

            tempCoord.x = Random.Range(0, configMapSize);
            tempCoord.y = Random.Range(0, configMapSize);

            ZeroProbe(ref probe);
            for(int i = 0; i < 4; i++)
            {
                int dx = i % 2;
                int dy = i / 2;

                if (WithinBorder(tempCoord.x + dx, tempCoord.y + dy))
                {
                    probe[dx, dy] = result[tempCoord.x + dx, tempCoord.y + dy];
                }                        
            }

            //Debug.Log(probe[0, 0] + " " + probe[1, 0] + "\n" + probe[0, 1] + " " + probe[1, 1]);

            DeterminePossibleFits(probe, ref possibleFits);

            if (possibleFits.Count > 0)
            {
                obstaclesLeft--;
                ObstacleType obstacleChosen = possibleFits[Random.Range(0, possibleFits.Count)];

                InsertObstacle(tempCoord.x, tempCoord.y, ref result, obstacleChosen);
            }

        }

        return result;
    }   

    private void ZeroProbe(ref char[,] probe)
    {
        probe.FillWithValue('0');
    }

    private bool WithinBorder(int x, int y)
    {
        return x >= 0 && x < configMapSize && y >= 0 && y < configMapSize;
    }

    private void DeterminePossibleFits(char[,] probe, ref List<ObstacleType> possibleFits)
    {
        if (FitOneByOne(probe))
        {
            possibleFits.Add(ObstacleType.OneByOne);

            bool secondStep = false;

            if (FitOneByTwo(probe))
            {
                secondStep = true;
                possibleFits.Add(ObstacleType.OneByTwo);
            }

            if (FitTwoByOne(probe))
            {
                secondStep = true;
                possibleFits.Add(ObstacleType.TwoByOne);
            }

            if (secondStep && FitTwoByTwo(probe))
            {
                possibleFits.Add(ObstacleType.TwoByTwo);
            }
        }
    }

    #region Fit Tests
    private bool FitOneByOne(char[,] probe)
    {
        return
            probe[0, 0] == TranslationRules.FreeField;
    }

    private bool FitTwoByOne(char[,] probe)
    {
        return
            probe[0, 0] == TranslationRules.FreeField &&
            probe[1, 0] == TranslationRules.FreeField;
    }

    private bool FitOneByTwo(char[,] probe)
    {
        return
            probe[0, 0] == TranslationRules.FreeField &&
            probe[0, 1] == TranslationRules.FreeField;
    }

    private bool FitTwoByTwo(char[,] probe)
    {
        return
            probe[0, 0] == TranslationRules.FreeField &&
            probe[1, 0] == TranslationRules.FreeField &&
            probe[0, 1] == TranslationRules.FreeField &&
            probe[1, 1] == TranslationRules.FreeField;
    }
    #endregion

    #region Insert Obstacle Methods

    private void InsertObstacle(int x, int y, ref char[,] map, ObstacleType chosenObstacle)
    {
        switch(chosenObstacle)
        {
            case ObstacleType.OneByOne:
                InsertOneByOne(ref map, x, y);
                break;

            case ObstacleType.OneByTwo:
                InsertOneByTwo(ref map, x, y);
                break;

            case ObstacleType.TwoByOne:
                InsertTwoByOne(ref map, x, y);
                break;

            case ObstacleType.TwoByTwo:
                InsertTwoByTwo(ref map, x, y);
                break;
        }
    }

    private void InsertOneByOne(ref char[,] map, int x, int y)
    {
        map[x, y] = TranslationRules.ObstacleOneByOne;
    }

    private void InsertOneByTwo(ref char[,] map, int x, int y)
    {
        map[x, y] = TranslationRules.ObstacleOneByTwoStart;
        map[x, y + 1] = TranslationRules.ObstacleOneByTwoEnd;
    }

    private void InsertTwoByOne(ref char[,] map, int x, int y)
    {
        map[x, y] = TranslationRules.ObstacleTwoByOneStart;
        map[x + 1, y] = TranslationRules.ObstacleTwoByOneEnd;
    }

    private void InsertTwoByTwo(ref char[,] map, int x, int y)
    {
        map[x, y] = TranslationRules.ObstacleTwoByTwoTopLeft;
        map[x, y + 1] =
            map[x + 1, y] =
                map[x + 1, y + 1] = TranslationRules.ObstacleTwoByTwoRest;
    }

    #endregion
}

public static class ArrayExtension
{
    public static void FillWithValue<T>(this T[] array, T value)
    {
        for(int i = 0; i < array.Length; ++i)
        {
            array[i] = value;
        }
    }

    public static void FillWithValue<T>(this T[,] array, T value)
    {
        for (int x = 0; x < array.GetLength(0); ++x)
        {
            for (int y = 0; y < array.GetLength(1); ++y)
            {
                array[x, y] = value;
            }
        }
    }
}

public enum ObstacleType
{
    OneByOne,
    TwoByOne,
    OneByTwo,
    TwoByTwo,
    None,
    PartOfObstacle
}

public static class ObstacleTypeExtension
{
    public static ObstacleType TranslateToObstacleType(this char c)
    {
        switch(c)
        {
            case TranslationRules.ObstacleOneByOne:
                return ObstacleType.OneByOne;

            case TranslationRules.ObstacleOneByTwoStart:
                return ObstacleType.OneByTwo;

            case TranslationRules.ObstacleTwoByOneStart:
                return ObstacleType.TwoByOne;

            case TranslationRules.ObstacleTwoByTwoTopLeft:
                return ObstacleType.TwoByTwo;

            case TranslationRules.FreeField:
                return ObstacleType.None;

            case TranslationRules.ObstacleOneByTwoEnd:
                return ObstacleType.PartOfObstacle;

            case TranslationRules.ObstacleTwoByOneEnd:
                return ObstacleType.PartOfObstacle;

            case TranslationRules.ObstacleTwoByTwoRest:
                return ObstacleType.PartOfObstacle;            
        }

        Debug.Log(c);

        throw new System.ArgumentException("Map contains illegal symbols!");
    }
}

