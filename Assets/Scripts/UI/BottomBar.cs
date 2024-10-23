using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomBar : MonoBehaviour
{
    public TextMeshProUGUI healthAmt, goldAmt;
    public Animator lastHand;
    public WonHandUI hand;
    private List<Card> lastHandCards = new List<Card>();
    private bool firstTime = true;

    public void UpdateLastHand(List<Card> hand) {
        lastHandCards = hand;

        if(firstTime) {
            firstTime = false;
            this.hand.SetupWonHand(lastHandCards);
            lastHand.SetTrigger("On");
        } else {
            lastHand.SetTrigger("Flip");
        }
    }

    public void CleanUp() {
        lastHand.SetTrigger("Off");
        firstTime = true;
    }

    public void SetPlayerVals(Player p) {
        healthAmt.text = p.currentGameStats.health.currentHealth.ToString();
        goldAmt.text = p.currentGameStats.scoring.currentGold.ToString();
    }

    public void UpdateHand() {
        hand.SetupWonHand(lastHandCards);
    }

}
