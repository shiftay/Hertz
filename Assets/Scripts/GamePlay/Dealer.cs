using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class HandPositions {
    public HandController dealPos;
    public Transform coinPos;
    public Transform cardPlayedPos;
}


public class Dealer : MonoBehaviour
{
#region Setup Vars
    public SpriteHandler spriteHandler;
    public List<CardGO> Deck = new List<CardGO>();
    List<Player> players = new List<Player>();
    public CardGO cardPrefab;
    public List<HandPositions> dealPositions;
    public static Dealer instance;
    public GameObject dealerCoin;
    public HandController playerController;
#endregion

#region Gameplay Vars
    public CardGO _currentSelected;

    private Player currentTurn;

    private bool _gameStarted;
    private float _turnTimer;

    private List<CardGO> cardsScheduledForDeletion = new List<CardGO>();
    private List<Card> playedCards = new List<Card>();

    private int currentHand;
    private int currentCard;
    private bool calculatingScore;
    private float playTime;

#endregion

    /* 
        "Turns"
            > One player at random will be labeled as the "Dealer" 
                > This means they are the first person to play their card.
            > Player should only really be able to interact with their cards if it is currently their turn to play.
            > Cards will be played out in clockwise order by default

            Post-Turn
                > The High card wins the round, collecting all of the cards in the middle.
                    > The cards will be moved towards the winning player, and a "Won Hands" UI Compartment will show up at the players area
                > No scoring occurs until every card is played.

            > "Won Hands"
                > Will be a UI Showing the cards that the person has won
                > Small and concise, will only show up on Hover of the Compartment itself. 
        "AI"
            > Multiple stages of Difficulty
            > At no point should they "know" anything before it has been played.
                > Focus on their ability to Attempt to swindle the player into winning Hearts
                > Focus on the ability to use different strategies
                > Search possible strategies through google.
            > Possible Strategies:
                > Attempting to dump a specific suit, so that they can start to play Hearts into the middle.
                    > Example: "Winning all club hands, in order to lose a low club hand, allowing them to be out of clubs for any specific reason"

    */

#region Initialization
    void Start()
    {
        instance = this;
        calculatingScore = false;

        CreateCards();
        SetupPlayers();
        Shuffle(Deck);
        Deal();
        currentCard = currentHand = 0;
    }

    private void SetupPlayers() {
        foreach(HandPositions handPos in dealPositions) {
            Player temp = new Player(handPos.dealPos.currentAXIS == CONSTS.PLAYERAXIS);
            temp.difficulty = (Difficulty.DIFFICULITIES)UnityEngine.Random.Range(0, 3);
            handPos.dealPos.player = temp;
            players.Add(temp);
        }

        int x = UnityEngine.Random.Range(0, dealPositions.Count);

        currentTurn = players[x];
        dealerCoin.transform.position = dealPositions[x].coinPos.position;
    }


    public void Deal() {
        for(int i = 0; i < Deck.Count; i++) {
            // I % 4 to make sure we deal to 4 different players.
            Player curPlayer = players[i % 4];
            Deck[i].transform.position = dealPositions[i % 4].dealPos.transform.position;
            Deck[i].transform.SetParent(dealPositions[i % 4].dealPos.transform);
            Deck[i]._currentCard.CURRENTOWNER = curPlayer;
            curPlayer._currentHand.cards.Add(Deck[i]);
            
            if(!curPlayer.isPlayer) Deck[i]._currentSprite.sprite = spriteHandler.CardBack();
        }

        _gameStarted = true;
        playTime = UnityEngine.Random.Range(1.25f, 2.5f);
    }

    private void CreateCards() {
        CardGO current = null;

        for(int i = 0; i <= (int)CONSTS.CARDSUIT.CLUB; i++) {
            for(int j = 2; j <= CONSTS.ACE; j++) {
                current = Instantiate(cardPrefab);
                current._currentCard = new Card(new CardInfo(j, (CONSTS.CARDSUIT)i));
                current._currentSprite.sprite = spriteHandler.FindCard((CONSTS.CARDSUIT)i, j);
                current.name = ((CONSTS.CARDSUIT)i).ToString("D") + " " + j.ToString();
                // current.transform.position = new Vector3(-7.5f + (j - CONSTS.CARDVALUEMODIFIER) * 1.5f, -3.5f + i * 2, 0);
                Deck.Add(current);
            }
        }
    }
#endregion 

#region Gameplay Loop
    private void FixedUpdate() {
        if(!_gameStarted) return;

        if(currentTurn.isPlayer) return;

        if(calculatingScore) return;

        _turnTimer += Time.deltaTime;
        if(_turnTimer > playTime) {
            _turnTimer = 0.0f;

            playTime = UnityEngine.Random.Range(1.25f, 2.5f);

            EndTurn(dealPositions.Find(n => n.dealPos.player == currentTurn).dealPos.PlayCard());
        }
    }

