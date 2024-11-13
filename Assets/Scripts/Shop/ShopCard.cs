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
    public GameObject sold_extra;
    public TextMeshProUGUI priceLabel;
    private int price;
    public int PRICE { get { return price; }}

    public void Setup(Card card, int price) {
        sold.SetActive(false);
        sold_extra.SetActive(true);
        this.price = price;
        priceLabel.text = price.ToString();
        cardUI.Setup(card, card.enhancements);
    }

    public void Buy() {
        GameManager.instance.shop.ShowComparison(cardUI);
    }

    public void Sold() {
        sold.SetActive(true);
        sold_extra.SetActive(false);
    }

    public void FlipComplete() {}

    private void OnMouseOver() {
        Debug.Log("Show Tool Tip ??");
    }

    private void OnMouseExit() {
        Debug.Log("Hide Tool Tip");
    }
}
