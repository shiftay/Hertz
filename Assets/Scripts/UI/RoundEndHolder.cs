using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RoundEndHolder : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI title;
    public Image Border;
    public Transform valuesParent;

    public void CleanUp() {
        for(int i = valuesParent.childCount - 1; i > 0; i--) Destroy(valuesParent.GetChild(i).gameObject);
    }

    public void Setup(Color color) {
        Border.color = color;
        title.color = color;
    }

    public void Init() {
        title.text = "";
    }


}
