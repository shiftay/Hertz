using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Player
{
    public List<Unlockable> unlocks;
    public GameplayChanges gamePlayChanges;
    public PlayerStats playerStats; // TODO Flood throughout code to update stats and to check against stats.
    public Settings settings;
    public CurrentGameStats currentGameStats;


    public void AdjustValue(RoundEndTypes type, int value) {
        switch(type) {
            case RoundEndTypes.Health:
                Debug.Log("Updating Health " + value);
                currentGameStats.health.currentHealth += value;
                playerStats.healingDone += value;
                break;
            case RoundEndTypes.Score:
                currentGameStats.scoring.currentScore += value;
                Debug.Log("Updating Score " + value);
                playerStats.LargestScore(value);
                playerStats.totalScore += value;
                break;
            case RoundEndTypes.Income:
                currentGameStats.scoring.currentGold += value;
                Debug.Log("Updating Income " + value);
                playerStats.goldCollected += value;
                playerStats.HighestGold(value);
                break;
        }
    }

    public bool HasTrinket(TRINKET id) { return currentGameStats.trinkets.FindAll(n=> n.baseTrinket.identifier == id).Count > 0; }

    public void ClearQueues() {
        currentGameStats.health.damageQueue.Clear();
        currentGameStats.scoring.goldQueue.Clear();
        currentGameStats.scoring.scoreQueue.Clear();
    }

    public int Value() {
        int retVal = 0;
        currentGameStats.trinkets.ForEach(n => retVal += n.sellValue);
        return retVal;
    }
    

    public Player() {
        unlocks = new List<Unlockable>();
        gamePlayChanges = new GameplayChanges();
        playerStats = new PlayerStats();
        settings = new Settings();
        currentGameStats = new CurrentGameStats();
    }
}

/* 
    Gameplay Changes used to track bools or values that trinkets can change.
*/
public class GameplayChanges {
    public int maxTrinkets = 3;
    public bool AcesLow = false;
    public bool QueenScoring = false; 
    public bool ShootWithoutQueen = false;

    public void Reset() { ShootWithoutQueen = AcesLow = QueenScoring = false; }
}


/* 
    Roguelike additions:

    Player Health.
        Any points received by the player count as damage and are scored at the end of the entire round.
        The game ends when the player loses all of their currentGameStats.health.
    

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
            > These are passives that allow the player to gain currentGameStats.health, gain more gold, gain more multiplier to their overall score
            > Ability to heal and various other iterations.
            > Trinkets can be sold to open up their slot for a different trinket.

    The player can reroll the shop, but as with most games each reroll makes it cost more and more.

    Unlocks:
    > Upon purchasing certain card types, or trinkets other such enhancements or trinkets will be unlocked for future runs.
    > Trinket slots will unlock with plateaus of scores allowing for even more passive gain.
    
*/