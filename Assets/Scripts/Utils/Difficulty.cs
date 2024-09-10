using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty 
{
    public const int LOW = 6;
    public const int HIGH = 11;

    public enum DIFFICULITIES { EASY, NORMAL, HARD }


    public static int ScaleValue(DIFFICULITIES difficulty) {
        return Random.Range(LOW, ConvertRange((int)DIFFICULITIES.HARD, (int)DIFFICULITIES.EASY, LOW,  HIGH, (int)difficulty) + 1);
    }

    public static int ConvertRange(
    int originalStart, int originalEnd, // original range
    int newStart, int newEnd, // desired range
    int value) // value to convert
    {
        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (int)(newStart + ((value - originalStart) * scale));
    }
}
