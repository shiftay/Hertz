using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;



public class RoundEnd : MonoBehaviour
{
    public List<RoundEndDescriptor> roundEndDescriptors;    

    public CardUI wonCardPrefab;
    public RoundEndValues incomePrefab;

    public Animator popUp;
    public delegate void CallBack();
    private Player currentPlayer;

    public void Setup(Player p) {
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.RoundEnd);
        currentPlayer = p;
        roundEndDescriptors.ForEach(n => n.holder.Init());
        ShowDamage();
    }

    public bool animPlaying;
    public void AnimationComplete() {
        animPlaying = false;
    }

    public Animator health, gold, score;

    /* 
        IMPLEMENT Decide Order of Healing / Damage


        IF Trinkets exist that effect the current tally, loop through the trinkets that do affect the one that is currently tallying
            Either flashing the trinket, having it slide down and add it's value, etc..

        TODO Implement Trinkets into scoring, damage, healing behvaiour      

    */

    public void ResetAnim() { animPlaying = true; }

    [Header("Transistions")]
    public Animator healthTransition;
    public Animator goldTransition, scoringTransition;
    public Animator incomeScreen;
    public void ShowDamage() {                                                 
        StartCoroutine(ShowScreen(healthTransition, currentPlayer.health.damageQueue, FindDescriptor(RoundEndTypes.Health), health, ShowScore));
    }

    public void ShowScore() {
        StartCoroutine(ShowScreen(scoringTransition, currentPlayer.scoring.scoreQueue, FindDescriptor(RoundEndTypes.Score), score, ShowGold));
    }

    public IEnumerator ShowScreen(Animator transition, List<Source> queue, RoundEndDescriptor descriptor,
                            Animator mainValue, CallBack callBack = null) 
    {
        descriptor.holder.Setup(descriptor.color);
        // Reset Anim
        ResetAnim();
        // Show Pop Up
        descriptor.holder.animator.SetTrigger("Display");
        // Wait for Anim
        yield return new WaitUntil(() => !animPlaying);


        //Reset Anim
        ResetAnim();
        // Slide Gold ontop of pop up
        transition.SetTrigger("Update" + descriptor.title);
        // Wait for Anim
        yield return new WaitUntil(() => !animPlaying);

        yield return StartCoroutine(GhostWrite(descriptor.title, descriptor.holder.title));
        // Load in each individual scoring reason
        int value = 0;
        for(int i = 0; i < queue.Count; i++) {
            yield return new WaitForSeconds(0.3f);  // TODO: Turn into constant
            RoundEndValues temp = Instantiate(incomePrefab);
            temp.SetValues(SourceLabels.FormatLabel(queue[i].type, descriptor.title), queue[i].VALUE, descriptor.color);
            temp.transform.SetParent(descriptor.holder.valuesParent);
            temp.transform.localScale = Vector3.one;
            value += queue[i].VALUE;
            if(queue[i].associatedCards.Count > 0) yield return StartCoroutine(ShowCards(queue[i].associatedCards, descriptor.cardParent));
        }

        // ResetAnim
        ResetAnim();
        //Pop Gold Value to new Gold Value

        currentPlayer.AdjustValue(descriptor.type, value);
        GameManager.instance.handlerUI.UpdateValues(currentPlayer);
        mainValue.SetTrigger("Pop");
        //wait for anim
        yield return new WaitUntil(() => !animPlaying);

        ResetAnim();
        transition.SetTrigger("Hide" + descriptor.title); 
        descriptor.holder.animator.SetTrigger("Hide");
        yield return new WaitUntil(() => !animPlaying);
        
        ScheduleCardsForDeletion(descriptor.cardParent);
        if(callBack != null) callBack();
    }

    private void ShowGold() {
        StartCoroutine(ShowScreen(goldTransition, currentPlayer.scoring.goldQueue, FindDescriptor(RoundEndTypes.Income), gold, MoveToShop));
    }

    private void MoveToShop() {
        StartCoroutine(Shop());
    }

    private IEnumerator Shop() {
        ResetAnim();
        GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
        // Bring Card Transistion In
        yield return new WaitUntil(() => !animPlaying);

        // Clean Up Cards
        CleanUp();

        // Setup Store

        yield return new WaitForSeconds(1.0f); // TODO: Remove
        // Anim to open the store
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.Store);
        // Remove Card Transition
        GameManager.instance.handlerUI.cardTransition.Remove();
    }


    // Clean up Cards
    private List<GameObject> objects = new List<GameObject>();
    private void ScheduleCardsForDeletion(Transform parent) {
        if(parent == null) return;

        for(int i = 0; i < parent.childCount; i++) {
            objects.Add(parent.GetChild(i).gameObject);
        }

        objects.ForEach(n => n.SetActive(false));
    }

    private void CleanUp() {
        for(int i = objects.Count - 1; i >= 0; i++) {
            Destroy(objects[i]);
        }

        objects.Clear();
    }


    private IEnumerator ShowCards(List<Card> cards, Transform parent) {
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

    public RoundEndDescriptor FindDescriptor(RoundEndTypes t) {
        return roundEndDescriptors.Find(n => n.type == t);
    }

}

#region RoundDescriptor
public enum RoundEndTypes { Health, Score, Income }
[System.Serializable]
public class RoundEndDescriptor {
    public string title;
    public Color color;
    public RoundEndTypes type;
    public RoundEndHolder holder;
    public Transform cardParent;
}
#endregion