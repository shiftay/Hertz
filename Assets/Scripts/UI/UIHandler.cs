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

    public void SetState(Utils.GAMEPLAYSTATES state) {
        // TODO: Store previous state so that we can swap back if the state is Settings.
        _currentState = state;
        stateAnimator.SetTrigger(state.ToString());
    }

    public void UpdateValues(Player p) {
        bottomBar.SetPlayerVals(p);
        topBar.SetPlayerVals(p);
    }

    public void UpdateGold(Player p) {
        bottomBar.SetPlayerVals(p);
        gold.SetTrigger("Pop");
    }

    public void UpdateTrinkets(Player p) {
        topBar.SetupTrinkets(p);
    }

    public void UpdateHealth(Player p) {
        bottomBar.SetPlayerVals(p);
        health.SetTrigger("Pop");
    }

    public void ShotTheMoon() {}
    
}
