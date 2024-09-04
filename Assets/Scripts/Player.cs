using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
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



