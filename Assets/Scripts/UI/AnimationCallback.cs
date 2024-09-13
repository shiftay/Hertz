using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationCallback : MonoBehaviour
{
    public void AnimCallBack() {
        GameManager.instance.handlerUI.roundEnd.AnimationComplete();
    }
}
