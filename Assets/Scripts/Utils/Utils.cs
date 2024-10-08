using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public enum CARDSUIT { HEART, SPADE, DIAMOND, CLUB, NULL }
    public enum AXIS { NORTH, EAST, SOUTH, WEST }
    public enum DIFFICULITIES { EASY, NORMAL, HARD }
    public enum SPEED { SLOW, NORMAL, FAST }
    public const int ACE = 14;
    public const int CARDVALUEMODIFIER = 2;
    public const int DEFAULTMAXDAMAGE = 23;
    public const int DEFAULTQUEENDAMAGE = 10;
    public const float HANDLINEVALUEX = 3.0f;
    public const float HANDLINEVALUEY = 0.75f;
    public const float ROTATEVALY = 10.0f;
    public const float HANDSIZE = 13.0f;
    public const float MAXZ = -14.0f;
    public const AXIS PLAYERAXIS = AXIS.SOUTH;
    public const float WONHANDUIMODIFIER = 35.0f;
    public const float HANDALIGNMENTMOD = 0.5f;


#region Coroutine Values
    public const float ROUNDENDTRANSITION = 0.3f;
#endregion

#region RogueLike Elements

    public enum UNLOCKTYPE { CARDENHANCEMENT, TRINKET, TRINKETSLOT }
    public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }

    public const int BASEREROLL = 5;
#endregion

#region UI ELEMENTS
    public enum GAMEPLAYSTATES { MainMenu, Gameplay, RoundEnd, Shop, Settings } // IMPLEMENT Figure out more gamestates that are applicable.
    public const string ROUNDCOMPLETED = "Round Over!";
    public const string CARDSWON = "Cards Won";
    public enum ICON { CPU, PLAYER }
#endregion

#region Scaling
    public const int LOW = 6;
    public const int HIGH = 11;

    // FIXME Can probably not scale with the Difficulty enum, instead use just reglar constants.
    public static int ScaleValue(Utils.DIFFICULITIES difficulty) {
        return Random.Range(LOW, ConvertRange((int)Utils.DIFFICULITIES.HARD, (int)Utils.DIFFICULITIES.EASY, LOW,  HIGH, (int)difficulty) + 1);
    }

    public static int ConvertRange(
    int originalStart, int originalEnd, // original range
    int newStart, int newEnd, // desired range
    int value) // value to convert
    {
        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (int)(newStart + ((value - originalStart) * scale));
    }
#endregion

#region Animation Keywords

    public const string DISPLAY = "Display";
    public const string HIDE = "Hide";

#endregion


}
