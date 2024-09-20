using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;



public class UIHandler : MonoBehaviour
{
    
    public Animator stateAnimator;
    public BottomBar bottomBar;
    public TopBar topBar;
    public RoundEnd roundEnd;
    public CardTransition cardTransition; 


    public void SetState(Utils.GAMEPLAYSTATES state) {
        stateAnimator.SetTrigger(state.ToString());
    }

    public void UpdateValues(Player p) {
        bottomBar.SetPlayerVals(p);
        topBar.SetPlayerVals(p);
    }
    
}
