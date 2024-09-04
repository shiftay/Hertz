using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CardGO : MonoBehaviour
{
    public Card _currentCard;
    public SpriteRenderer _currentSprite;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button("Width")]
    public void Width() {
        Debug.Log("Width ? " + _currentSprite.bounds.size.x);
    }

    [Button("Height")]
    public void Height() {
        Debug.Log("Height ? " + _currentSprite.bounds.size.y);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
