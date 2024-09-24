using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    public CardUI cardUI;
    public Animator animator;
    public GameObject sold;
    public TextMeshProUGUI priceLabel;
    private int price;
    public int PRICE { get { return price; }}

    public void Setup(Card card, int price) {
        sold.SetActive(false);
        this.price = price;
        priceLabel.text = price.ToString();
        cardUI.Setup(card, card.enhancements);
    }

    public void Buy() {
        GameManager.instance.shop.ShowComparison(cardUI);
    }
}
