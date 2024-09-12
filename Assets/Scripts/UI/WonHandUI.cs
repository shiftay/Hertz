using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WonHandUI : MonoBehaviour
{
    public Image handWinner;
    public List<CardHolderUI> cardHolders;

    public void SetupWonHand(List<Card> wonHand) {
        handWinner.sprite = Dealer.instance.spriteHandler.Icon(wonHand[0].CURRENTOWNER.isPlayer);

        for(int i = 0; i < wonHand.Count; i++) {
            cardHolders[i].playingCard.sprite = Dealer.instance.spriteHandler.WonHandCard(wonHand[i].cardInfo.cardSuit, wonHand[i].cardInfo.cardValue);
            cardHolders[i].winningCardGlow.enabled = wonHand[i].winningCard;
        }
    }
}
