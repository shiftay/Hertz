using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro.EditorUtilities;
using UnityEngine;

public class CardGO : MonoBehaviour
{
    public Card _currentCard;
    public SpriteRenderer _currentSprite;
    public Animator _animator;
    public List<EnhancementObjects> enhancementObjects;

    public void Setup(Card card) {
        _currentSprite.maskInteraction = SpriteMaskInteraction.None;
        _currentSprite.sprite = GameManager.instance.spriteHandler.FindCard(card.cardInfo);

        enhancementObjects.ForEach(n => {
            n.Object.SetActive(false);
            n.spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
        });

        card.enhancements.ForEach(n => {
            enhancementObjects.Find(x => x.type == n.type).Activate();
        });

        if(card.enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.DAMAGE).Count > 0) {
            enhancementObjects.ForEach(n => n.MaskInteraction());
            _currentSprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
            
    }


    void OnMouseOver(){
        if(!_currentCard.CURRENTOWNER.isPlayer) return;
        
        if(!GameManager.instance.dealer.CardAttached(this.transform)) return;

        GameManager.instance.dealer.playerController.CardMouseOver(this);
    }

    void OnMouseDown() {
        if(!_currentCard.CURRENTOWNER.isPlayer || !GameManager.instance.dealer.IsPlayerTurn) return;

        if(!GameManager.instance.dealer.IsCardPlayable(_currentCard)) {   
            return;
        } else {
            GameManager.instance.dealer.Clicked(this);
        }   
    }

    private void OnMouseExit()
    {
         GameManager.instance.dealer.playerController.CardMouseExit(this);
    }
}

[System.Serializable]
public class EnhancementObjects {
    public GameObject Object;
    public Utils.CARDENHANCEMENT type;
    public SpriteRenderer spriteRenderer;

    public void Activate() {
        Object.SetActive(true);
    }

    public void MaskInteraction() {
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }
}