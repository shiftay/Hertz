using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image _cardImage;

    public void SetImage(Sprite sprite) {
        _cardImage.sprite = sprite;
    }

}
