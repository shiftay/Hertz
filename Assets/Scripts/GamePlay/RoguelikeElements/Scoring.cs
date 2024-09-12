using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scoring 
{
    public int currentGold;
    public int goldQueue;
    public float currentScore;

    public float scoreQueue;
    public float multiplierQueue;

    public Scoring() {
        currentGold = goldQueue = 0;

        scoreQueue = multiplierQueue = 0;

        currentScore = 0;
    }

    public bool Buy(int amount) {
        if(amount > currentGold) return false;

        // TODO: Call UI to animate money loss.
        currentGold -= amount;

        return true;
    }

}
