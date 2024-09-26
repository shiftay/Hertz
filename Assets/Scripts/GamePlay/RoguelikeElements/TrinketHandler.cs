using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinketHandler : MonoBehaviour
{
    public List<TrinketIdentifier> trinketIdentifiers;
}


#region Trinket Identifier
[System.Serializable]
public class TrinketIdentifier {
    public Sprite baseSprite;
    public TRINKET identifier;
}
#endregion