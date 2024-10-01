using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketUI : MonoBehaviour
{
    public Animator animator;
    public Image trinketImage, trinketOutline;
    public TextMeshProUGUI priceLabel;
    public Trinket trinket;

    public void Setup(PlayerTrinket trinket) {
        this.trinket = trinket.baseTrinket;
        trinketImage.sprite = trinketOutline.sprite = GameManager.instance.trinketHandler.ReturnSprite(trinket.baseTrinket.IDENTIFIER);
        priceLabel.text = trinket.sellValue.ToString();
    }
}
