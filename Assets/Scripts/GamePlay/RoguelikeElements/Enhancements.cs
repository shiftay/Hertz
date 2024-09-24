using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }
[System.Serializable]
public sealed class Enhancements {
    public Utils.CARDENHANCEMENT type;
    public delegate void Effect(Player target); 
    public Effect currentEffect;

    private Enhancements(Utils.CARDENHANCEMENT t, Effect effct) {
        type = t;
        currentEffect = effct;
    }

    public static Enhancements DAMAGE = new Enhancements(Utils.CARDENHANCEMENT.DAMAGE, null);
    public static Enhancements HEAL = new Enhancements(Utils.CARDENHANCEMENT.HEAL, null);
    public static Enhancements GOLD = new Enhancements(Utils.CARDENHANCEMENT.GOLD, null);
    public static Enhancements XRAY = new Enhancements(Utils.CARDENHANCEMENT.XRAY, null);

    /*
        IMPLEMENT
        Should these be void?
        Will I need to return.

        Damage and HEal
    */

    public static List<Enhancements> enhancements = new List<Enhancements>() { DAMAGE, HEAL, GOLD/*, XRAY*/ };

    public static Enhancements ReturnRandom() {
        return enhancements[Random.Range(0, enhancements.Count)];
    }

#region Delegates

#endregion
}