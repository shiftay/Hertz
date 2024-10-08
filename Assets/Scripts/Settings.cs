using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings
{

    public float SFX = 0.5f;
    public float MUSIC = 0.5f;
    public Utils.SPEED gameSpeed = Utils.SPEED.NORMAL;

    public Settings() {}

    /*
        IMPLEMENT

        Volume      
                    > Music
                    > SFX
        
        Gameplay speed
                    > Allow speeding up of animations or CPU Play
                    > Enum { SLOW, NORMAL, FAST }

        IDEA Bindings    
                    > Allow remapping of controller buttons.


        Reset 
                    > Reset "Progress"

        Credits
                    > Loads a page for "Credits"
    */
}
