using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
