using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
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
    private int currentChildAmount, lastUpdateChildAmount;
    private float _rotationMod;
    public BoxCollider2D boxCollider2D; 
    public Utils.AXIS currentAXIS;

    private bool shootForTheMoon;

    public List<CardGO> children = new List<CardGO>();


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

        children = transform.GetComponentsInChildren<CardGO>().ToList();

        for(int i = 0; i < transform.childCount; i++) {
            _currentChild = transform.GetChild(i);
            _currentChild.position = ReturnPosition(i);
            _currentChild.rotation = Quaternion.Euler(new Vector3(0, 0, _rotationMod));
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
            case Utils.AXIS.SOUTH:
            case Utils.AXIS.NORTH:
                return new Vector3(Mathf.Lerp(_startingPointX, _endPointX, (float)i / ((float)transform.childCount - 1)), transform.position.y);
            case Utils.AXIS.WEST:
            case Utils.AXIS.EAST:
                return new Vector3(transform.position.x, Mathf.Lerp(_startingPointY, _endPointY, (float)i / ((float)transform.childCount - 1)));
            default:
                return Vector3.zero;
        }
    }
    
    public void Sort() {
        List<CardGO> children = transform.GetComponentsInChildren<CardGO>().ToList<CardGO>();

        children = children.OrderBy(x => x._currentCard.cardInfo.cardSuit).ThenBy(x => x._currentCard.cardInfo.cardValue).ToList();

        for(int i = 0; i < children.Count; i++) {
            children[i].transform.SetSiblingIndex(i);
            // children[i]._currentSprite.sortingOrder = i+1;
        }
    }

    public void UpdateAxis() {
        _currentXVal = Mathf.Lerp(0, Utils.HANDLINEVALUEX, transform.childCount / Utils.HANDSIZE);
        _currentYVal = Mathf.Lerp(0, Utils.HANDLINEVALUEY, transform.childCount / Utils.HANDSIZE);

        switch(currentAXIS) {
            case Utils.AXIS.SOUTH: // Player
                _rotationMod = 0;
                _startingPointY = transform.position.y;
                _endPointY = transform.position.y + _currentYVal;
                _startingPointX = transform.position.x - _currentXVal;
                _endPointX = transform.position.x + _currentXVal;
                break;

            case Utils.AXIS.EAST:
                _rotationMod = 90;
                _startingPointY = transform.position.y - _currentXVal;
                _endPointY = transform.position.y + _currentXVal;
                _startingPointX = transform.position.x;
                _endPointX = transform.position.x - _currentYVal;
                break;

            case Utils.AXIS.NORTH:
                _rotationMod = 0;
                _startingPointY = transform.position.y;
                _endPointY = transform.position.y - _currentYVal;
                _startingPointX = transform.position.x + _currentXVal;
                _endPointX = transform.position.x - _currentXVal;
                break;

            case Utils.AXIS.WEST:
                _rotationMod = 90;
                _startingPointY = transform.position.y + _currentXVal;
                _endPointY = transform.position.y - _currentXVal;
                _startingPointX = transform.position.x;
                _endPointX = transform.position.x  + _currentYVal;
                break;
        }
    }

    public void CardMouseOver(CardGO selected) {
        SetTransforms(children.IndexOf(selected));
    }

    public void CardMouseExit(CardGO selected) {
        if(selected == GameManager.instance.dealer._currentSelected)  GameManager.instance.dealer._currentSelected = null;
        SetTransforms();
    }

    public void HighLightCard(CardGO card) {
        // TODO: Make sure player nows which card they've selected, maybe flash highlight or something?
    }

    private void SetTransforms(int selected = -999) {
        for(int i = 0; i < transform.childCount; i++) {
            children[i].transform.position = new Vector3(children[i].transform.position.x,                                       
                                                        (selected - 1 == i || selected == i || selected + 1 == i) ? 
                                                        transform.position.y + Utils.HANDALIGNMENTMOD / ((selected - 1 == i || selected + 1 == i) ? 2.0f : 1.0f) : transform.position.y,
                                                        children[i].transform.position.z);
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
            Current suit of the hand?
                If I dont have any > Play highcard unless I'm considering "Shooting the moon"

            Has a heart been played?

            TODO:
                Add Value to High Spades
        */
        List<CardGO> playableCards = new List<CardGO>();

        Utils.CARDSUIT CurrentSuit =  GameManager.instance.dealer.CurrentSUIT();

        if (CurrentSuit == Utils.CARDSUIT.NULL) // WE ARE THE CURRENT DEALER
        {          
            if(shootForTheMoon) {           
                /*
                    Are we shooting the moon?
                        > Play highest card for w/e suit.
                */
                playableCards = player._currentHand.cards.OrderByDescending(x => x._currentCard.cardInfo.cardValue).ToList();
                if(! GameManager.instance.dealer.HaveHeartsBeenPlayed() && playableCards[0]._currentCard.cardInfo.cardSuit == Utils.CARDSUIT.HEART) {
                    throw new Exception();
                } else {
                    return playableCards[0];
                }
                
            } else {
                /*
                    Play out a card in the suit we don't have a lot of.
                */
                if( GameManager.instance.dealer.HaveHeartsBeenPlayed()) {
                    if(CoinFlip()) {
                        if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == Utils.CARDSUIT.HEART).Count > 0) {
                            if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == Utils.CARDSUIT.HEART)[0]._currentCard.cardInfo.cardValue < 7) return player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == Utils.CARDSUIT.HEART)[0];
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
                    > If not? Dump lowest
            */
            if(shootForTheMoon) {
                if(playableCards.FindAll(n => n._currentCard.cardInfo.cardValue >  GameManager.instance.dealer.CurrentHighCardInSuit()).Count > 0) { // Attempt to win // TODO: Make sure it's our highest.
                    return playableCards.FindAll(n => n._currentCard.cardInfo.cardValue >  GameManager.instance.dealer.CurrentHighCardInSuit())[0];
                } else {
                    return playableCards[0]; // Play lowest of the suit.
                }

            } else {   
            /*    
                Try to play under the current highest.
                FIXME CPU Still will drop Queen of Spade in a way that they would always win it back.
            */
                if(GameManager.instance.dealer.CurrentPlayed() > 2 && !GameManager.instance.dealer.HeartInActiveHand()) { // If we're last card to be played && no heart
                    return ReturnHighCardSUITINCLUSIVE( GameManager.instance.dealer.CurrentSUIT());
                } 
                
                // TODO: Make sure to look at the current highest card and look for under that.
                if(playableCards.FindAll(x => x._currentCard.cardInfo.cardValue <  GameManager.instance.dealer.CurrentHighCardInSuit()).Count > 0)
                    return playableCards.FindAll(x => x._currentCard.cardInfo.cardValue < GameManager.instance.dealer.CurrentHighCardInSuit()).OrderByDescending(n => n._currentCard.cardInfo.cardValue).ToList()[0];
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
                int index = playableCards.FindIndex(n => n._currentCard.cardInfo.cardSuit != Utils.CARDSUIT.HEART);
                return playableCards[index];
            } else {


                if(CoinFlip()) {
                    if(player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == Utils.CARDSUIT.HEART).Count > 0)
                        return ReturnHighCardSUITINCLUSIVE(Utils.CARDSUIT.HEART);
                    else if (player._currentHand.cards.Find(n => n._currentCard.IsQueenOfSpades())) // This might fail.
                        return player._currentHand.cards.Find(n => n._currentCard.IsQueenOfSpades());
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

    private CardGO ReturnHighCardSUITINCLUSIVE(Utils.CARDSUIT suit) {
        List<CardGO> temp = player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == suit).OrderByDescending(n => n._currentCard.cardInfo.cardValue).ToList();
        for(int i = 0; i < temp.Count; i++) {
            if(temp[i]._currentCard.IsQueenOfSpades()) continue;

            return temp[i];
        }

        return null;
    }


    private CardGO DealerLowCard() {
        int currentAmt = 13;
        Utils.CARDSUIT currentLow = Utils.CARDSUIT.NULL;
        // TODO: We look through all of our cards for our "lowest" card. 
        //    
        CardGO toPlay = null;
        List<Utils.CARDSUIT> checkedVals = new List<Utils.CARDSUIT>();
        bool checkAmt = false;
        do 
        {
            checkAmt = false;
            currentAmt = 13;
            toPlay = null;
            currentLow = Utils.CARDSUIT.NULL;
            for(int i = 0; i < 4; i++) {
                int tempAmt = player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == (Utils.CARDSUIT)i).Count;
                if(tempAmt == 0) continue;
                if(checkedVals.Contains((Utils.CARDSUIT)i)) continue;

                if(tempAmt < currentAmt) {
                    
                    if((Utils.CARDSUIT)i == Utils.CARDSUIT.HEART && ! GameManager.instance.dealer.HaveHeartsBeenPlayed()) continue;

                    currentAmt = tempAmt;
                    currentLow = (Utils.CARDSUIT)i;
                }
            }

            if(currentLow != Utils.CARDSUIT.NULL) {
                toPlay = player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == currentLow).OrderBy(n => n._currentCard.cardInfo.cardValue).ToList()[0];

                checkedVals.Add(currentLow);
                checkAmt = toPlay._currentCard.cardInfo.cardValue > Utils.ScaleValue(player.difficulty);
            }

        } while(checkAmt);

        // Look into the following statement.
        if(currentLow == Utils.CARDSUIT.NULL) toPlay = player._currentHand.cards.FindAll(n => n._currentCard.cardInfo.cardSuit == checkedVals[0]).OrderBy(n => n._currentCard.cardInfo.cardValue).ToList()[0];

        return toPlay;
    }
#endregion

#region Utility
    public bool CoinFlip() {
        return UnityEngine.Random.Range(0, 2) >= 1;
    }

    public Vector3 FixZ(Vector3 position) {
        position.z = 0;
        return position;
    }
#endregion
}
