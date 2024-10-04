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

    public Sprite Icon(bool isPlayer) { return playerIcons[isPlayer ? (int)Utils.ICON.PLAYER : (int)Utils.ICON.CPU]; }

    public Sprite FindCard(Utils.CARDSUIT suit, int cardValue) {
        return baseSprites.Find(n => n.suit == suit).cardSprites[ReturnValue(cardValue) - Utils.CARDVALUEMODIFIER];
    }

    public Sprite FindCard(CardInfo info) {
        return baseSprites.Find(n => n.suit == info.cardSuit).cardSprites[ReturnValue(info.cardValue) - Utils.CARDVALUEMODIFIER];
    }

    public Sprite WonHandCard(Utils.CARDSUIT suit, int cardValue) {
        return wonHandSprites.Find(n => n.suit == suit).cardSprites[ReturnValue(cardValue) - Utils.CARDVALUEMODIFIER];
    }
    public Sprite RandomCard() {
        return baseSprites.Find(n => n.suit == Utils.CARDSUIT.HEART).cardSprites[Random.Range(0, 13)];
    }

    public int ReturnValue(int value) {
        return (value == 1) ? Utils.ACE : value;
    }

}

[System.Serializable]
public class BaseCardSprites {
    public Utils.CARDSUIT suit;
    public List<Sprite> cardSprites;
}