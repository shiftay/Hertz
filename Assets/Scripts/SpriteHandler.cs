using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    public List<BaseCardSprites> baseSprites;
    public List<Sprite> cardBacks;

    public Sprite FindCard(CONSTS.CARDSUIT suit, int cardValue) {
        return baseSprites.Find(n => n.suit == suit).cardSprites[cardValue - CONSTS.CARDVALUEMODIFIER];
    }
}

[System.Serializable]
public class BaseCardSprites {
    public CONSTS.CARDSUIT suit;
    public List<Sprite> cardSprites;
}