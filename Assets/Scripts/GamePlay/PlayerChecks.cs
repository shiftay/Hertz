using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecks
{
    public Hand _currentHand;
    public bool isPlayer;
    public Utils.DIFFICULITIES difficulty;

    public PlayerChecks(bool isPlayer) {
        this.isPlayer = isPlayer;

        _currentHand = new Hand();
    }
}
