using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public class Organization : MonoBehaviour
{

    private float _startingPointX, _endPointX;
    private float _startingPointY, _endPointY;
    public float _currentXVal, _currentYVal;
    public float _signOfMiddle;
    private Vector3 _rotatePointA, _rotatePointB;
    private int currentChildAmount, lastUpdateChildAmount;
    public BoxCollider2D boxCollider2D;
    
    public CONSTS.AXIS currentAXIS;
    private float _rotationMod;
    public bool isCPU;

    private void OnDrawGizmos() {
        
        UpdateAxis();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(_startingPointX, transform.position.y, transform.position.z), new Vector3(_endPointX, transform.position.y, transform.position.z)); 
        Gizmos.DrawLine(new Vector3(transform.position.x, _startingPointY, transform.position.z), new Vector3(transform.position.x, _endPointY, transform.position.z));  
       
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _rotatePointA);

        for(int i = 0; i< transform.childCount; i++) {
            Gizmos.DrawLine( _rotatePointA, transform.GetChild(i).transform.position);
        }
    
    }

    private void Awake() {
        lastUpdateChildAmount = currentChildAmount = 0;
    }


    [Button("UpdateX")]
    public void ScaleValues() {
        _currentXVal = Mathf.Lerp(0, CONSTS.HANDLINEVALUEX, transform.childCount / CONSTS.HANDSIZE);

        _currentYVal = Mathf.Lerp(0, CONSTS.HANDLINEVALUEY, transform.childCount / CONSTS.HANDSIZE);

        _startingPointX = transform.position.x - _currentXVal;
        _endPointX = transform.position.x + _currentXVal;
        _startingPointY = transform.position.y;
        _endPointY = transform.position.y + _currentYVal;
    }




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
            _currentChild.position = new Vector3(_currentChild.position.x, _currentChild.position.y, i * -1);
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
            case CONSTS.AXIS.NORTH:
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
            case CONSTS.AXIS.SOUTH:
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


    [Button("Position Dump")]
    public void PositionDump() {
        Debug.Log("Axis: " + currentAXIS.ToString());
        Debug.Log("X Points " + _startingPointX + " | " + _endPointX);
        Debug.Log("Y Points " + _startingPointY + " | " + _endPointY);

    }
}
