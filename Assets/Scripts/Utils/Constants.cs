using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CONSTS
{
    public enum CARDSUIT { HEART, SPADE, DIAMOND, CLUB, NULL }
    public enum AXIS { NORTH, EAST, SOUTH, WEST }
    public enum ICON { CPU, PLAYER }
    public const int ACE = 14;
    public const int CARDVALUEMODIFIER = 2;
    public const float HANDLINEVALUEX = 3.0f;
    public const float HANDLINEVALUEY = 0.75f;
    public const float ROTATEVALY = 10.0f;
    public const float HANDSIZE = 13.0f;
    public const float MAXZ = -14.0f;
    public const AXIS PLAYERAXIS = AXIS.SOUTH;
    public const float WONHANDUIMODIFIER = 35.0f;
    public const float HANDALIGNMENTMOD = 0.5f;


#region UI ELEMENTS
    public const string ROUNDCOMPLETED = "Round Completed";
    public const string CARDSWON = "Cards Won";
#endregion
}
