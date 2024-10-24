using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public void ApplyDamage(float healthLost, Vector3 forceDir, float force);
}
