using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public SpriteHandler spriteHandler;
    public List<CardGO> cardGOs = new List<CardGO>();

    public CardGO cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        CreateCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void CreateCards() {
        CardGO current = null;

        for(int i = 0; i <= (int)CONSTS.CARDSUIT.CLUB; i++) {
            for(int j = 2; j <= CONSTS.ACE; j++) {
                current = Instantiate(cardPrefab);
                current._currentCard = new Card(new CardInfo(j, (CONSTS.CARDSUIT)i));
                current._currentSprite.sprite = spriteHandler.FindCard((CONSTS.CARDSUIT)i, j);
                current.transform.position = new Vector3(-7.5f + (j - CONSTS.CARDVALUEMODIFIER) * 1.5f, -3.5f + i * 2, 0);
                cardGOs.Add(current);
            }
        }
    }
}
