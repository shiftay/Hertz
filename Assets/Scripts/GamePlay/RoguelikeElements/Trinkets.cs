using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    Trinket Descriptions:
    IMPLEMENT
        Full Moon - When player shoots the moon they double their total score.
        
        Jacks Wild (SUIT) - Jacks of the Show suit now cause the player to heal. [ Can include Hearts ]
        Aces Low - Aces are now considered low cards.
        
        

    BASELINE
        Triage - Heals the player for every won hand.
        Pennies - All 2s won count as an extra money.
        Stocks - Get more interest between rounds. ( 10 % ) 
        Lottery Ticket - Chance to get double gold earned at the end of each round.
        Calculator - Player gains score at the end of the round equal to their trinkets sell value.
        Coupon - Each shop will have one random free item.
        No Queens Allowed - Queen of Spades doesn't count towards damage of the player (Base Damage Only);
        Eclipse - Allows the player to not have to get the Queen of Spades to shoot the moon.

*/
public enum TRINKET {   FULL_MOON, TRIAGE, ECLIPSE, CALCULATOR, JACKS_WILD_HEARTS, JACKS_WILD_DIAMOND, JACKS_WILD_SPADE, JACKS_WILD_CLUB, ACES_LOW,
                        PENNIES, STOCKS, COUPON, LOTTERYTICKET, QUEEN_SCORING }
// Used to check when to activate the trinket
public enum VALUECHECK { SCORING, SHOP, MOON, PLAY }

public sealed class Trinket 
{
    private TRINKET identifier;
    public int ID { get { return (int)identifier; } }
    public TRINKET IDENTIFIER { get { return identifier; } }

    private int baseValue;
    public int VALUE { get { return baseValue; } }
    private string title;
    public string TITLE { get { return title; } }

    public delegate void Effect(Player p, int id);
    public Effect effect;
    public VALUECHECK check;

    private Trinket(Effect effect, TRINKET id, VALUECHECK check, string name, int value) {  this.effect = effect; identifier = id; title = name; 
                                                                                            this.check = check; baseValue = value; }

    public static Trinket Triage = new Trinket(TrinketEffect.Triage, TRINKET.TRIAGE, VALUECHECK.SCORING, "Triage", 5);
    public static Trinket Pennies = new Trinket(TrinketEffect.Pennies, TRINKET.PENNIES, VALUECHECK.SCORING,"Pennies", 5);
    public static Trinket Stocks = new Trinket(TrinketEffect.Stocks, TRINKET.STOCKS, VALUECHECK.SCORING, "Stocks", 5);
    public static Trinket LotteryTicket = new Trinket(TrinketEffect.LotteryTicket, TRINKET.LOTTERYTICKET, VALUECHECK.SCORING, "Lottery Ticket", 5);
    public static Trinket Calculator = new Trinket(TrinketEffect.Calculator, TRINKET.CALCULATOR, VALUECHECK.SCORING, "Calculator", 5);
    public static Trinket Coupon = new Trinket(TrinketEffect.Coupon, TRINKET.COUPON, VALUECHECK.SHOP, "Coupon", 5);
    public static Trinket AcesLow = new Trinket(TrinketEffect.AcesLow, TRINKET.ACES_LOW, VALUECHECK.PLAY, "Aces Low", 5);
    public static Trinket QueenScoring = new Trinket(TrinketEffect.AcesLow, TRINKET.QUEEN_SCORING, VALUECHECK.PLAY, "No Queens Allowed", 5);
    public static Trinket Eclipse = new Trinket(TrinketEffect.Eclipse, TRINKET.ECLIPSE, VALUECHECK.PLAY, "Eclipse", 5);
    //public static Trinket Default = new Trinket(TrinketEffect.Default, TRINKET.Default, VALUECHECK.Default, "Default", 5);


    public static List<Trinket> trinkets = new List<Trinket>() { Calculator, Triage, Pennies, Stocks, LotteryTicket, Coupon, AcesLow, QueenScoring };
    public static Trinket ReturnTrinket() { return trinkets[Random.Range(0, trinkets.Count)]; }
    public static Trinket FindTrinket(TRINKET id) { return trinkets.Find(n => n.IDENTIFIER == id); }
}

public static class TrinketEffect {
    // Triage - Heals the player for every won hand.
    public static void Triage(Player p, int id) {
        if(GameManager.instance.dealer.PlayerCards().Count / 4 > 0)
            p.health.damageQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().Count / 4, false, id));
    }

    // Pennies - All 2s won count as an extra money.
    public static void Pennies(Player p, int id) {
        if(GameManager.instance.dealer.PlayerCards().FindAll(n => n.cardInfo.cardValue == 2).Count > 0)
           p.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().FindAll(n => n.cardInfo.cardValue == 2).Count, false, id));
    }

    // Stocks - Get more interest between rounds. 
    public static void Stocks(Player p, int id) {
        p.scoring.goldQueue.Add(new Source(SourceType.TRINKET, 
        p.scoring.currentGold / 5 > 5 ? 5 : p.scoring.currentGold / 5, // Basic INTEREST Math.
        false, id));
    }
    // Lottery Ticket - Chance to get double gold earned at the end of each round.
    public static void LotteryTicket(Player p, int id) {
        if(Random.Range(0, 10) < 2)
            p.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.MAINPLAYER.scoring.currentGold, false, id));
    } 
    // Player gains score at the end of the round equal to their trinkets sell value.
    public static void Calculator(Player p, int id) {
        if(p.Value() > 0)
            p.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.MAINPLAYER.Value(), false, id));
    }

    public static void Coupon(Player p, int id) {

    }

    public static void AcesLow(Player p, int id) { p.gamePlayChanges.AcesLow = true; }
    public static void QueenScoring(Player p, int id) { p.gamePlayChanges.QueenScoring = true; }
    public static void Eclipse(Player p, int id) {  p.gamePlayChanges.ShootWithoutQueen = true; }
    //public static void Default(Player p, int id) {}
}