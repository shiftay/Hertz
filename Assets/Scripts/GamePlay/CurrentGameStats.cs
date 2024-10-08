using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CurrentGameStats {
    /*
        Used to keep current game stats and trinkets and deck, so that the player can potentially reload into it.
    */

    public Deck deck;
    public Scoring scoring;
    public Health health;
    public List<PlayerTrinket> trinkets;
    public Token token;


    public CurrentGameStats() {
        trinkets = new List<PlayerTrinket>();
        scoring = new Scoring();
        health = new Health();
        deck = new Deck();
    }
}
