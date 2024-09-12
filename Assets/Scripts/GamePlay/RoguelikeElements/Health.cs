using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Health {
    public int currentHealth;
    public const int MAXHEALTH = 100;
    public int damageQueue;
    public int healingQueue;   

    public Health() {
        currentHealth = MAXHEALTH;
        damageQueue = healingQueue = 0;
    }

}
