using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Shop : MonoBehaviour
{
    /*
        IMPLEMENT  
        View Deck   
                    > Clickable and opens up the deck, and shows currently the cards inside it.
                    > Shows a count of the different enhancements on the side
        
        Buy Individual Card
                    > Purchase a card and replaces it into the deck
                    IF  Replacing another enhanced card show the player so they can decide to continue and sell the card it's replacing
                    OR  Cancel the purchase
        
        Buy Trinket
                    > Trinkets until trinket slots are filled.
        
        Buy Featured Pack
                    > Pack slides to center
                    > Rips open
                    > Player can choose what to do with however many cards are inside the pack
                    > Sell the cards
                    > Replace cards in the deck

        IDEA BASE CARDS DO NOT SELL FOR ANYTHING

        Reroll
                    > Reroll starts at X
                    > Goes up each reroll.
                    > Reroll Trinkets and individual cards.
                        > Cards will flip over & Maybe Morph from Marvel snap scrunch up then turn back over.
                        > Trinkets will poof and then poof back.        

        
        Continue
                    > Button to take you to the next round.
    */

    [Header("View Deck")]
    public List<CardUI> cards;
    public Animator cardViewer;

    [Header("Individual Cards")]
    public List<ShopCard> shopCards;

#region Initialization
    public void SetupShop() {
        SetupDeck();
    }
#endregion


#region View Deck
    public void SetupDeck() {
        for(int i = 0; i < cards.Count; i++) {
            cards[i].Setup(GameManager.instance.spriteHandler.FindCard(GameManager.instance.dealer.MAINPLAYER._currentDeck.cards[i].cardInfo),
                                                                       GameManager.instance.dealer.MAINPLAYER._currentDeck.cards[i].enhancements);
        }
    }

    public void ViewDeck() {
        cardViewer.SetTrigger("Display");
    }

    public void CloseDeckView() {
        cardViewer.SetTrigger("Hide");
    }
#endregion


}
