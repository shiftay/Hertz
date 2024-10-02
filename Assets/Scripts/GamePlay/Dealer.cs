using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Diagnostics;

[System.Serializable]
public class HandPositions {
    public HandController dealPos;
    public Transform coinPos;
    public Transform cardPlayedPos;
}

public class Dealer : MonoBehaviour
{
#region DEBUG
    public bool shortGame;
    public int startingGold;
#endregion

#region Setup Vars
    public List<CardGO> Deck = new List<CardGO>();
    List<Player> players = new List<Player>();
    public Player MAINPLAYER { get { return players.Find(n => n.isPlayer); }}
    public CardGO cardPrefab;
    public List<HandPositions> dealPositions;
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
#region Initialization
    void Awake()
    {
        Init(); // TODO Move this into another portion of the game. Doesn't need to be in Awake.
        GameSetup();
    }

    private void Init() {
        SetupPlayers();
    }

    public void GameSetup() { 
        int x = UnityEngine.Random.Range(0, dealPositions.Count);

        currentTurn = shortGame ? players.Find(n=> n.isPlayer) : players[x];
        dealerCoin.SetActive(true);
        dealerCoin.transform.position = dealPositions[x].coinPos.position;

        calculatingScore = false;
        CreateCards();
        Shuffle(Deck);
        Deal();

        GameManager.instance.handlerUI.UpdateValues(MAINPLAYER);
        currentCard = currentHand = 0;
    }

    private void SetupPlayers() {
        foreach(HandPositions handPos in dealPositions) {
            Player temp = new Player(handPos.dealPos.currentAXIS == Utils.PLAYERAXIS);
            temp.difficulty = (Utils.DIFFICULITIES)UnityEngine.Random.Range(0, 3);
            handPos.dealPos.player = temp;
            players.Add(temp);
        }

        MAINPLAYER.scoring.currentGold = startingGold;


    }


    public void Deal() {
        for(int i = 0; i < (shortGame ? 4 : Deck.Count); i++) {
            // I % 4 to make sure we deal to 4 different players.
            Player curPlayer = players[i % 4];
            Deck[i].transform.position = dealPositions[i % 4].dealPos.transform.position;
            Deck[i].transform.SetParent(dealPositions[i % 4].dealPos.transform);
            Deck[i]._currentCard.CURRENTOWNER = curPlayer;
            curPlayer._currentHand.cards.Add(Deck[i]);
            
            if(!curPlayer.isPlayer && !Deck[i]._currentCard.ContainsXRAY) Deck[i].ComputerOwned();
        }

        _gameStarted = true;
        playTime = UnityEngine.Random.Range(1.25f, 2.5f);
    }

