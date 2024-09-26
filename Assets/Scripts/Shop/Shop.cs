using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private Player _currentPlayer;
    /*
        IMPLEMENT  

        Buy Trinket
                    > Trinkets until trinket slots are filled.
        
        Buy Featured Pack
                    > Pack slides to center
                    > Rips open
                    > Player can choose what to do with however many cards are inside the pack
                    > Sell the cards
                    > Replace cards in the deck

        IDEA BASE CARDS DO NOT SELL FOR ANYTHING

        BASELINE

        View Deck   
                    > Clickable and opens up the deck, and shows currently the cards inside it.
                    > Shows a count of the different enhancements on the side
        
        Buy Individual Card
                    > Purchase a card and replaces it into the deck
                    IF  Replacing another enhanced card show the player so they can decide to continue and sell the card it's replacing
                    OR  Cancel the purchase
        
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

#region Initialization
    public void SetupShop(Player player) {
        _currentPlayer = player;

        currentAmount = Utils.BASEREROLL;

        currentOpen = null;

        SetLabel();
        SetupDeck();
        SetupCards();
    }
#endregion

#region Trinket

#endregion

#region View Deck
    public void SetupDeck() {
        _currentPlayer._currentDeck.Sort();

        for(int i = 0; i < cards.Count; i++) {
            cards[i].Setup( GameManager.instance.dealer.MAINPLAYER._currentDeck.cards[i],
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

#region IndividualCards
    [Header("Individual Cards")]
    public List<ShopCard> shopCards;
    public Comparison comparison;
    private List<Card> currentForSale = new List<Card>();

    public void BuyCard() {
        shopCards.Find(n => n.cardUI.card.Compare(currentOpen)).animator.SetTrigger("Hide");
        _currentPlayer._currentDeck.AddCard(currentOpen);
        _currentPlayer.scoring.Buy(ConvertRarityToPrice(currentOpen.rarity));
        shopCards.Find(n => n.cardUI.card.Compare(currentOpen)).Sold();

        GameManager.instance.handlerUI.UpdateGold(_currentPlayer);

        currentOpen = null;
        
        SetupDeck();
    }

    public void SetupCards() {
        for(int i = 0; i < shopCards.Count; i++) {
            Rarity t = ReturnRarity();
            Card temp = CreateCard(t);

            while(Contains(temp)) temp = CreateCard(t); 

            shopCards[i].Setup(temp, ConvertRarityToPrice(t));
            currentForSale.Add(temp);
        }
    }

    private bool Contains(Card card) {
        return shopCards.FindAll(n => n.cardUI.card == card).Count > 0;
    }

    public Card CreateCard(Rarity CardRarity) {
        CardInfo test = new CardInfo(randomValue(), RandomSuit());

        int amount = ConvertRarity(CardRarity);

        List<Enhancements> enhancements = new List<Enhancements>();

        // FIXME    Allows Hearts to get the heal enhancment which is not allowed.

        for(int i = 0; i < amount; i++) {
            enhancements.Add(RandomEnhancement(enhancements));
        }

        return new Card(new CardInfo(randomValue(), RandomSuit()), enhancements, CardRarity);
    }

    public void ShowButton(CardUI card) {
        if(currentOpen == null) currentOpen = card.card;
        else if(!card.card.Compare(currentOpen)) {
            shopCards.Find(n => n.cardUI.card.Compare(currentOpen)).animator.SetTrigger("Hide");
            currentOpen = card.card;
        } else if (card.card.Compare(currentOpen)) return;

        shopCards.Find(n => n.cardUI == card).animator.SetTrigger("Display");
    }

    private Card currentOpen;

    public void ShowComparison(CardUI card) {
        currentOpen = card.card;
        comparison.Setup(GameManager.instance.dealer.MAINPLAYER._currentDeck.cards.Find(n => n.Compare(card.card)), card.card);
    }

    /*
        Pop Shows Card you plan on replacing.

        Title says "Do you want to switch this card into your deck?"

        Shows both cards
            // Maybe shows tooltips
            // [HEAL][DAMAGE][GOLD]
        
        Yes || No

        IF Yes > Take players money, change value and play pop on the animator

        IF No > Close the pop up and show store.
    */

