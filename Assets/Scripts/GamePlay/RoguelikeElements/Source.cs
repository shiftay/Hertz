using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UIElements;

public enum SourceType { TRINKET, INTEREST, ENHANCEMENT, ENDOFROUND }
public class Source {
    public SourceType type;
    public int refID; // IDEA Used for Trinkets
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
    public int refID; // IDEA Used for Trinkets
    public bool needsFormat;

    private SourceLabels(SourceType t, string l, int id, bool format) {
        type = t; Label = l; refID = id; needsFormat = format;
    }

    public static SourceLabels Interest = new SourceLabels(SourceType.INTEREST, "INTEREST GAINED", -1, false);
    public static SourceLabels Enhancement = new SourceLabels(SourceType.ENHANCEMENT, "UPGRADED CARDS", -1, true);
    public static SourceLabels EndOfRound = new SourceLabels(SourceType.ENDOFROUND, "FROM ROUND", -1, true);
    public static SourceLabels Trink = new SourceLabels(SourceType.TRINKET, "FROM", -1, false);
  


    // TODO When you add Labels add to the List
    public static List<SourceLabels> sourceLabels = new List<SourceLabels>() { Interest, Enhancement, EndOfRound, Trink };
    public static SourceLabels FindLabel(SourceType t, int id = -1) {
        return sourceLabels.Find(n => n.type == t);
    }

    public static string FormatLabel(SourceType t, string primaryVal, int id = -1) {
        if(FindLabel(t).needsFormat)
            return String.Format("{0} {1}", primaryVal, FindLabel(t).Label);
        else if(t == SourceType.TRINKET) 
            return Trinket.FindTrinket((TRINKET)id).TITLE;
        else
            return FindLabel(t).Label;
    }
}