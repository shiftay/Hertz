using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comparison : MonoBehaviour
{
    public CardUI newCard;
    public CardUI deckCard;
    public Animator animator;

    public void Setup(Card deckCard, Card newCard) {
        this.newCard.Setup(newCard, newCard.enhancements);
        this.deckCard.Setup(deckCard, deckCard.enhancements);
        animator.SetTrigger("Display");
    }

    public void Yes() {
       
        // TODO: > Trigger animation of the other card being destroyed  
        //       > Close
        animator.SetTrigger("Hide");
        //       > Take Money
        GameManager.instance.shop.BuyCard();
        //       > Update View Deck
    }

    public void No() {
        animator.SetTrigger("Hide");
    }
}
