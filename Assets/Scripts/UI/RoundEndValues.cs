using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundEndValues : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI value;
    public Image background;

    public void SetValues(string title, int value, Color color, string valueModifier = "") {
        this.title.text = title;
        this.value.text = value.ToString();
        background.color = color;
    }
}
