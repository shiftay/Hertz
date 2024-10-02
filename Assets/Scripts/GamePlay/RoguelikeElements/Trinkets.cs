using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    Trinket Descriptions:
    IMPLEMENT
        Full Moon - When player shoots the moon they double their total score.
        Eclipse - Allows the player to not have to get the Queen of Spades to shoot the moon.

        Jacks Wild (SUIT) - Jacks of the Show suit now cause the player to heal. [ Can include Hearts ]
        Aces Low - Aces are now considered low cards.
        Coupon - Each shop will have one random free item.
        
        

    BASELINE
        Triage - Heals the player for every won hand.
        Pennies - All 2s won count as an extra money.
        Stocks - Get more interest between rounds. ( 10 % ) 
        Lottery Ticket - Chance to get double gold earned at the end of each round.
        Calculator - Player gains score at the end of the round equal to their trinkets sell value.

*/
public enum TRINKET {   FULL_MOON, TRIAGE, ECLIPSE, CALCULATOR, JACKS_WILD_HEARTS, JACKS_WILD_DIAMOND, JACKS_WILD_SPADE, JACKS_WILD_CLUB, ACES_LOW,
                        PENNIES, STOCKS, COUPON, LOTTERYTICKET }
// Used to check when to activate the trinket
public enum VALUECHECK { SCORING, SHOP, MOON }

//TODO Add a CATEGORY to the trinket to track type of trinket { Healing, Scoring, Income, Shop, etc.. }
public sealed class Trinket 
{
    private TRINKET identifier;
    public int ID { get { return (int)identifier; } }
    public TRINKET IDENTIFIER { get { return identifier; } }

    private int baseValue;
    public int VALUE { get { return baseValue; } }
    private string title;
    public string TITLE { get { return title; } }

    public delegate void Effect(int id);
    public Effect effect;
    public VALUECHECK check;

    private Trinket(Effect effect, TRINKET id, VALUECHECK check, string name, int value) {  this.effect = effect; identifier = id; title = name; 
                                                                                            this.check = check; baseValue = value; }

    public static Trinket Triage = new Trinket(TrinketEffect.Triage, TRINKET.TRIAGE, VALUECHECK.SCORING, "Triage", 5);
    public static Trinket Pennies = new Trinket(TrinketEffect.Pennies, TRINKET.PENNIES, VALUECHECK.SCORING,"Pennies", 5);
    public static Trinket Stocks = new Trinket(TrinketEffect.Stocks, TRINKET.STOCKS, VALUECHECK.SCORING, "Stocks", 5);
    public static Trinket LotteryTicket = new Trinket(TrinketEffect.LotteryTicket, TRINKET.LOTTERYTICKET, VALUECHECK.SCORING, "Lottery Ticket", 5);
    public static Trinket Calculator = new Trinket(TrinketEffect.Calculator, TRINKET.CALCULATOR, VALUECHECK.SCORING, "Calculator", 5);

    
    public static List<Trinket> trinkets = new List<Trinket>() { Calculator, Triage, Pennies, Stocks, LotteryTicket };
    public static Trinket ReturnTrinket() { return trinkets[Random.Range(0, trinkets.Count)]; }
    public static Trinket FindTrinket(TRINKET id) { return trinkets.Find(n => n.IDENTIFIER == id); }
}

public static class TrinketEffect {
    // Triage - Heals the player for every won hand.
    public static void Triage(int id) {
        if(GameManager.instance.dealer.PlayerCards().Count / 4 > 0)
            GameManager.instance.dealer.MAINPLAYER.health.damageQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().Count / 4, id));
    }

    // Pennies - All 2s won count as an extra money.
    public static void Pennies(int id) {
        if(GameManager.instance.dealer.PlayerCards().FindAll(n => n.cardInfo.cardValue == 2).Count > 0)
            GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().FindAll(n => n.cardInfo.cardValue == 2).Count, id));
    }

    // Stocks - Get more interest between rounds. 
    public static void Stocks(int id) {
        GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, 
        GameManager.instance.dealer.MAINPLAYER.scoring.currentGold / 5 > 5 ? 5 : GameManager.instance.dealer.MAINPLAYER.scoring.currentGold / 5, // Basic INTEREST Math.
        id));
    }
    // Lottery Ticket - Chance to get double gold earned at the end of each round.
    public static void LotteryTicket(int id) {
        if(Random.Range(0, 10) < 2)
            GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.MAINPLAYER.scoring.currentGold, id));
    } 
    // Player gains score at the end of the round equal to their trinkets sell value.
    public static void Calculator(int id) {
        if(GameManager.instance.dealer.MAINPLAYER.Value() > 0)
            GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.MAINPLAYER.Value(), id));
    }
}