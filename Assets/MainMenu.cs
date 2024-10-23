using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartRun() {
        GameManager.instance.handlerUI.topBar.trinkets.alpha = 0;
        GameManager.instance.handlerUI.SetState(Utils.GAMEPLAYSTATES.Gameplay, GameManager.instance.dealer.GameSetup);
    }
}
