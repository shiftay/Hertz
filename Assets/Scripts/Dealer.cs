using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    public SpriteHandler spriteHandler;
    public List<CardGO> Deck = new List<CardGO>();
    List<Player> players = new List<Player>();
    public CardGO cardPrefab;
    public List<Transform> dealPositions;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < 4; i++) players.Add(new Player(i % 4 == 3));
        CreateCards();
        Shuffle(Deck);
        Deal();
    }

    [Button("Shuffle")]
    public void ShuffleDeck() {
        Shuffle(Deck);
    }

    public void Deal() {
        for(int i = 0; i < Deck.Count; i++) {
            Player curPlayer =  players[i%4];
            Deck[i].transform.position = dealPositions[i % 4].transform.position;
            Deck[i]._currentCard.CURRENTOWNER = curPlayer;
            curPlayer._currentHand.cards.Add(Deck[i]._currentCard);
            if(!curPlayer.isPlayer) Deck[i]._currentSprite.sprite = spriteHandler.CardBack();
        }
    }


    public void FindCard() {

    }

    private void CreateCards() {
        CardGO current = null;

        for(int i = 0; i <= (int)CONSTS.CARDSUIT.CLUB; i++) {
            for(int j = 2; j <= CONSTS.ACE; j++) {
                current = Instantiate(cardPrefab);
                current._currentCard = new Card(new CardInfo(j, (CONSTS.CARDSUIT)i));
                current._currentSprite.sprite = spriteHandler.FindCard((CONSTS.CARDSUIT)i, j);
                current.name = ((CONSTS.CARDSUIT)i).ToString("D") + " " + j.ToString();
                // current.transform.position = new Vector3(-7.5f + (j - CONSTS.CARDVALUEMODIFIER) * 1.5f, -3.5f + i * 2, 0);
                Deck.Add(current);
            }
        }
    }

    private static System.Random random = new System.Random();
    private static void Shuffle<T>(List<T> array) 
    {
        for (int i = 0; i < array.Count - 1; ++i) 
        {
            int r = random.Next(i, array.Count);
            (array[r], array[i]) = (array[i], array[r]);
        }
    }   
}
