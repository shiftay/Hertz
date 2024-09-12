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

    public TextMeshProUGUI mainTitle, cardsWonTitle;

    public Animator popUp;
    public delegate void CallBack();
    public CanvasGroup canvasGroup;

    private Player currentPlayer;

    public void Setup(Player p) {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;

        currentPlayer = p;

        cardsWonTitle.text = mainTitle.text = "";

        StartCoroutine(GhostWrite(CONSTS.ROUNDCOMPLETED, mainTitle, CardsWon));
        // healthCurrent.text = p.Health.ToString();
        // coinsCurrent.text = p.scoring.money.ToString();
    }


    IEnumerator GhostWrite(string toBeWritten, TextMeshProUGUI element, CallBack callBack = null) {
        string write = "";
        for(int i = 0; i < toBeWritten.Length; i++) {
            write += toBeWritten[i];
            element.text = write;
            yield return new WaitForSeconds(0.1f); // TODO: Turn into constant
        }


        if(callBack != null) {
            callBack(); 
        }
    }

    public void CardsWon() {
        StartCoroutine(ShowCards());
    }

    IEnumerator ShowCards(CallBack callBack = null) {
        yield return StartCoroutine(GhostWrite(CONSTS.CARDSWON, cardsWonTitle));

        for(int i = 0; i < currentPlayer.wonHands.Count; i++) {
            for(int j = 0; j < currentPlayer.wonHands[i].cards.Count; j++) {
                yield return new WaitForSeconds(0.2f);  // TODO: Turn into constant
                CardHolderUI temp = Instantiate(wonCardPrefab);
                temp.playingCard.sprite = Dealer.instance.spriteHandler.WonHandCard(currentPlayer.wonHands[i].cards[j].cardInfo.cardSuit, currentPlayer.wonHands[i].cards[j].cardInfo.cardValue);
                temp.transform.SetParent(cardWonParent);
            }
        }

        if(callBack != null) callBack();  // FIXME: Check don't think this is needed at all. 
    }

}
