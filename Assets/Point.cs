using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point 
{
    public float x;
    public float y;

    public Point(float xPos, float yPos)
    {
        x = xPos;
        y = yPos;
    }

    public static Point Interpolate(Point p1, Point p2, float f)
    {
        return new Point(Mathf.Lerp(p1.x,p2.x,f), Mathf.Lerp(p1.y,p2.y,f));
    }
}
