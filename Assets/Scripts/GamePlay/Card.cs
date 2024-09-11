using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

[System.Serializable]
public class Card 
{
    public CardInfo cardInfo;
    private Player _currentOwner;
    public int cardPlayed, handPlayed;
    public bool winningCard;
    public List<Enhancements> enhancements;




    public Player CURRENTOWNER {
        get { return _currentOwner; }
        set { _currentOwner = value; }
    }

    public Card(CardInfo cI) { cardInfo = cI; Reset(); }

    public Card(CardInfo cI, List<Enhancements> enhancements) { cardInfo = cI; Reset(); this.enhancements = new List<Enhancements>(enhancements); }

    public void Reset() { winningCard = false; handPlayed = cardPlayed = -1; }
}
