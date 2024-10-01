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

    public void SetPlayerVals(Player p) {
        scoreLabel.text = p.scoring.currentScore.ToString();
    }

    public void SetupTrinket(PlayerTrinket trinket) {
        TrinketUI temp = Instantiate(trinketPrefab);
        temp.transform.SetParent(trinketParent);
        temp.transform.localScale = Vector3.one;
        temp.Setup(trinket);
        currentTrinkets.Add(temp);
    }

    public void SetupTrinkets(Player p) {
        for(int i = 0; i < p.trinkets.Count; i++) {
            if(currentTrinkets.FindAll(n => n.trinket == p.trinkets[i].baseTrinket).Count > 0) continue;

            SetupTrinket(p.trinkets[i]);
        }
    }

    public void RemoveTrinket() {}
    public void ShowSellValue() {}
}
