using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Deck
{
    public List<Card> cards = new List<Card>();

    public Deck() {
        for(int i = 0; i <= (int)Utils.CARDSUIT.CLUB; i++) {
            for(int j = 2; j <= Utils.ACE; j++) {
                cards.Add(new Card(new CardInfo(j, (Utils.CARDSUIT)i)));
            }
        }
    }

    public void Sort() {
        cards = cards.OrderBy(n => n.cardInfo.cardSuit).ThenBy(n => n.cardInfo.cardValue).ToList();
    }

    public void AddCard(Card card) {
        int index = cards.FindIndex(n => n.Compare(card));
        Debug.Log("index " + index + " | " + cards[index].cardInfo.DebugInfo());
        cards.RemoveAt(index);
        cards.Add(card);
    }
}
