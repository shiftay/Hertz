using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WonHandUI : MonoBehaviour
{
    public Image handWinner;
    public List<CardHolderUI> cardHolders;

    public void SetupWonHand(List<Card> wonHand) {
        Debug.Log("Wonhand count: " + wonHand.Count);
        handWinner.sprite =  GameManager.instance.spriteHandler.Icon(wonHand[0].CURRENTOWNER.isPlayer);

        for(int i = 0; i < wonHand.Count; i++) {
            cardHolders[i].playingCard.sprite = GameManager.instance.spriteHandler.WonHandCard(wonHand[i].cardInfo.cardSuit, wonHand[i].cardInfo.cardValue);
            cardHolders[i].winningCardGlow.enabled = wonHand[i].winningCard;
        }
    }
}
