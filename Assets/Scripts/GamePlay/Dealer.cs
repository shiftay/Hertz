using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class HandPositions {
    public HandController dealPos;
    public Transform coinPos;
    public Transform cardPlayedPos;
}

public class Dealer : MonoBehaviour
{

#region DEBUG
    public bool debug_shortGame;
    public int debug_startingGold;
    public bool debug_shotTheMoon;
#endregion

#region Setup Vars
    public List<CardGO> Deck = new List<CardGO>();
    List<PlayerChecks> players = new List<PlayerChecks>();
    // public Player GameManager.instance.MAINPLAYER { get { return players.Find(n => n.isPlayer); }}
    public CardGO cardPrefab;
    public List<HandPositions> dealPositions;
    public DealerToken dealerCoin;
    public HandController playerController;
#endregion

#region Gameplay Vars
    public CardGO _currentSelected;
    private PlayerChecks currentTurn;
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

        currentTurn = debug_shortGame ? players.Find(n=> n.isPlayer) : players[x];
        dealerCoin.gameObject.SetActive(true);
        dealerCoin.transform.position = dealPositions[x].coinPos.position;

        GameManager.instance.MAINPLAYER.currentGameStats.trinkets.ForEach(n => {
            if(n.baseTrinket.check == VALUECHECK.PLAY) n.baseTrinket.effect(GameManager.instance.MAINPLAYER, n.baseTrinket.ID);
        });

        calculatingScore = false;
        ValidateDeck();
        CreateCards();
        Shuffle(Deck);
        Deal();



        GameManager.instance.handlerUI.UpdateValues(GameManager.instance.MAINPLAYER);
        currentCard = currentHand = 0;
    }

    // IDEA
    //   To be used for Validating that the deck is conformed to any gameplay changes that effect card value.
    public void ValidateDeck() {
        for(int i = 0; i < GameManager.instance.MAINPLAYER.currentGameStats.deck.cards.Count; i++) {
            if(GameManager.instance.MAINPLAYER.gamePlayChanges.AcesLow && GameManager.instance.MAINPLAYER.currentGameStats.deck.cards[i].cardInfo.cardValue == Utils.ACE)
                GameManager.instance.MAINPLAYER.currentGameStats.deck.cards[i].cardInfo.cardValue = 1;
        }
    }

    private void SetupPlayers() {
        foreach(HandPositions handPos in dealPositions) {
            PlayerChecks temp = new PlayerChecks(handPos.dealPos.currentAXIS == Utils.PLAYERAXIS);
            temp.difficulty = (Utils.DIFFICULITIES)UnityEngine.Random.Range(0, 3);
            handPos.dealPos.player = temp;
            players.Add(temp);
        }

        GameManager.instance.MAINPLAYER.currentGameStats.scoring.currentGold = debug_startingGold;
    }

    private void CreateCards() {
        CardGO current = null;

        for(int i = 0; i < GameManager.instance.MAINPLAYER.currentGameStats.deck.cards.Count; i++) {
            current = Instantiate(cardPrefab);


            current._currentCard = new Card(GameManager.instance.MAINPLAYER.currentGameStats.deck.cards[i].cardInfo);
            current.Setup(GameManager.instance.MAINPLAYER.currentGameStats.deck.cards[i]);
            current.name = GameManager.instance.MAINPLAYER.currentGameStats.deck.cards[i].cardInfo.DebugInfo();
            // current.transform.position = new Vector3(-7.5f + (j - CONSTS.CARDVALUEMODIFIER) * 1.5f, -3.5f + i * 2, 0);
            Deck.Add(current);
        }
    }

    public void Deal() {
        for(int i = 0; i < (debug_shortGame ? 4 : Deck.Count); i++) {
            // I % 4 to make sure we deal to 4 different players.
            PlayerChecks curPlayer = players[i % 4];
            Deck[i].transform.position = dealPositions[i % 4].dealPos.transform.position;
            Deck[i].transform.SetParent(dealPositions[i % 4].dealPos.transform);
            Deck[i]._currentCard.CURRENTOWNER = curPlayer;
            curPlayer._currentHand.cards.Add(Deck[i]);
            
            if(!curPlayer.isPlayer && !Deck[i]._currentCard.ContainsXRAY) Deck[i].ComputerOwned();
        }

        _gameStarted = true;
        playTime = UnityEngine.Random.Range(1.25f, 2.5f);
    }


#endregion 

#region Gameplay Loop+
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

        PlayerChecks winnerOfHand = highCard.CURRENTOWNER;
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
            dealerCoin.gameObject.SetActive(false);