#endregion

#region ReRoll
    [Header("Reroll")]
    public TextMeshProUGUI rerollLabel;
    private int currentAmount;
    private bool rerolling = false;

    private void SetLabel() {
        rerollLabel.text = "$" + currentAmount.ToString();
    }

    public void ReRoll() {
        if(rerolling) return;

        if(_currentPlayer.scoring.CanBuy(currentAmount))
        {
            rerolling = true;
            //          Update Gold.
            _currentPlayer.scoring.Buy(currentAmount);
            // TODO     Update reroll Amount
            //          Update Label
            SetLabel();
            //          ReRoll / Trinkets and Individual Cards
            StartCoroutine(UpdateCardsAndTrinkets());
            GameManager.instance.handlerUI.UpdateGold(_currentPlayer);
        } 
        else
        {
            // Shake / Pop Label for Gold.
            Debug.Log("Failing the can buy");
        } 
    }

    private IEnumerator UpdateCardsAndTrinkets() {
        // TODO:
        //      Show Animation Of Hiding the Current cards / Trinkets
        for(int i = 0; i < shopCards.Count; i++) {
            shopCards[i].animator.SetTrigger("Flip");
            yield return new WaitForSeconds(0.25f);
        }
        
        yield return new WaitForSeconds(0.25f);
        // Setup Cards 
        SetupCards();


        for(int i = 0; i < shopCards.Count; i++) {
            shopCards[i].animator.SetTrigger("Revert");
            yield return new WaitForSeconds(0.25f);
        }
        // Setup Trinkets
        // Featured does not reroll.
        rerolling = false;
    }
#endregion

#region Continue

    public void Continue() {
        // TODO:    Cleanup
        
        //          Animate an exit.
        StartCoroutine(ToGamePlay());
    }

    private IEnumerator ToGamePlay() {
        GameManager.instance.handlerUI.roundEnd.ResetAnim();
        GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
        // Wait for Anim
        yield return new WaitUntil(() => !GameManager.instance.handlerUI.roundEnd.animPlaying);

        // Set State to gameplay
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.Gameplay);
        GameManager.instance.dealer.GameSetup();

        GameManager.instance.handlerUI.cardTransition.Remove();
    }

#endregion

#region Utilities

    public int randomValue() {
        return Random.Range(2, Utils.ACE);
    }

    public Utils.CARDSUIT RandomSuit() {
        return (Utils.CARDSUIT)Random.Range(0, (int)Utils.CARDSUIT.CLUB);
    }

    public int ConvertRarity(Rarity rarity) {
        if(rarity == Rarity.RARE) return 3;
        else if(rarity == Rarity.UNCOMMON) return 2;
        else return 1;
    }

    public int ConvertRarityToPrice(Rarity rarity) {
        if(rarity == Rarity.RARE) return 7;
        else if(rarity == Rarity.UNCOMMON) return 5;
        else return 3;
    }

    public Enhancements RandomEnhancement(List<Enhancements> currentEnhancements) {
        Enhancements retVal = Enhancements.ReturnRandom();


        while(currentEnhancements.FindAll(n => n.type == retVal.type).Count > 0) {
            retVal = Enhancements.ReturnRandom();
        }

        return retVal;
    }


    public enum Rarity { COMMON = 100, UNCOMMON = 45, RARE = 15 }

    public Rarity ReturnRarity() {
        int randomVal = Random.Range(0, (int)Rarity.COMMON);

        if(randomVal < (int)Rarity.RARE) return Rarity.RARE;
        else if(randomVal < (int)Rarity.UNCOMMON) return Rarity.UNCOMMON;
        else return Rarity.COMMON;
    }


#endregion
}
