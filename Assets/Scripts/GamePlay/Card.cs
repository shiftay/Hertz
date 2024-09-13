using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

    public bool ContainsXRAY() { return enhancements.FindAll(n => n.type == CARDENHANCEMENT.XRAY).Count > 0; }

    public Player CURRENTOWNER {
        get { return _currentOwner; }
        set { _currentOwner = value; }
    }

    public Card(CardInfo cI) { cardInfo = cI; Reset(); enhancements = new List<Enhancements>(); }
    public Card(CardInfo cI, List<Enhancements> enhancements) { cardInfo = cI; Reset(); this.enhancements = new List<Enhancements>(enhancements); }
    public void Reset() { winningCard = false; handPlayed = cardPlayed = -1; }
    public bool isHeart() { return cardInfo.cardSuit == UTILS.CARDSUIT.HEART; }
    public bool IsQueenOfSpades() { return cardInfo.cardSuit == UTILS.CARDSUIT.SPADE && cardInfo.cardValue == 12; }
}
