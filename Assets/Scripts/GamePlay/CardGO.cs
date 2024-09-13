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


    // BUG      Player sometimes drops card infront of their cards 
    //          Occurs because Dealer is trying to organize card when it shouldn't be.
    // ISSUE    Doesn't seem to occur anymore, However the fix in MouseOver causes an  
    

    void OnMouseOver(){
        if(!_currentCard.CURRENTOWNER.isPlayer) return;
        if(GameManager.instance.dealer._currentSelected == this) return;

        GameManager.instance.dealer.playerController.CardMouseOver(this);
    }

    void OnMouseDown() {
        if(!_currentCard.CURRENTOWNER.isPlayer || !GameManager.instance.dealer.IsPlayerTurn()) return;

        if(! GameManager.instance.dealer.IsCardPlayable(_currentCard)) {
            // TODO: Play SFX / Show VFX
            return;
        } else {
            GameManager.instance.dealer.Clicked(this);
        }   
    }

    private void OnMouseExit()
    {
        //  GameManager.instance.dealer.playerController.CardMouseExit();
    }
    
    

}