#region Scoring  
            int shotTheMoon = ShotTheMoon();

            if(debug_shotTheMoon) shotTheMoon = players.FindIndex(n => n.isPlayer);

            if(shotTheMoon > 0) {
                
                // If player shot the moon we give them default max score, and a multiplier
                // Player gets max gold and a gold multiplier.
                if(IsitPlayer(shotTheMoon)) {
                    GameManager.instance.MAINPLAYER.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.SHOTTHEMOON, Utils.DEFAULTMAXDAMAGE));
                    GameManager.instance.MAINPLAYER.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.SHOTTHEMOON, 3, true));
                    GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.SHOTTHEMOON, 10));
                    GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.SHOTTHEMOON, 2, true));

                    GameManager.instance.MAINPLAYER.playerStats.shotTheMoonAmount++;
                } else {
                    // If CPU shot the moon player takes default max damage
                    if(GameManager.instance.MAINPLAYER.gamePlayChanges.QueenScoring)
                        GameManager.instance.MAINPLAYER.currentGameStats.health.damageQueue.Add(new Source(SourceType.SHOTTHEMOON, Utils.DEFAULTMAXDAMAGE - Utils.DEFAULTQUEENDAMAGE));
                    else
                        GameManager.instance.MAINPLAYER.currentGameStats.health.damageQueue.Add(new Source(SourceType.SHOTTHEMOON, Utils.DEFAULTMAXDAMAGE));
                }

                Debug.Log("Run animation");

                // Will have to show animation, after we update the queue, then 
                GameManager.instance.ResetAnim();
                GameManager.instance.handlerUI.shotTheMoon.Setup(IsitPlayer(shotTheMoon));

                yield return new WaitUntil(() => !GameManager.instance.animPlaying);

                GameManager.instance.handlerUI.shotTheMoon.Reset();
            } else {
                // BASE Scoring
                int MAXDAMAGE = DamageEnhancements + Utils.DEFAULTMAXDAMAGE;
                GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.ENDOFROUND, 
                                                            Utils.ConvertRange(0, MAXDAMAGE, 10, 1, (GameManager.instance.MAINPLAYER.currentGameStats.health.CurrentDamageQueue())) + Mathf.FloorToInt(1.25f * (MAXDAMAGE - Utils.DEFAULTMAXDAMAGE)))); 
                Score(DamageOrScoring(true), true);
                Score(DamageOrScoring(false) , false);
            }


             

            Debug.Log("Animation should be over");

            // INTEREST
            if(GameManager.instance.MAINPLAYER.currentGameStats.scoring.currentGold / 5 > 5) {
                GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.INTEREST, 5));
            } else {
                GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.INTEREST, GameManager.instance.MAINPLAYER.currentGameStats.scoring.currentGold / 5));
            }

            // Gold Cards
            if(GoldCards().Count > 0) GameManager.instance.MAINPLAYER.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.ENHANCEMENT, GoldCards().Count, GoldCards()));

            // Trinkets
            GameManager.instance.MAINPLAYER.currentGameStats.trinkets.ForEach(n => {
                if(n.baseTrinket.check == VALUECHECK.SCORING) n.baseTrinket.effect(GameManager.instance.MAINPLAYER, n.baseTrinket.ID);
            });

            GameManager.instance.MAINPLAYER.playerStats.handsWon += WonHands();
            
            GameManager.instance.ResetAnim();
            GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
            // Wait for Anim
            yield return new WaitUntil(() => !GameManager.instance.animPlaying);

            GameManager.instance.handlerUI.cardTransition.Remove();

            GameManager.instance.handlerUI.roundEnd.Setup(GameManager.instance.MAINPLAYER);
            CleanUp();

#endregion Scoring
        } else {
            calculatingScore = false;
        }
    }

#endregion

#region Utility Callbacks

    public int ShotTheMoon() {
        int count = 0;
        int shotTheMoon = -1;

        for(int i = 0; i < players.Count; i++) {
            count = 0;
            DamageCards(false).ForEach(n => {
                if(n.CURRENTOWNER == players[i]) count++;
            });

            if(players[i].isPlayer && GameManager.instance.MAINPLAYER.gamePlayChanges.ShootWithoutQueen && count == 13) shotTheMoon = i;
            else if(count == 14) shotTheMoon = i;
        }

        return shotTheMoon;
    }

    public bool IsitPlayer(int index) {
        return players[index].isPlayer; 
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

            if(card.IsQueenOfSpades() && !GameManager.instance.MAINPLAYER.gamePlayChanges.QueenScoring) scoreFromBase += 10;

            if(card.ContainsDamage) scoreFromEnhancements += 1;
        }

        UpdateVal(isDamage, scoreFromBase, scoreFromEnhancements);
    }

    public void UpdateVal(bool isDamage, int fromBase, int fromEnhancements) {
        if(isDamage) {
            if(fromBase > 0) GameManager.instance.MAINPLAYER.currentGameStats.health.damageQueue.Add(new Source(SourceType.ENDOFROUND, fromBase * -1, GatherCards(false, true)));
            if(fromEnhancements > 0) GameManager.instance.MAINPLAYER.currentGameStats.health.damageQueue.Add(new Source(SourceType.ENHANCEMENT, fromEnhancements * -1, GatherCards(true, true)));
        } else {
            if(fromBase > 0) GameManager.instance.MAINPLAYER.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.ENDOFROUND, fromBase, GatherCards(false, false)));
            if(fromEnhancements > 0) GameManager.instance.MAINPLAYER.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.ENHANCEMENT, fromEnhancements, GatherCards(true, false)));
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

    public List<Card> DamageOrScoring(bool isPlayer) {
        return playedCards.FindAll(n => (n.isHeart() || n.IsQueenOfSpades() || n.ContainsDamage) && n.CURRENTOWNER.isPlayer == isPlayer).ToList();
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

#region Input
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
#endregion

}