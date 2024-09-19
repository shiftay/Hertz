using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }

public sealed class Enhancements {
    public Utils.CARDENHANCEMENT type;
    public delegate void Effect(Player target); 
    public Effect currentEffect;

    private Enhancements(Utils.CARDENHANCEMENT t, Effect effct) {
        type = t;
        currentEffect = effct;
    }

    public Enhancements DAMAGE = new Enhancements(Utils.CARDENHANCEMENT.DAMAGE, null);
    public Enhancements HEAL = new Enhancements(Utils.CARDENHANCEMENT.HEAL, null);
    public Enhancements GOLD = new Enhancements(Utils.CARDENHANCEMENT.GOLD, null);
    public Enhancements XRAY = new Enhancements(Utils.CARDENHANCEMENT.XRAY, null);

    /*
        IMPLEMENT
        Should these be void?
        Will I need to return.

        Damage and HEal
    */

#region Delegates

#endregion
}