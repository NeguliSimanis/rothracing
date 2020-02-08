using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils
{
    public static int Modulus(int a, int n)
    {
        int result = a % n;
        if ((result < 0 && n > 0) || (result > 0 && n < 0))
        {
            result += n;
        }
        return result;
    }

    public static bool RoughlyEqual(float a, float b, float treshold)
    {
        //float treshold = 0.22f;
        return (Mathf.Abs(a - b) < treshold);
    }
}