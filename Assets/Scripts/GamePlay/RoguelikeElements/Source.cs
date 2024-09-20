using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UIElements;

public enum SourceType { TRINKET, INTEREST, ENHANCEMENT, ENDOFROUND }
public class Source {
    public SourceType type;
    public int refID; // Used for Trinket ID
    public int value;
    public List<Card> associatedCards;

    public int VALUE { get { return value; }}

    public Source(SourceType t, int v, List<Card> cards, int id = -1) {
        associatedCards = new List<Card>(cards);
        type = t;
        value = v;
        refID = id;
    }   

    public Source(SourceType t, int v, int id = -1) {
        associatedCards = new List<Card>();
        type = t;
        value = v;
        refID = id;
    }   
}


public sealed class SourceLabels {
    public SourceType type;
    public string Label;
    public int refID;

    private SourceLabels(SourceType t, string l, int id) {
        type = t; Label = l; refID = id;
    }

    public static SourceLabels Interest = new SourceLabels(SourceType.INTEREST, "INTEREST_TITLE", -1);
    public static SourceLabels Enhancement = new SourceLabels(SourceType.ENHANCEMENT, "UPGRADED_CARDS_TITLE", -1);
    public static SourceLabels EndOfRound = new SourceLabels(SourceType.ENDOFROUND, "ROUND_END_TITLE", -1);

    // TODO When you add Labels add to the List
    public static List<SourceLabels> sourceLabels = new List<SourceLabels>() { Interest, Enhancement, EndOfRound };
    public static SourceLabels FindLabel(SourceType t, int id = -1) {
        return sourceLabels.Find(n => n.type == t);
    }
}