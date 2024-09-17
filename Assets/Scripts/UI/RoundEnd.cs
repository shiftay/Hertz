using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.Serialization.Formatters;


public class RoundEnd : MonoBehaviour
{
    public Transform scoreCardParent, damageCardParent;
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

        healingValue.text = damageValue.text = mainTitle.text = "";

        StartCoroutine(GhostWrite(Utils.ROUNDCOMPLETED, mainTitle, ShowDamage));

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
    private delegate IEnumerator extraElements();

    /* 
        IDEA 

        When Showing Cards:

        IMPLEMENT Decide Order of Healing / Damage

        Show Damaging Cards > Total Damage > [ Flash Trinket > Total Damage ] > Show Damage Happen to Play

        Show Healing Cards > Total Healing > [ Flash Trinket > Total Healing ] > Heal Player
        
        Show Scoring Cards > Total Score > [ Flash Trinket > Total Score ] > Add it to Current Score
        
        Gold  > Pop Up
            Round Score     +6 gold
            Trinket Source  +X gold
            Trinket Source  +x gold
            Interest        +x gold

            Gold thing will be down here
            Will Get update with a pop anim
            Slide back into position, and Shop button will load in.

        IF Trinkets exist that effect the current tally, loop through the trinkets that do affect the one that is currently tallying
            Either flashing the trinket, having it slide down and add it's value, etc..

        FIXME Change the WonCardPrefab to full size card to fill more space. Test with base amount of damage cards = 14 Scale up from there. Test sizing.

    */

    private IEnumerator Task(Animator transistion, string Trigger, List<Card> cardsToBeShown, Transform cardParent, bool moveAway, extraElements extraElements = null, CallBack callBack = null) {

        // Play Anim on Moving piece. 
        ResetAnim();
        transistion.SetTrigger("Update" + Trigger);
        // Wait till that's complete.
        yield return new WaitUntil(() => !animPlaying);
        
        // Show Cards + Wait till that's complete
        yield return StartCoroutine(ShowCards(cardsToBeShown, cardParent));

        // Show Value of change (Score/Healing/Damage)
        if(extraElements != null) yield return StartCoroutine(extraElements());
        // Update Value
        // Pop Anim
        // wait for pop Anim

        
        if(moveAway) {
        // Play Anim to move away
            ResetAnim();
            transistion.SetTrigger("Hide" + Trigger); 
            yield return new WaitUntil(() => !animPlaying);
            // Play Fade on cards
            // Wait on Fade.
        }

        // Do next Task
        if(callBack != null) callBack();

        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator Damage() {
        ResetAnim();
        damageValue.text = "-" + currentPlayer.health.damageQueue.ToString();
        damage.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);

        currentPlayer.health.currentHealth -= currentPlayer.health.damageQueue;

        ResetAnim();
        healthCurrent.text = currentPlayer.health.currentHealth.ToString();
        health.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);
    }


    private IEnumerator Heal() {
        ResetAnim();
        healingValue.text = currentPlayer.health.healingQueue.ToString();
        heal.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);

        currentPlayer.health.currentHealth += currentPlayer.health.healingQueue;

        ResetAnim();
        healthCurrent.text = currentPlayer.health.currentHealth.ToString();
        health.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);
    }


    private void ResetAnim() { animPlaying = true; }

    public void ShowHealth() {


    }

    [Header("Transistions")]
    public Animator healthTransistion, goldTransistion, scoringTransistion;
    public void ShowDamage() {                                                  // TODO Constant
        if(currentPlayer.health.damageQueue > 0) StartCoroutine(Task(healthTransistion, "Health", GameManager.instance.dealer.DamageCards(), damageCardParent, true, Damage, ShowScore));
        else ShowScore(); 
    }

    public void ShowScore() {
 

    }

    public void ShowButtons() {
        // TODO: Show buttons for going to the shop.
    }

    private IEnumerator ShowCards(List<Card> cards, Transform parent) {
        // yield return StartCoroutine(GhostWrite(Utils.CARDSWON, cardsWonTitle));

        Debug.Log("Show Cards: " + cards.Count);

        for(int i = 0; i < cards.Count; i++) {
                yield return new WaitForSeconds(0.2f);  // TODO: Turn into constant
                CardHolderUI temp = Instantiate(wonCardPrefab);
                temp.playingCard.sprite =  GameManager.instance.dealer.spriteHandler.WonHandCard(cards[i].cardInfo.cardSuit, cards[i].cardInfo.cardValue);
                temp.transform.SetParent(parent);
                temp.transform.localScale = Vector3.one;
        }
    }

}
