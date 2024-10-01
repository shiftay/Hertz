using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TRINKETMOD { BASE = 100, GOLD = 20, /*PLATINUM,*/ DIAMOND = 5 }

public class PlayerTrinket 
{
    public Trinket baseTrinket;
    public int sellValue;

    public PlayerTrinket(Trinket trinket, TRINKETMOD mod) { baseTrinket = trinket; this.mod = mod; sellValue = ScaleValue(trinket.VALUE); }

#region Modifier
    /*
        TODO Modifier

        Base        - Nothing

        Golden      - 2 Gold at end of Round

        Platinum    - ???
    
        Diamond     - 5x Multiplier to score
    */
    
    public TRINKETMOD mod;

    public static TRINKETMOD ReturnMod() {
        int randomVal = Random.Range(0, (int)TRINKETMOD.BASE);

        if(randomVal < (int)TRINKETMOD.DIAMOND) return TRINKETMOD.DIAMOND;
        else if(randomVal < (int)TRINKETMOD.GOLD) return TRINKETMOD.GOLD;
        else return TRINKETMOD.BASE;
    }

    private int ScaleValue(int value) {
        switch(mod) {
            case TRINKETMOD.BASE:
                return value;
            case TRINKETMOD.GOLD:
                return (int)(value * 1.5f);
            case TRINKETMOD.DIAMOND:
                return value * 2;
        }

        return -1;
    }
#endregion
}
