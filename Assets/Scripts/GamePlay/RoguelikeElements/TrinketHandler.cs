using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinketHandler : MonoBehaviour
{
    public List<TrinketIdentifier> trinketIdentifiers;

    public Sprite ReturnSprite(TRINKET t) {
        return trinketIdentifiers.Find(n => n.identifier == t).baseSprite;
    }
}


#region Trinket Identifier
[System.Serializable]
public class TrinketIdentifier {
    public Sprite baseSprite;
    public TRINKET identifier;
}
#endregion