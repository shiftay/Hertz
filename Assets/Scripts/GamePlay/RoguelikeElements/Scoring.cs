using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Scoring 
{
    public int currentGold;
    // public int goldQueue;
    public List<Source> goldQueue;
    public List<Source> scoreQueue;
    public float currentScore;
    public float multiplierQueue;

    public Scoring() {
        currentGold =  0;
        multiplierQueue = 0;
        currentScore = 0;

        scoreQueue = new List<Source>();
        goldQueue = new List<Source>();
    }

    public bool Buy(int amount) {
        if(amount > currentGold) return false;

        // TODO: Call UI to animate money loss. ??
        currentGold -= amount;

        return true;
    }

}

