using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class CardToolTip : MonoBehaviour
{
    public List<EnhancementUI> enhancementUIs;
    public List<EnhancementTag> enhancementTags;
    public TextMeshProUGUI rarity;

    public void Setup(Shop.Rarity rarity, List<Enhancements> enhancements) {
        this.rarity.text = rarity.ToString();

        enhancementTags.ForEach(n => n.mainObj.SetActive(false));
        // for(int i = 0; i < enh)
    }
}

[System.Serializable]
public struct EnhancementUI {
    public Color color;
    public string Title;
    public Utils.CARDENHANCEMENT type;
}

[System.Serializable]
public struct EnhancementTag {
    public Image image;
    public TextMeshProUGUI label;
    public GameObject mainObj;
}