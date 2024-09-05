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

    public static Dealer instance;

    public CardGO _currentSelected;
    public PlayArea playArea;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        for(int i = 0; i < 4; i++) players.Add(new Player(i % 4 == 3));
        isDragging = false;
        CreateCards();
        Shuffle(Deck);
        Deal();
    }

    [Button("Shuffle")]
    public void ShuffleDeck() {
        Shuffle(Deck);
    }

    [Button("Play A Card")]
    public void PlayCard() {
        CardGO objectToDestroy;
        for(int i =0 ; i < players.Count; i++) {
            objectToDestroy = players[i]._currentHand.cards[0];
            Deck.Remove(objectToDestroy);
            players[i]._currentHand.cards.Remove(objectToDestroy);
            Destroy(objectToDestroy.gameObject);
        }
    }

    public void Deal() {
        for(int i = 0; i < Deck.Count; i++) {
            Player curPlayer = players[i%4];
            Deck[i].transform.position = dealPositions[i % 4].transform.position;
            Deck[i].transform.SetParent(dealPositions[i % 4]);
            Deck[i]._currentCard.CURRENTOWNER = curPlayer;
            curPlayer._currentHand.cards.Add(Deck[i]);
            
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

    public BoxCollider2D playerArea;

    public bool insideBox(Vector3 point) {
        return playerArea.bounds.Contains(point);
    }

    private bool isDragging;

    public bool IsDragging { get { return isDragging; }
                             set {  if(isDragging != value) playArea.animator.SetTrigger("Trigger"); 
                                    isDragging = value; }}

    public void Dragging(CardGO clicked, bool dragging) {
        if(_currentSelected == null) _currentSelected = clicked;

        if(_currentSelected != clicked) {
            _currentSelected = clicked;
        }

        IsDragging = dragging;
    }

    public Vector3 EndDrag(Vector3 currentPos) {
        Debug.Log(currentPos);

        Vector3 retVal = playArea.boxCollider.bounds.Contains(currentPos) ? currentPos : _currentSelected._startingPosition;

        IsDragging = false;
        _currentSelected = null;       

        return retVal;
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