    private void CreateCards() {
        CardGO current = null;

        for(int i = 0; i < MAINPLAYER._currentDeck.cards.Count; i++) {
            current = Instantiate(cardPrefab);
            current._currentCard = new Card(MAINPLAYER._currentDeck.cards[i].cardInfo);
            current.Setup(MAINPLAYER._currentDeck.cards[i]);
            current.name = MAINPLAYER._currentDeck.cards[i].cardInfo.DebugInfo();
            // current.transform.position = new Vector3(-7.5f + (j - CONSTS.CARDVALUEMODIFIER) * 1.5f, -3.5f + i * 2, 0);
            Deck.Add(current);
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
        playedCard._currentSprite.sprite = GameManager.instance.spriteHandler.FindCard(playedCard._currentCard.cardInfo.cardSuit, playedCard._currentCard.cardInfo.cardValue);

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
        // WonHand temp = new WonHand(hand, winnerOfHand);
        // winnerOfHand.wonHands.Add(temp);
        

        for(int i = cardsScheduledForDeletion.Count - 1; i >= 0; i--) {
            Destroy(cardsScheduledForDeletion[i].gameObject);
        }
        cardsScheduledForDeletion.Clear();

        currentTurn = winnerOfHand;
        dealerCoin.transform.position = dealPositions[players.FindIndex(n => n == currentTurn)].coinPos.position;
        playedCards.FindAll(n => n.handPlayed == currentHand).ForEach(n => n.CURRENTOWNER = winnerOfHand);

        GameManager.instance.handlerUI.bottomBar.CreateWonHand(WonHand(currentHand));

        currentHand++;
        currentCard = 0;

        if(winnerOfHand._currentHand.cards.Count == 0)  { // No more cards for the winner to play.
            dealerCoin.SetActive(false);
#region Scoring  
             /* 
                TODO > Look through trinkets.
                    > Find Scoring / Healing trinkets
                        > Add the values.
            */

            Score(DamageOrScoring(true), true);
            Score(DamageOrScoring(false) , false);

            // FIXME Make this under utilities as a quick and easy call back.
            // int count = 0;
            // bool shotTheMoon = false;
            // for(int i = 0; i < players.Count; i++) {
            //     count = 0;
            //     damageCards.ForEach(n => {
            //         if(n.CURRENTOWNER == players[i]) count++;
            //     });
            //     if(count == 14) shotTheMoon = true;
            // }
            // TODO Start defining "Shoot the moon"
            // Debug.Log("Did someone shoot the moon? " + shotTheMoon);
     
            int MAXDAMAGE = DamageEnhancements + Utils.DEFAULTMAXDAMAGE;
            MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.ENDOFROUND, 
                                                        Utils.ConvertRange(0, MAXDAMAGE, 10, 1, (MAINPLAYER.health.CurrentDamageQueue())) + Mathf.FloorToInt(1.25f * (MAXDAMAGE - Utils.DEFAULTMAXDAMAGE))));  
            if(MAINPLAYER.scoring.currentGold / 5 > 5) {
                MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.INTEREST, 5));
            } else {
                MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.INTEREST, MAINPLAYER.scoring.currentGold / 5));
            }

            if(GoldCards().Count > 0) MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.ENHANCEMENT, GoldCards().Count, GoldCards()));

            MAINPLAYER.trinkets.ForEach(n => {
                if(n.baseTrinket.check == VALUECHECK.SCORING) n.baseTrinket.effect(n.baseTrinket.ID);
            });
            
            GameManager.instance.handlerUI.roundEnd.ResetAnim();
            GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
            // Wait for Anim
            yield return new WaitUntil(() => !GameManager.instance.handlerUI.roundEnd.animPlaying);

            GameManager.instance.handlerUI.cardTransition.Remove();

            GameManager.instance.handlerUI.roundEnd.Setup(MAINPLAYER);
            CleanUp();

#endregion Scoring
        } else {
            calculatingScore = false;
        }
    }

#endregion

