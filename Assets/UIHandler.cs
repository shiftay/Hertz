using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
#region Instance Creation / Awake
    public static UIHandler instance;
    private void Awake() {
        instance = this;
    }
#endregion


#region Won Hand
    public WonHandUI prefab;
    public RectTransform ScrollViewContent;

    public void CreateWonHand(WonHand hand) {
        WonHandUI temp = Instantiate(prefab);

        temp.SetupWonHand(hand);

        temp.transform.SetParent(ScrollViewContent);
        temp.transform.SetAsLastSibling();

        ScrollViewContent.sizeDelta = new Vector2(0, ScrollViewContent.childCount * 46.1475f + CONSTS.WONHANDUIMODIFIER);
        
    }

    [Button("Content Info")]
    public void ContentInfo() {
        
        Debug.Log("Size Delta: " + ScrollViewContent.sizeDelta);
        Debug.Log("Height? " + ScrollViewContent.rect.height);
    }

#endregion
}
