﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastMath 
{
	public const float TWOPI = Mathf.PI* 2;
	public float _180divPI = 180 / Mathf.PI;
	public float PIdiv180 = Mathf.PI / 180;
	
		
	private float fastABS(float n) 
	{
			if (n< 0) return -n;
			return n;
	}

	public static Vector3 SimpleClamp(Vector3 toClamp)
    {
		Vector3 newV = new Vector3();
		if (toClamp.x >= 0.45f)
			newV.x = 1f;
		else if (toClamp.x <= -0.45f)
			newV.x = -1f;
		

		if (toClamp.y >= 0.45f)
			newV.y = 1f;
		else if (toClamp.y <= -0.45f)
			newV.y = -1f;
		


		return newV;
    }

	public static Vector3 GetWorldPositionOnPlane(float z)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
		float distance;
		xy.Raycast(ray, out distance);
		return ray.GetPoint(distance);
	}

	/**
	* Get distance between two points as a Number
	* @param	x1
	* @param	y1
	* @param	x2
	* @param	y2
	* @return the Distance
	*/

	private float getDist(float x1, float y1, float x2, float y2)
	{
		return Mathf.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
	}

	/**
	* Get distance between two points as an integer
	* @param	x1
	* @param	y1
	* @param	x2
	* @param	y2
	* @return the Distance 
	*/

	private int getDisti(int x1, int y1, int x2, int y2)
	{
		return (int)getDist(x1,y1,x2,y2);
	}

	/**
	* Get square of the distance between two points (faster than getDist) as a Number
	* @param	x1
	* @param	y1
	* @param	x2
	* @param	y2
	* @return the Distance^2
	*/

	public static float getDist2(float x1, float y1, float x2, float y2)
	{
		return (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
	}

	/**
	* Get square of the distance between two points (faster than getDisti) as an int
	* @param	x1
	* @param	y1
	* @param	x2
	* @param	y2
	* @return the Distance^2
	*/

	private static int getDist2i(int x1, int y1, int x2, int y2)
	{
		return (((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2)));
	}

	/**
	 * Returns a list of points in a circle. Does NOT list them as class Point, get them 2 at a time.
	 * @param	radius size of circle
	 * @param	MAX number of points
	 * @return a list of circle coordinates in a Vector.<Number>
	 */

	public static List<Vector2> circlePoints(float radius, float MAX) 
	{
		
		List<Vector2> circ = new List<Vector2> ();
		for (float i = 0; i < MAX; i++){
			circ.Add( new Vector2(((Mathf.Cos(i / MAX * TWOPI)) * radius), ((Mathf.Sin(i / MAX * TWOPI)) * radius) ) );
		}
		return circ;
	}

	/**
	 * Returns a list of points in a circle. Does NOT list them as class Point, get them 2 at a time.
	 * @param	radius size of circle
	 * @param	MAX number of points
	 * @param   xo amount to offset x
	 * @param   yo amount to offset y
	 * @return a list of circle coordinates in a Vector.<Number>
	 */

	public static List<float> circlePointsOffset(float radius, float MAX, float xo, float yo)
	{
		List<float> circ = new List<float>();
		for (float i = 0; i < MAX; i++){
			circ.Add(((Mathf.Cos(i / MAX * TWOPI)) * radius) + xo);
			circ.Add(((Mathf.Sin(i / MAX * TWOPI)) * radius) + yo);
		}
		return circ;
	}

	public static float angleTo(Vector2 sourceVector,  Vector2 destVector )
 	{
 		return Mathf.Acos(Vector2.Dot(sourceVector, destVector) / (sourceVector.magnitude * destVector.magnitude ) );
 	}

	public static Color ConvertFromUint(uint uCol)
    {
		Color c = new Color();
		c.b = (byte)(uCol & 0xff) / 255f;
		c.g = (byte)( (uCol >> 8) & 0xff) / 255f;
		c.r = (byte)((uCol >> 16) & 0xff) / 255f;
		c.a = 1;
		//Debug.Log("color " + c);
		return c;
    }

public static Vector2 rotateVector(float radians, Vector2 v) 
	{
		float ca = Mathf.Cos(radians);
		float sa = Mathf.Sin(radians);
		float rx = v.x* ca-v.y* sa;
		float ry = v.x* sa+v.y* ca;
		v.x = rx;
		v.y = ry;

	return v;
	}

	public static float toRotation(Vector2 v)
 		{
		//calc the angle
		if (v.x == 0 || v.y == 0)
			return 0;
 			float ang = Mathf.Atan(v.y / v.x);
 			
 			//if it is in the first quadrant
 			if(v.y< 0 && v.x > 0)
 			{
 				return ang;
 			}
			//if its in the 2nd or 3rd quadrant
			if ((v.y < 0 && v.x < 0) ||
				(v.y > 0 && v.x < 0))
			{
				return ang + 3.141592653589793f;
			}
			//it must be in the 4th quadrant so:
			return ang + 6.283185307179586f;
 		}
   
}
