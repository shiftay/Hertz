using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    
    public List<BaseCardSprites> baseSprites;
    public List<BaseCardSprites> wonHandSprites;
    public List<Sprite> cardBacks;

    public List<Sprite> playerIcons;

    public List<Sprite> HandsBTN;

    private int _currentBack;
    private void Awake() {
        _currentBack = Random.Range(0, cardBacks.Count);
    }

    public Sprite CardBack() { return cardBacks[_currentBack]; }

    public Sprite Icon(bool isPlayer) { return playerIcons[isPlayer ? (int)CONSTS.ICON.PLAYER : (int)CONSTS.ICON.CPU]; }

    public Sprite FindCard(CONSTS.CARDSUIT suit, int cardValue) {
        return baseSprites.Find(n => n.suit == suit).cardSprites[cardValue - CONSTS.CARDVALUEMODIFIER];
    }

    public Sprite WonHandCard(CONSTS.CARDSUIT suit, int cardValue) {
        return wonHandSprites.Find(n => n.suit == suit).cardSprites[cardValue - CONSTS.CARDVALUEMODIFIER];
    }
}

[System.Serializable]
public class BaseCardSprites {
    public CONSTS.CARDSUIT suit;
    public List<Sprite> cardSprites;
}