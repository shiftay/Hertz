using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopBar : MonoBehaviour
{
    public TextMeshProUGUI scoreLabel;
    public Transform trinketParent;
    public TrinketUI trinketPrefab;
    public List<TrinketUI> currentTrinkets;
    public CanvasGroup trinkets;


    public void SetPlayerVals(Player p) {
        scoreLabel.text = p.currentGameStats.scoring.currentScore.ToString();
        trinkets.alpha = p.currentGameStats.trinkets.Count;
    }

    public void SetupTrinket(PlayerTrinket trinket) {
        if(trinkets.alpha == 0) trinkets.alpha = 1;
        
        TrinketUI temp = Instantiate(trinketPrefab);
        temp.transform.SetParent(trinketParent);
        temp.transform.localScale = Vector3.one;
        temp.Setup(trinket, -1);
        currentTrinkets.Add(temp);
    }

    public void SetupTrinkets(Player p) {
        for(int i = 0; i < p.currentGameStats.trinkets.Count; i++) {
            if(currentTrinkets.FindAll(n => n.trinket == p.currentGameStats.trinkets[i].baseTrinket).Count > 0) continue;

            SetupTrinket(p.currentGameStats.trinkets[i]);
        }
    }

    public void RemoveTrinket() {}
    public void ShowSellValue() {}
}
