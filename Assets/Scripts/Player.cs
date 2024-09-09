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

[System.Serializable]
public class WonHand {
    public List<Card> cards = new List<Card>();

    public WonHand(List<Card> hand) { cards = hand; }
}