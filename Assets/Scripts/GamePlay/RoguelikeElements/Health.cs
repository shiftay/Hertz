using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Health {
    public int currentHealth;
    public const int MAXHEALTH = 100;
    public List<Source> damageQueue;

    public int CurrentDamageQueue() {
        int retVal = 0;
        foreach(Source source in damageQueue) retVal += source.VALUE;
        return retVal;
    }

    public Health() {
        currentHealth = MAXHEALTH;
        damageQueue = new List<Source>();
    }

}
