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

    public Shop.Rarity rarity;

    public bool Compare(Card card) {
        return (card.cardInfo.cardSuit == cardInfo.cardSuit && card.cardInfo.cardValue == cardInfo.cardValue);
    }

    public Player CURRENTOWNER {
        get { return _currentOwner; }
        set { _currentOwner = value; }
    }

    public Card(CardInfo cI) { cardInfo = cI; Reset(); enhancements = new List<Enhancements>(); }
    public Card(CardInfo cI, List<Enhancements> enhancements, Shop.Rarity rarity) { cardInfo = cI; Reset(); 
                                                                                    this.enhancements = new List<Enhancements>(enhancements); 
                                                                                    this.rarity = rarity; }
    public Card() { Reset(); enhancements = new List<Enhancements>(); }
#region Utils
    public void Reset() { winningCard = false; handPlayed = cardPlayed = -1; }
    public bool isHeart() { return cardInfo.cardSuit == Utils.CARDSUIT.HEART; }
    public bool IsQueenOfSpades() { return cardInfo.cardSuit == Utils.CARDSUIT.SPADE && cardInfo.cardValue == 12; }
    public bool ContainsHealing => enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.HEAL).Count > 0;
    public bool ContainsXRAY => enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.XRAY).Count > 0;
    public bool ContainsDamage => enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.DAMAGE).Count > 0;
    public bool ContainsGold => enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.GOLD).Count > 0;
#endregion
}
