using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Runtime.Serialization.Formatters;


public class RoundEnd : MonoBehaviour
{
    public Transform scoreCardParent, damageCardParent, goldParent;
    public CardUI wonCardPrefab;
    public GoldIncome incomePrefab;
    public TextMeshProUGUI healthCurrent, healingValue, damageValue;
    public TextMeshProUGUI goldCurrent, goldChange;
    public TextMeshProUGUI scoreAmount;
    public TextMeshProUGUI mainTitle;
    public TextMeshProUGUI incomeTitle;
    public Animator popUp;
    public delegate void CallBack();
    private Player currentPlayer;

    public void Setup(Player p) {
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.RoundEnd);

        currentPlayer = p;

        incomeTitle.text = healingValue.text = damageValue.text = mainTitle.text = "";

        StartCoroutine(GhostWrite(Utils.ROUNDCOMPLETED, mainTitle, ShowDamage));

    }



    private bool animPlaying;
    public bool ANIMPLAYING() { return animPlaying; }
    public void AnimationComplete() {
        Debug.LogWarning("Anim Complete");
        animPlaying = false;
    }

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

        TODO Implement Trinkets into scoring, damage, healing behvaiour      

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

        yield return new WaitForSeconds(0.2f);
        ScheduleCardsForDeletion(cardParent);
        // Do next Task
        if(callBack != null) callBack();
    }

#region extraElements
    private IEnumerator Damage() {
        // FIXME:
        // Currently a bastardize version of damage queue
        // Allows for showing of different types of damage.
        // Can change extraElements to take a TextElement + Value
        // So that we can update it multiple times.
        int value = 0;
        currentPlayer.health.damageQueue.ForEach(n => {
            value += n.VALUE;
        });

        ResetAnim();
        damageValue.text = "-" + value.ToString();
        damage.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);
        damage.SetTrigger("FadeOut");
        

        currentPlayer.health.currentHealth -= value;

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

    private IEnumerator Score() {
        currentPlayer.scoring.currentScore += currentPlayer.scoring.scoreQueue;

        ResetAnim();
        scoreAmount.text = currentPlayer.scoring.currentScore.ToString();
        score.SetTrigger("Pop");
        yield return new WaitUntil(() => !animPlaying);
    }

#endregion

    private void ResetAnim() { animPlaying = true; }

    public void ShowHealth() {
        ShowScore();
    }

    [Header("Transistions")]
    public Animator healthTransition;
    public Animator goldTransition, scoringTransition;

    public Animator incomeScreen;
    public void ShowDamage() {                                                  // TODO Constant
        if(currentPlayer.health.damageQueue.Count > 0) StartCoroutine(Task(healthTransition, "Health", GameManager.instance.dealer.DamageCards(), damageCardParent, true, Damage, ShowHealth));
        else ShowScore(); 
    }

    public void ShowScore() {
        StartCoroutine(Task(scoringTransition, "Score", GameManager.instance.dealer.ScoringCards(), scoreCardParent, true, Score, ShowGold));
    }

    public IEnumerator Gold() {

        // Reset Anim
        ResetAnim();
        // Show Pop Up
        incomeScreen.SetTrigger("Display");
        // Wait for Anim
        yield return new WaitUntil(() => !animPlaying);


        //Reset Anim
        ResetAnim();
        // Slide Gold ontop of pop up
        goldTransition.SetTrigger("UpdateMoney");
        // Wait for Anim
        yield return new WaitUntil(() => !animPlaying);

        yield return StartCoroutine(GhostWrite("Income", incomeTitle));
        // Load in each individual scoring reason
        int value = 0;
        for(int i = 0; i < currentPlayer.scoring.goldQueue.Count; i++) {
            yield return new WaitForSeconds(0.3f);  // TODO: Turn into constant
            GoldIncome temp = Instantiate(incomePrefab);
            temp.SetValues(SourceLabels.FindLabel(currentPlayer.scoring.goldQueue[i].type).Label, currentPlayer.scoring.goldQueue[i].VALUE);
            temp.transform.SetParent(goldParent);
            temp.transform.localScale = Vector3.one;
            value += currentPlayer.scoring.goldQueue[i].VALUE;
        }

        // ResetAnim
        ResetAnim();
        //Pop Gold Value to new Gold Value
        currentPlayer.scoring.currentGold += value;
        gold.SetTrigger("Pop");
        //wait for anim
        yield return new WaitUntil(() => !animPlaying);

        ResetAnim();
        goldTransition.SetTrigger("HideMoney"); 
        yield return new WaitUntil(() => !animPlaying);
        
        // Load in button for Go to Shop
        yield return null;
    }

    private void ShowGold() {
        StartCoroutine(Gold());
    }


    // TODO
    // Clean up Cards
    private List<GameObject> objects = new List<GameObject>();
    private void ScheduleCardsForDeletion(Transform parent) {
        for(int i = 0; i < parent.childCount; i++) {
            objects.Add(parent.GetChild(i).gameObject);
        }

        objects.ForEach(n => n.SetActive(false));
    }

    private void CleanUp() {

    }


    private IEnumerator ShowCards(List<Card> cards, Transform parent) {
        // yield return StartCoroutine(GhostWrite(Utils.CARDSWON, cardsWonTitle));

        Debug.Log("Show Cards: " + cards.Count);

        for(int i = 0; i < cards.Count; i++) {
                yield return new WaitForSeconds(0.3f);  // TODO: Turn into constant
                CardUI temp = Instantiate(wonCardPrefab);
                temp.SetImage(GameManager.instance.dealer.spriteHandler.FindCard(cards[i].cardInfo.cardSuit, cards[i].cardInfo.cardValue));
                temp.transform.SetParent(parent);
                temp.transform.localScale = Vector3.one;
        }
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

}
