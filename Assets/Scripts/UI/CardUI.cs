using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    public Image _cardImage;
    public List<EnhancementObjectsUI> enhancementObjects;
    public Mask mask;

    public void Setup(Sprite sprite, List<Enhancements> enhancements) {
        mask.enabled = enhancements.FindAll(n => n.type == Utils.CARDENHANCEMENT.DAMAGE).Count > 0;
        _cardImage.sprite = sprite;

        enhancementObjects.ForEach(n => {
            n.Disable();
        });

        enhancements.ForEach(n => {
            enhancementObjects.Find(x => x.type == n.type).Activate();
        });
    }

    public void SetImage(Sprite sprite) {
        _cardImage.sprite = sprite;
    }

}

[System.Serializable]
public class EnhancementObjectsUI {
    public GameObject Object;
    public Utils.CARDENHANCEMENT type;

    public void Activate() {
        if(Object) 
            Object.SetActive(true);
    }

    public void Disable() {
        if(Object) 
            Object.SetActive(false);
    }
}