#region Utility Callbacks

    [Button("How many enhanced cards")]
    public void DebugDeck(){
        Debug.Log(MAINPLAYER._currentDeck.cards.FindAll(n=>n.enhancements.Count > 0).Count);
    }

    private void CleanUp() {
        Deck.Clear();
        playedCards.Clear();
        GameManager.instance.handlerUI.bottomBar.CleanUp();
        // calculatingScore = false;
    }

    public void Score(List<Card> cardsToValue, bool isDamage) {
        int scoreFromBase = 0;
        int scoreFromEnhancements = 0;
        foreach(Card card in cardsToValue) {
            if(card.isHeart()) scoreFromBase += 1;

            if(card.IsQueenOfSpades()) scoreFromBase += 10;

            if(card.ContainsDamage) scoreFromEnhancements += 1;
        }

        UpdateVal(isDamage, scoreFromBase, scoreFromEnhancements);
    }

    public void UpdateVal(bool isDamage, int fromBase, int fromEnhancements) {
        if(isDamage) {
            MAINPLAYER.health.damageQueue.Add(new Source(SourceType.ENDOFROUND, fromBase * -1, GatherCards(false, true)));
            if(fromEnhancements > 0) MAINPLAYER.health.damageQueue.Add(new Source(SourceType.ENHANCEMENT, fromEnhancements * -1, GatherCards(true, true)));
        } else {
            MAINPLAYER.scoring.scoreQueue.Add(new Source(SourceType.ENDOFROUND, fromBase, GatherCards(false, false)));
            if(fromEnhancements > 0) MAINPLAYER.scoring.scoreQueue.Add(new Source(SourceType.ENHANCEMENT, fromEnhancements, GatherCards(true, false)));
        }
    }

    public bool CardAttached(Transform t) => t.IsChildOf(playerController.transform);

    public bool IsPlayerTurn => currentTurn.isPlayer;
    public Utils.CARDSUIT CurrentSUIT() {
        Card temp = playedCards.FindAll(n => n.handPlayed == currentHand).Find(x => x.cardPlayed == 0);
        return temp == null ? Utils.CARDSUIT.NULL : temp.cardInfo.cardSuit;
    }

    public bool HaveHeartsBeenPlayed() => playedCards.FindAll(n => n.cardInfo.cardSuit == Utils.CARDSUIT.HEART
        || (n.cardInfo.cardSuit == Utils.CARDSUIT.SPADE && n.cardInfo.cardValue == 12)).Count > 0 || HeartInActiveHand();

    public bool HeartInActiveHand() => playedCards.FindAll(n => (n.isHeart() && n.handPlayed == currentHand)
        || (n.IsQueenOfSpades() && n.handPlayed == currentHand)).Count > 0;


    public List<Card> WonHand(int id) {
        return playedCards.FindAll(n=> n.handPlayed == id);
    }

    public List<Card> PlayerCards() {
        return playedCards.FindAll(n => n.CURRENTOWNER.isPlayer);
    }

    // This should never be called if it can possibly be null.
    public int CurrentHighCardInSuit() {
        List<Card> temp = playedCards.FindAll(n => n.handPlayed == currentHand && n.cardInfo.cardSuit == CurrentSUIT());

        temp.OrderByDescending(x => x.cardInfo.cardValue).ToList();

        return temp[0].cardInfo.cardValue;
    }

    public List<Card> GatherCards(bool enhanced, bool isDamage) {
        if(enhanced) 
            return playedCards.FindAll(n => n.ContainsDamage && n.CURRENTOWNER.isPlayer == isDamage).ToList();
        else
            return playedCards.FindAll(n => (n.isHeart() || n.IsQueenOfSpades()) && n.CURRENTOWNER.isPlayer == isDamage).ToList();
    }

    public List<Card> DamageCards(bool enhanced) {
        if(enhanced) 
            return playedCards.FindAll(n => n.ContainsDamage && n.CURRENTOWNER.isPlayer).ToList();           
        else
            return playedCards.FindAll(n => (n.isHeart() || n.IsQueenOfSpades())  && n.CURRENTOWNER.isPlayer).ToList();
    }


    public int CurrentPlayed() {
        return currentCard;
    }

    public List<Card> DamageOrScoring(bool isDamage) {
        return playedCards.FindAll(n => (n.isHeart() || n.IsQueenOfSpades() || n.ContainsDamage) && n.CURRENTOWNER.isPlayer == isDamage).ToList();
    }

    public int DamageEnhancements => playedCards.FindAll(n => n.ContainsDamage).Count;
    public List<Card> HealingCards() {
        return playedCards.FindAll(n => n.ContainsHealing && n.CURRENTOWNER.isPlayer).ToList();
    }

    public List<Card> GoldCards() {
        return playedCards.FindAll(n => n.ContainsGold && n.CURRENTOWNER.isPlayer).ToList();
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
            if(card.cardInfo.cardSuit == Utils.CARDSUIT.HEART && !HaveHeartsBeenPlayed()) {
                currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit != Utils.CARDSUIT.HEART).ForEach(n => n._animator.SetTrigger("Glow"));
                return false;
            } 

            return true;
        }

        if((card.cardInfo.cardSuit == Utils.CARDSUIT.HEART && !HaveHeartsBeenPlayed()) || card.cardInfo.cardSuit != CurrentSUIT())  {

            if(currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit ==  CurrentSUIT()).Count == 0) {
                return true;
            }

            currentTurn._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CurrentSUIT()).ForEach(n => n._animator.SetTrigger("Glow"));

            return false;
        }

        return true;
    }

    public int WonHands() {
        return PlayerCards().Count / 4;
    }

#endregion

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
