using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoonTransition : MonoBehaviour
{
    public Animator animator;
    public List<Image> icons;

    public void Setup(bool isPlayer) {
        icons.ForEach(n => n.sprite = GameManager.instance.spriteHandler.Icon(isPlayer));
        Show();
    }

    public void Show() {
        animator.SetTrigger(Utils.DISPLAY);
    }

    public void Reset() {
        animator.SetTrigger(Utils.HIDE);
    }
}
