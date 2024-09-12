using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomBar : MonoBehaviour
{
    public Animator completedHandsAnim;
    public TextMeshProUGUI healthAmt, goldAmt;
    public WonHandUI prefab;
    public RectTransform ScrollViewContent;
    public Image completedBTN;
    private bool _switch = false;

    public void CreateWonHand(WonHand hand) {
        WonHandUI temp = Instantiate(prefab);

        temp.SetupWonHand(hand);

        temp.transform.SetParent(ScrollViewContent);
        temp.transform.SetAsLastSibling();

        ScrollViewContent.sizeDelta = new Vector2(0, ScrollViewContent.childCount * 46.1475f + CONSTS.WONHANDUIMODIFIER);
    }

    public void Setup(float health, float money) {
        
    }

    public void ToggleCompletedHands() {
        
        completedBTN.sprite = Dealer.instance.spriteHandler.HandsBTN[_switch ? 0 : 1];
        _switch = !_switch;
        completedHandsAnim.SetTrigger("Trigger");
    }
}
