using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   

    public static GameManager instance;
    public Dealer dealer;
    public UIHandler handlerUI;
    public SpriteHandler spriteHandler;
    public Shop shop;

    public bool animPlaying;
    public void AnimationComplete() {
        animPlaying = false;
    }

    public void ResetAnim() { animPlaying = true; }

    private void Awake() {
        instance = this;
    }
}
