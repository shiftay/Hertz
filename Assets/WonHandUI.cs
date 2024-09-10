using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WonHandUI : MonoBehaviour
{
    public Image handWinner;
    public List<CardHolderUI> cardHolders;

    public void SetupWonHand(WonHand wonHand) {
        handWinner.sprite = Dealer.instance.spriteHandler.Icon(wonHand.isPlayer);

        for(int i = 0; i < wonHand.cards.Count; i++) {
            cardHolders[i].playingCard.sprite = Dealer.instance.spriteHandler.WonHandCard(wonHand.cards[i].cardInfo.cardSuit, wonHand.cards[i].cardInfo.cardValue);
            cardHolders[i].winningCardGlow.enabled = wonHand.cards[i].winningCard;
        }
    }
}
