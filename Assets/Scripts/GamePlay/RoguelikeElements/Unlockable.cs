using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;




/* 
    IDEA
    Goal: (6)

    CARD ENHANCEMENT DESCRIPTIONS:
        Damage - Causes this card to also inflict damage / Add to Score 
        Heal - Causes this card to heal the player when won by the player. [ Can not go on Hearts ]
        Gold - Causes this card to give the player gold when won by the player.
        Xray - Allows the player to always see this card, even if in the hand of CPU
*/

// public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }



[System.Serializable]
public sealed class Unlockable
{
    public Utils.UNLOCKTYPE type;
    public int id;
}
