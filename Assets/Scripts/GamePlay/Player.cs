using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Player
{
    public Deck _currentDeck;
    public Hand _currentHand;
    public bool isPlayer;
    public Utils.DIFFICULITIES difficulty;
    public Health health;
    public Scoring scoring;
    public List<PlayerTrinket> trinkets;
    public List<Unlockable> unlocks;
    public GameplayChanges gamePlayChanges;
    

    public void AdjustValue(RoundEndTypes type, int value) {
        switch(type) {
            case RoundEndTypes.Health:
                health.currentHealth += value;
                break;
            case RoundEndTypes.Score:
                scoring.currentScore += value;
                break;
            case RoundEndTypes.Income:
                scoring.currentGold += value;
                break;
        }
    }

    public bool HasTrinket(TRINKET id) { return trinkets.FindAll(n=> n.baseTrinket.IDENTIFIER == id).Count > 0; }

    public void ClearQueues() {
        health.damageQueue.Clear();
        scoring.goldQueue.Clear();
        scoring.scoreQueue.Clear();
    }

    public int Value() {
        int retVal = 0;
        trinkets.ForEach(n => retVal += n.sellValue);
        return retVal;
    }
    

    public Player(bool player) {
        if(player) { 
            unlocks = new List<Unlockable>();
            _currentDeck = new Deck();
            trinkets = new List<PlayerTrinket>();
            gamePlayChanges = new GameplayChanges();
        }
        health = new Health();
        scoring = new Scoring();
        isPlayer = player;
        _currentHand = new Hand();
    }
}

/* 
    Gameplay Changes used to track bools or values that trinkets can change.
*/
public class GameplayChanges {
    public int maxTrinkets = 3;
    public bool AcesLow = false;
    public bool QueenScoring = false; 

    public void Reset() { AcesLow = QueenScoring = false; }
}


/* 
    Roguelike additions:

    Player Health.
        Any points received by the player count as damage and are scored at the end of the entire round.
        The game ends when the player loses all of their health.
    

    Any points given to the AI count as points the player gains
        A portion of these points will be transfered into "gold"
    
    After each round a shop similar to balatro will appear
    The shop will offer:
        Packs with "enhanced" cards to be swapped into the deck
            > If the player does not want to put the card into the deck, it can be sold for a portion of the purchase price
            > Enhancements include:
                > Gold multipliers for if they player wins the hand with the card.
                > Score multipliers for if the player gets the AI to win the card.
                > Healing if the player wins the card
                > ..etc
            > If a player gets a different enhanced card they can replace the current one and sell it for a portion of it's initial price.
                > Example: Player gets a Gold and Healing mutliplier Ace of Diamonds, they can then sell their current Score Multiplier Ace of Diamonds back to the store.
        Trinkets
            > These are passives that allow the player to gain health, gain more gold, gain more multiplier to their overall score
            > Ability to heal and various other iterations.
            > Trinkets can be sold to open up their slot for a different trinket.

    The player can reroll the shop, but as with most games each reroll makes it cost more and more.

    Unlocks:
    > Upon purchasing certain card types, or trinkets other such enhancements or trinkets will be unlocked for future runs.
    > Trinket slots will unlock with plateaus of scores allowing for even more passive gain.
    
*/