using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BottomBar : MonoBehaviour
{
    public Animator completedHandsAnim;
    public Animator roundEndAnim;
    public TextMeshProUGUI healthAmt, goldAmt;
    public WonHandUI prefab;
    public RectTransform ScrollViewContent;
    public RectTransform Viewport;
    public Image completedBTN;
    private bool _switch = false;

    public void CreateWonHand(List<Card> hand) {
        WonHandUI temp = Instantiate(prefab);

        temp.SetupWonHand(hand);

        temp.transform.SetParent(ScrollViewContent);
        temp.transform.SetAsLastSibling();
        temp.transform.localScale = Vector3.one;

        ScrollViewContent.sizeDelta = new Vector2(0, ScrollViewContent.childCount * 46.1475f + Utils.WONHANDUIMODIFIER);
    }

    public void CleanUp() {
        List<GameObject> objs = new List<GameObject>();

        for(int i = 0; i < ScrollViewContent.childCount; i++) objs.Add(ScrollViewContent.GetChild(i).gameObject);

        objs.ForEach(n => Destroy(n));
    }

    public void SetPlayerVals(Player p) {
        healthAmt.text = p.health.currentHealth.ToString();
        goldAmt.text = p.scoring.currentGold.ToString();
    }

    public void ToggleCompletedHands() {
        if(ScrollViewContent.childCount < 1) return; 

        completedBTN.sprite =  GameManager.instance.spriteHandler.HandsBTN[_switch ? 0 : 1];
        _switch = !_switch;

        completedHandsAnim.SetTrigger("Trigger" + (ScrollViewContent.childCount < 3 ? ScrollViewContent.childCount : 3));  
    }

}
