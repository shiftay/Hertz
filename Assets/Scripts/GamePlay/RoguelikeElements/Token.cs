using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TOKENTYPES { Multi, Gold, Score }

[System.Serializable]
public sealed class Token 
{
    /*
        IMPLEMENT
            Things to think of:
                ID for sprite reference. // Can potentially use the TOKENTYPE enum as the id.

        BASELINE

            Type of token
            What does the type do 
            Sell Value / Price

    */

    public TOKENTYPES type;
    public delegate void Effect(Player p);
    public Effect effect;
    public int price;

    private Token(TOKENTYPES t, Effect e, int p) { type = t; effect = e; price = p; }

    public static Token Multi = new Token(TOKENTYPES.Multi, TokenEffects.Multi, 15);
    public static Token Gold = new Token(TOKENTYPES.Gold, TokenEffects.Gold, 12);
    public static Token Score = new Token(TOKENTYPES.Score, TokenEffects.Score, 10);

    public static List<Token> tokens = new List<Token> { Multi, Gold, Score };
    public Token returnToken(TOKENTYPES t) { return tokens.Find(n => n.type == t); }
}

public static class TokenEffects {
    public static void Multi(Player p) { p.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.TOKEN, 2, true)); }
    public static void Gold(Player p) { p.currentGameStats.scoring.goldQueue.Add(new Source(SourceType.TOKEN, 5));}
    public static void Score(Player p) { p.currentGameStats.scoring.scoreQueue.Add(new Source(SourceType.TOKEN, 25)); }
}
