using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopBar : MonoBehaviour
{
    public TextMeshProUGUI scoreLabel;
    public Transform trinketParent;

    public void SetPlayerVals(Player p) {
        scoreLabel.text = p.scoring.currentScore.ToString();
    }
}