    private void EndTurn(CardGO playedCard) {
        playedCard.transform.SetParent(dealPositions.Find(n => n.dealPos.player == currentTurn).cardPlayedPos);
        playedCard.transform.localPosition = Vector3.zero;
        playedCard._currentSprite.sprite = spriteHandler.FindCard(playedCard._currentCard.cardInfo.cardSuit, playedCard._currentCard.cardInfo.cardValue);

        currentTurn._currentHand.cards.Remove(playedCard);
        
        playedCard._currentCard.handPlayed = currentHand;
        playedCard._currentCard.cardPlayed = currentCard;
        playedCards.Add(playedCard._currentCard);
        cardsScheduledForDeletion.Add(playedCard);

        currentCard++;
        if(currentCard == 4) {
            calculatingScore = true;
            StartCoroutine(DetermineHandWinner());
        } else {
            int x = dealPositions.FindIndex(x => x.dealPos.player == currentTurn);
            x++;
            if(x >= players.Count) x = 0;
            currentTurn = players[x];
        }
    }

    private IEnumerator DetermineHandWinner() {
        yield return new WaitForSeconds(1.0f);

        Card highCard = playedCards[currentHand * 4];
        List<Card> hand = new List<Card>();
        for(int i = currentHand * 4; i < (currentHand * 4) + 4; i++) {
            hand.Add(playedCards[i]);
            if(CurrentSUIT() == playedCards[i].cardInfo.cardSuit && highCard.cardInfo.cardValue < playedCards[i].cardInfo.cardValue) {
                highCard = playedCards[i];
            }
        }

        hand.Find(n => n.cardInfo.cardSuit == highCard.cardInfo.cardSuit && n.cardInfo.cardValue == highCard.cardInfo.cardValue).winningCard = true;

        Player winnerOfHand = highCard.CURRENTOWNER;
        WonHand temp = new WonHand(hand, winnerOfHand);
        winnerOfHand.wonHands.Add(temp);
        

        for(int i = cardsScheduledForDeletion.Count - 1; i >= 0; i--) {
            Destroy(cardsScheduledForDeletion[i].gameObject);
        }
        cardsScheduledForDeletion.Clear();


        UIHandler.instance.CreateWonHand(temp);

        currentTurn = winnerOfHand;
        dealerCoin.transform.position = dealPositions[players.FindIndex(n => n == currentTurn)].coinPos.position;
        currentHand++;
        currentCard = 0;

        if(winnerOfHand._currentHand.cards.Count == 0)  { // No more cards for the winner to play.
            Debug.Log("Score teh round.");
            /* 
                "Scoring"
                > Each Heart is worth 1 point
                > Queen of Spade is worth 13

                > IF a player collected all hearts and the queen of spades
                    >"Shot the Moon"
                    > 26 points are given to the other players.
                    > Score to 100
                    > Attempt to be lowest score.

            
            
            */
        } else {

            calculatingScore = false;
        }
    }

#endregion

#region Utility Callbacks

    public bool IsPlayerTurn() {
        return currentTurn.isPlayer;
    }


    public CONSTS.CARDSUIT CurrentSUIT() {
        Card temp = playedCards.FindAll(n => n.handPlayed == currentHand).Find(x => x.cardPlayed == 0);
        return temp == null ? CONSTS.CARDSUIT.NULL : temp.cardInfo.cardSuit;
    }

    public bool HaveHeartsBeenPlayed() {
        return playedCards.FindAll(n => n.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART 
        || (n.cardInfo.cardSuit == CONSTS.CARDSUIT.SPADE && n.cardInfo.cardValue == 12)).Count > 0 || HeartInActiveHand();
    }

    public bool HeartInActiveHand() {
        return playedCards.FindAll(n => (n.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART && n.handPlayed == currentHand) 
        || (n.cardInfo.cardSuit == CONSTS.CARDSUIT.SPADE && n.cardInfo.cardValue == 12 && n.handPlayed == currentHand)).Count > 0;
    }

    // This should never be called if it can possibly be null.
    public int CurrentHighCardInSuit() {
        List<Card> temp = playedCards.FindAll(n => n.handPlayed == currentHand && n.cardInfo.cardSuit == CurrentSUIT());

        temp.OrderByDescending(x => x.cardInfo.cardValue).ToList();

        return temp[0].cardInfo.cardValue;
    }

    public int CurrentPlayed() {
        return currentCard;
    }

    
    private static System.Random random = new System.Random();
    private static void Shuffle<T>(List<T> array) 
    {
        for (int i = 0; i < array.Count - 1; ++i) 
        {
            int r = random.Next(i, array.Count);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }

    public bool IsCardPlayable(Card card)
    {
        if(currentCard == 0) {
            if(card.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART && !HaveHeartsBeenPlayed()) {
                currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit != CONSTS.CARDSUIT.HEART).ForEach(n => n._animator.SetTrigger("Glow"));
                return false;
            } 

            return true;
        }

        if((card.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART && !HaveHeartsBeenPlayed()) || card.cardInfo.cardSuit != CurrentSUIT())  {

            if(currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit ==  CurrentSUIT()).Count == 0) {
                return true;
            }

            currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CurrentSUIT()).ForEach(n => n._animator.SetTrigger("Glow"));

            return false;
        }

        return true;
    }

#endregion

    public Transform playerArea;

    public void Clicked(CardGO clicked) {
        if(_currentSelected == null) {
            _currentSelected = clicked;
        } else if(_currentSelected != clicked) {
            // Send something to handcontroller to highlight it?

            _currentSelected = clicked;
        } else if (_currentSelected == clicked) {
            // Play the card.
            EndTurn(clicked);
        }
    }

}
