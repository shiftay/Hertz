using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrinket : MonoBehaviour
{
    public TrinketUI trinket;
    public PlayerTrinket playerTrinket;
    public GameObject extras;
    public GameObject sold;

    public void Setup() {
        sold.SetActive(false);
        extras.SetActive(true);
        Trinket temp = Trinket.ReturnTrinket();

        while(  GameManager.instance.dealer.MAINPLAYER.HasTrinket(temp.IDENTIFIER)
                || GameManager.instance.shop.ContainsTrinket(temp)) temp = Trinket.ReturnTrinket();

        TRINKETMOD mod = PlayerTrinket.ReturnMod();

        playerTrinket = new PlayerTrinket(temp, mod);

        trinket.Setup(playerTrinket, playerTrinket.sellValue);
    }

    public bool Compare(Trinket t) {
        if(playerTrinket == null) return false;

        return playerTrinket.baseTrinket == t;
    } 
        

    public void Clicked() {
        GameManager.instance.shop.ShowButton(this);
    }

    public void Buy() {
        GameManager.instance.shop.BuyTrinket(this);
    }

    public void Bought() {
        extras.SetActive(false);
        sold.SetActive(true);
    }
}
