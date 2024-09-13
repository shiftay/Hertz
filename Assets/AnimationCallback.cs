using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallback : MonoBehaviour
{
    public void AnimCallBack() {
        GameManager.instance.handlerUI.roundEnd.AnimationComplete();
    }
}
