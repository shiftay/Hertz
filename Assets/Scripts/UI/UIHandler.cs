using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
#region Instance Creation / Awake
    public static UIHandler instance;
    private void Awake() {
        instance = this;
    }
#endregion

    public BottomBar bottomBar;
    public RoundEnd roundEnd;
}
