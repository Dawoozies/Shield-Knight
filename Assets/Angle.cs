using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorCalculations
{
 public static class Angle
 {
     public static float AngleFromXAxis(Vector2 input)
     {
         Vector2 v = input.normalized;
         float h = v.magnitude;
         //All = can use anything so pick Arcsin
         if(v.x > 0 && v.y > 0)
         {
             return Mathf.Asin(v.y/h)*Mathf.Rad2Deg;
         }
         //Sin = 180 - Arcsin SOH
         if(v.x < 0 && v.y > 0)
         {
             return 180 - (Mathf.Asin(v.y/h)*Mathf.Rad2Deg);
         }
         //Tan = 180 + Arctan TOA
         if(v.x < 0 && v.y < 0)
         {
             return 180 + (Mathf.Atan(v.y/v.x)*Mathf.Rad2Deg);
         }
         //Cos = 360 - Arccos CAH
         if(v.x > 0 && v.y < 0)
         {
             return 360 - (Mathf.Acos(v.x / h) * Mathf.Rad2Deg);
         }
         //Perpendicular / Parallel cases
         if(v.x == 0)
         {
             if(v.y > 0)
             {
                 return 90;
             }
             if(v.y < 0)
             {
                 return 270;
             }
         }
         if(v.y == 0)
         {
             if(v.x > 0)
             {
                 return 0;
             }
             if(v.x < 0)
             {
                 return 180;
             }
         }
         return 0;
     }
 }   
}