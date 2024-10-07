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
    private int currentPrice;
    public int SELLPRICE { get { return currentPrice; } }

    public void Setup(PlayerTrinket trinket, int price) {
        currentPrice = price;
        this.trinket = trinket.baseTrinket;
        trinketImage.sprite = trinketOutline.sprite = GameManager.instance.spriteHandler.ReturnSprite(trinket.baseTrinket.IDENTIFIER);
        priceLabel.text = currentPrice.ToString();
    }
}
