using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Weapon/Gun Variant")]
public class GunVariantSO : ScriptableObject
{

    [Space(10)]
    [Header("Standard")]

    public TriggerMode standardTriggerMode;

    public Bullet standardBulletPrefab;

    public int standardDefaultCapacity;

    public int standardMaxCapacity;

    public float standardSwitchDelay;

    [Space(5)]
    [Header("Standard Charging Settings")]

    public float standardDamageMultiplier;

    public float standardTotalChargingTime;



    [Space(10)]
    [Header("Special")]

    public TriggerMode specialTriggetMode;

    public Bullet specialBulletPrefab;

    public int specialDefaultCapacity;

    public int specialMaxCapacity;

    public float specialSwitchDelay;

    [Space(5)]
    [Header("Special Charging Settings")]

    public float specialDamageMultiplier;

    public float specialTotalChargingTime;

}
