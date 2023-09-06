using System.Collections.Generic;
using UnityEngine;

public static class DrawArrow
{
    
    /// <summary>
    /// Draws a wireframe of an arrow with specified parameters using Unity's Gizmos.
    /// </summary>
    /// <param name="from">starting position of an arrow</param>
    /// <param name="to">ending position of an arrow</param>
    /// <param name="color">color to draw with</param>
    /// <param name="headRadius">radius of arrow's head's base circle</param>
    /// <param name="pointsOnCircle">how dense the base circle should be</param>
    /// <param name="headRadiusToLengthRatio">head's base circle radius / head's height ratio</param>
    public static void ForGizmos
    (
        Vector3 from, 
        Vector3 to, 
        Color color, 
        float headRadius = 0.3f, 
        int pointsOnCircle = 100, 
        float headRadiusToLengthRatio = 3f
    )
    {
        Gizmos.color = color;
        
        Gizmos.DrawLine(from, to);
        var direction = (to - from).normalized;
        var baseCenterPoint = to - direction * headRadius;
        
        var radiusVector = Quaternion.LookRotation(direction) * Vector3.up * headRadius / headRadiusToLengthRatio;
        var points = new List<Vector3>();
        for (int i = 0; i < pointsOnCircle; i++)
        {
            points.Add(baseCenterPoint + radiusVector);
            radiusVector = Quaternion.AngleAxis(360f / pointsOnCircle * i, direction) * radiusVector;
        }
        
        Gizmos.DrawLineList(points.ToArray());

        foreach (var point in points)
            Gizmos.DrawLine(point, to);
    }
}