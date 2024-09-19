using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldIncome : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI value;

    public void SetValues(string title, int value) {
        this.title.text = title;
        this.value.text = value.ToString();
    }
}
