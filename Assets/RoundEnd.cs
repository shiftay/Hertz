using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.Serialization.Formatters;


public class RoundEnd : MonoBehaviour
{

    public Transform cardWonParent; 
    public CardHolderUI wonCardPrefab;

    public TextMeshProUGUI healthCurrent, healingValue, damageValue;
    public TextMeshProUGUI goldCurrent, goldChange;
    public TextMeshProUGUI scoreAmount;

    public TextMeshProUGUI mainTitle, cardsWonTitle;

    public Animator popUp;
    public delegate void CallBack();

    private Player currentPlayer;

    public void Setup(Player p) {
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.RoundEnd);

        currentPlayer = p;

        cardsWonTitle.text = mainTitle.text = "";

        StartCoroutine(GhostWrite(Utils.ROUNDCOMPLETED, mainTitle, CardsWon));
        healthCurrent.text = "";
        goldCurrent.text = "";
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

    private bool animPlaying;
    public bool ANIMPLAYING() { return animPlaying; }
    public void AnimationComplete() => animPlaying = false;

    public Animator health, gold, score;

    public Animator heal, damage, goldDiff;

    private delegate IEnumerator extraElements(int FinalValue);


    // IEnumerator LoadIn(CanvasGroup group) {

    // }

    // IEnumerator CountUp(int startingVal, int additons, TextMeshProUGUI element) {

    // }

    public void CardsWon() {
        StartCoroutine(ShowCards(ShowHealth));
    }

    private IEnumerator ShowVals(Animator anim, string toBeWritten, TextMeshProUGUI element, int FinalValue, CallBack callBack = null, extraElements extraElements = null) {

        animPlaying = true;

        anim.SetTrigger("TurnOn");
        
        yield return new WaitUntil(() => !animPlaying);

        yield return StartCoroutine(GhostWrite(toBeWritten, element));

        if(extraElements != null) yield return StartCoroutine(extraElements(FinalValue));

        if(callBack != null) callBack();
    }


    private IEnumerator HealthChanges(int FinalValue) {
        if(currentPlayer.health.healingQueue > 0) {
            ResetAnim();
            healingValue.text = currentPlayer.health.healingQueue.ToString();
            heal.SetTrigger("Pop");
            yield return new WaitUntil(() => !animPlaying);
        }
        
        if(currentPlayer.health.damageQueue > 0) {
            ResetAnim();
            damageValue.text = currentPlayer.health.damageQueue.ToString();
            damage.SetTrigger("Pop");
            yield return new WaitUntil(() => !animPlaying);
        }
        
        ResetAnim();
        healthCurrent.text = FinalValue.ToString();
        health.SetTrigger("TextSplash");
        yield return new WaitUntil(() => !animPlaying);
    }

    private IEnumerator GoldChanges(int FinalValue) {
        ResetAnim();
        goldChange.text = currentPlayer.scoring.goldQueue.ToString();
        goldDiff.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);

        ResetAnim();
        goldCurrent.text = FinalValue.ToString();
        gold.SetTrigger("TextSplash");
        yield return new WaitUntil(() => !animPlaying);
    }

    private void ResetAnim() { animPlaying = true; }

    public void ShowHealth() {
        int finalVal = currentPlayer.health.currentHealth - currentPlayer.health.damageQueue + currentPlayer.health.healingQueue;

        StartCoroutine(ShowVals(health, currentPlayer.health.currentHealth.ToString(), healthCurrent, finalVal, ShowCoins, HealthChanges));
    }

    public void ShowCoins(){
        int finalVal = currentPlayer.scoring.currentGold + currentPlayer.scoring.goldQueue;

        StartCoroutine(ShowVals(gold, currentPlayer.scoring.currentGold.ToString(), goldCurrent, finalVal, ShowScore, GoldChanges));
    }

    public void ShowScore() {
        int finalVal = currentPlayer.health.currentHealth - currentPlayer.health.damageQueue + currentPlayer.health.healingQueue;

    }

    public void ShowButtons() {
        // TODO: Show buttons for going to the shop.
    }

    private IEnumerator ShowCards(CallBack callBack = null) {
        yield return StartCoroutine(GhostWrite(Utils.CARDSWON, cardsWonTitle));

        List<Card> playerCards =  GameManager.instance.dealer.PlayerCards();


        for(int i = 0; i < playerCards.Count; i++) {
                yield return new WaitForSeconds(0.2f);  // TODO: Turn into constant
                CardHolderUI temp = Instantiate(wonCardPrefab);
                temp.playingCard.sprite =  GameManager.instance.dealer.spriteHandler.WonHandCard(playerCards[i].cardInfo.cardSuit, playerCards[i].cardInfo.cardValue);
                temp.transform.SetParent(cardWonParent);
        }

        if(callBack != null) callBack();  
    }

}
