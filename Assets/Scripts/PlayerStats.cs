using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats 
{
#region Score
    public int handsWon = 0;
    public int largestScoreGained = 0;
    public int shotTheMoonAmount = 0;
#endregion

#region Damage
    public int healingDone = 0;
#endregion

#region Gold
    public int goldCollected = 0;
    public int highestGoldAmount = 0;
#endregion

    public PlayerStats() {}

#region Utils
    public void HighestGold(int val) {
        if(val > highestGoldAmount) highestGoldAmount = val;
    }

    public void LargestScore(int val) {
        if(largestScoreGained < val) largestScoreGained = val;
    }
#endregion
}
