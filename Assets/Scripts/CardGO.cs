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

    public Vector3 _startingPosition;


    void OnMouseDown() {
        if(!_currentCard.CURRENTOWNER.isPlayer) return;

        _startingPosition = transform.position;
        Dealer.instance.Selected(this);
        Debug.Log("Pressed: " + _currentCard.cardInfo.cardValue + " of " + _currentCard.cardInfo.cardSuit.ToString());
    }


    void OnMouseDrag()
    {
        if(!_currentCard.CURRENTOWNER.isPlayer) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        mousePos.z = Dealer.instance.insideBox(mousePos) ? _startingPosition.z : CONSTS.MAXZ;
        transform.position = mousePos;
    }


    void OnMouseUp()
    {
        transform.position = _startingPosition;
    }

}
