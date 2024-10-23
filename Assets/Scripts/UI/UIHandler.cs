using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;



public class UIHandler : MonoBehaviour
{
    private Utils.GAMEPLAYSTATES _currentState;
    public Utils.GAMEPLAYSTATES CURRENTSTATE { get { return _currentState; } }
    private Utils.GAMEPLAYSTATES _previousState;
    public Animator stateAnimator;
    public BottomBar bottomBar;
    public TopBar topBar;
    public RoundEnd roundEnd;
    public CardTransition cardTransition; 
    public MoonTransition shotTheMoon;

    [Header("Label Anims")]
    public Animator score;
    public Animator gold, health;

    public delegate void Callback();

    public Animator movableHolder;


    private void Awake() {
        _currentState = Utils.GAMEPLAYSTATES.MainMenu;
    }

    public void SetState(Utils.GAMEPLAYSTATES state, Action callback) {
        // TODO: Settings need to run into a different set of logic, so that it stores previous state. Since Settings will overlay ontop of currentState;
        
        if(state == Utils.GAMEPLAYSTATES.Gameplay && _currentState == Utils.GAMEPLAYSTATES.MainMenu)
            movableHolder.SetTrigger("Gameplay");
        else if(state == Utils.GAMEPLAYSTATES.MainMenu && _currentState == Utils.GAMEPLAYSTATES.Settings)
            movableHolder.SetTrigger("PlayToMenu");
        else if(state == Utils.GAMEPLAYSTATES.GameOver && _currentState == Utils.GAMEPLAYSTATES.RoundEnd)
            movableHolder.SetTrigger("GameOver");
        else if(state == Utils.GAMEPLAYSTATES.MainMenu && _currentState == Utils.GAMEPLAYSTATES.GameOver)
            movableHolder.SetTrigger("OverToMenu");

        /* 
            Current state : Extra Actions
            Main Menu -> movable to gameplay
            Gameplay -> movable to game over
            Settings -> movable to main menu (Abandon run)
        */


        StartCoroutine(ChangeState(state, callback));
    }


    private IEnumerator ChangeState(Utils.GAMEPLAYSTATES state, Action callback) {
        GameManager.instance.ResetAnim();
        GameManager.instance.handlerUI.cardTransition.RandomizeAndShow();
        // Wait for Anim
        yield return new WaitUntil(() => !GameManager.instance.animPlaying);

        _currentState = state;
        stateAnimator.SetTrigger(state.ToString());

        if(callback != null) callback();

        GameManager.instance.handlerUI.cardTransition.Remove();
    }

    public void UpdateValues(Player p) {
        bottomBar.SetPlayerVals(p);
        topBar.SetPlayerVals(p);
    }

    public void UpdateGold(Player p) {
        bottomBar.SetPlayerVals(p);
        // gold.SetTrigger("Pop");
    }

    public void UpdateTrinkets(Player p) {
        topBar.SetupTrinkets(p);
    }

    public void UpdateHealth(Player p) {
        bottomBar.SetPlayerVals(p);
        // health.SetTrigger("Pop");
    }

    public void ShotTheMoon() {}
    
}
