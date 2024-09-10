using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoring 
{
    public float money;
    public float currentScore;

    public bool Buy(int amount) {
        if(amount > money) return false;

        // TODO: Call UI to animate money loss.
        money -= amount;

        return true;
    }

}
