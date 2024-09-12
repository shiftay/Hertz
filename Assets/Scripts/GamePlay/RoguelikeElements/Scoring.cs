using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring 
{
    public int currentGold;
    public int goldQueue;
    public float currentScore;

    public float scoreQueue;
    public float multiplierQueue;



    public bool Buy(int amount) {
        if(amount > currentGold) return false;

        // TODO: Call UI to animate money loss.
        currentGold -= amount;

        return true;
    }

}
