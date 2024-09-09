using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public class HandController : MonoBehaviour
{
    public Player player;

    private float _startingPointX, _endPointX;
    private float _startingPointY, _endPointY;
    public float _currentXVal, _currentYVal;
    public float _signOfMiddle;
    private Vector3 _rotatePointA, _rotatePointB;
    private int currentChildAmount, lastUpdateChildAmount;
    private float _rotationMod;
    public BoxCollider2D boxCollider2D; 
    public CONSTS.AXIS currentAXIS;

    private bool shootForTheMoon;




    private void Awake() {
        lastUpdateChildAmount = currentChildAmount = 0;
    }


#region Organize Hand
     // Update is called once per frame
    void Update()
    {
        currentChildAmount = transform.childCount;
        if(currentChildAmount != lastUpdateChildAmount) OrganizeHand();
    }


    private Transform _currentChild;

    private void OrganizeHand() {
        lastUpdateChildAmount = transform.childCount;
        UpdateAxis();
        Sort();

        for(int i = 0; i < transform.childCount; i++) {
            _currentChild = transform.GetChild(i);
            _currentChild.position = ReturnPosition(i);
            _currentChild.rotation = Quaternion.Euler(new Vector3(0, 0, _signOfMiddle * GetVectorInternalAngle(_rotatePointA, _currentChild.position,  _rotatePointB) - _rotationMod));
            _currentChild.position = new Vector3(_currentChild.position.x, _currentChild.position.y, (i * -1) - 1);
        }
    }

    private Vector3 ReturnPosition(int i) {
        float midVal = (transform.childCount - 1) / 2.0f;

        if(midVal <= 0) return transform.position;
        
        _signOfMiddle = Mathf.Sign(midVal - (float)i);
        float distanceFromMid = midVal - Mathf.Abs(midVal - (float)i);

        if(distanceFromMid == midVal) distanceFromMid--;

        switch(currentAXIS) {
            case CONSTS.AXIS.SOUTH:
            case CONSTS.AXIS.NORTH:
                return new Vector3(Mathf.Lerp(_startingPointX, _endPointX, (float)i / ((float)transform.childCount - 1)), Mathf.Lerp(_startingPointY, _endPointY, distanceFromMid / midVal));
            case CONSTS.AXIS.WEST:
            case CONSTS.AXIS.EAST:
                return new Vector3(Mathf.Lerp(_startingPointX, _endPointX, distanceFromMid / midVal), Mathf.Lerp(_startingPointY, _endPointY, (float)i / ((float)transform.childCount - 1)));
            default:
                return Vector3.zero;
        }
    }

    private float GetVectorInternalAngle(Vector3 a, Vector3 b, Vector3 c) {
        return Vector3.Angle(b-a, c-a);
    }
    
    [Button("Sort")]
    public void Sort() {
        List<CardGO> children = transform.GetComponentsInChildren<CardGO>().ToList<CardGO>();

        children = children.OrderBy(x => x._currentCard.cardInfo.cardSuit).ThenBy(x => x._currentCard.cardInfo.cardValue).ToList();

        for(int i = 0; i < children.Count; i++) {
            children[i].transform.SetSiblingIndex(i);
            // children[i]._currentSprite.sortingOrder = i+1;
        }
    }

    public void UpdateAxis() {
        _currentXVal = Mathf.Lerp(0, CONSTS.HANDLINEVALUEX, transform.childCount / CONSTS.HANDSIZE);
        _currentYVal = Mathf.Lerp(0, CONSTS.HANDLINEVALUEY, transform.childCount / CONSTS.HANDSIZE);

        switch(currentAXIS) {
            case CONSTS.AXIS.SOUTH: // Player
                _rotationMod = 0;
                _startingPointY = transform.position.y;
                _endPointY = transform.position.y + _currentYVal;
                _startingPointX = transform.position.x - _currentXVal;
                _endPointX = transform.position.x + _currentXVal;
                _rotatePointA = new Vector3(transform.position.x, transform.position.y - CONSTS.ROTATEVALY, transform.position.z);
                _rotatePointB = new Vector3(transform.position.x, transform.position.y + CONSTS.HANDLINEVALUEY, transform.position.z);
                break;

            case CONSTS.AXIS.EAST:
                _rotationMod = 90;
                _startingPointY = transform.position.y - _currentXVal;
                _endPointY = transform.position.y + _currentXVal;
                _startingPointX = transform.position.x;
                _endPointX = transform.position.x - _currentYVal;
                _rotatePointA = new Vector3(transform.position.x + CONSTS.ROTATEVALY, transform.position.y , transform.position.z);
                _rotatePointB = new Vector3(transform.position.x - CONSTS.HANDLINEVALUEY, transform.position.y, transform.position.z);
                break;

            case CONSTS.AXIS.NORTH:
                _rotationMod = 0;
                _startingPointY = transform.position.y;
                _endPointY = transform.position.y - _currentYVal;
                _startingPointX = transform.position.x + _currentXVal;
                _endPointX = transform.position.x - _currentXVal;
                _rotatePointA = new Vector3(transform.position.x, transform.position.y + CONSTS.ROTATEVALY, transform.position.z);
                _rotatePointB = new Vector3(transform.position.x, transform.position.y - CONSTS.HANDLINEVALUEY, transform.position.z);
                break;

            case CONSTS.AXIS.WEST:
                _rotationMod = 90;
                _startingPointY = transform.position.y + _currentXVal;
                _endPointY = transform.position.y - _currentXVal;
                _startingPointX = transform.position.x;
                _endPointX = transform.position.x  + _currentYVal;
                _rotatePointA = new Vector3(transform.position.x - CONSTS.ROTATEVALY, transform.position.y , transform.position.z);
                _rotatePointB = new Vector3(transform.position.x + CONSTS.HANDLINEVALUEY, transform.position.y, transform.position.z);
                break;
        }
    }
#endregion

#region Gameplay Logic
    public void SetForTheMoon() {
        float temp = 0.0f;
        player._currentHand.cards.ForEach(n => temp += n._currentCard.cardInfo.cardValue);

        shootForTheMoon = temp / 12.0f > 8;
    }


    public CardGO PlayCard() {
        /*
        "Rules"
            > Players must follow the suit of the card played if at all possible.
                > If player picks up a card that is not playable, snap it back to starting spot in hand and play animation on playable cards.
            > High card wins the round.
                > Eventually will have to put more logic forth
                > The goal isn't necessarily to always win the round
            > Collection of Hearts are inherently bad.
                > Hearts are points against you. 
                > Queen of Spades is a significant amount against you.
            > Hearts can not be the starting suit played
                > Only after a heart has been dumped in a non heart started suit, can a heart be the starting suit.
            > IF a player is able to collect all of the hearts and the Queen of Spades they are rewarded.
         */

        /* 
            Current suit of the hand?
                If I dont have any > Play highcard unless I'm considering "Shooting the moon"

            Has a heart been played?

        
        */
        List<CardGO> playableCards = new List<CardGO>();

        CONSTS.CARDSUIT CurrentSuit = Dealer.instance.CurrentSUIT();
        if (CurrentSuit == CONSTS.CARDSUIT.NULL) // WE ARE THE CURRENT DEALER
        {          
            if(shootForTheMoon) {           
                /*
                    Are we shooting the moon?
                        > Play highest card for w/e suit.
                */
                playableCards = player._currentHand.cards.OrderByDescending(x => x._currentCard.cardInfo.cardValue).ToList();
                if(!Dealer.instance.HaveHeartsBeenPlayed() && playableCards[0]._currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART) {
                    throw new Exception();
                } else {
                    return playableCards[0];
                }
                
            } else {
                /*
                    Play out a card in the suit we don't have a lot of.

                    TODO: Doesn't work if current amount = 0.
                */

                if(Dealer.instance.HaveHeartsBeenPlayed()) {
                    if(CoinFlip()) {
                        if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART).Count > 0) {
                            if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART)[0]._currentCard.cardInfo.cardValue < 7) return player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART)[0];
                            else return DealerLowCard();
                        } else {
                            return DealerLowCard();
                        }
                    } else {
                        return DealerLowCard();
                    }
                } else {
                    return DealerLowCard();
                }
            }

        } 
        else if(player._currentHand.cards.FindAll(x => x._currentCard.cardInfo.cardSuit == CurrentSuit).Count > 0)  // We have a card matching the current suit.
        {
            playableCards = player._currentHand.cards.FindAll(x => x._currentCard.cardInfo.cardSuit == CurrentSuit).OrderBy(x => x._currentCard.cardInfo.cardValue).ToList();
            /*
                Are we shooting the moon?
                    > Can we win hand? Win it.
                    > If not? Dump lowest.
            */
            if(shootForTheMoon) {
                if(playableCards.FindAll(n => n._currentCard.cardInfo.cardValue > Dealer.instance.CurrentHighCardInSuit()).Count > 0) { // Attempt to win // TODO: Make sure it's our highest.
                    return playableCards.FindAll(n => n._currentCard.cardInfo.cardValue > Dealer.instance.CurrentHighCardInSuit())[0];
                } else {
                    return playableCards[0]; // Play lowest of the suit.
                }

            } else {   
            /*    
                Try to play under the current highest.
            */

                if(Dealer.instance.CurrentPlayed() > 2 && !Dealer.instance.HeartInActiveHand()) {
                    return ReturnHighCardSUITINCLUSIVE(Dealer.instance.CurrentSUIT());
                } else if (Dealer.instance.CurrentPlayed() > 2 && Dealer.instance.HeartInActiveHand()) {
                    if(playableCards[0]._currentCard.cardInfo.cardValue > Dealer.instance.CurrentHighCardInSuit()) {
                        return ReturnHighCardSUITINCLUSIVE(Dealer.instance.CurrentSUIT());
                    }
                }

                if(playableCards.FindAll(x => x._currentCard.cardInfo.cardValue < Dealer.instance.CurrentHighCardInSuit()).Count > 0)
                    return playableCards.FindAll(x => x._currentCard.cardInfo.cardValue < Dealer.instance.CurrentHighCardInSuit()).OrderByDescending(n => n._currentCard.cardInfo.cardValue).ToList()[0];
                else
                    return playableCards[0]; // Our lowest of Suit
            }
        } 
        else // WE DO NOT HAVE A CARD MATCHING THE SUIT 
        {
            /*
                Are we shooting the moon? > dump low cards
                Otherwise > dump high cards or hearts.
            
            */
            if(shootForTheMoon) {
                playableCards = player._currentHand.cards.OrderBy(x => x._currentCard.cardInfo.cardValue).ToList();
                int index = playableCards.FindIndex(n => n._currentCard.cardInfo.cardSuit != CONSTS.CARDSUIT.HEART);
                return playableCards[index];
            } else {


                if(CoinFlip()) {
                    if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == CONSTS.CARDSUIT.HEART).Count > 0)
                        return ReturnHighCardSUITINCLUSIVE(CONSTS.CARDSUIT.HEART);
                    else 
                        return ReturnHighCardSUITEXCLUSIVE(); 
                } else {
                    return ReturnHighCardSUITEXCLUSIVE();         // Play High Card.
                }


            }
            
        }
    }

    private CardGO ReturnHighCardSUITEXCLUSIVE() {
        return player._currentHand.cards.OrderByDescending(x => x._currentCard.cardInfo.cardValue).ToList()[0];
    }

    private CardGO ReturnHighCardSUITINCLUSIVE(CONSTS.CARDSUIT suit) {
        return player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == suit).OrderByDescending(n => n._currentCard.cardInfo.cardValue).ToList()[0];
    }


    private CardGO DealerLowCard() {
        int currentAmt = 13;
        CONSTS.CARDSUIT currentLow = CONSTS.CARDSUIT.NULL;

        for(int i = 0; i < 4; i++) {
            int tempAmt = player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == (CONSTS.CARDSUIT)i).Count;
            if(tempAmt == 0) continue;

            if(tempAmt < currentAmt) {
                if((CONSTS.CARDSUIT)i == CONSTS.CARDSUIT.HEART && !Dealer.instance.HaveHeartsBeenPlayed()) continue;

                currentAmt = tempAmt;
                currentLow = (CONSTS.CARDSUIT)i;
            }
        }


        return player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == currentLow)[0];

    }


#endregion

    public bool CoinFlip() {
        return UnityEngine.Random.Range(0, 2) >= 1;
    }


}
