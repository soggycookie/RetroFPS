using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public static Vector2 RandomInsideCircleWithBias(float bias, float radius){
        Vector2 random = Random.insideUnitCircle ;

        random.x = Mathf.Pow(random.x, bias);
        random.y = Mathf.Pow(random.y, bias);


        return random * radius ;
    }
}
