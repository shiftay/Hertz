using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{   

    public static GameManager instance;
    public Dealer dealer;
    public UIHandler handlerUI;
    public SpriteHandler spriteHandler;
    public Shop shop;
    public IO inputOutput;

#region Animator Callback
    private bool playing;
    public bool animPlaying => playing;
    public void AnimationComplete() {
        playing = false;
    }

    public void ResetAnim() { playing = true; }
#endregion

    private Player player;
    public Player MAINPLAYER { get { return player; } }

    private void Awake() {
        instance = this;

        if(checkLoad) player = inputOutput.ReadData();
        else          player = new Player();
    }

#region DEBUG
    [Header("Debug")]
    public bool checkLoad;

    [Button("Print Player")]
    public void Print() { Debug.Log(player); }
#endregion
}
