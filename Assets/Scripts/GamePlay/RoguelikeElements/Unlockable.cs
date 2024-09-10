using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum UNLOCKTYPE { CARDENHANCEMENT, TRINKET, TRINKETSLOT }


/* 
    Goal: (6)

    CARD ENHANCEMENT DESCRIPTIONS:
        Damage - Causes this card to also inflict damage / Add to Score 
        Heal - Causes this card to heal the player when won by the player. [ Can not go on Hearts ]
        Gold - Causes this card to give the player gold when won by the player.
        Xray - Allows the player to always see this card, even if in the hand of CPU
*/

public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }

/*
    Goal: (20)
    
    Trinket Descriptions:
        Full Moon - When player shoots the moon they double their total score.
        Triage - Heals the player for every won hand.
        Eclipse - Allows the player to not have to get the Queen of Spades to shoot the moon.
        Calculator - Player gains score at the end of the round equal to their trinkets sell value.
        Jacks Wild (SUIT) - Jacks of the Show suit now cause the player to heal. [ Can include Hearts ]
        Aces Low - Aces are now considered low cards.
        Pennies - All 2s won count as an extra money.
        Stocks - You gain interest on your gold between rounds ( 10 % ) 
        Coupon - Each shop will have one random free item.
        

*/
public enum TRINKET { }


public sealed class Unlockable
{
    public UNLOCKTYPE type;
    public int id;
}
