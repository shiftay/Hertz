using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image _cardImage;
    public List<EnhancementObjectsUI> enhancementObjects;
    public Mask mask;
    public Card card;

    public void Setup(Card card, List<Enhancements> enhancements) {
        mask.enabled = enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.DAMAGE).Count > 0;
        this.card = card;
        _cardImage.sprite = GameManager.instance.spriteHandler.FindCard(card.cardInfo);

        enhancementObjects.ForEach(n => {
            n.Disable();
        });

        enhancements.ForEach(n => {
            enhancementObjects.Find(x => x.type == n.type).Activate();
        });
    }

    public void SetImage(Sprite sprite) {
        _cardImage.sprite = sprite;
    }

#region Mouse Events
    public void OnPointerEnter(PointerEventData eventData)
    {
        // TODO Show Tooltip
        // Debug.Log("Mouse Over " + card.cardInfo.DebugInfo());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // TODO Hide Tooltip
        // Debug.Log("Mouse Exit " + card.cardInfo.DebugInfo());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Hello");
        GameManager.instance.shop.ShowButton(this);
    }
#endregion
}

[System.Serializable]
public class EnhancementObjectsUI {
    public GameObject Object;
    public Utils.CARDENHANCEMENT type;

    public void Activate() {
        if(Object) 
            Object.SetActive(true);
    }

    public void Disable() {
        if(Object) 
            Object.SetActive(false);
    }
}