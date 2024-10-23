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
    public GameObject RoundEndHolder;
    public Animator popUp;
    public delegate void CallBack();
    private Player currentPlayer;

    public void Setup(Player p) {
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.RoundEnd, ShowDamage);
        currentPlayer = p;
        roundEndDescriptors.ForEach(n => n.holder.Init());
        // ShowDamage();
    }




    /* 
        IMPLEMENT Decide Order of Healing / Damage

        IF Trinkets exist that effect the current tally, loop through the trinkets that do affect the one that is currently tallying
            Either flashing the trinket, having it slide down and add it's value, etc..

        TODO Implement Trinkets into currentGameStats.scoring, damage, healing behvaiour      

    */




    public Animator incomeScreen;
    public void ShowDamage() {                                                 
        StartCoroutine(ShowScreen(currentPlayer.currentGameStats.health.damageQueue, FindDescriptor(RoundEndTypes.Health), TextAnchor.UpperRight, ShowScore));
    }

    public void ShowScore() {
        StartCoroutine(ShowScreen( currentPlayer.currentGameStats.scoring.scoreQueue, FindDescriptor(RoundEndTypes.Score), TextAnchor.UpperLeft, ShowGold));
    }

    public IEnumerator ShowScreen(List<Source> queue, RoundEndDescriptor descriptor, TextAnchor corner, CallBack callBack = null) 
    {
        descriptor.holder.Setup(descriptor.color);
        // Reset Anim
        GameManager.instance.ResetAnim();
        // Show Pop Up
        descriptor.holder.animator.SetTrigger("Display");
        // Wait for Anim
        yield return new WaitUntil(() => !GameManager.instance.animPlaying);


        // //Reset Anim
        // GameManager.instance.ResetAnim();
        // // Slide Gold ontop of pop up
        // // Wait for Anim
        // yield return new WaitUntil(() => !GameManager.instance.animPlaying);

        yield return StartCoroutine(GhostWrite(descriptor.title, descriptor.holder.title));
        // Load in each individual currentGameStats.scoring reason

        Debug.Log("Count: " + queue.Count);
        int value = 0;
        int multiplier = 0;

        for(int i = 0; i < queue.Count; i++) {
            yield return new WaitForSeconds(Utils.ROUNDENDTRANSITION); 
            RoundEndValues temp = Instantiate(incomePrefab);
            temp.SetValues(SourceLabels.FormatLabel(queue[i].type, descriptor.title, queue[i].refID), queue[i].VALUE, descriptor.color, queue[i].isMulti ? "x" : "");
            temp.transform.SetParent(descriptor.holder.valuesParent);
            temp.transform.localScale = Vector3.one;

            if(queue[i].isMulti) multiplier += queue[i].VALUE;
            else value += queue[i].VALUE;

            if(queue[i].associatedCards.Count > 0) {
                GameObject reHolder = Instantiate(RoundEndHolder);
                reHolder.transform.SetParent(descriptor.cardParent);
                reHolder.transform.localScale = Vector3.one;
                reHolder.GetComponent<GridLayoutGroup>().childAlignment = corner;
                yield return StartCoroutine(ShowCards(queue[i].associatedCards, reHolder.transform));
            }
        }
        // ResetAnim
        // GameManager.instance.ResetAnim();
        //Pop Gold Value to new Gold Value
        currentPlayer.AdjustValue(descriptor.type, value * multiplier);
        GameManager.instance.handlerUI.UpdateValues(currentPlayer);
        
        //wait for anim
        // yield return new WaitUntil(() => !GameManager.instance.animPlaying);

        GameManager.instance.ResetAnim();
        descriptor.holder.animator.SetTrigger("Hide");
        yield return new WaitUntil(() => !GameManager.instance.animPlaying);
        
        ScheduleCardsForDeletion(descriptor.cardParent);
        if(callBack != null) {
            Debug.Log("Hello? #");
            callBack();
        } 
    }

    private void ShowGold() {
        StartCoroutine(ShowScreen( currentPlayer.currentGameStats.scoring.goldQueue, FindDescriptor(RoundEndTypes.Income), TextAnchor.UpperLeft, MoveToShop));
    }

    private void MoveToShop() {
        StartCoroutine(Shop());
    }

    private IEnumerator Shop() {
        // GameManager.instance.ResetAnim();
        // GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
        // Bring Card Transistion In
        // yield return new WaitUntil(() => !GameManager.instance.animPlaying);
        GameManager.instance.shop.PLAYER = currentPlayer;
        // Clean Up Cards
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.Shop, GameManager.instance.shop.SetupShop);
        CleanUp();

        yield return new WaitForSeconds(1.0f); // TODO: Remove
        // Setup Store
        
        // Anim to open the store

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
        objects.ForEach(n => Destroy(n));
        // for(int i = objects.Count - 1; i >= 0; i++) {
        //     Destroy(objects[i]);
        // }
        objects.Clear();

        List<GameObject> scheduletodestroy = new List<GameObject>();
        roundEndDescriptors.ForEach(n => {
            for(int i = 0; i < n.holder.valuesParent.transform.childCount; i++) {
                if(i == 0) continue;
                scheduletodestroy.Add(n.holder.valuesParent.transform.GetChild(i).gameObject);
            }   
        });

        scheduletodestroy.ForEach(n => Destroy(n));

        currentPlayer.ClearQueues();
    }

    private IEnumerator ShowCards(List<Card> cards, Transform parent) {
        for(int i = 0; i < cards.Count; i++) {
            yield return new WaitForSeconds(0.3f);  // TODO: Turn into constant
            CardUI temp = Instantiate(wonCardPrefab);
            temp.SetImage(GameManager.instance.spriteHandler.FindCard(cards[i].cardInfo.cardSuit, cards[i].cardInfo.cardValue));
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

        if(callBack != null) callBack(); 
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