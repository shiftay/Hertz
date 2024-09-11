using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;


public class RoundEnd : MonoBehaviour
{

    public Transform cardWonParent; 
    public CardHolderUI wonCardPrefab;

    public TextMeshProUGUI healthCurrent, healthChange;
    public TextMeshProUGUI coinsCurrent, coinChange;
    public TextMeshProUGUI scoreAmount;

    public Animator popUp;

    public void Setup(Player p) {
        for(int i = 0; i < p.wonHands.Count; i++) {
            for(int j = 0; j < p.wonHands[i].cards.Count; j++) {
                CardHolderUI temp = Instantiate(wonCardPrefab);
                temp.playingCard.sprite = Dealer.instance.spriteHandler.WonHandCard(p.wonHands[i].cards[j].cardInfo.cardSuit, p.wonHands[i].cards[j].cardInfo.cardValue);
                temp.transform.SetParent(cardWonParent);
            }
        }

        healthCurrent.text = p.Health.ToString();
        coinsCurrent.text = p.scoring.money.ToString();
    }

}
