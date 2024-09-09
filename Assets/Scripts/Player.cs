using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public Deck _currentDeck;
    public Hand _currentHand;

    public List<WonHand> wonHands;
    
    public bool isPlayer;

    public Player(bool player) {
        isPlayer = player;
        wonHands = new List<WonHand>();
        _currentHand = new Hand();
    }
}


public class WonHand {
    public List<Card> cards = new List<Card>();

    public WonHand(List<Card> hand) { cards = hand; }
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