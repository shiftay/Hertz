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

    private Vector3 mousePos;


    void OnMouseDown() {
        if(!_currentCard.CURRENTOWNER.isPlayer || !Dealer.instance.IsPlayerTurn()) return;

        if(!Dealer.instance.IsCardPlayable(_currentCard)) {
            // TODO: Play SFX / Show VFX
            return;
        }

        _startingPosition = transform.position;
        Dealer.instance.StartDrag(this);
    }


    void OnMouseDrag()
    {
        
        if(!_currentCard.CURRENTOWNER.isPlayer || !Dealer.instance.IsPlayerTurn() || !Dealer.instance.IsCardPlayable(_currentCard)) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.z = Dealer.instance.insideBox(mousePos) ? _startingPosition.z : CONSTS.MAXZ;
        transform.position = mousePos;
    }


    void OnMouseUp()
    {
        if(!_currentCard.CURRENTOWNER.isPlayer || !Dealer.instance.IsPlayerTurn() || !Dealer.instance.IsCardPlayable(_currentCard)) return;

        mousePos.z = 0;
        transform.position = Dealer.instance.EndDrag(mousePos);
        // transform.position = _startingPosition;
    }

    public bool IsQueenOfSpades() { return _currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.SPADE && _currentCard.cardInfo.cardValue == 12; }

}
