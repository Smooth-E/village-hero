using System;
using UnityEngine;

[ExecuteInEditMode]
public class ArrowDebugger : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        DrawArrow.ForGizmos(Vector3.zero, Vector3.one, new Color32( 242, 34, 136, 255));
    }
}
