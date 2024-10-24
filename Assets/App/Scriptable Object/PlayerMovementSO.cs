using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Player/Movement")]
public class PlayerMovementSO : ScriptableObject
{
    [Header("Player Movement Settings")]

    [Space(10)]
    [Header("Walk Settings")]
    public float moveSpeed = 10f;
    public float slopeDownwardForce = 10f;
    public float groundDrag = 10f;
    public float maxWalkingVelocity = 18f;
    public float maxSlopeDegAngle = 30f;


    [Space(10)]
    public LayerMask ground;
    public LayerMask enemy;


    [Space(10)]
    [Header("Jump Settings")]
    public float jumpForce = 10f;
    public float wallJumpForce = 10f;
    public float jumpCoolDown = 1f;
    public int   maxWallJumpTime = 3;
    public float maxAirMultiplier = 5f;
    public float fallingAirMultiplier = 5f;
    public float maxAirVelocity = 200f;
    public float maxJumpVelocity = 16f;

    [Space(10)]
    [Header("Dash Settings")]
    public float airDragWhileDash = 10f;
    public float dashForce = 10f;
    public float dashDuration = 2f;
    public float maxDashVelocity = 25f;
    public float totalDashEnergy = 100f;
    public int   maxDashCharge = 4;
    public float rechargeEnergySpeed = 2f;
    public float timeBeforeRecharge = 1f;

    [Space(10)]
    [Header("Slide Settings")]
    public float slideForce = 10f;
    public float maxSlidingVelocity = 25f;

    [Space(10)]
    [Header("Slam Settings")]
    public float groundSlamForce = 10f;
    public float frozenSlamTime = 0.2f;
}
