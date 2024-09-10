using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

[System.Serializable]
public class Card 
{

    public CardInfo cardInfo;
    public Sprite cardSprite; // Is this needed?
    private Player _currentOwner;
    public int cardPlayed, handPlayed;
    public bool winningCard;
    
    public Player CURRENTOWNER {
        get { return _currentOwner; }
        set { _currentOwner = value; }
    }

    public Card(CardInfo cI) { cardInfo = cI; Reset(); }

    public void Reset() { winningCard = false; handPlayed = cardPlayed = -1; }

}
