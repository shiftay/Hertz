using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoPoint : MonoBehaviour
{

    public Color gizmoColor = Color.black;
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
