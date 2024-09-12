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


    private IEnumerator GhostWrite(string toBeWritten, TextMeshProUGUI element, CallBack callBack = null) {
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


    // IEnumerator LoadIn(CanvasGroup group) {

    // }

    // IEnumerator CountUp(int startingVal, int additons, TextMeshProUGUI element) {

    // }

    public void CardsWon() {
        StartCoroutine(ShowCards());
    }

    private IEnumerator ShowCards(CallBack callBack = null) {
        yield return StartCoroutine(GhostWrite(CONSTS.CARDSWON, cardsWonTitle));

        List<Card> playerCards =  GameManager.instance.dealer.PlayerCards();


        for(int i = 0; i < playerCards.Count; i++) {
                yield return new WaitForSeconds(0.2f);  // TODO: Turn into constant
                CardHolderUI temp = Instantiate(wonCardPrefab);
                temp.playingCard.sprite =  GameManager.instance.dealer.spriteHandler.WonHandCard(playerCards[i].cardInfo.cardSuit, playerCards[i].cardInfo.cardValue);
                temp.transform.SetParent(cardWonParent);
        }

        if(callBack != null) callBack();  // FIXME: Check don't think this is needed at all. 
    }

}
