using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro.EditorUtilities;
using UnityEngine;

public class CardGO : MonoBehaviour
{
    public Card _currentCard;
    public SpriteRenderer _currentSprite;
    public GameObject _glow;
    public Animator _animator;
    public Vector3 _startingPosition;

    void OnMouseOver(){
        if(!_currentCard.CURRENTOWNER.isPlayer || (Dealer.instance._currentSelected == this && !Dealer.instance.IsPlayerTurn())) return;

        Dealer.instance.playerController.CardMouseOver(this);
    }

    void OnMouseDown() {
        if(!_currentCard.CURRENTOWNER.isPlayer || !Dealer.instance.IsPlayerTurn()) return;

        if(!Dealer.instance.IsCardPlayable(_currentCard)) {
            // TODO: Play SFX / Show VFX
            return;
        } else {
            Dealer.instance.Clicked(this);
        }   
    }

    private void OnMouseExit()
    {
        Dealer.instance.playerController.CardMouseExit();
    }
    
    public bool IsQueenOfSpades() { return _currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.SPADE && _currentCard.cardInfo.cardValue == 12; }

}
