using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum CARDENHANCEMENT { DAMAGE, HEAL, GOLD, XRAY }
public enum EFFECT { PRE, DURING, POST }
public sealed class Enhancements {
    public CARDENHANCEMENT type;
    public delegate void Effect(Player target); 
    public Effect currentEffect;

    private Enhancements(CARDENHANCEMENT t, Effect effct) {
        type = t;
        currentEffect = effct;
    }

    public Enhancements DAMAGE = new Enhancements(CARDENHANCEMENT.DAMAGE, Damage);
    public Enhancements HEAL = new Enhancements(CARDENHANCEMENT.HEAL, Heal);
    public Enhancements GOLD = new Enhancements(CARDENHANCEMENT.GOLD, Gold);
    public Enhancements XRAY = new Enhancements(CARDENHANCEMENT.XRAY, XRay);

    /*
        IMPLEMENT
        Should these be void?
        Will I need to return.

        Damage and HEal
    */

#region Delegates
    public static void Heal(Player target) { target.health.healingQueue++; }
    public static void Damage(Player target) {  if(target.isPlayer) target.health.damageQueue++;
                                                else  GameManager.instance.dealer.MAINPLAYER.scoring.scoreQueue++;
                                                     }
    public static void Gold(Player target) { target.scoring.goldQueue++; }
    public static void XRay(Player target) { return; }
#endregion
}