using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Muzzle Flash")]
public class MuzzleFlashSO : ScriptableObject
{
    public Material material;

    public float scale;
    public float fadeOutTime;
}
