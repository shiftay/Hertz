using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerToken : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Token currentToken;

    public void Setup(Token t) {
        currentToken = t;

        spriteRenderer.sprite = GameManager.instance.spriteHandler.ReturnSprite(t.type);
    }
}
