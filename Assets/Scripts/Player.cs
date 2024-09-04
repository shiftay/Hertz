using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Deck _currentDeck;
    public Hand _currentHand;
    
    public bool isPlayer;

    public Player(bool player) {
        isPlayer = player;
        _currentHand = new Hand();
    }
}



