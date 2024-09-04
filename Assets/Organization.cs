using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Organization : MonoBehaviour
{

    private float _startingPointX, _endPointX;
    private float _startingPointY, _endPointY;
    private float _currentXVal, _currentYVal;
    public float _signOfMiddle;
    private Vector3 _rotatePointA, _rotatePointB;
    private int currentChildAmount, lastUpdateChildAmount;
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x - CONSTS.HANDLINEVALUEX, transform.position.y, transform.position.z), new Vector3(transform.position.x + CONSTS.HANDLINEVALUEX, transform.position.y, transform.position.z)); 
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, transform.position.y + CONSTS.HANDLINEVALUEY, transform.position.z));  
    
        for(int i = 0; i< transform.childCount; i++) {
            Gizmos.DrawLine( new Vector3(transform.position.x, transform.position.y - CONSTS.ROTATEVALY, transform.position.z), transform.GetChild(i).transform.position);
        }
    
    }

    private void Awake() {
        lastUpdateChildAmount = currentChildAmount = 0;
        _rotatePointA = new Vector3(transform.position.x, transform.position.y - CONSTS.ROTATEVALY, transform.position.z);
        _rotatePointB = new Vector3(transform.position.x, transform.position.y + CONSTS.HANDLINEVALUEY, transform.position.z);

        _currentXVal = CONSTS.HANDLINEVALUEX;
        _currentYVal = CONSTS.HANDLINEVALUEY;
        _startingPointY = transform.position.y;
        _endPointY = transform.position.y + _currentYVal;
        _startingPointX = transform.position.x - _currentXVal;
        _endPointX = transform.position.x + _currentXVal;
    }


     // Update is called once per frame
    void Update()
    {
        currentChildAmount = transform.childCount;
        if(currentChildAmount != lastUpdateChildAmount) OrganizeHand();

    }

    private void OrganizeHand() {
        /* 
            TODO: Scale pointA and pointB by childcount.
            
            After a card is played, scale the VALUE
        */
        lastUpdateChildAmount = transform.childCount;


        for(int i = 0; i < transform.childCount; i++) {
            transform.GetChild(i).transform.position = ReturnPosition(i);
            transform.GetChild(i).transform.rotation = Quaternion.Euler(new Vector3(0, 0, _signOfMiddle * GetVectorInternalAngle(_rotatePointA, transform.GetChild(i).transform.position, _rotatePointB)));
        }
    }


    private Vector3 ReturnPosition(int i) {
        float midVal = (transform.childCount - 1) / 2.0f;
        
        _signOfMiddle = Mathf.Sign(midVal - (float)i);
        float distanceFromMid = midVal - Mathf.Abs(midVal - (float)i);

        Debug.Log("I " + i + " | midVal " + midVal + " | distance " + distanceFromMid);
        return new Vector3(Mathf.Lerp(_startingPointX, _endPointX, (float)i / ((float)transform.childCount - 1)), Mathf.Lerp(_startingPointY, _endPointY, distanceFromMid / midVal));
    }
    private float GetVectorInternalAngle(Vector3 a, Vector3 b, Vector3 c) {
        return Vector3.Angle(b-a, c-a);
    }
    
}
