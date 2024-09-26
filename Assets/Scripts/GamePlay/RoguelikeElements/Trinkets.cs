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

public sealed class Trinket 
{
    private TRINKET identifier;
    public int ID { get { return (int)identifier; } }

    private string title;
    public string TITLE { get { return title; } }

    public delegate void Effect(int id, int value);
    public Effect effect;

    private Trinket(Effect effect, TRINKET id, string name) { this.effect = effect; identifier = id; title = name; }
    // TODO: Come back and redo the id make it easier to understand elsewhere.
    public static Trinket Triage = new Trinket(TrinketEffect.Triage, TRINKET.TRIAGE, "Triage");
    public static Trinket Pennies = new Trinket(TrinketEffect.Pennies, TRINKET.PENNIES, "Pennies");
    public static Trinket Stocks = new Trinket(TrinketEffect.Stocks, TRINKET.STOCKS, "Stocks");
    public static Trinket LotteryTicket = new Trinket(TrinketEffect.LotteryTicket, TRINKET.LOTTERYTICKET, "Lottery Ticket");
    public static Trinket Calculator = new Trinket(TrinketEffect.Calculator, TRINKET.CALCULATOR, "Calculator");
}

public static class TrinketEffect {

    // Triage - Heals the player for every won hand.
    public static void Triage(int id, int value) {
        GameManager.instance.dealer.MAINPLAYER.health.damageQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().Count / 4, id));
    }

    // Pennies - All 2s won count as an extra money.
    public static void Pennies(int id, int value) {
        GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.PlayerCards().FindAll(n => n.cardInfo.cardValue == 2).Count, id));
    }

    // Stocks - Get more interest between rounds. 
    public static void Stocks(int id, int value) {
        GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, 
        GameManager.instance.dealer.MAINPLAYER.scoring.currentGold / 5 > 5 ? 5 : GameManager.instance.dealer.MAINPLAYER.scoring.currentGold / 5, // Basic INTEREST Math.
        id));
    }
    // Lottery Ticket - Chance to get double gold earned at the end of each round.
    public static void LotteryTicket(int id, int value) {
        GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, GameManager.instance.dealer.MAINPLAYER.scoring.currentGold, id));
    } 
    // Player gains score at the end of the round equal to their trinkets sell value.
    public static void Calculator(int id, int value) {
        GameManager.instance.dealer.MAINPLAYER.scoring.goldQueue.Add(new Source(SourceType.TRINKET, value, id));
    }
}

