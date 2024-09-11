using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/* 
    CARD Value
    Ace - 14
    King - 13
    Queen - 12
    Jack - 11
*/
[System.Serializable]
public class CardInfo
{
    public delegate void CardBehaviour();
    public CardBehaviour cardBehaviour;
    public int cardValue;
    public CONSTS.CARDSUIT cardSuit;

    public string DebugInfo() {
        return cardSuit.ToString() + ", " + cardValue;
    }
    
    public CardInfo(int cV, CONSTS.CARDSUIT cs, CardBehaviour behaviour = null) {
        cardValue = cV; cardSuit = cs; cardBehaviour = behaviour;
    }
}
